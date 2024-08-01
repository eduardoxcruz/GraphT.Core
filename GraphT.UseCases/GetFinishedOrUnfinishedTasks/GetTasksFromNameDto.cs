using SeedWork;

namespace GraphT.UseCases.GetFinishedAndUnfinishedTasks;

public class GetTasksFromNameDto
{
	public PagingParams PagingParams { get; set; }
	public string? TaskName { get; set; }

	public GetTasksFromNameDto()
	{
		PagingParams = new PagingParams();
	}
}
