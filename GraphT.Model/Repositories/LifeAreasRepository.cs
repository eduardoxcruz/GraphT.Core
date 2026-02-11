using GraphT.Model.Aggregates;

using SeedWork;

namespace GraphT.Model.Repositories;

public interface IFindLifeAreaIdsByTaskIdPort : IFullPort<Guid, List<Guid>>;
public interface ILinkLifeAreasToTaskIdPort : IPortWithInput<LinkLifeAreasToTaskIdDto>;

public record struct LinkLifeAreasToTaskIdDto(Guid TaskId, List<Guid> LifeAreaIds);
