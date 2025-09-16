using GraphT.Model.Entities;

using SeedWork;

namespace GraphT.Model.Services.Repositories;

public interface ITaskDownstreamsRepository
{
	ValueTask<PagedList<OldTodoTask>> FindTaskDownstreamsById(Guid id);
	ValueTask AddDownstreamAsync(Guid taskId, Guid downstreamId);
	ValueTask RemoveDownstreamAsync(Guid taskId, Guid downstreamId);
}
