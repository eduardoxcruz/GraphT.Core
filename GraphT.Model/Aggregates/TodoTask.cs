using GraphT.Model.DomainEvents;
using GraphT.Model.DomainServices;
using GraphT.Model.Enums;
using GraphT.Model.ValueObjects;

using SeedWork;

namespace GraphT.Model.Aggregates;

public class TodoTask : IEntity<Guid>, IEquatable<TodoTask>
{
	private readonly List<IDomainEvent> _domainEvents;
	public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
	
	public Guid Id { get; }
	public string Name { get; private set; }
	public bool IsFun { get; private set; }
	public bool IsProductive { get; private set; }
	public Complexity Complexity { get; private set; }
	public Priority Priority { get; private set; }
	public TaskState Status { get; private set; }
	
	public DateTimeOffset CreatedAt { get; }
	public DateTimeOffset? StartDate { get; private set; }
	public DateTimeOffset? FinishDate { get; private set; }
	public DateTimeOffset? LimitDateTime { get; private set; }
	public DateTimeOffset? CurrentWorkSessionStartedAt { get; private set; }
	public TimeSpan ElapsedTime { get; private set; }

	public Relevance Relevance => TaskRelevanceService.Calculate(IsFun, IsProductive);
	public int Progress => TaskProgressService.Calculate(_children, Status);
	public Punctuality Punctuality => TaskPunctualityService.Calculate(LimitDateTime, FinishDate);
	
	private List<TodoTask> _parents;
	public IReadOnlyList<TodoTask> Parents => _parents;
	
	private List<TodoTask> _children;
	public IReadOnlyList<TodoTask> Children => _children;
	
	private List<LifeArea> _lifeAreas;
	public IReadOnlyList<LifeArea> LifeAreas => _lifeAreas;
	
	private List<StatusChangelog> _statusChangeLogs;
	public IReadOnlyList<StatusChangelog> StatusChangeLogs => _statusChangeLogs;

	public TodoTask() : this("New Task") {}
	
	public TodoTask(string name, 
		bool? isFun = null, 
		bool? isProductive = null, 
		Complexity? complexity = null, 
		Priority? priority = null, 
		TaskState? status = null, 
		DateTimeOffset? startDate = null, 
		DateTimeOffset? finishDate = null, 
		DateTimeOffset? limitDateTime = null,
		List<TodoTask>? parents = null, 
		List<TodoTask>? children = null, 
		List<LifeArea>? lifeAreas = null, 
		List<StatusChangelog>? statusChangeLogs = null)
	{
		if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty");
		
		Id = Guid.NewGuid();
		CreatedAt = DateTimeOffset.Now;
		Name = name;
		IsFun = isFun ?? false;
		IsProductive = isProductive ?? false;
		Complexity = complexity ?? Complexity.Undefined;
		Priority = priority ?? Priority.Distraction;
		Status = status ?? TaskState.Created;
		StartDate = startDate;
		FinishDate = finishDate;
		LimitDateTime = limitDateTime;
		_parents = parents ?? [];
		_children = children ?? [];
		_lifeAreas = lifeAreas ?? [];
		_domainEvents = [];
		
		switch (statusChangeLogs is null)
		{
			case true:
				_statusChangeLogs = [ new StatusChangelog(DateTimeOffset.Now, TaskState.Created) ];
				break;
			case false:
				_statusChangeLogs = statusChangeLogs;
				break;
		}
	}
	
	public void Update(string? name = null, 
		bool? isFun = null, 
		bool? isProductive = null, 
		Complexity? complexity = null, 
		Priority? priority = null, 
		DateTimeOffset? startDate = null, 
		DateTimeOffset? finishDate = null, 
		DateTimeOffset? limitDateTime = null)
	{
		if (name is not null) Name = name;
		
		if (isFun is not null) IsFun = isFun.Value;
		
		if (isProductive is not null) IsProductive = isProductive.Value;
		
		if (complexity is not null) Complexity = complexity.Value;
		
		if (priority is not null) Priority = priority.Value;
		
		if (startDate is not null) StartDate = startDate.Value;
		
		if (finishDate is not null) FinishDate = finishDate.Value;
		
		if (limitDateTime is not null) LimitDateTime = limitDateTime.Value;
	}
	
	public DomainResult SetStatus(TaskState newStatus, DateTimeOffset? dateTime = null)
	{
		if (newStatus == Status) return DomainResult.Failure("Task already have this state.");
		
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
		StatusChangelog newLog = new(dateTime.Value, newStatus); 
		_statusChangeLogs.Add(newLog);
		AddDomainEvent(new StatusChangelogCreatedDomainEvent(newLog));
		
		return DomainResult.Success();
	}
	
	public DomainResult AddParents(List<TodoTask> parents)
	{
		if (parents.Contains(this)) return DomainResult.Failure("Parent cannot be a child of itself.");
		
		_parents.AddRange(parents.Except(_parents));
		
		return DomainResult.Success();
	}
	
	public DomainResult AddChildren(List<TodoTask> children)
	{
		if (children.Contains(this)) return DomainResult.Failure("Parent cannot be a child of itself.");
		
		_children.AddRange(children.Except(_children));
		
		return DomainResult.Success();
	}

	public DomainResult AddLifeAreas(List<LifeArea> lifeAreas)
	{
		_lifeAreas.AddRange(lifeAreas.Except(_lifeAreas));
		
		return DomainResult.Success();
	}
	
	public void RemoveParents(List<TodoTask> parents)
	{
		_parents.RemoveAll(parents.Contains);
	}

	public void RemoveChildren(List<TodoTask> children)
	{
		_children.RemoveAll(children.Contains);
	}

	public void RemoveLifeAreas(List<LifeArea> lifeAreas)
	{
		_lifeAreas.RemoveAll(lifeAreas.Contains);
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
