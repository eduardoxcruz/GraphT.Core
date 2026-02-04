using GraphT.Model.Aggregates;
using GraphT.Model.Enums;

using SeedWork;

namespace GraphT.Model.Repositories;

public interface IGetParentsByIdPort : IFullPort<Guid, List<TodoTask>>;
public interface IGetChildrenStatusesByIdPort : IFullPort<Guid, List<TaskState>>;
public interface IUpdateTaskStatusPort : IPortWithInput<UpdateTaskStatusDto>;
public interface ITaskHasParentsAndChildrenPort : IFullPort<Guid, bool>;
public interface IGetChildrenByIdPort : IFullPort<Guid, List<TodoTask>>;
public interface IDeleteTaskByIdPort : IPortWithInput<Guid>;
public interface ITaskHasOnlyThisParentPort : IFullPort<TaskHasOnlyThisParentDto, bool>;
public interface IAddParentByIdPort : IPortWithInput<AddParentDto>;
public interface IUpdateTaskPriorityPort : IPortWithInput<Priority>;

public record struct AddParentDto(Guid TaskId, Guid ParentId);
public record struct TaskHasOnlyThisParentDto(Guid TaskId, Guid ParentId);
public record struct UpdateTaskStatusDto(Guid TaskId, TaskState TaskStatus);
