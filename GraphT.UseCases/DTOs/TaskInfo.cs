using GraphT.Model.ValueObjects;

namespace GraphT.UseCases.DTOs;

public class TaskInfo
{
	public Guid? Id { get; set; }
	public string? Name { get; set; }
	public Status? Status { get; set; }
	public bool? IsFun { get; set; }
	public bool? IsProductive { get; set; }
	public Complexity? Complexity { get; set; }
	public Priority? Priority { get; set; }
	public DateTimeOffset? StartDateTime { get; set; }
	public DateTimeOffset? FinishDateTime { get; set; }
	public DateTimeOffset? LimitDateTime { get; set; }
}
