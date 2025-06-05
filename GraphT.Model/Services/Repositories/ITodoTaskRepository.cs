using GraphT.Model.Entities;
using GraphT.Model.ValueObjects;

using SeedWork;

namespace GraphT.Model.Services.Repositories;

public interface ITodoTaskRepository
{
	ValueTask<TodoTask?> FindByIdAsync(Guid id);
	ValueTask<PagedList<TodoTask>> FindTasksCompletedOrDropped(PagingParams pagingParams);
	ValueTask<PagedList<TodoTask>> FindTasksInProgress(PagingParams pagingParams);
	ValueTask<PagedList<TodoTask>> FindTasksReadyToStart(PagingParams pagingParams);
	ValueTask<PagedList<TodoTask>> GetTasksOrderedByCreationDateAsync(PagingParams pagingParams);
	ValueTask AddAsync(TodoTask task);
	ValueTask AddRangeAsync(IEnumerable<TodoTask> tasks);
	ValueTask RemoveAsync(TodoTask task);
	ValueTask RemoveRangeAsync(IEnumerable<TodoTask> tasks);
	ValueTask UpdateAsync(TodoTask task);
	ValueTask UpdateRangeAsync(IEnumerable<TodoTask> tasks);
	ValueTask<bool> ContainsAsync(Guid id);
}
