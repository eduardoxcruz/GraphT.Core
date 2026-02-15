using GraphT.Model.Aggregates;
using GraphT.Model.Enums;

using SeedWork;

namespace GraphT.Model.Repositories;

public interface IAddTaskPort : IPortWithInput<TodoTask>;
public interface IUpdateTaskPort : IPortWithInput<TodoTask>;
public interface IDeleteTaskByIdPort : IPortWithInput<Guid>;
public interface ITaskExistPort : IFullPort<Guid, bool>;
public interface IFindTaskByIdPort : IFullPort<FindTaskByIdIncludesDto, TodoTask?>;
public interface IFindRecurringTasksPort : IPortWithOutput<List<TodoTask>>;

public interface IFindChildrenByParentIdPort : IFullPort<Guid, List<TodoTask>>;
public interface IFindParentsByChildIdPort : IFullPort<Guid, List<TodoTask>>;

public record struct FindTaskByIdIncludesDto(Guid TaskId, bool IncludeStatusLogs = false);
