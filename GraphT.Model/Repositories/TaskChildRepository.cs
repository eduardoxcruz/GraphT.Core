using GraphT.Model.Aggregates;

using SeedWork;

namespace GraphT.Model.Repositories;

public interface IGetChildrenByIdPort : IFullPort<Guid, List<TodoTask>>;
public interface IGetOnlyChildrenIdsPort : IFullPort<Guid, List<Guid>>;
