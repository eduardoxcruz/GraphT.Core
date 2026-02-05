using GraphT.Model.Aggregates;

using SeedWork;

namespace GraphT.Model.Repositories;

public interface IFindLifeAreaIdsByTaskIdPort : IFullPort<Guid, List<Guid>>;
public interface IAddLifeAreasByTaskIdPort : IPortWithInput<AddLifeAreasByTaskIdDto>;

public record struct AddLifeAreasByTaskIdDto(Guid TaskId, List<Guid> LifeAreaIds);
