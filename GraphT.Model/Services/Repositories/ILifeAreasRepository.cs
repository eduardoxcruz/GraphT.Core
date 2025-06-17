using GraphT.Model.Entities;

using SeedWork;

namespace GraphT.Model.Services.Repositories;

public interface ILifeAreasRepository
{
	ValueTask<PagedList<LifeArea>> FindTaskLifeAreasById(Guid id);
	ValueTask LinkTaskAsync(Guid lifeAreaId, Guid taskId);
	ValueTask RemoveTaskAsync(Guid lifeAreaId, Guid taskId);
	ValueTask AddAsync(LifeArea lifeArea);
	ValueTask AddRangeAsync(IEnumerable<LifeArea> lifeAreas);
	ValueTask RemoveAsync(LifeArea lifeArea);
	ValueTask RemoveRangeAsync(IEnumerable<LifeArea> lifeAreas);
	ValueTask UpdateAsync(LifeArea lifeArea);
	ValueTask UpdateRangeAsync(IEnumerable<LifeArea> lifeAreas);
	ValueTask<bool> ContainsAsync(Guid id);
}
