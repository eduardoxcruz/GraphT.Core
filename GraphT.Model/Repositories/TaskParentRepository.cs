using GraphT.Model.Aggregates;

using SeedWork;

namespace GraphT.Model.Repositories;

public interface IGetParentsCountByChildIdPort : IFullPort<Guid, int>;
public interface IChildHasSingleParentWithIdPort : IFullPort<ChildHasSingleParentWithIdDto, bool>;
public interface ILinkParentIdToChildIdPort : IPortWithInput<LinkParentIdToChildIdDto>;
public interface IFindParentsByChildIdPort : IFullPort<Guid, List<TodoTask>>;
public interface IRemoveParentById : IPortWithInput<RemoveParentDto>;

public record struct LinkParentIdToChildIdDto(Guid ChildId, Guid ParentId);
public record struct RemoveParentDto(Guid TaskId, Guid ParentId);
public record struct ChildHasSingleParentWithIdDto(Guid TaskId, Guid ParentId);
