using GraphT.Model.Aggregates;
using GraphT.Model.Entities;
using GraphT.Model.ValueObjects;

namespace GraphT.Model.Tests.Aggregates;

public class TodoTaskTests
{
	[Fact]
	public void TodoTask_ShouldHave_TodoItem_WhenCreated()
	{
		TodoItem item = new();
		TodoTask task = new(item);
		
		Assert.NotNull(typeof(TodoTask).GetProperty("Item"));
		Assert.True(task.Item.Id != Guid.Empty);
	}

	[Fact]
	public void Item_ShouldBeReadOnly()
	{
		Assert.False(typeof(TodoTask).GetProperty("Item").CanWrite);
	}

	[Fact]
	public void TodoTask_ShouldHave_MultipleParents()
	{
		Assert.NotNull(typeof(TodoTask).GetProperty("Parents"));
		Assert.True(typeof(TodoTask).GetProperty("Parents").PropertyType == typeof(IReadOnlySet<TodoItem>));
	}

	[Fact]
	public void Parents_ShouldBeReadOnly()
	{
		Assert.False(typeof(TodoTask).GetProperty("Parents").CanWrite);
	}
	
	[Fact]
	public void Parents_OnlyChangedVia_SetParents()
	{
		TodoItem parent1 = new("Parent 1");
		TodoItem parent2 = new("Parent 2");
		TodoItem parent3 = new("Parent 3");
		TodoItem item = new("Item");
		TodoTask task = new(item);

		task.SetParents([ parent1, parent2, parent3 ]);
		
		Assert.True(task.Parents.Count != 0);
		Assert.Contains(parent1, task.Parents);
		Assert.Contains(parent2, task.Parents);
		Assert.Contains(parent3, task.Parents);
	}

	[Fact]
	public void TodoItem_ShouldHave_MultipleChildren()
	{
		Assert.NotNull(typeof(TodoTask).GetProperty("Children"));
		Assert.True(typeof(TodoTask).GetProperty("Children").PropertyType == typeof(IReadOnlySet<TodoItem>));
	}
	
	[Fact]
	public void Children_ShouldBeReadOnly()
	{
		Assert.False(typeof(TodoTask).GetProperty("Children").CanWrite);
	}

	[Fact]
	public void Children_OnlyChangedVia_SetChildren()
	{
		TodoItem child1 = new("Parent 1");
		TodoItem child2 = new("Parent 2");
		TodoItem child3 = new("Parent 3");
		TodoItem item = new("Item");
		TodoTask task = new(item);

		task.SetChildren([ child1, child2, child3 ]);
		
		Assert.True(task.Children.Count != 0);
		Assert.Contains(child1, task.Children);
		Assert.Contains(child2, task.Children);
		Assert.Contains(child3, task.Children);
	}

	[Fact]
	public void TodoTask_ShouldHave_ElapsedTime()
	{
		Assert.NotNull(typeof(TodoTask).GetProperty("ElapsedTime"));
	}
	
	[Fact]
	public void ElapsedTime_ShouldBeReadOnly()
	{
		Assert.False(typeof(TodoTask).GetProperty("ElapsedTime").CanWrite);
	}

	[Fact]
	public void ElapsedTime_ShouldBe_StringFormated_With_Days_Hours_Minutes_Seconds_WhenCalledToString()
	{
		TodoTask task = new(new TodoItem());
		
		Assert.Equal("\u23f0 0 day(s) - 0 hour(s) - 0 minute(s) - 0 second(s)", task.ElapsedTimeFormatted);
	}

	[Fact]
	public void ElapsedTime_ShouldBe_Calculated_FromSum_DoingStates_TillNextState()
	{
		TodoTask task = new(new TodoItem());
		DateTimeOffset now = DateTimeOffset.Now;
		DateTimeOffset dateTimeLog1 = now.AddSeconds(5);
		DateTimeOffset dateTimeLog2 = dateTimeLog1.AddSeconds(10);
		DateTimeOffset currentlyDoingLog1 = dateTimeLog2.AddSeconds(10);
		DateTimeOffset dateTimeLog4 = currentlyDoingLog1.AddMinutes(15);
		DateTimeOffset dateTimeLog5 = dateTimeLog4.AddSeconds(10);
		DateTimeOffset currentlyDoingLog2 = dateTimeLog5.AddSeconds(10);
		DateTimeOffset dateTimeLog7 = currentlyDoingLog2.AddDays(3);
		DateTimeOffset currentlyDoingLog3 = dateTimeLog7.AddSeconds(10);
		DateTimeOffset dateTimeLog9 = currentlyDoingLog3.AddSeconds(15);
		DateTimeOffset currentlyDoingLog4 = dateTimeLog9.AddSeconds(25);
		DateTimeOffset dateTimeLog11 = currentlyDoingLog4.AddHours(4);
		DateTimeOffset currentlyDoingLog5 = dateTimeLog11.AddSeconds(10);
		DateTimeOffset dateTimeLog13 = currentlyDoingLog5.AddSeconds(50);
		
		task.Item.SetStatus(dateTimeLog1, Status.Backlog);
		task.Item.SetStatus(dateTimeLog2, Status.ReadyToStart);
		task.Item.SetStatus(currentlyDoingLog1, Status.CurrentlyDoing);
		task.Item.SetStatus(dateTimeLog4, Status.Paused);
		task.Item.SetStatus(dateTimeLog5, Status.ReadyToStart);
		task.Item.SetStatus(currentlyDoingLog2, Status.CurrentlyDoing);
		task.Item.SetStatus(dateTimeLog7, Status.Dropped);
		task.Item.SetStatus(currentlyDoingLog3, Status.CurrentlyDoing);
		task.Item.SetStatus(dateTimeLog9, Status.Backlog);
		task.Item.SetStatus(currentlyDoingLog4, Status.CurrentlyDoing);
		task.Item.SetStatus(dateTimeLog11, Status.Completed);
		task.Item.SetStatus(currentlyDoingLog5, Status.CurrentlyDoing);
		task.Item.SetStatus(dateTimeLog13, Status.Completed);
		
		Assert.Equal("\u23f0 3 day(s) - 4 hour(s) - 16 minute(s) - 5 second(s)", task.ElapsedTimeFormatted);
	}
}
