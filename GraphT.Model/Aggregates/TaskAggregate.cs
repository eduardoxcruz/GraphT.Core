using GraphT.Model.ValueObjects;

namespace GraphT.Model.Aggregates;

public class TaskAggregate
{
	public Guid Id { get; private set; }
	public string Name { get; set; }
	public Status Status { get; set; }
	private bool _isFun;
	private bool _isProductive;
	private Relevance _relevance;
	private DateTimeInfo _dateTimeInfo;
	private HashSet<TaskAggregate> _upstreams = null!;
	private HashSet<TaskAggregate> _downstreams = null!;
	private HashSet<LifeAreaAggregate> _lifeAreas = null!;
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
	public Complexity Complexity { get; set; }
	public Priority Priority { get; set; }
	public float Progress => GetProgress();
	public Relevance Relevance => _relevance;
	public DateTimeInfo DateTimeInfo => _dateTimeInfo;
	public IReadOnlySet<TaskAggregate> Upstreams => _upstreams;
	public IReadOnlySet<TaskAggregate> Downstreams => _downstreams;
	public IReadOnlySet<LifeAreaAggregate> LifeAreas => _lifeAreas;

	private TaskAggregate(){ }

	public TaskAggregate(string name, 
							Status status = Status.Backlog,
							bool isFun = false, 
							bool isProductive = false,
							Complexity complexity = Complexity.Indefinite, 
							Priority priority = Priority.MentalClutter
							)
	{
		Id = Guid.NewGuid();
		Name = name;
		Status = status;
		_isFun = isFun;
		_isProductive = isProductive;
		_dateTimeInfo = new DateTimeInfo();
		_upstreams = [];
		_downstreams = [];
		_lifeAreas = [];
		Complexity = complexity;
		Priority = priority;
		UpdateRelevance();
	}

	private void UpdateRelevance()
	{
		this._relevance = IsFun switch
		{
			true when IsProductive => Relevance.Purposeful,
			false when IsProductive => Relevance.Necessary,
			true when !IsProductive => Relevance.Entertaining,
			_ => Relevance.Superfluous
		};
	}
	
	public void AddUpstream(TaskAggregate upstream)
	{
		ValidateTaskAggregate(upstream);
		
		_upstreams.Add(upstream);
	}

	public void RemoveUpstream(TaskAggregate upstream)
	{
		ValidateTaskAggregate(upstream);
		
		_upstreams.RemoveWhere(TaskAggregate => TaskAggregate.Id.Equals(upstream.Id));
	}

	public void AddUpstreams(HashSet<TaskAggregate> upstreams)
	{
		ValidateTaskCollection(upstreams);
		
		_upstreams.UnionWith(upstreams);
	}

	public void RemoveUpstreams(HashSet<TaskAggregate> upstreams)
	{
		ValidateTaskCollection(upstreams);
		
		_upstreams.ExceptWith(upstreams);
	}

	public void ReplaceUpstreams(HashSet<TaskAggregate> newUpstreams)
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

	public void AddDownstream(TaskAggregate downstream)
	{
		ValidateTaskAggregate(downstream);
		
		_downstreams.Add(downstream);
	}
	
	public void RemoveDownstream(TaskAggregate downstream)
	{
		ValidateTaskAggregate(downstream);
		
		_downstreams.RemoveWhere(TaskAggregate => TaskAggregate.Id.Equals(downstream.Id));
	}

	public void AddDownstreams(HashSet<TaskAggregate> downstreams)
	{
		ValidateTaskCollection(downstreams);
		
		_downstreams.UnionWith(downstreams);
	}

	public void RemoveDownstreams(HashSet<TaskAggregate> downstreams)
	{
		ValidateTaskCollection(downstreams);

		_downstreams.ExceptWith(downstreams);
	}

	public void ReplaceDownstreams(HashSet<TaskAggregate> newDownstreams)
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
	
	private void ValidateTaskAggregate(TaskAggregate taskAggregate)
	{
		if (taskAggregate.Id.Equals(Guid.Empty)) throw new ArgumentException("Task id cannot be empty");
	}

	private void ValidateTaskCollection(HashSet<TaskAggregate> taskCollection)
	{
		if (taskCollection is null) throw new ArgumentException("Task collection cannot be null");

		if (taskCollection.Count == 0) throw new ArgumentException("Task collection cannot be empty");

		if (taskCollection.Any(task => task.Id.Equals(Guid.Empty)))
			throw new ArgumentException("Task collection cannot contain tasks with empty Id");
	}
	
	public void AddLifeArea(LifeAreaAggregate lifeArea)
	{
		ValidateLifeArea(lifeArea);
		
		_lifeAreas.Add(lifeArea);
	}
	
	public void RemoveLifeArea(LifeAreaAggregate lifeArea)
	{
		ValidateLifeArea(lifeArea);
		
		_lifeAreas.RemoveWhere(lifeAreaAggregate => lifeAreaAggregate.Id.Equals(lifeArea.Id));
	}

	public void AddLifeAreas(HashSet<LifeAreaAggregate> lifeAreas)
	{
		ValidateLifeAreaCollection(lifeAreas);
		
		_lifeAreas.UnionWith(lifeAreas);
	}

	public void RemoveLifeAreas(HashSet<LifeAreaAggregate> lifeAreas)
	{
		ValidateLifeAreaCollection(lifeAreas);

		_lifeAreas.ExceptWith(lifeAreas);
	}

	public void ReplaceLifeAreas(HashSet<LifeAreaAggregate> newLifeAreas)
	{
		ValidateLifeAreaCollection(newLifeAreas);
		
		_lifeAreas.Clear();
		_lifeAreas = newLifeAreas;
	}

	public void ClearLifeAreas()
	{
		if (_lifeAreas.Count == 0) return;
		
		_lifeAreas.Clear();
	}
	
	private void ValidateLifeArea(LifeAreaAggregate lifeArea)
	{
		if (lifeArea.Id.Equals(Guid.Empty)) throw new ArgumentException("Life Area id cannot be empty");
	}

	private void ValidateLifeAreaCollection(HashSet<LifeAreaAggregate> lifeAreaCollection)
	{
		if (lifeAreaCollection is null) throw new ArgumentException("Life Area collection cannot be null");

		if (lifeAreaCollection.Count == 0) throw new ArgumentException("Life Area collection cannot be empty");

		if (lifeAreaCollection.Any(lifeAreaAggregate => lifeAreaAggregate.Id.Equals(Guid.Empty)))
			throw new ArgumentException("Life Area collection cannot contain life areas with empty Id");
	}
	
	public void SetStartDate(DateTimeOffset startDate)
	{
		if (_dateTimeInfo.FinishDateTime is not null && startDate > _dateTimeInfo.FinishDateTime)
		{
			throw new ArgumentException("Start date cannot be after of finish date");
		}
		
		_dateTimeInfo.StartDateTime = startDate;
	}

	public void SetFinishDate(DateTimeOffset finishDate)
	{
		if (_dateTimeInfo.StartDateTime is not null && finishDate < _dateTimeInfo.StartDateTime)
		{
			throw new ArgumentException("Finish date cannot be before start date");
		}

		_dateTimeInfo.FinishDateTime = finishDate;
	}

	public void SetLimitDate(DateTimeOffset limitDate)
	{
		_dateTimeInfo.LimitDateTime = limitDate;
	}
	
	public void SetTimeSpend(string timeSpend)
	{
		_dateTimeInfo.TimeSpend = timeSpend;
	}
	
	private float GetProgress()
	{
		int totalDownstreams = _downstreams.Count;
		int backlogTasks = _downstreams.Count(task => task.Status is Status.Backlog);
		int completedOrDroppedTasks = _downstreams.Count(task => task.Status is Status.Dropped or Status.Completed);
		const int currentTask = 1;
		const float isFinished = 100f;
		const float isUnfinished = 0f;

		switch (totalDownstreams)
		{
			case 0 when (Status is not Status.Completed):
				return isUnfinished;
			case 0 when (Status is Status.Completed):
				return isFinished;
		}

		if (completedOrDroppedTasks >= totalDownstreams) return isFinished;
		
		if (backlogTasks == totalDownstreams) return isUnfinished;

		return (completedOrDroppedTasks * isFinished) / (totalDownstreams + currentTask);
	}
}
