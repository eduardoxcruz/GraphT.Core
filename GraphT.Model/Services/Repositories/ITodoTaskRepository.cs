using GraphT.Model.Entities;
using GraphT.Model.ValueObjects;

using SeedWork;

namespace GraphT.Model.Services.Repositories;

public interface ITodoTaskRepository
{
	ValueTask<OldTodoTask?> FindByIdAsync(Guid id);
	ValueTask<PagedList<OldTodoTask>> FindTasksCompletedOrDropped(PagingParams pagingParams);
	ValueTask<PagedList<OldTodoTask>> FindTasksInProgress(PagingParams pagingParams);
	ValueTask<PagedList<OldTodoTask>> FindTasksReadyToStart(PagingParams pagingParams);
	ValueTask<PagedList<OldTodoTask>> GetTasksOrderedByCreationDateDescAsync(PagingParams pagingParams);
	ValueTask AddAsync(OldTodoTask task);
	ValueTask AddRangeAsync(IEnumerable<OldTodoTask> tasks);
	ValueTask RemoveAsync(OldTodoTask task);
	ValueTask RemoveRangeAsync(IEnumerable<OldTodoTask> tasks);
	ValueTask UpdateAsync(OldTodoTask task);
	ValueTask UpdateRangeAsync(IEnumerable<OldTodoTask> tasks);
	ValueTask<bool> ContainsAsync(Guid id);
}
