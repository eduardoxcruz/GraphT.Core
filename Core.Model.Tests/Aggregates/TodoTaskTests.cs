using System.Reflection;

using Core.Model.Aggregates;
using Core.Model.Enums;
using Core.Model.ValueObjects;

namespace Core.Model.Tests.Aggregates;

public class TodoTaskTests
{
	[Fact]
	public void CreateTodo_ShouldCreateTodo_WhenNameIsExtremelyLong()
	{
		string name = new('A', 10000);

		TodoTask todo = TodoTask.Create(name);

		Assert.NotNull(todo);
		Assert.Equal(name, todo.Name);
	}
	
	[Fact]
	public void CreateTodo_ShouldThrowException_WhenNameIsEmpty()
	{
		string name = string.Empty;

		ArgumentException exception = Assert.Throws<ArgumentException>(() => TodoTask.Create(name));
		Assert.Equal("Name cannot be empty", exception.Message);
	}
	
	[Fact]
	public void CreateTodo_ShouldThrowException_WhenNameIsWhitespace()
	{
		string name = "   ";

		ArgumentException exception = Assert.Throws<ArgumentException>(() => TodoTask.Create(name));
		Assert.Equal("Name cannot be empty", exception.Message);
	}

	[Fact]
	public void TodoTasks_WithSameName_ShouldBeDifferentInstances()
	{
		string name = "Test";
    
		TodoTask todo1 = TodoTask.Create(name);
		TodoTask todo2 = TodoTask.Create(name);
		
		Assert.NotEqual(todo1, todo2);
	}

	[Fact]
	public void TodoTaskRelevance_ShouldBeSuperficial_WhenCreated()
	{
		TodoTask todo = TodoTask.Create();
		
		Assert.Equal(Relevance.Superficial, todo.Relevance);
	}

	[Fact]
	public void Relevance_ShouldUpdate_WhenIsFunOrIsProductiveChanges()
	{
		TodoTask todo = TodoTask.Create();
		Assert.True(todo.Relevance is Relevance.Superficial);
		
		todo.IsFun = true;
		todo.IsProductive = false;
		Assert.True(todo.Relevance is Relevance.Entertaining);

		todo.IsFun = false;
		todo.IsProductive = true;
		Assert.True(todo.Relevance is Relevance.Necessary);
		
		todo.IsFun = true;
		todo.IsProductive = true;
		Assert.True(todo.Relevance is Relevance.Purposeful);
	}
	
	[Fact]
	public void TodoTask_ShouldHave_ComplexityUndefined_WhenCreated()
	{
		TodoTask item = TodoTask.Create();
		
		Assert.Equal(Complexity.Undefined, item.Complexity);
	}
	
	[Fact]
	public void TodoTask_ShouldHave_PriorityDistraction_WhenCreated()
	{
		TodoTask item = TodoTask.Create();
		
		Assert.Equal(Priority.Distraction, item.Priority);
	}
	
	[Fact]
	public void TodoTask_ShouldHave_StatusCreated_WhenCreated()
	{
		TodoTask item = TodoTask.Create();
		
		Assert.Equal(TaskState.Created, item.Status);
	}

	[Fact]
	public void Status_OnlyChangedVia_SetStatus()
	{
		TodoTask todo = TodoTask.Create();
		TaskState expected = TaskState.Backlog;
		
		todo.SetStatus(expected);
		
		Assert.Equal(expected, todo.Status);
	}

	[Fact]
	public void TodoTask_ShouldHave_LimitDateTime()
	{
		Assert.NotNull(typeof(TodoTask).GetProperty("LimitDateTime"));
	}
	
	[Fact]
	public void TodoTask_ShouldHave_ListOfStatusChangelogs()
	{
		Assert.NotNull(typeof(TodoTask).GetProperty("StatusChangeLogs"));
		Assert.True(typeof(TodoTask).GetProperty("StatusChangeLogs").PropertyType == typeof(List<StatusChangelog>));
	}
	
	[Fact]
	public void StatusChangelogs_ShouldBeReadOnly()
	{
		Assert.False(typeof(TodoTask).GetProperty("StatusChangeLogs").CanWrite);
	}

	[Fact]
	public void TodoTask_ShouldAddStatusCreated_ToStatusChangelogWhenCreated()
	{
		TodoTask todo = TodoTask.Create();
		TaskState expectedStatus = TaskState.Created;
		StatusChangelog log = todo.StatusChangeLogs.First();
		
		Assert.True(todo.StatusChangeLogs.Count != 0);
		Assert.True(todo.StatusChangeLogs.Count == 1);
		Assert.Equal(expectedStatus, log.NewState);
	}
	
	[Fact]
	public void ChangeStatus_ShouldAddStatusChangelog()
	{
		TodoTask todo = TodoTask.Create();
		TaskState expectedStatus = TaskState.Backlog;
		
		todo.SetStatus(expectedStatus);
		StatusChangelog log = todo.StatusChangeLogs.Last();
		
		Assert.Equal(2, todo.StatusChangeLogs.Count);
		Assert.Equal(expectedStatus, log.NewState);
	}

	[Fact]
	public void TodoTask_ShouldHave_MultipleParents()
	{
		Assert.NotNull(typeof(TodoTask).GetProperty("Parents"));
		Assert.True(typeof(TodoTask).GetProperty("Parents").PropertyType == typeof(List<Guid>));
	}

	[Fact]
	public void Parents_ShouldBeReadOnly()
	{
		Assert.False(typeof(TodoTask).GetProperty("Parents").CanWrite);
	}
	
	[Fact]
	public void Parents_OnlyAddedVia_AddParents()
	{
		TodoTask parent1 = TodoTask.Create("Parent 1");
		TodoTask parent2 = TodoTask.Create("Parent 2");
		TodoTask parent3 = TodoTask.Create("Parent 3");
		TodoTask task = TodoTask.Create();

		task.AddParent(parent1, false, false);
		task.AddParent(parent2, false, false);
		task.AddParent(parent3, false, false);
		
		Assert.True(task.Parents.Count != 0);
		Assert.Contains(parent1.Id, task.Parents);
		Assert.Contains(parent2.Id, task.Parents);
		Assert.Contains(parent3.Id, task.Parents);
	}

	[Fact]
	public void AddParents_ShouldNotAddParent_WhenItIsAlreadyParent()
	{
		TodoTask parent = TodoTask.Create("Parent");
		TodoTask task = TodoTask.Create();
		
		task.AddParent(parent, false, false);
		task.AddParent(parent, false,false);
		
		Assert.Single(task.Parents);
	}

	[Fact]
	public void TodoTask_ShouldHave_MultipleChildren()
	{
		Assert.NotNull(typeof(TodoTask).GetProperty("Children"));
		Assert.True(typeof(TodoTask).GetProperty("Children").PropertyType == typeof(List<Guid>));
	}
	
	[Fact]
	public void Children_ShouldBeReadOnly()
	{
		Assert.False(typeof(TodoTask).GetProperty("Children").CanWrite);
	}

	[Fact]
	public void Children_OnlyAddedVia_SetChildren()
	{
		TodoTask child1 = TodoTask.Create("Parent 1");
		TodoTask child2 = TodoTask.Create("Parent 2");
		TodoTask child3 = TodoTask.Create("Parent 3");
		TodoTask task = TodoTask.Create();

		task.AddChild(child1.Id);
		task.AddChild(child2.Id);
		task.AddChild(child3.Id);
		
		Assert.True(task.Children.Count != 0);
		Assert.Contains(child1.Id, task.Children);
		Assert.Contains(child2.Id, task.Children);
		Assert.Contains(child3.Id, task.Children);
	}
	
	[Fact]
	public void AddChildren_ShouldNotAddChildren_WhenItIsAlreadyChildren()
	{
		TodoTask children = TodoTask.Create();
		TodoTask task = TodoTask.Create();
		
		task.AddChild(children.Id);
		task.AddChild(children.Id);
		
		Assert.Single(task.Children);
	}
	
	[Fact]
	public void TodoTask_ShouldHave_ElapsedTime()
	{
		Assert.NotNull(typeof(TodoTask).GetProperty("ElapsedTime"));
	}

	[Fact]
	public void ElapsedTime_ShouldBe_Calculated_FromSum_DoingStates_TillNextState()
	{
		TodoTask task = TodoTask.Create();
		DateTimeOffset now = DateTimeOffset.UtcNow;
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
		
		task.SetStatus(TaskState.Backlog, dateTimeLog1);
		task.SetStatus(TaskState.ReadyToStart, dateTimeLog2);
		task.SetStatus(TaskState.Doing, currentlyDoingLog1);
		task.SetStatus(TaskState.Paused, dateTimeLog4);
		task.SetStatus(TaskState.ReadyToStart, dateTimeLog5);
		task.SetStatus(TaskState.Doing, currentlyDoingLog2);
		task.SetStatus(TaskState.Discarded, dateTimeLog7);
		task.SetStatus(TaskState.Doing, currentlyDoingLog3);
		task.SetStatus(TaskState.Backlog, dateTimeLog9);
		task.SetStatus(TaskState.Doing, currentlyDoingLog4);
		task.SetStatus(TaskState.Finished, dateTimeLog11);
		task.SetStatus(TaskState.Doing, currentlyDoingLog5);
		task.SetStatus(TaskState.Finished, dateTimeLog13);
		
		Assert.Equal(new TimeSpan(3, 4, 16, 5), task.ElapsedTime);
	}

	[Fact]
	public void TodoTask_ShouldHave_Progress()
	{
		Assert.NotNull(typeof(TodoTask).GetProperty("Progress"));
	}

	[Fact]
	public void TodoTask_ShouldHave_MultipleLifeAreas()
	{
		Assert.NotNull(typeof(TodoTask).GetProperty("LifeAreas"));
		Assert.True(typeof(TodoTask).GetProperty("LifeAreas").PropertyType == typeof(List<Guid>));
	}
	
	[Fact]
	public void LifeAreas_ShouldBeReadOnly()
	{
		Assert.False(typeof(TodoTask).GetProperty("LifeAreas").CanWrite);
	}

	[Fact]
	public void LifeAreas_OnlyAddedVia_AddLifeArea()
	{
		LifeArea la1 = LifeArea.Create("Life Area 1");
		LifeArea la2 = LifeArea.Create("Life Area 2");
		TodoTask task = TodoTask.Create();
		
		task.AddLifeArea(la1.Id);
		task.AddLifeArea(la2.Id);
		
		Assert.True(task.LifeAreas.Count != 0);
		Assert.Contains(la1.Id, task.LifeAreas);
		Assert.Contains(la2.Id, task.LifeAreas);
		Assert.Equal(2, task.DomainEvents.Count);
	}
	
	[Fact]
	public void AddLifeAreas_ShouldNotAddLifeArea_WhenItIsAlreadyLifeArea()
	{
		LifeArea lifeArea = LifeArea.Create("Life Area");
		TodoTask task = TodoTask.Create();
		task.AddLifeArea(lifeArea.Id);
		task.AddLifeArea(lifeArea.Id);
		
		Assert.Single(task.LifeAreas);
		Assert.Single(task.DomainEvents);
	}
}
