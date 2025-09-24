using GraphT.Model.Entities;
using GraphT.Model.ValueObjects;

namespace GraphT.Model.Aggregates;

public class TodoTask
{
	private HashSet<TodoItem> _parents;
	private HashSet<TodoItem> _children;
	
	public TodoItem Item { get; }
	public TimeSpan ElapsedTime => GetElapsedTime();
	public string ElapsedTimeFormatted => ElapsedTime.ToElapsedTime();
	public double Progress => GetProgress();
	public IReadOnlySet<TodoItem> Parents => _parents;
	public IReadOnlySet<TodoItem> Children => _children;
	
	private TodoTask() {}
	
	public TodoTask(TodoItem item)
	{
		Item = item;
		_parents = [];
		_children = [];
	}
	
	public void SetParents(IEnumerable<TodoItem> parents)
	{
		_parents = parents.ToHashSet();
	}
	
	public void SetChildren(IEnumerable<TodoItem> children)
	{
		_children = children.ToHashSet();
	}

	private TimeSpan GetElapsedTime()
	{
		TimeSpan elapsedTime = TimeSpan.Zero;
		LinkedListNode<StatusChangelog> log = Item.StatusChangeLogs.First!;

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
