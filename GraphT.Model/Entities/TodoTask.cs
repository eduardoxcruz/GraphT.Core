using GraphT.Model.ValueObjects;
using GraphT.Model.ValueObjects.EnumLabel;

using SeedWork;

namespace GraphT.Model.Entities;

public class TodoTask : Entity
{
	private bool _isFun;
	private bool _isProductive;
	private DateTimeInfo _dateTimeInfo;
	private uint _upstreamsCount;
	private uint _downstreamsCount;
	private uint _lifeAreasCount;

	public bool IsFun
	{
		get => _isFun;
		set => SetProperty(ref _isFun, value);
	}
	public bool IsProductive
	{
		get => _isProductive;
		set => SetProperty(ref _isProductive, value);
	}
	public OldComplexity Complexity { get; set; }
	public Priority Priority { get; set; }
	public OldRelevance OldRelevance { get; private set; }
	public Status Status { get; set; }
	public float Progress { get; set; }
	public uint UpstreamsCount => _upstreamsCount;
	public uint DownstreamsCount => _downstreamsCount;
	public uint LifeAreasCount => _lifeAreasCount;
	public DateTimeInfo DateTimeInfo => _dateTimeInfo;
	public string ComplexityLabel => this.Complexity.GetLabel();
	public string PriorityLabel => this.Priority.GetLabel();
	public string RelevanceLabel => this.OldRelevance.GetLabel();
	public string StatusLabel => this.Status.GetLabel();

	private TodoTask() : base("New Task"){ }

	public TodoTask(string name, 
							Status? status = null,
							bool? isFun = null, 
							bool? isProductive = null,
							OldComplexity? complexity = null, 
							Priority? priority = null,
							Guid? id = null,
							DateTimeInfo? dateTimeInfo = null,
							uint? upstreamsCount = null,
							uint? downstreamsCount = null,
							uint? lifeAreasCount = null) : base(name, id)
	{
		Status = status ?? Status.Backlog;
		IsFun = isFun ?? false;
		IsProductive = isProductive ?? false;
		Complexity = complexity ?? OldComplexity.Indefinite;
		Priority = priority ?? Priority.Distraction;
		Progress = 0;
		_dateTimeInfo = dateTimeInfo ?? new DateTimeInfo();
		_upstreamsCount = upstreamsCount ?? 0;
		_downstreamsCount = downstreamsCount ?? 0;
		_lifeAreasCount = lifeAreasCount ?? 0;
		UpdateRelevance();
	}

	private void SetProperty(ref bool field, bool value)
	{
		if (field == value) return;
        
		field = value;
		UpdateRelevance();
	}

	private void UpdateRelevance()
	{
		OldRelevance = (IsFun, IsProductive) switch
		{
			(true, true) => OldRelevance.Purposeful,
			(false, true) => OldRelevance.Necessary,
			(true, false) => OldRelevance.Entertaining,
			(false, false) => OldRelevance.Superfluous
		};
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
}
