using GraphT.Model.Aggregates;

using SeedWork;

namespace GraphT.Model.Repositories;

public interface IGetParentsCountByChildIdPort : IFullPort<Guid, int>;
public interface IChildHasSingleParentWithIdPort : IFullPort<ChildHasSingleParentWithIdDto, bool>;
public interface ILinkParentToChildPort : IPortWithInput<LinkParentToChildDto>;
public interface IFindParentsByChildIdPort : IFullPort<Guid, List<TodoTask>>;
public interface IUnlinkParentFromChildPort : IPortWithInput<UnlinkParentFromChildDto>;

public record struct LinkParentToChildDto(Guid ChildId, Guid ParentId);
public record struct UnlinkParentFromChildDto(Guid TaskId, Guid ParentId);
public record struct ChildHasSingleParentWithIdDto(Guid TaskId, Guid ParentId);
