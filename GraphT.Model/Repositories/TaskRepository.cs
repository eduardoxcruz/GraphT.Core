using GraphT.Model.Aggregates;
using GraphT.Model.Enums;

using SeedWork;

namespace GraphT.Model.Repositories;

public interface IAddTaskPort : IPortWithInput<TodoTask>;
public interface ITaskExistPort : IFullPort<Guid, bool>;
public interface IDeleteTaskByIdPort : IPortWithInput<Guid>;
public interface IGetTaskByIdPort : IFullPort<Guid, TodoTask?>;

public interface IUpdateTaskPriorityPort : IPortWithInput<UpdateTaskPriorityDto>;
public interface IUpdateTaskStatusPort : IPortWithInput<UpdateTaskStatusDto>;

public record struct UpdateTaskPriorityDto(Guid TaskId, Priority PriorityValue);
public record struct UpdateTaskStatusDto(Guid TaskId, TaskState TaskStatus);
