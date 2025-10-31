using GraphT.Model.Aggregates;
using GraphT.Model.ValueObjects;

using SeedWork;

namespace GraphT.Model.Services.Repositories;

public interface IFindTaskByStatusPort : IFullPort<PagingOptionsWithTaskStatusDto, PagedList<TodoTask>>;
public interface IFindTasksWithoutParentPort : IFullPort<PagingOptions, PagedList<TodoTask>>;
public interface IGetTasksOrderedByCreationDateDescendingPort : IFullPort<PagingOptions, PagedList<TodoTask>>;
public interface IFindTaskByIdAsync : IFullPort<Guid, TodoTask>;
public interface IAddTaskPort : IPortWithInput<TodoTask>;
public interface IUpdateTaskPort : IPortWithInput<TodoTask>;
public interface IRemoveTaskPort : IPortWithInput<Guid>;
public interface IContainsTaskPort : IFullPort<bool, Guid>;

public record struct PagingOptionsWithTaskStatusDto(PagingOptions PagingOptions, Status Status);
