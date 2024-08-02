using SeedWork;

namespace GraphT.UseCases;

public class TaskIdAndNamePagedListDto
{
	public PagedList<TaskIdAndName> Tasks { get; set; }

	public TaskIdAndNamePagedListDto(PagedList<TaskIdAndName> tasks)
	{
		Tasks = tasks;
	}
}
