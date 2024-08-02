using GraphT.Model.Aggregates;

using SeedWork;

namespace GraphT.UseCases;

public class OnlyTodoTaskPagedListDto(PagedList<TaskAggregate> todoTasks)
{
	public readonly PagedList<TaskAggregate> TodoTasks = todoTasks;
}
