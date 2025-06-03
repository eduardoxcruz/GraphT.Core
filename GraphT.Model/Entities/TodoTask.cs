using GraphT.Model.ValueObjects;
using GraphT.Model.ValueObjects.EnumLabel;

using SeedWork;

namespace GraphT.Model.Entities;

public class TodoTask : Entity
{
	private bool _isFun;
	private bool _isProductive;
	private DateTimeInfo _dateTimeInfo;

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
	public Relevance Relevance { get; private set; }
	public Status Status { get; set; }
	public float Progress { get; set; }
	public DateTimeInfo DateTimeInfo => _dateTimeInfo;
	public string ComplexityLabel => this.Complexity.GetLabel();
	public string PriorityLabel => this.Priority.GetLabel();
	public string RelevanceLabel => this.Relevance.GetLabel();
	public string StatusLabel => this.Status.GetLabel();

	private TodoTask() : base("New Task"){ }

	public TodoTask(string name, 
							Status? status = null,
							bool? isFun = null, 
							bool? isProductive = null,
							Complexity? complexity = null, 
							Priority? priority = null,
							Guid? id = null,
							DateTimeInfo? dateTimeInfo = null ) : base(name, id)
	{
		Status = status ?? Status.Backlog;
		IsFun = isFun ?? false;
		IsProductive = isProductive ?? false;
		Complexity = complexity ?? Complexity.Indefinite;
		Priority = priority ?? Priority.Distraction;
		Progress = 0;
		_dateTimeInfo = dateTimeInfo ?? new DateTimeInfo();
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
