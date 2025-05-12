using GraphT.Model.Aggregates;
using GraphT.Model.ValueObjects;
using GraphT.Model.ValueObjects.EnumLabel;

namespace GraphT.UseCases;

public class PlainTask
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public bool IsFun { get; set; }
	public bool IsProductive { get; set; }
	public Complexity Complexity { get; set; }
	public Priority Priority { get; set; }
	public Relevance Relevance { get; set; }
	public Status Status { get; set; }
	public float Progress { get; set; }
	public DateTimeInfo DateTimeInfo { get; set; }
	public string ComplexityLabel => this.Complexity.GetLabel();
	public string PriorityLabel => this.Priority.GetLabel();
	public string RelevanceLabel => this.Relevance.GetLabel();
	public string StatusLabel => this.Status.GetLabel();

	public static PlainTask MapFrom(TodoTask source)
	{
		return new PlainTask()
		{
			Id = source.Id,
			Name = source.Name,
			IsFun = source.IsFun,
			IsProductive = source.IsProductive,
			Complexity = source.Complexity,
			Priority = source.Priority,
			Relevance = source.Relevance,
			Status = source.Status,
			Progress = source.Progress,
			DateTimeInfo = source.DateTimeInfo
		};
	}
}
