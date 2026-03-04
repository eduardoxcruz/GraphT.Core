using GraphT.Model.Enums;
using GraphT.Model.Events;
using GraphT.Model.Services;
using GraphT.Model.ValueObjects;

using SeedWork;

namespace GraphT.Model.Aggregates;

public class TodoTask : IEntity<Guid>, IEquatable<TodoTask>
{
	public Guid Id { get; private init; }
	public string Name { get; private set; }
	public bool IsFun { get; set; }
	public bool IsProductive { get; set; }
	public Complexity Complexity { get; set; }
	public Priority Priority { get; set; }
	public TaskState Status { get; private set; }
	public int Progress { get; private set; }
	
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset? StartDate { get; private set; }
	public DateTimeOffset? FinishDate { get; private set; }
	public DateTimeOffset? LimitDateTime { get; private set; }
	public DateTimeOffset? CurrentWorkSessionStartedAt { get; private set; }
	public TimeSpan ElapsedTime { get; private set; }
	public RecurrenceInfo RecurrenceInfo { get; private set; }
	
	public Relevance Relevance => TaskRelevanceService.Calculate(IsFun, IsProductive);
	public Punctuality Punctuality => TaskPunctualityService.Calculate(LimitDateTime, FinishDate);
	
	public List<Guid> Parents { get; private set; }
	public List<Guid> Children { get; private set; }
	public List<Guid> LifeAreas { get; private set; }
	public List<StatusChangelog> StatusChangeLogs { get; private set; }
	public List<IDomainEvent> DomainEvents { get; private set; }

	private TodoTask() { }

	public static TodoTask Create(string name = "New Task",
		bool? isFun = null,
		bool? isProductive = null,
		Complexity? complexity = null,
		Priority? priority = null,
		DateTimeOffset? startDate = null,
		DateTimeOffset? finishDate = null,
		DateTimeOffset? limitDateTime = null)
	{
		if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty");

		DateTimeOffset now =  DateTimeOffset.UtcNow;
		
		TodoTask task =  new() { Id = Guid.NewGuid(), 
			CreatedAt = now, 
			Name = name, 
			IsFun = isFun ?? false, 
			IsProductive = isProductive ?? false, 
			Complexity = complexity ?? Complexity.Undefined, 
			Priority = priority ?? Priority.Distraction, 
			Status = TaskState.Created,
			StartDate = startDate, 
			FinishDate = finishDate, 
			LimitDateTime = limitDateTime, 
			RecurrenceInfo = new RecurrenceInfo(false, null, null),
			Parents = [],
			Children = [],
			LifeAreas = [],
			DomainEvents = [],
			StatusChangeLogs = []
		};
		
		task.SetStatus(TaskState.Created, now);
		
		return task;
	}

	public static TodoTask Rehydrate(Guid id,
		string name,
		bool isFun,
		bool isProductive,
		Complexity complexity,
		Priority priority,
		TaskState status,
		int progress,
		DateTimeOffset createdAt,
		DateTimeOffset? startDate,
		DateTimeOffset? finishDate,
		DateTimeOffset? limitDateTime,
		DateTimeOffset? currentWorkSessionStartedAt,
		TimeSpan elapsedTime,
		RecurrenceInfo recurrenceInfo,
		List<Guid> parents,
		List<Guid> children,
		List<Guid> lifeAreas,
		List<StatusChangelog> statusChangeLogs)
	{
		TodoTask task =  new() { DomainEvents = [], 
			Parents = parents, 
			Children = children, 
			LifeAreas = lifeAreas, 
			StatusChangeLogs = statusChangeLogs, 
			Id = id, 
			Name = name, 
			IsFun = isFun , 
			IsProductive = isProductive, 
			Complexity = complexity, 
			Priority = priority, 
			Status = status, 
			CreatedAt = createdAt, 
			StartDate = startDate, 
			FinishDate = finishDate, 
			LimitDateTime = limitDateTime, 
			CurrentWorkSessionStartedAt = currentWorkSessionStartedAt, 
			ElapsedTime = elapsedTime, 
			RecurrenceInfo = recurrenceInfo,
			Progress = progress
		};

		return task;
	}

	public void SetName(string newName)
	{
		if (string.IsNullOrWhiteSpace(newName)) 
			throw new ArgumentException("Task new name cannot be empty");
		
		Name = newName;
	}
	
	public void SetStatus(TaskState newStatus, DateTimeOffset? dateTime = null)
	{
		dateTime ??= DateTimeOffset.UtcNow;

		if (Status == TaskState.Doing && newStatus != TaskState.Doing)
		{
			ElapsedTime += (dateTime.Value) - CurrentWorkSessionStartedAt!.Value;
			CurrentWorkSessionStartedAt = null;
		}
		
		switch (newStatus)
		{
			case TaskState.Doing:
				CurrentWorkSessionStartedAt = dateTime;
				StartDate ??= dateTime;
				break;
			case TaskState.Discarded or TaskState.Finished:
				FinishDate ??= dateTime;
				Progress = 100;
				break;
		}

		Status = newStatus;
		AddStatusChangelog(new StatusChangelog(dateTime.Value, newStatus));
	}
	
	public void RefreshStatusAndProgress(List<TodoTask> children, 
		bool automaticStatusUpdate, 
		bool? shouldFinishIfPossible = null)
	{
		Progress = TaskProgressService.Calculate(children, Status);
		
		if (automaticStatusUpdate)
		{
			if (shouldFinishIfPossible is null) 
				throw new ArgumentException("Automatic status update requires shouldFinishIfPossible to be set");
			
			SetStatus(TaskStatusCalculatorService.Calculate(children, shouldFinishIfPossible.Value));
		}
	}
	
	public void SetRecurrence(RecurrencePattern pattern, DateTimeOffset? baseLimitDate = null)
	{
		RecurrenceInfo.RecurrencePattern = pattern;
		RecurrenceInfo.IsRecurring = true;
		DateTimeOffset referenceDate = baseLimitDate ?? DateTimeOffset.UtcNow;
		SetLimitDateTime(pattern.CalculateNextLimitDate(referenceDate));
	}
	
	public void RemoveRecurrence()
	{
		RecurrenceInfo.IsRecurring = false;
		RecurrenceInfo.RecurrencePattern = null;
		RecurrenceInfo.LastRecurrenceReset = null;
	}
	
	public bool ShouldReset(DateTimeOffset currentDate)
	{
		if (!RecurrenceInfo.IsRecurring || RecurrenceInfo.RecurrencePattern == null || LimitDateTime == null)
			return false;

		return currentDate > LimitDateTime.Value;
	}

	public DomainResult ResetRecurringTask(DateTimeOffset currentDate)
	{
		if (!RecurrenceInfo.IsRecurring || RecurrenceInfo.RecurrencePattern == null) 
			return DomainResult.Failure("Task is not recurring");

		if (!ShouldReset(currentDate)) return DomainResult.Failure("Task does not need to be reset yet");

		ResetTask(RecurrenceInfo.RecurrencePattern!.Value.CalculateNextLimitDate(LimitDateTime!.Value));
		RecurrenceInfo.LastRecurrenceReset = currentDate;
        
		return DomainResult.Success();
	}

	public void ResetTask(DateTimeOffset? newLimitDateTime = null)
	{
		SetStatus(TaskState.ReadyToStart);
		ResetStartDateTime();
		ResetFinishDateTime();
		
		if (newLimitDateTime is not null) SetLimitDateTime(newLimitDateTime.Value);
		
		ResetLimitDateTime();
	}
	
	public void SetStartDateTime(DateTimeOffset dateTime)
	{
		StartDate = dateTime;
	}

	public void ResetStartDateTime()
	{
		StartDate = null;
	}

	public void SetFinishDateTime(DateTimeOffset dateTime)
	{
		FinishDate = dateTime;
	}

	public void ResetFinishDateTime()
	{
		FinishDate = null;
	}

	public void SetLimitDateTime(DateTimeOffset dateTime)
	{
		LimitDateTime = dateTime;
	}

	public void ResetLimitDateTime()
	{
		LimitDateTime = null;
	}

	public void AddParent(TodoTask parent, bool inheritHighestPriority, bool inheritLifeAreas)
	{
		if (Parents.Contains(parent.Id)) return;

		if (Children.Contains(parent.Id)) return;
		
		if (inheritHighestPriority) Priority = parent.Priority > Priority ? parent.Priority : Priority;

		if (inheritLifeAreas)
		{
			foreach (Guid lifeAreaId in parent.LifeAreas)
			{
				AddLifeArea(lifeAreaId);
			}
		}
		
		Parents.Add(parent.Id);
		AddDomainEvent(new ParentAddedDomainEvent(parent.Id));
	}

	public void RemoveParent(Guid parentId)
	{
		if (!Parents.Contains(parentId)) return;
		
		Parents.Remove(parentId);
		AddDomainEvent(new ParentRemovedDomainEvent(parentId));
	}

	public void AddChild(Guid childId)
	{
		if (Children.Contains(childId)) return;
		
		if (Parents.Contains(childId)) return;
		
		Children.Add(childId);
		AddDomainEvent(new ChildAddedDomainEvent(childId));
	}

	public void RemoveChild(Guid childId)
	{
		if (!Children.Contains(childId)) return;
		
		Children.Remove(childId);
		AddDomainEvent(new ChildedRemovedDomainEvent(childId));
	}

	public void AddLifeArea(Guid lifeAreaId)
	{
		if (LifeAreas.Contains(lifeAreaId)) return;
		
		LifeAreas.Add(lifeAreaId);
		AddDomainEvent(new LifeAreaAddedDomainEvent(lifeAreaId));
	}

	public void RemoveLifeArea(Guid lifeAreaId)
	{
		if (!LifeAreas.Contains(lifeAreaId)) return;
		
		LifeAreas.Remove(lifeAreaId);
		AddDomainEvent(new LifeAreaRemovedDomainEvent(lifeAreaId));
	}
	
	private void AddStatusChangelog(StatusChangelog newLog)
	{
		StatusChangeLogs.Add(newLog);
		AddDomainEvent(new StatusChangelogCreatedDomainEvent(newLog));
	}
	
	public void AddDomainEvent(IDomainEvent domainEvent)
	{
		DomainEvents.Add(domainEvent);
	}

	public void RemoveDomainEvent(IDomainEvent domainEvent)
	{
		DomainEvents.Remove(domainEvent);
	}
	
	public void ClearDomainEvents()
	{
		DomainEvents.Clear();
	}
	
	public bool Equals(TodoTask? other)
	{
		if (other is null)
		{
			return false;
		}

		if (ReferenceEquals(this, other))
		{
			return true;
		}

		return Id.Equals(other.Id);
	}

	public override bool Equals(object? obj)
	{
		if (obj is null)
		{
			return false;
		}

		if (ReferenceEquals(this, obj))
		{
			return true;
		}

		if (obj.GetType() != GetType())
		{
			return false;
		}

		return Equals((TodoTask)obj);
	}

	public override int GetHashCode()
	{
		return Id.GetHashCode();
	}

	public static bool operator ==(TodoTask? left, TodoTask? right)
	{
		return Equals(left, right);
	}

	public static bool operator !=(TodoTask? left, TodoTask? right)
	{
		return !Equals(left, right);
	}
}
