using GraphT.Model.Aggregates;
using GraphT.Model.Enums;

using SeedWork;

namespace GraphT.Model.Repositories;

public interface IAddTaskPort : IPortWithInput<TodoTask>;
public interface IUpdateTaskPort : IPortWithInput<TodoTask>;
public interface IDeleteTaskByIdPort : IPortWithInput<Guid>;
public interface ITaskExistPort : IFullPort<Guid, bool>;

public interface IFindTaskByIdPort : IFullPort<Guid, TodoTask?>;
public interface IFindRecurringTasksPort : IPortWithOutput<List<TodoTask>>;
public interface IFindTasksByIdPort : IFullPort<List<Guid>, List<TodoTask>>;
