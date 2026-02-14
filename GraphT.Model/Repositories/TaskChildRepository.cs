using GraphT.Model.Aggregates;

using SeedWork;

namespace GraphT.Model.Repositories;

public interface IFindChildrenByParentIdPort : IFullPort<Guid, List<TodoTask>>;
public interface IFindChildIdsByParentIdPort : IFullPort<Guid, List<Guid>>;
