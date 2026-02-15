using GraphT.Model.Enums;
using GraphT.Model.Events;
using GraphT.Model.Services;
using GraphT.Model.ValueObjects;

using SeedWork;

namespace GraphT.Model.Aggregates;

public class TodoTask : IEntity<Guid>, IEquatable<TodoTask>
{
	private List<IDomainEvent> _domainEvents;
	public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents;
	
	public Guid Id { get; private set; }
	public string Name { get; private set; }
	public bool IsFun { get; set; }
	public bool IsProductive { get; set; }
	public Complexity Complexity { get; set; }
	public Priority Priority { get; set; }
	public TaskState Status { get; private set; }
	public int Progress { get; set; }
	
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset? StartDate { get; set; }
	public DateTimeOffset? FinishDate { get; set; }
	public DateTimeOffset? LimitDateTime { get; set; }
	public DateTimeOffset? CurrentWorkSessionStartedAt { get; private set; }
	public TimeSpan ElapsedTime { get; private set; }
	
	public bool IsRecurring { get; private set; }
	public RecurrencePattern? RecurrencePattern { get; private set; }
	public DateTimeOffset? LastRecurrenceReset { get; private set; }

	public Relevance Relevance => TaskRelevanceService.Calculate(IsFun, IsProductive);
	public Punctuality Punctuality => TaskPunctualityService.Calculate(LimitDateTime, FinishDate);
	
	private List<Guid> _parents;
	public IReadOnlyList<Guid> Parents => _parents;
	
	private List<Guid> _children;
	public IReadOnlyList<Guid> Children => _children;
	
	private List<Guid> _lifeAreas;
	public IReadOnlyList<Guid> LifeAreas => _lifeAreas;
	
	private List<StatusChangelog> _statusChangeLogs;
	public IReadOnlyList<StatusChangelog> StatusChangeLogs => _statusChangeLogs;

	private TodoTask() { }

	public static TodoTask Create(string name = "New Task",
		bool? isFun = null,
		bool? isProductive = null,
		Complexity? complexity = null,
		Priority? priority = null,
		DateTimeOffset? startDate = null,
		DateTimeOffset? finishDate = null,
		DateTimeOffset? limitDateTime = null,
		List<Guid>? parents = null,
		List<Guid>? children = null,
		List<Guid>? lifeAreas = null)
	{
		if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty");

		DateTimeOffset now =  DateTimeOffset.Now;
		
		TodoTask task =  new() { Id = Guid.NewGuid(), 
			CreatedAt = now, 
			Name = name, 
			IsFun = isFun ?? false, 
			IsProductive = isProductive ?? false, 
			Complexity = complexity ?? Complexity.Undefined, 
			Priority = priority ?? Priority.Distraction, 
			StartDate = startDate, 
			FinishDate = finishDate, 
			LimitDateTime = limitDateTime, 
			_parents = parents ?? [], 
			_children = children ?? [], 
			_lifeAreas = lifeAreas ?? [], 
			_domainEvents = [], 
			_statusChangeLogs = [ new StatusChangelog(now, TaskState.Created) ]
		};

		return task;
	}

	public static TodoTask Rehydrate(Guid id,
		string name,
		bool isFun,
		bool isProductive,
		Complexity complexity,
		Priority priority,
		TaskState status,
		DateTimeOffset createdAt,
		DateTimeOffset? startDate,
		DateTimeOffset? finishDate,
		DateTimeOffset? limitDateTime,
		DateTimeOffset? currentWorkSessionStartedAt,
		TimeSpan elapsedTime,
		bool isRecurring,
		RecurrencePattern? recurrencePattern,
		DateTimeOffset? lastRecurrenceReset,
		List<Guid> parents,
		List<Guid> children,
		List<Guid> lifeAreas,
		List<StatusChangelog> statusChangeLogs)
	{
		TodoTask task =  new() { _domainEvents = [], 
			_parents = parents, 
			_children = children, 
			_lifeAreas = lifeAreas, 
			_statusChangeLogs = statusChangeLogs, 
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
			IsRecurring = isRecurring, 
			RecurrencePattern = recurrencePattern, 
			LastRecurrenceReset = lastRecurrenceReset
		};

		return task;
	}
	
	public DomainResult SetRecurrence(RecurrencePattern pattern, DateTimeOffset? baseLimitDate = null)
	{
		RecurrencePattern = pattern;
		IsRecurring = true;
		DateTimeOffset referenceDate = baseLimitDate ?? DateTimeOffset.Now;
		LimitDateTime = pattern.CalculateNextLimitDate(referenceDate);
        
		return DomainResult.Success();
	}

	public void RemoveRecurrence()
	{
		IsRecurring = false;
		RecurrencePattern = null;
		LastRecurrenceReset = null;
	}
	
	public bool ShouldReset(DateTimeOffset currentDate)
	{
		if (!IsRecurring || RecurrencePattern == null || LimitDateTime == null)
			return false;

		return currentDate > LimitDateTime.Value;
	}

	public DomainResult ResetRecurringTask(DateTimeOffset currentDate)
	{
		if (!IsRecurring || RecurrencePattern == null) return DomainResult.Failure("Task is not recurring");

		if (!ShouldReset(currentDate)) return DomainResult.Failure("Task does not need to be reset yet");

		SetStatus(TaskState.ReadyToStart);
		StartDate = null;
		FinishDate = null;
		LastRecurrenceReset = currentDate;
		LimitDateTime = RecurrencePattern.Value.CalculateNextLimitDate(LimitDateTime!.Value);
        
		return DomainResult.Success();
	}
	
	public void SetStatus(TaskState newStatus, DateTimeOffset? dateTime = null)
	{
		if (newStatus == Status) return;
		
		dateTime ??= DateTimeOffset.Now;

		if (Status == TaskState.Doing && newStatus != TaskState.Doing)
		{
			ElapsedTime += (dateTime.Value) - CurrentWorkSessionStartedAt!.Value;
			CurrentWorkSessionStartedAt = null;
		}
		
		switch (newStatus)
		{
			case TaskState.Doing:
				CurrentWorkSessionStartedAt = dateTime;
				break;
			case TaskState.Discarded or TaskState.Finished:
				FinishDate = dateTime;
				break;
		}

		Status = newStatus;
		AddStatusChangelog(new StatusChangelog(dateTime.Value, newStatus));
	}

	public void AddParent(Guid parentId)
	{
		if (parentId == Id) return;
		
		if (_parents.Contains(parentId)) return;
		
		_parents.Add(parentId);
		AddDomainEvent(new ParentAddedDomainEvent(parentId));
	}

	public void AddChild(Guid childId)
	{
		if (childId == Id) return;
		
		if (_children.Contains(childId)) return;
		
		_children.Add(childId);
		AddDomainEvent(new ChildAddedDomainEvent(childId));
	}

	public void AddLifeArea(Guid lifeAreaId)
	{
		if (_lifeAreas.Contains(lifeAreaId)) return;
		
		_lifeAreas.Add(lifeAreaId);
		AddDomainEvent(new LifeAreaAddedDomainEvent(lifeAreaId));
	}

	public void RemoveParent(Guid parentId)
	{
		if (!_parents.Contains(parentId)) return;
		
		_parents.Remove(parentId);
		AddDomainEvent(new ParentRemovedDomainEvent(parentId));
	}

	public void RemoveChild(Guid childId)
	{
		if (!_children.Contains(childId));
		
		_children.Remove(childId);
		AddDomainEvent(new ChildRemovedDomainEvent(childId));
	}

	public void RemoveLifeArea(Guid lifeAreaId)
	{
		if (!_lifeAreas.Contains(lifeAreaId)) return;
		
		_lifeAreas.Remove(lifeAreaId);
		AddDomainEvent(new LifeAreaRemovedDomainEvent(lifeAreaId));
	}
	
	private void AddStatusChangelog(StatusChangelog newLog)
	{
		_statusChangeLogs.Add(newLog);
		AddDomainEvent(new StatusChangelogCreatedDomainEvent(newLog));
	}
	
	public void AddDomainEvent(IDomainEvent domainEvent)
	{
		_domainEvents.Add(domainEvent);
	}

	public void RemoveDomainEvent(IDomainEvent domainEvent)
	{
		_domainEvents.Remove(domainEvent);
	}
	
	public void ClearDomainEvents()
	{
		_domainEvents.Clear();
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
