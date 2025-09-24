using GraphT.Model.Aggregates;
using GraphT.Model.Entities;

using SeedWork;

namespace GraphT.Model.Services.Repositories;

public interface ITodoTaskRepository
{
	ValueTask<TodoTask> FindByIdAsync(Guid id);
	ValueTask<PagedList<TodoTask>> FindTasksCompletedOrDropped(PagingParams pagingParams);
	ValueTask<PagedList<TodoTask>> FindTasksInProgress(PagingParams pagingParams);
	ValueTask<PagedList<TodoTask>> FindTasksReadyToStart(PagingParams pagingParams);
	ValueTask<PagedList<TodoTask>> GetTasksOrderedByCreationDateDescAsync(PagingParams pagingParams);
	ValueTask AddAsync(TodoItem task);
	ValueTask UpdateAsync(TodoItem task);
	ValueTask RemoveAsync(Guid id);
	ValueTask<bool> ContainsAsync(Guid id);
	ValueTask<PagedList<TodoItem>> FindParentsByIdAsync(Guid id);
	ValueTask<PagedList<TodoItem>> FindTasksWithoutParentsAsync(PagingParams pagingParams);
	ValueTask AddParentAsync(Guid taskId, Guid parentId);
	ValueTask RemoveParentAsync(Guid taskId, Guid parentId);
	ValueTask<PagedList<TodoItem>> FindChildrenById(Guid id);
	ValueTask AddChildAsync(Guid taskId, Guid downstreamId);
	ValueTask RemoveChildAsync(Guid taskId, Guid downstreamId);
}
