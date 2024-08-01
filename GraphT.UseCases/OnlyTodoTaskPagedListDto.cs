using GraphT.Model.Entities;

using SeedWork;

namespace GraphT.UseCases;

public class OnlyTodoTaskPagedListDto(PagedList<TodoTask> todoTasks)
{
	public readonly PagedList<TodoTask> TodoTasks = todoTasks;
}
