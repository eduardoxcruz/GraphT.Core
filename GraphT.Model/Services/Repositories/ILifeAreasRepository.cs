using GraphT.Model.Entities;

using SeedWork;

namespace GraphT.Model.Services.Repositories;

public interface ILifeAreasRepository
{
	ValueTask<PagedList<LifeArea>> FindTaskLifeAreasById(Guid id);
	ValueTask AddTaskAsync(Guid lifeAreaId, Guid taskId);
	ValueTask RemoveTaskAsync(Guid lifeAreaId, Guid taskId);
}
