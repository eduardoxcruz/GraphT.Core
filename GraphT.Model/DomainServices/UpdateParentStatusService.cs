using GraphT.Model.Aggregates;
using GraphT.Model.Enums;
using GraphT.Model.Repositories;

using SeedWork;

namespace GraphT.Model.DomainServices;

public interface IUpdateParentStatusPort : IPortWithInput<List<TodoTask>>;

public class UpdateParentStatusService : IUpdateParentStatusPort
{
	private readonly IGetChildrenStatusesByIdPort _getHighestStatusFromChildrenByIdPort;
	private readonly IUpdateTaskStatusPort _updateTaskStatusPort;
	private readonly IGetParentsByIdPort _getParentsByIdPort;

	public UpdateParentStatusService(IGetChildrenStatusesByIdPort getHighestStatusFromChildrenByIdPort, IUpdateTaskStatusPort updateTaskStatusPort, IGetParentsByIdPort getParentsByIdPort)
	{
		_getHighestStatusFromChildrenByIdPort = getHighestStatusFromChildrenByIdPort;
		_updateTaskStatusPort = updateTaskStatusPort;
		_getParentsByIdPort = getParentsByIdPort;
	}

	public async ValueTask HandleAsync(List<TodoTask> input)
	{
		foreach (TodoTask parent in input)
		{
			await UpdateParentStatus(parent);
			await HandleAsync(await _getParentsByIdPort.HandleAsync(parent.Id));
		}
	}

	private async ValueTask UpdateParentStatus(TodoTask parent)
	{
		List<TaskState> childrenStatuses = parent.Children.Any() ? 
			parent.Children.Select(child => child.Status).ToList() : 
			await _getHighestStatusFromChildrenByIdPort.HandleAsync(parent.Id);

		TaskState newTaskState = childrenStatuses.Max();

		if (parent.Status == newTaskState) return;

		if (newTaskState is TaskState.Discarded or TaskState.Finished)
		{
			newTaskState = childrenStatuses.Contains(TaskState.Doing) ? 
				TaskState.Doing :
				childrenStatuses.Contains(TaskState.ReadyToStart) ? 
					TaskState.ReadyToStart : 
					TaskState.Paused;
		}
		
		parent.SetStatus(newTaskState);

		await _updateTaskStatusPort.HandleAsync(new UpdateTaskStatusDto(parent.Id, newTaskState));
	}
}
