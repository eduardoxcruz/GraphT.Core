using GraphT.Model.Aggregates;

using SeedWork;

namespace GraphT.Model.Repositories;

public interface IFindLifeAreasByTaskIdPort : IFullPort<Guid, List<LifeArea>>;
