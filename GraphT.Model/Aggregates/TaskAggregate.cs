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

	protected TaskAggregate(string name, 
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
		_upstreams.Add(upstream);
	}

	public void RemoveUpstream(TodoTask upstream)
	{
		_upstreams.RemoveWhere(todoTask => todoTask.Id.Equals(upstream.Id));
	}
	
	public void AddUpstreams(HashSet<TodoTask> upstreams)
	{
		_upstreams.UnionWith(upstreams);
	}
	
	public void RemoveUpstreams(HashSet<TodoTask> upstreams)
	{
		_upstreams.ExceptWith(upstreams);
	}
	
	public void ReplaceUpstreams(HashSet<TodoTask> newUpstreams)
	{
		_upstreams.Clear();
		_upstreams = newUpstreams;
	}

	public void AddDownstream(TodoTask downstream)
	{
		_downstreams.Add(downstream);
	}

	public void AddDownstreams(HashSet<TodoTask> downstreams)
	{
		_downstreams.UnionWith(downstreams);
	}

	public void RemoveDownstream(TodoTask downstream)
	{
		_downstreams.RemoveWhere(todoTask => todoTask.Id.Equals(downstream.Id));
	}

	public void RemoveDownstreams(HashSet<TodoTask> downstreams)
	{
		_downstreams.ExceptWith(downstreams);
	}

	public void ReplaceDownstreams(HashSet<TodoTask> newDownstreams)
	{
		_downstreams.Clear();
		_downstreams = newDownstreams;
	}

	public void SetStartDate(DateTime startDate)
	{
		_timeInfo.StartDate = startDate;
	}

	public void SetFinishDate(DateTime finishDate)
	{
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
}
