using GraphT.Model.Aggregates;

namespace GraphT.UseCases;

public class TaskIdAndName
{
	public Guid Id { get; set; }
	public string Name { get; set; }

	public static TaskIdAndName MapFrom(TodoTask source)
	{
		return new TaskIdAndName { Id = source.Id, Name = source.Name };
	}
}
