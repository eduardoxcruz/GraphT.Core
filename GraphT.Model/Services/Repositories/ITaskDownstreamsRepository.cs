using GraphT.Model.Entities;

using SeedWork;

namespace GraphT.Model.Services.Repositories;

public interface ITaskDownstreamsRepository
{
	ValueTask<PagedList<TodoTask>> FindTaskDownstreamsById(Guid id);
}
