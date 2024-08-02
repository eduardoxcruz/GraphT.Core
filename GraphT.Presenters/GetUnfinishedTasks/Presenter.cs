using GraphT.UseCases;
using GraphT.UseCases.GetUnfinishedTasks;

using SeedWork;

namespace GraphT.Presenters.GetUnfinishedTasks;

public class Presenter : IPresenter<TaskIdAndNamePagedListDto>, IOutputPort
{
	public TaskIdAndNamePagedListDto Content { get; private set; }
	
	public ValueTask Handle(TaskIdAndNamePagedListDto dto)
	{
		Content = dto;
		return ValueTask.CompletedTask;
	}
}
