using GraphT.UseCases;
using GraphT.UseCases.GetFinishedTasks;

using SeedWork;

namespace GraphT.Presenters.GetFinishedTasks;

public class Presenter : IPresenter<TaskIdAndNamePagedListDto>, IOutputPort
{
	public TaskIdAndNamePagedListDto Content { get; private set; }
	
	public ValueTask Handle(TaskIdAndNamePagedListDto dto)
	{
		Content = dto;
		return ValueTask.CompletedTask;
	}
}
