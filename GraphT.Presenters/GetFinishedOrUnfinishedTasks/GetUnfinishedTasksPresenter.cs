using GraphT.UseCases;
using GraphT.UseCases.GetFinishedAndUnfinishedTasks;

using SeedWork;

namespace GraphT.Presenters.GetFinishedOrUnfinishedTasks;

public class GetUnfinishedTasksPresenter : IPresenter<OnlyTodoTaskPagedListDto>, IGetUnfinishedTasksOutputPort
{
	public OnlyTodoTaskPagedListDto Content { get; private set; }
	
	public ValueTask Handle(OnlyTodoTaskPagedListDto dto)
	{
		Content = dto;
		return ValueTask.CompletedTask;
	}
}
