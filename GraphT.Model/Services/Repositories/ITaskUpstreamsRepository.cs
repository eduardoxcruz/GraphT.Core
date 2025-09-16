using GraphT.Model.Entities;

using SeedWork;

namespace GraphT.Model.Services.Repositories;

public interface ITaskUpstreamsRepository
{
	ValueTask<PagedList<OldTodoTask>> FindTaskUpstreamsById(Guid id);
	ValueTask<PagedList<OldTodoTask>> FindTasksWithoutUpstreams(PagingParams pagingParams);
	ValueTask AddUpstreamAsync(Guid taskId, Guid upstreamId);
	ValueTask RemoveUpstreamAsync(Guid taskId, Guid upstreamId);
}
