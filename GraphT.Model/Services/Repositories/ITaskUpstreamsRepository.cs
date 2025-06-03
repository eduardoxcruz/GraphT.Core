using GraphT.Model.Entities;

using SeedWork;

namespace GraphT.Model.Services.Repositories;

public interface ITaskUpstreamsRepository
{
	ValueTask<PagedList<TodoTask>> FindTaskUpstreamsById(Guid id);
	ValueTask<PagedList<TodoTask>> FindTasksWithoutUpstreams(PagingParams pagingParams);
}
