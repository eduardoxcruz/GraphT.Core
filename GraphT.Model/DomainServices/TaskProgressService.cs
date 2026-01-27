using GraphT.Model.Aggregates;
using GraphT.Model.Enums;

namespace GraphT.Model.DomainServices;

public static class TaskProgressService
{
	public static int Calculate(List<TodoTask> taskChildren, TaskState taskStatus)
	{
		int totalChildren = taskChildren.Count;
		
		if (taskStatus is TaskState.Finished) return 100;

		if (totalChildren == 0) return 0;
		
		int completedChildren = taskChildren.Count(t => t.Status >= TaskState.Discarded);
		
		if (completedChildren == taskChildren.Count) return 99;
		
		return (completedChildren * 100) / totalChildren;
	}
}
