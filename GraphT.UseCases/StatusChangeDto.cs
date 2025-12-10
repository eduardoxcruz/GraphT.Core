using GraphT.Model.ValueObjects;

namespace GraphT.UseCases;

public record struct StatusChangeDto
{
	public DateTimeOffset? ChangeDateTime { get; set; }
	public Status Status { get; set; }
}
