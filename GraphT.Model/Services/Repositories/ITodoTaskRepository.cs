using GraphT.Model.Aggregates;
using GraphT.Model.ValueObjects;

namespace GraphT.Model.Services.Repositories;

public interface ITodoTaskRepository
{
	ValueTask<PagedList<TodoTask>> FindByStatusAsync(PagingParams pagingParams, Status status);
	ValueTask<PagedList<TodoTask>> FindTasksWithoutParentsAsync(PagingParams pagingParams);
	ValueTask<PagedList<TodoTask>> GetTasksOrderedByCreationDateDescAsync(PagingParams pagingParams);
	ValueTask<TodoTask> FindByIdAsync(Guid id);
	ValueTask AddAsync(TodoTask task);
	ValueTask UpdateAsync(TodoTask task);
	ValueTask RemoveAsync(Guid id);
	ValueTask<bool> ContainsAsync(Guid id);
}
