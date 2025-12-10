using GraphT.Model.ValueObjects;

namespace GraphT.Model.Aggregates;

public class TodoTask : IEquatable<TodoTask>
{
	private List<TodoTask> _parents;
	private List<TodoTask> _children;
	private List<LifeArea> _lifeAreas;
	private LinkedList<StatusChangelog> _statusChangeLogs;
	
	public Guid Id { get; }
	public string Name { get; set; }
	public bool IsFun { get; set; }
	public bool IsProductive { get; set; }
	public Complexity Complexity { get; set; }
	public Priority Priority { get; set; }
	
	public Status Status { get; private set; }
	public DateTimeOffset? LimitDateTime { get; private set; }
	
	public Relevance Relevance => new(IsFun, IsProductive);
	public string Punctuality => GetPunctuality();
	public double Progress => GetProgress();
	public TimeSpan ElapsedTime => GetElapsedTime();
	public string ElapsedTimeFormatted => ElapsedTime.ToElapsedTime();

	public IReadOnlyCollection<StatusChangelog> StatusChangeLogs => _statusChangeLogs;
	public IReadOnlyList<TodoTask> Parents => _parents;
	public IReadOnlyList<TodoTask> Children => _children;
	public IReadOnlyList<LifeArea> LifeAreas => _lifeAreas;
	
	public TodoTask() : this("New Todo Task") {}
	
	public TodoTask(string name)
	{
		if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty");
		
		Id = Guid.NewGuid();
		Name = name;
		Complexity = Complexity.Indefinite;
		Priority = Priority.Distraction;
		Status = Status.Created;
		_statusChangeLogs = [];
		_statusChangeLogs.AddFirst(new StatusChangelog(DateTimeOffset.Now, Status.Created));
		_parents = [];
		_children = [];
		_lifeAreas = [];
	}

	public TodoTask(string name, 
		Status? status = null,
		DateTimeOffset? limitDateTime = null,
		LinkedList<StatusChangelog>? statusChangeLogs = null, 
		List<TodoTask>? parents = null, 
		List<TodoTask>? children = null, 
		List<LifeArea>? lifeAreas = null)
	{
		if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty");
		
		Id = Guid.NewGuid();
		Name = name;
		Complexity = Complexity.Indefinite;
		Priority = Priority.Distraction;
		Status = status ?? Status.Created;
		LimitDateTime = limitDateTime;
		_parents = parents ?? [];
		_children = children ?? [];
		_lifeAreas = lifeAreas ?? [];

		switch (statusChangeLogs is null)
		{
			case true:
				_statusChangeLogs = [];
				_statusChangeLogs.AddFirst(new StatusChangelog(DateTimeOffset.Now, Status.Created));
				break;
			case false:
				_statusChangeLogs = statusChangeLogs;
				break;
		}
	}
	
	public void SetStatus(Status newStatus)
	{
		SetStatus(DateTimeOffset.Now, newStatus);
	}
	
	public void SetStatus(DateTimeOffset dateTime, Status newStatus)
	{
		if (newStatus == this.Status) return;
		
		_statusChangeLogs.AddLast(new StatusChangelog(dateTime, newStatus));
		
		Status = newStatus;
	}
	
	public void SetLimitDateTime(DateTimeOffset dateTime)
	{
		LimitDateTime = dateTime;
	}
	
	private string GetPunctuality()
	{
		if (LimitDateTime is null) return "\u26a0 No Target";

		StatusChangelog lastLog = _statusChangeLogs.Last!.Value;

		bool lastLogIsCompletedOrDropped = Equals(lastLog.Status, Status.Completed) || Equals(lastLog.Status, Status.Dropped);

		TimeSpan timeDifference;
		
		if (lastLogIsCompletedOrDropped)
		{
			timeDifference = lastLog.ChangeDateTime - LimitDateTime.Value;

			switch (timeDifference.TotalMilliseconds)
			{
				case 0: return "\u2705 On Time!";
				case > 0:
					{
						if (timeDifference.TotalSeconds < 1) return "\u2705 On Time!";
						
						return $"\ud83d\udea8 Late {Math.Abs(timeDifference.Days)} day(s) - {Math.Abs(timeDifference.Hours)} hours(s) - {Math.Abs(timeDifference.Minutes)} minute(s) - {Math.Abs(timeDifference.Seconds)} second(s)!";
					}
				case < 0: return timeDifference.TotalMilliseconds < -86_400_000 ? $"\u2b50 Early {Math.Abs(timeDifference.Days)} day(s) - {Math.Abs(timeDifference.Hours)} hours(s) - {Math.Abs(timeDifference.Minutes)} minute(s) - {Math.Abs(timeDifference.Seconds)} second(s)!" : "\u2705 On Time!";
			}
		}
		
		DateTimeOffset now = DateTimeOffset.Now;
		timeDifference = (LimitDateTime.Value - now);
		
		if (timeDifference.Seconds > 0) timeDifference = timeDifference.Add(TimeSpan.FromMilliseconds(10));
		
		if (timeDifference.TotalMilliseconds < 86_400_010)
		{
			if (timeDifference.TotalMilliseconds >= -10) return "\u26a0 Finish Today!";
			
			return $"\ud83d\udea8 Late {Math.Abs(timeDifference.Days)} day(s) - {Math.Abs(timeDifference.Hours)} hours(s) - {Math.Abs(timeDifference.Minutes)} minute(s) - {Math.Abs(timeDifference.Seconds)} second(s)!";
		}
		
		return $"\u23f1 {Math.Abs(timeDifference.Days)} day(s) - {Math.Abs(timeDifference.Hours)} hours(s) - {Math.Abs(timeDifference.Minutes)} minute(s) - {Math.Abs(timeDifference.Add(TimeSpan.FromMilliseconds(10)).Seconds)} second(s) To Go!";
	}
	
	public void AddParents(List<TodoTask> parents)
	{
		foreach (TodoTask parent in parents)
		{
			if (parent is null) throw new ArgumentNullException(nameof(parents), "Parent cannot be null");
			
			if (!_parents.Contains(parent)) _parents.Add(parent);
		}
	}
	
	public void AddChildren(List<TodoTask> children)
	{
		foreach (TodoTask child in children)
		{
			if (child is null) throw new ArgumentNullException(nameof(children), "Child cannot be null");
			
			if (!_children.Contains(child)) _children.Add(child);
		}
	}

	private TimeSpan GetElapsedTime()
	{
		TimeSpan elapsedTime = TimeSpan.Zero;
		LinkedListNode<StatusChangelog> log = _statusChangeLogs.First!;

		while (log is not null)
		{
			if (log.Previous is null || !Equals(log.Previous.Value.Status, Status.CurrentlyDoing))
			{
				log = log.Next;
				continue;
			}

			TimeSpan temporal = log.Value.ChangeDateTime - log.Previous.Value.ChangeDateTime;
			elapsedTime += temporal;
			log = log.Next;
		}
		
		return elapsedTime;
	}

	private double GetProgress()
	{
		int totalChildren = _children.Count;
		
		if (Status.Index > 4) return 100;

		if (totalChildren == 0) return 0;
		
		int completedChildren = _children.Count(t => t.Status.Index > 4);
		
		if (completedChildren == _children.Count) return 99;
		
		return (completedChildren * 100) / totalChildren;
	}

	public void AddLifeAreas(List<LifeArea> lifeAreas)
	{
		foreach (LifeArea lifeArea in lifeAreas)
		{
			if (!_lifeAreas.Contains(lifeArea)) _lifeAreas.Add(lifeArea);
		}
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
}
