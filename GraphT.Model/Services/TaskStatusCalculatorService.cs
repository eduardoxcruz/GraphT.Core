using GraphT.Model.Aggregates;
using GraphT.Model.Enums;

namespace GraphT.Model.Services;

public static class TaskStatusCalculatorService
{
	public static TaskState Calculate(List<TodoTask> children, bool shouldFinishIfPossible)
	{
		int childrenCreatedCount = children.Count(child => child.Status == TaskState.Created);
		int childrenBacklogCount = children.Count(child => child.Status == TaskState.Backlog);
		int childrenDiscardedCount = children.Count(child => child.Status == TaskState.Discarded);
		int childrenFinishedCount = children.Count(child => child.Status == TaskState.Finished);

		bool childrenPending = 
			(childrenCreatedCount + childrenBacklogCount + childrenFinishedCount + childrenDiscardedCount) != children.Count;
		
		switch (childrenPending)
		{
			case true:
				List<TaskState> childrenStatuses = children.Select(child => child.Status).ToList();

				if (childrenStatuses.Contains(TaskState.Doing))
				{
					return TaskState.Doing;
				}

				if (childrenStatuses.Contains(TaskState.ReadyToStart))
				{
					return TaskState.ReadyToStart;
				}
				
				return TaskState.Paused;
			case false:
				switch (shouldFinishIfPossible)
				{
					case true:
						return TaskState.Finished;
					case false:
						return TaskState.Paused;
				}
		};
	}
}
