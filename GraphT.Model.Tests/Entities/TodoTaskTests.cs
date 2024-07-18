using GraphT.Model.Entities;

namespace GraphT.Model.Tests.Entities;

public class TodoTaskTests
{
	private class TestableTodoTask(string name) : TodoTask(name);

	[Fact]
	public void Constructor_ShouldAssignNewGuidToId()
	{
		string taskName = "Test Task";
		
		TestableTodoTask task = new(taskName);

		Assert.NotEqual(Guid.Empty, task.Id);
	}

	[Fact]
	public void Constructor_ShouldAssignName()
	{
		string taskName = "Test Task";
		
		TestableTodoTask task = new(taskName);
		
		Assert.Equal(taskName, task.Name);
	}

	[Fact]
	public void Name_ShouldBeAssignable()
	{
		string taskName = "Initial Name";
		TestableTodoTask task = new(taskName);
		string newName = "Updated Name";

		task.Name = newName;

		Assert.Equal(newName, task.Name);
	}
}
