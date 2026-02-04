using GraphT.Model.Aggregates;
using GraphT.Model.Enums;

using SeedWork;

namespace GraphT.Model.Repositories;

public interface IGetParentsCountPort : IFullPort<Guid, int>;
public interface ITaskHasOnlyThisParentPort : IFullPort<TaskHasOnlyThisParentDto, bool>;
public interface IAddParentByIdPort : IPortWithInput<AddParentDto>;
public interface IGetParentsByIdPort : IFullPort<Guid, List<TodoTask>>;
public interface IRemoveParentById : IPortWithInput<RemoveParentDto>;

public interface IGetChildrenByIdPortPort : IFullPort<Guid, List<TodoTask>>;
public interface IGetOnlyChildrenIdsPort : IFullPort<Guid, List<Guid>>;

public interface IAddTaskPort : IPortWithInput<TodoTask>;
public interface ITaskExistPort : IFullPort<Guid, bool>;
public interface IDeleteTaskByIdPort : IPortWithInput<Guid>;
public interface IGetTaskByIdPort : IFullPort<Guid, TodoTask?>;

public interface IUpdateTaskPriorityPort : IPortWithInput<IUpdateTaskPriorityDto>;
public interface IUpdateTaskStatusPort : IPortWithInput<UpdateTaskStatusDto>;

public record struct IUpdateTaskPriorityDto(Guid TaskId, Priority PriorityValue);
public record struct AddParentDto(Guid TaskId, Guid ParentId);
public record struct RemoveParentDto(Guid TaskId, Guid ParentId);
public record struct TaskHasOnlyThisParentDto(Guid TaskId, Guid ParentId);
public record struct UpdateTaskStatusDto(Guid TaskId, TaskState TaskStatus);
