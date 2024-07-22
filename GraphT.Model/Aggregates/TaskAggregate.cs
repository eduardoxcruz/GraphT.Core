using GraphT.Model.Entities;
using GraphT.Model.ValueObjects;

namespace GraphT.Model.Aggregates;

public class TaskAggregate : TodoTask
{
	private bool _isFun;
	private bool _isProductive;
	private TimeInfo _timeInfo;
	private readonly HashSet<TodoTask> _upstreams = null!;
	private readonly HashSet<TodoTask> _downstreams = null!;
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
	public Complexity Complexity { get; set; }
	public Priority Priority { get; set; }
	public Status Status { get; set; }
	public Relevance Relevance { get; private set; }
	public TimeInfo TimeInfo => _timeInfo;
	public IReadOnlySet<TodoTask> Upstreams => _upstreams;
	public IReadOnlySet<TodoTask> Downstreams => _downstreams;

	private TaskAggregate() : base(String.Empty) { }

	protected TaskAggregate(string name, 
							bool isFun, 
							bool isProductive, 
							Complexity complexity = Complexity.Indefinite, 
							Priority priority = Priority.MentalClutter, 
							Status status = Status.Backlog) : base(name)
	{
		_isFun = isFun;
		_isProductive = isProductive;
		_timeInfo = new TimeInfo();
		_upstreams = [];
		_downstreams = [];
		Complexity = complexity;
		Priority = priority;
		Status = status;
		UpdateRelevance();
	}

	private void UpdateRelevance()
	{
		this.Relevance = IsFun switch
		{
			true when IsProductive => Relevance.Purposeful,
			false when IsProductive => Relevance.Necessary,
			true when !IsProductive => Relevance.Entertaining,
			_ => Relevance.Superfluous
		};
	}
}
