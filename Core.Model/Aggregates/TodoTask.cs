using Core.Model.Enums;
using Core.Model.Services;
using Core.Model.ValueObjects;
using Core.SeedWork;

namespace Core.Model.Aggregates;

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
	public DateTimeOffset? StartDateTime { get; private set; }
	public DateTimeOffset? FinishDateTime { get; private set; }
	public DateTimeOffset? LimitDateTime { get; private set; }
	public DateTimeOffset? CurrentWorkSessionStartedAt { get; private set; }
	public TimeSpan ElapsedTime { get; private set; }
	
	public Relevance Relevance => TaskRelevanceService.Calculate(IsFun, IsProductive);
	public Punctuality Punctuality => TaskPunctualityService.Calculate(LimitDateTime, FinishDateTime);
	
	public List<Guid> Parents { get; private set; }
	public List<Guid> Children { get; private set; }
	public List<Guid> LifeAreas { get; private set; }

	private TodoTask() { }

	public static TodoTask Create(string name = "New Task",
		bool? isFun = null,
		bool? isProductive = null,
		Complexity? complexity = null,
		Priority? priority = null,
		DateTimeOffset? startDateTime = null,
		DateTimeOffset? finishDateTime = null,
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
			StartDateTime = startDateTime, 
			FinishDateTime = finishDateTime, 
			LimitDateTime = limitDateTime, 
			Parents = [],
			Children = [],
			LifeAreas = [],
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
		DateTimeOffset? startDateTime,
		DateTimeOffset? finishDateTime,
		DateTimeOffset? limitDateTime,
		DateTimeOffset? currentWorkSessionStartedAt,
		TimeSpan elapsedTime,
		List<Guid> parents,
		List<Guid> children,
		List<Guid> lifeAreas)
	{
		TodoTask task =  new() {
			Parents = parents, 
			Children = children, 
			LifeAreas = lifeAreas,  
			Id = id, 
			Name = name, 
			IsFun = isFun , 
			IsProductive = isProductive, 
			Complexity = complexity, 
			Priority = priority, 
			Status = status, 
			CreatedAt = createdAt, 
			StartDateTime = startDateTime, 
			FinishDateTime = finishDateTime, 
			LimitDateTime = limitDateTime, 
			CurrentWorkSessionStartedAt = currentWorkSessionStartedAt, 
			ElapsedTime = elapsedTime, 
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
				StartDateTime ??= dateTime;
				break;
			case TaskState.Discarded or TaskState.Finished:
				StartDateTime ??= dateTime;
				FinishDateTime ??= dateTime;
				Progress = 100;
				break;
		}

		Status = newStatus;
	}

	public void SetStartDateTime(DateTimeOffset dateTime)
	{
		StartDateTime = dateTime;
	}

	public void ResetStartDateTime()
	{
		StartDateTime = null;
	}

	public void SetFinishDateTime(DateTimeOffset dateTime)
	{
		FinishDateTime = dateTime;
	}

	public void ResetFinishDateTime()
	{
		FinishDateTime = null;
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
	}

	public void RemoveParent(Guid parentId)
	{
		if (!Parents.Contains(parentId)) return;
		
		Parents.Remove(parentId);
	}

	public void AddChild(Guid childId)
	{
		if (Children.Contains(childId)) return;
		
		if (Parents.Contains(childId)) return;
		
		Children.Add(childId);
	}

	public void RemoveChild(Guid childId)
	{
		if (!Children.Contains(childId)) return;
		
		Children.Remove(childId);
	}

	public void AddLifeArea(Guid lifeAreaId)
	{
		if (LifeAreas.Contains(lifeAreaId)) return;
		
		LifeAreas.Add(lifeAreaId);
	}

	public void RemoveLifeArea(Guid lifeAreaId)
	{
		if (!LifeAreas.Contains(lifeAreaId)) return;
		
		LifeAreas.Remove(lifeAreaId);
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
