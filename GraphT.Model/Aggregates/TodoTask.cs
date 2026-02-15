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
	
	public Guid Id { get; private init; }
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
	public RecurrenceInfo RecurrenceInfo { get; private set; }
	
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
		TaskState? status = null,
		DateTimeOffset? startDate = null,
		DateTimeOffset? finishDate = null,
		DateTimeOffset? limitDateTime = null,
		RecurrencePattern? recurrencePattern = null,
		List<Guid>? lifeAreas = null,
		List<TodoTask>? children = null,
		List<TodoTask>? parents = null,
		ParentLinkingStrategy? priorityLinkingStrategy = null, 
		ParentLinkingStrategy? lifeAreasLinkingStrategy = null)
	{
		if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty");

		DateTimeOffset now =  DateTimeOffset.Now;
		
		int parentsToAdd = parents?.Count ?? 0;
		int childrenToAdd = children?.Count ?? 0;
		
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
			_parents = new List<Guid>(parentsToAdd),
			_children = new List<Guid>(childrenToAdd),
			_lifeAreas = [],
			_domainEvents = [],
			_statusChangeLogs = [ new StatusChangelog(now, TaskState.Created) ]
		};
		
		if (status is not null) task.SetStatus(status.Value);
		
		if (recurrencePattern is not null) task.SetRecurrence(recurrencePattern.Value, task.LimitDateTime);

		if (parents is not null)
		{
			if (priorityLinkingStrategy is null) throw new ArgumentException("Priority linking strategy must be set when parents are set");
			
			if (lifeAreasLinkingStrategy is null) throw new ArgumentException("Life areas linking strategy must be set when parents are set");
			
			task.SetParents(parents, priorityLinkingStrategy.Value, lifeAreasLinkingStrategy.Value);
		}
		
		if (children is not null)
		{
			task.SetChildren(children);
		}
		
		if (lifeAreas is not null) task.SetLifeAreas(lifeAreas);

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
		RecurrenceInfo recurrenceInfo,
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
			RecurrenceInfo = recurrenceInfo
		};

		return task;
	}

	public void SetName(string newName)
	{
		if (string.IsNullOrWhiteSpace(newName)) 
			throw new ArgumentException("Task new name cannot be empty");
		
		Name = newName;
	}
	
	public void SetRecurrence(RecurrencePattern pattern, DateTimeOffset? baseLimitDate = null)
	{
		RecurrenceInfo.RecurrencePattern = pattern;
		RecurrenceInfo.IsRecurring = true;
		DateTimeOffset referenceDate = baseLimitDate ?? DateTimeOffset.Now;
		LimitDateTime = pattern.CalculateNextLimitDate(referenceDate);
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

		SetStatus(TaskState.ReadyToStart);
		StartDate = null;
		FinishDate = null;
		RecurrenceInfo.LastRecurrenceReset = currentDate;
		LimitDateTime = RecurrenceInfo.RecurrencePattern.Value.CalculateNextLimitDate(LimitDateTime!.Value);
        
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

	public void SetParents(List<TodoTask> parents, 
		ParentLinkingStrategy priorityLinkingStrategy, 
		ParentLinkingStrategy lifeAreasLinkingStrategy)
	{
		List<TodoTask> newParents = parents.Distinct().Where(parent => parent.Id != Id).ToList();
		
		if (priorityLinkingStrategy is ParentLinkingStrategy.InheritHighestPriority)
		{
			Priority newPriority = newParents.Max(parent => parent.Priority);
			Priority = newPriority;
		}
		
		if (lifeAreasLinkingStrategy is ParentLinkingStrategy.InheritLifeAreas)
		{
			SetLifeAreas(_lifeAreas.Union(newParents.SelectMany(parent => parent.LifeAreas)).ToList());
		}
		
		_parents = newParents.Select(p => p.Id).ToList();
	}

	public void SetChildren(List<TodoTask> children)
	{
		List<TodoTask> newChildren = children.Distinct().Where(child => child.Id != Id).ToList();
		
		Progress = TaskProgressService.Calculate(newChildren, Status);
		
		_children = newChildren.Select(c => c.Id).ToList();
	}

	public void SetLifeAreas(List<Guid> lifeAreas)
	{
		List<Guid> newLifeAreas = lifeAreas.Distinct().ToList();
		
		_lifeAreas = newLifeAreas;
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
