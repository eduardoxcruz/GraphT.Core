using GraphT.Model.Aggregates;

using SeedWork;

namespace GraphT.Model.Repositories;

public interface IGetChildrenByIdPortPort : IFullPort<Guid, List<TodoTask>>;
public interface IGetOnlyChildrenIdsPort : IFullPort<Guid, List<Guid>>;
