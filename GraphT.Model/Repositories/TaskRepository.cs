using GraphT.Model.Aggregates;
using GraphT.Model.Enums;

using SeedWork;

namespace GraphT.Model.Repositories;

public interface IGetParentsByIdPort : IFullPort<Guid, List<TodoTask>>;
public interface IGetChildrenStatusesByIdPort : IFullPort<Guid, List<TaskState>>;
public interface IUpdateTaskStatusPort : IPortWithInput<UpdateTaskStatusDto>;
public interface IUpdateTaskPriorityPort : IPortWithInput<Priority>;

public record struct UpdateTaskStatusDto(Guid TaskId, TaskState TaskStatus);
