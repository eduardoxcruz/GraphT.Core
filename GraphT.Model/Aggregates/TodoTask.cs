using GraphT.Model.ValueObjects;

namespace GraphT.Model.Aggregates;

public class TodoTask
{
	private HashSet<TodoTask> _parents;
	private HashSet<TodoTask> _children;
	
	public Guid Id { get; }
	public string Name { get; set; }
	public bool IsFun { get; set; }
	public bool IsProductive { get; set; }
	public Relevance Relevance => new(IsFun, IsProductive);
	public Complexity Complexity { get; set; }
	public Priority Priority { get; set; }
	public Status Status { get; private set; }
	public DateTimeOffset? LimitDateTime { get; private set; }
	public string Punctuality => GetPunctuality();
	public LinkedList<StatusChangelog> StatusChangeLogs { get; private set; }
	public TimeSpan ElapsedTime => GetElapsedTime();
	public string ElapsedTimeFormatted => ElapsedTime.ToElapsedTime();
	public double Progress => GetProgress();
	public IReadOnlySet<TodoTask> Parents => _parents;
	public IReadOnlySet<TodoTask> Children => _children;
	
	public TodoTask() : this("New Todo Task") {}
	
	public TodoTask(string name)
	{
		if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty");
		
		Id = Guid.NewGuid();
		Name = name;
		Complexity = Complexity.Indefinite;
		Priority = Priority.Distraction;
		Status = Status.Created;
		StatusChangeLogs = new LinkedList<StatusChangelog>();
		StatusChangeLogs.AddFirst(new StatusChangelog(DateTimeOffset.Now, Status.Created));
		_parents = [];
		_children = [];
	}
	
	public void SetStatus(Status status)
	{
		SetStatus(DateTimeOffset.Now, status);
	}
	
	public void SetStatus(DateTimeOffset dateTime, Status status)
	{
		StatusChangeLogs.AddLast(new StatusChangelog(dateTime, status));
		
		Status = status;
	}
	
	public void SetLimitDateTime(DateTimeOffset dateTime)
	{
		LimitDateTime = dateTime;
	}
	
	private string GetPunctuality()
	{
		if (LimitDateTime is null) return "\u26a0 No Target";

		StatusChangelog lastLog = StatusChangeLogs.Last!.Value;

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
	
	public void SetParents(IEnumerable<TodoTask> parents)
	{
		_parents = parents.ToHashSet();
	}
	
	public void SetChildren(IEnumerable<TodoTask> children)
	{
		_children = children.ToHashSet();
	}

	private TimeSpan GetElapsedTime()
	{
		TimeSpan elapsedTime = TimeSpan.Zero;
		LinkedListNode<StatusChangelog> log = StatusChangeLogs.First!;

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
		int completedChildren = _children.Count(t => Equals(t.Status, Status.Completed));
		return (completedChildren * 100) / totalChildren;
	}
}
