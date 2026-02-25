using GraphT.Model.Aggregates;
using GraphT.Model.Services;
using GraphT.Model.Enums;

namespace GraphT.Model.Tests.DomainServices;

public class TaskProgressServiceTests
{
	[Theory]
	[InlineData(2)]
	[InlineData(5)]
	[InlineData(10)]
	[InlineData(50)]
	[InlineData(100)]
	[InlineData(1000)]
	public void Progress_ShouldBe_Calculated_From_CompletedChildren(int maxChildren)
	{
		List<TodoTask> children = [];

		for (int i = 1; i <= maxChildren; i++)
		{
			TodoTask child = TodoTask.Create($"Child {i}");

			if (i < maxChildren)
			{
				Random random = new();
			
				if (random.Next(0, 2) == 1)
				{
					child.SetStatus(random.Next(0, 2) == 1 ? TaskState.Finished : TaskState.Discarded);
				}
			}
			
			children.Add(child);
		}
		
		int childrenCompleted = children.Count(t => t.Status >= TaskState.Discarded);
		int expected = ((childrenCompleted * 100) / maxChildren);
		
		Assert.Equal(expected, TaskProgressService.Calculate(children, TaskState.Created));
	}
	
	[Fact]
	public void Progress_ShouldBe99_WhenAllChildrenAreCompletedOrDropped_ButCurrentTaskNotMarkedAsFinishedOrDropped()
	{
		List<TodoTask> children = [];

		for (int i = 1; i <= 10; i++)
		{
			TodoTask child = TodoTask.Create($"Child {i}");
			
			Random random = new();

			child.SetStatus(random.Next(0, 2) == 1 ? TaskState.Finished : TaskState.Discarded);

			children.Add(child);
		}
		
		Assert.Equal(99, TaskProgressService.Calculate(children, TaskState.Created));
	}
	
	[Fact]
	public void Progress_ShouldBe100_When_CurrentTaskIsCompletedOrDropped()
	{
		List<TodoTask> children = [];

		for (int i = 1; i <= 10; i++)
		{
			TodoTask child = TodoTask.Create($"Child {i}");
			child.SetStatus(TaskState.ReadyToStart, DateTimeOffset.UtcNow);
			children.Add(child);
		}
		
		Assert.Equal(100, TaskProgressService.Calculate(children, TaskState.Finished));
		Assert.Equal(100, TaskProgressService.Calculate(children, TaskState.Discarded));
	}

	[Fact]
	public void Progress_ShouldBe0_When_NoChildrenAndTaskIsNotCompletedNorDropped()
	{
		Assert.Equal(0, TaskProgressService.Calculate([], TaskState.Created));
	}
}
