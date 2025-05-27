using GraphT.Model.Aggregates;
using GraphT.Model.ValueObjects;

namespace GraphT.Model.Services;

public static class TaskProgressCalculatorService
{
	public static float GetProgress(HashSet<TaskAggregate> downstreams, bool finishCurrentTask)
	{
		int totalDownstreams = downstreams.Count;
		int backlogTasks = downstreams.Count(task => task.Status is Status.Backlog);
		int completedOrDroppedTasks = downstreams.Count(task => task.Status is Status.Dropped or Status.Completed);
		const float isFinished = 100f;
		const float isUnfinished = 0f;
		const float almostFinished = 99f;

		if (totalDownstreams == 0 || totalDownstreams == backlogTasks)
		{
			return finishCurrentTask ? isFinished : isUnfinished;
		}

		if (totalDownstreams == completedOrDroppedTasks)
		{
			return finishCurrentTask ? isFinished : almostFinished;
		} 
		
		return (completedOrDroppedTasks * isFinished) / (totalDownstreams);
	}
}
