using GraphT.Model.Aggregates;

using SeedWork;

namespace GraphT.Model.Services.Repositories;

public interface ITodoTaskRepository
{
	ValueTask<TodoTask> FindByIdAsync(Guid id);
	ValueTask<PagedList<TodoTask>> FindTasksCompletedOrDropped(PagingParams pagingParams);
	ValueTask<PagedList<TodoTask>> FindTasksInProgress(PagingParams pagingParams);
	ValueTask<PagedList<TodoTask>> FindTasksReadyToStart(PagingParams pagingParams);
	ValueTask<PagedList<TodoTask>> GetTasksOrderedByCreationDateDescAsync(PagingParams pagingParams);
	ValueTask AddAsync(TodoTask task);
	ValueTask UpdateAsync(TodoTask task);
	ValueTask RemoveAsync(Guid id);
	ValueTask<bool> ContainsAsync(Guid id);
	ValueTask<PagedList<TodoTask>> FindParentsByIdAsync(Guid id);
	ValueTask<PagedList<TodoTask>> FindTasksWithoutParentsAsync(PagingParams pagingParams);
	ValueTask AddParentAsync(Guid taskId, Guid parentId);
	ValueTask RemoveParentAsync(Guid taskId, Guid parentId);
	ValueTask<PagedList<TodoTask>> FindChildrenById(Guid id);
	ValueTask AddChildAsync(Guid taskId, Guid downstreamId);
	ValueTask RemoveChildAsync(Guid taskId, Guid downstreamId);
}
