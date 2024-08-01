using GraphT.UseCases;
using GraphT.UseCases.GetUnfinishedTasks;

using SeedWork;

namespace GraphT.Presenters.GetUnfinishedTasks;

public class Presenter : IPresenter<OnlyTodoTaskPagedListDto>, IOutputPort
{
	public OnlyTodoTaskPagedListDto Content { get; private set; }
	
	public ValueTask Handle(OnlyTodoTaskPagedListDto dto)
	{
		Content = dto;
		return ValueTask.CompletedTask;
	}
}
