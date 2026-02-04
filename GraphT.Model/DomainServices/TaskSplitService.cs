using GraphT.Model.Aggregates;

namespace GraphT.Model.DomainServices;

public static class TaskSplitService
{
	public static TodoTask Split(TodoTask task)
	{
		TodoTask newTask = new(
			$"{task.Name} (2)",
			task.IsFun,
			task.IsProductive,
			task.Complexity,
			task.Priority,
			lifeAreas: task.LifeAreas.ToList(),
			parents: [ task ]);

		task.AddChildren([ newTask ]);
		
		return newTask;
	}
}
