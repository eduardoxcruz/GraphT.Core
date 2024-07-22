using FluentValidation;

using GraphT.Model.Entities;
using GraphT.Model.ValueObjects;

namespace GraphT.Model.Aggregates;

public class TaskAggregate : TodoTask
{
	private bool _isFun;
	private bool _isProductive;
	private Priority _priority;
	private Complexity _complexity;
	private Relevance _relevance;
	private Status _status;
	private TimeInfo _timeInfo;
	private HashSet<TodoTask> _upstreams = null!;
	private HashSet<TodoTask> _downstreams = null!;
	public bool IsFun
	{
		get => _isFun;
		set
		{
			_isFun = value;
			UpdateRelevance();
		}
	}
	public bool IsProductive
	{
		get => _isProductive;
		set
		{
			_isProductive = value;
			UpdateRelevance();
		}
	}
	public float Progress { get; set; }
	public string Complexity => _complexity.FormatedName();
	public string Priority => _priority.FormatedName();
	public string Relevance => _relevance.FormatedName();
	public string Status => _status.FormatedName();
	public TimeInfo TimeInfo => _timeInfo;
	public IReadOnlySet<TodoTask> Upstreams => _upstreams;
	public IReadOnlySet<TodoTask> Downstreams => _downstreams;

	private TaskAggregate() : base(String.Empty) { }

	public TaskAggregate(string name, 
							bool isFun, 
							bool isProductive,
							Complexity complexity = ValueObjects.Complexity.Indefinite, 
							Priority priority = ValueObjects.Priority.MentalClutter, 
							Status status = ValueObjects.Status.Backlog) : base(name)
	{
		_isFun = isFun;
		_isProductive = isProductive;
		_timeInfo = new TimeInfo();
		_upstreams = [];
		_downstreams = [];
		_complexity = complexity;
		_priority = priority;
		_status = status;
		UpdateRelevance();
	}

	private void UpdateRelevance()
	{
		this._relevance = IsFun switch
		{
			true when IsProductive => ValueObjects.Relevance.Purposeful,
			false when IsProductive => ValueObjects.Relevance.Necessary,
			true when !IsProductive => ValueObjects.Relevance.Entertaining,
			_ => ValueObjects.Relevance.Superfluous
		};
	}
	
	public void AddUpstream(TodoTask upstream)
	{
		ValidateTodoTask(upstream);
		
		_upstreams.Add(upstream);
	}

	public void RemoveUpstream(TodoTask upstream)
	{
		ValidateTodoTask(upstream);
		
		_upstreams.RemoveWhere(todoTask => todoTask.Id.Equals(upstream.Id));
	}

	public void AddUpstreams(HashSet<TodoTask> upstreams)
	{
		ValidateTaskCollection(upstreams);
		
		_upstreams.UnionWith(upstreams);
	}

	public void RemoveUpstreams(HashSet<TodoTask> upstreams)
	{
		ValidateTaskCollection(upstreams);
		
		_upstreams.ExceptWith(upstreams);
	}

	public void ReplaceUpstreams(HashSet<TodoTask> newUpstreams)
	{
		ValidateTaskCollection(newUpstreams);
		
		_upstreams.Clear();
		_upstreams = newUpstreams;
	}

	public void ClearUpstreams()
	{
		if (_upstreams.Count == 1) return;
		
		_upstreams.Clear();
	}

	public void AddDownstream(TodoTask downstream)
	{
		ValidateTodoTask(downstream);
		
		_downstreams.Add(downstream);
	}
	
	public void RemoveDownstream(TodoTask downstream)
	{
		ValidateTodoTask(downstream);
		
		_downstreams.RemoveWhere(todoTask => todoTask.Id.Equals(downstream.Id));
	}

	public void AddDownstreams(HashSet<TodoTask> downstreams)
	{
		ValidateTaskCollection(downstreams);
		
		_downstreams.UnionWith(downstreams);
	}

	public void RemoveDownstreams(HashSet<TodoTask> downstreams)
	{
		ValidateTaskCollection(downstreams);

		_downstreams.ExceptWith(downstreams);
	}

	public void ReplaceDownstreams(HashSet<TodoTask> newDownstreams)
	{
		ValidateTaskCollection(newDownstreams);
		
		_downstreams.Clear();
		_downstreams = newDownstreams;
	}

	public void ClearDownstreams()
	{
		if (_downstreams.Count == 0) return;
		
		_downstreams.Clear();
	}
	
	private void ValidateTodoTask(TodoTask todoTask)
	{
		if (todoTask.Id.Equals(Guid.Empty)) throw new ArgumentException("Task id cannot be empty");
	}

	private void ValidateTaskCollection(HashSet<TodoTask> taskCollection)
	{
		if (taskCollection is null) throw new ArgumentException("Task collection cannot be null");

		if (taskCollection.Count == 0) throw new ArgumentException("Task collection cannot be empty");
	}

	public void SetStartDate(DateTime startDate)
	{
		if (_timeInfo.FinishDate is not null && startDate > _timeInfo.FinishDate)
		{
			throw new ArgumentException("Start date cannot be after of finish date");
		}
		
		_timeInfo.StartDate = startDate;
	}

	public void SetFinishDate(DateTime finishDate)
	{
		if (_timeInfo.StartDate is not null && finishDate < _timeInfo.StartDate)
		{
			throw new ArgumentException("Finish date cannot be before start date");
		}

		_timeInfo.FinishDate = finishDate;
	}

	public void SetLimitDate(DateTime limitDate)
	{
		_timeInfo.LimitDate = limitDate;
	}

	public void SetPriority(ValueObjects.Priority priority)
	{
		_priority = priority;
	}

	public void SetComplexity(Complexity complexity)
	{
		_complexity = complexity;
	}

	public void SetStatus(Status status)
	{
		_status = status;
	}
}
