using System.Reflection;

using GraphT.Model.Aggregates;
using GraphT.Model.Enums;
using GraphT.Model.ValueObjects;

namespace GraphT.Model.Tests.Aggregates;

public class TodoTaskTests
{
	[Fact]
	public void CreateTodo_ShouldCreateTodo_WhenNameIsExtremelyLong()
	{
		string name = new('A', 10000);

		TodoTask todo = new(name);

		Assert.NotNull(todo);
		Assert.Equal(name, todo.Name);
	}
	
	[Fact]
	public void CreateTodo_ShouldThrowException_WhenNameIsEmpty()
	{
		string name = string.Empty;

		ArgumentException exception = Assert.Throws<ArgumentException>(() => new TodoTask(name));
		Assert.Equal("Name cannot be empty", exception.Message);
	}
	
	[Fact]
	public void CreateTodo_ShouldThrowException_WhenNameIsWhitespace()
	{
		string name = "   ";

		ArgumentException exception = Assert.Throws<ArgumentException>(() => new TodoTask(name));
		Assert.Equal("Name cannot be empty", exception.Message);
	}

	[Fact]
	public void TodoTasks_WithSameName_ShouldBeDifferentInstances()
	{
		string name = "Test";
    
		TodoTask todo1 = new(name);
		TodoTask todo2 = new(name);
		
		Assert.NotEqual(todo1, todo2);
	}

	[Fact]
	public void TodoTaskRelevance_ShouldBeSuperficial_WhenCreated()
	{
		TodoTask todo = new();
		
		Assert.Equal(Relevance.Superficial, todo.Relevance);
	}

	[Fact]
	public void Relevance_ShouldUpdate_WhenIsFunOrIsProductiveChanges()
	{
		TodoTask todo = new();
		Assert.True(todo.Relevance is Relevance.Superficial);
		
		todo.SetValues(isFun: true, isProductive: false);
		Assert.True(todo.Relevance is Relevance.Entertaining);

		todo.SetValues(isFun: false, isProductive: true);
		Assert.True(todo.Relevance is Relevance.Necessary);
		
		todo.SetValues(isFun: true, isProductive: true);
		Assert.True(todo.Relevance is Relevance.Purposeful);
	}
	
	[Fact]
	public void TodoTask_ShouldHave_ComplexityUndefined_WhenCreated()
	{
		TodoTask item = new();
		
		Assert.Equal(Complexity.Undefined, item.Complexity);
	}
	
	[Fact]
	public void TodoTask_ShouldHave_PriorityDistraction_WhenCreated()
	{
		TodoTask item = new();
		
		Assert.Equal(Priority.Distraction, item.Priority);
	}
	
	[Fact]
	public void TodoTask_ShouldHave_StatusCreated_WhenCreated()
	{
		TodoTask item = new();
		
		Assert.Equal(TaskState.Created, item.Status);
	}

	[Fact]
	public void Status_OnlyChangedVia_SetStatus()
	{
		TodoTask todo = new();
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
	public void Punctuality_ReturnsNoLimitWhenLimitDateTimeNotSet()
	{
		TodoTask todo = new();
		
		Assert.Equal(PunctualityType.NoLimit, todo.Punctuality.Type);
	}

	[Theory]
	[InlineData(-2, 0, 0, 0)]
	[InlineData(-1, -1, 0, 0)]
	[InlineData(-1, 0, -1, 0)]
	[InlineData(-1, 0, 0, -1)]
	public void Punctuality_ReturnsEarlyDaysWhenFinishedBeforeLimitDate(double daysDifference, double hoursDifference, double minutesDifference = 0, double secondsDifference = 0)
	{
		TodoTask todo = new("Test");
		DateTimeOffset limitDateTime = DateTimeOffset.Now;
		
		todo.SetValues(limitDateTime: limitDateTime);
		todo.SetStatus(
			TaskState.Finished,
			limitDateTime
				.AddDays(daysDifference)
				.AddHours(hoursDifference)
				.AddMinutes(minutesDifference)
				.AddSeconds(secondsDifference));

		Assert.True(todo.Punctuality.Type == PunctualityType.Early);
		Assert.Equal(daysDifference, todo.Punctuality.TimeDifference.Days);
		Assert.Equal(hoursDifference, todo.Punctuality.TimeDifference.Hours);
		Assert.Equal(minutesDifference, todo.Punctuality.TimeDifference.Minutes);
		Assert.Equal(secondsDifference, todo.Punctuality.TimeDifference.Seconds);
	}

	[Theory]
	[InlineData(-1, 0, 0, 0)]
	[InlineData(0, -1, 0, 0)]
	[InlineData(0, 0, -1, 0)]
	[InlineData(0, 0, 0, -1)]
	[InlineData(0, 0, 0, 0)]
	public void Punctuality_ReturnsOnTimeWhenFinishedOnLimitDateOrWithin24Hours(double daysDifference, double hoursDifference, double minutesDifference = 0, double secondsDifference = 0)
	{
		TodoTask todo = new();
		DateTimeOffset limitDateTime = DateTimeOffset.Now;
		
		todo.SetValues(limitDateTime: limitDateTime);
		todo.SetStatus(TaskState.Finished,
			limitDateTime
				.AddDays(daysDifference)
				.AddHours(hoursDifference)
				.AddMinutes(minutesDifference)
				.AddSeconds(secondsDifference));
		
		Assert.True(todo.Punctuality.Type == PunctualityType.OnTime);
	}

	[Theory]
	[InlineData(0, 0, 0, 1)]
	[InlineData(0, 0, 1, 0)]
	[InlineData(0, 1, 0, 0)]
	[InlineData(1, 0, 0, 0)]
	public void Punctuality_ReturnsLateDaysWhenFinishedAfterLimitDate(double daysDifference, double hoursDifference, double minutesDifference = 0, double secondsDifference = 0)
	{
		TodoTask todo = new();
		DateTimeOffset limitDateTime = DateTimeOffset.Now;
		
		todo.SetValues(limitDateTime: limitDateTime);
		todo.SetStatus(TaskState.Finished,
			limitDateTime
				.AddDays(daysDifference)
				.AddHours(hoursDifference)
				.AddMinutes(minutesDifference)
				.AddSeconds(secondsDifference));
		
		Assert.True(todo.Punctuality.Type == PunctualityType.Late);
		Assert.Equal(daysDifference, todo.Punctuality.TimeDifference.Days);
		Assert.Equal(hoursDifference, todo.Punctuality.TimeDifference.Hours);
		Assert.Equal(minutesDifference, todo.Punctuality.TimeDifference.Minutes);
		Assert.Equal(secondsDifference, todo.Punctuality.TimeDifference.Seconds);
	}

	[Theory]
	[InlineData(2, 0, 0, 0)]
	[InlineData(1, 1, 0, 0)]
	[InlineData(1, 0, 1, 0)]
	[InlineData(1, 0, 0, 1)]
	public void Punctuality_ReturnsDaysToGoWhenCurrentDateIsBeforeLimitDate(double daysDifference, double hoursDifference, double minutesDifference = 0, double secondsDifference = 0)
	{
		TodoTask todo = new();
		DateTimeOffset limitDateTime = DateTimeOffset.Now;
		
		todo.SetValues(limitDateTime: limitDateTime
			.AddDays(daysDifference)
			.AddHours(hoursDifference)
			.AddMinutes(minutesDifference)
			.AddSeconds(secondsDifference));
		
		Assert.True(todo.Punctuality.Type == PunctualityType.TimeRemaining);
		Assert.Equal(daysDifference, todo.Punctuality.TimeDifference.Days);
		Assert.Equal(hoursDifference, todo.Punctuality.TimeDifference.Hours);
		Assert.Equal(minutesDifference, todo.Punctuality.TimeDifference.Minutes);
		Assert.Equal(secondsDifference, todo.Punctuality.TimeDifference.Seconds);
	}

	[Theory]
	[InlineData(1, 0, 0, 0)]
	[InlineData(0, 1, 0, 0)]
	[InlineData(0, 0, 1, 0)]
	[InlineData(0, 0, 0, 1)]
	[InlineData(0, 0, 0, 0)]
	public void Punctuality_ReturnsFinishTodayWhenLimitDateIsTodayOrWithin24Hours(double daysDifference,
		double hoursDifference, double minutesDifference = 0, double secondsDifference = 0)
	{
		TodoTask todo = new();
		DateTimeOffset limitDateTime = DateTimeOffset.Now;
		
		todo.SetValues(limitDateTime: limitDateTime
			.AddDays(daysDifference)
			.AddHours(hoursDifference)
			.AddMinutes(minutesDifference)
			.AddSeconds(secondsDifference));
		
		Assert.True(todo.Punctuality.Type == PunctualityType.FinishToday);
	}

	[Theory]
	[InlineData(0, 0, 0, -1)]
	[InlineData(0, 0, -1, 0)]
	[InlineData(0, -1, 0, 0)]
	[InlineData(-1, 0, 0, 0)]
	public void Punctuality_ReturnsDaysLateWhenCurrentDateIsAfterLimitDate(double daysDifference, double hoursDifference, double minutesDifference = 0, double secondsDifference = 0)
	{
		string expected =
			$"\ud83d\udea8 Late {Math.Abs(daysDifference)} day(s) - {Math.Abs(hoursDifference)} hours(s) - {Math.Abs(minutesDifference)} minute(s) - {Math.Abs(secondsDifference)} second(s)!";
		TodoTask todo = new();
		DateTimeOffset limitDateTime = DateTimeOffset.Now;
		
		todo.SetValues(limitDateTime: limitDateTime
			.AddDays(daysDifference)
			.AddHours(hoursDifference)
			.AddMinutes(minutesDifference)
			.AddSeconds(secondsDifference));
		
		Assert.True(todo.Punctuality.Type == PunctualityType.Late);
		Assert.Equal(daysDifference, todo.Punctuality.TimeDifference.Days);
		Assert.Equal(hoursDifference, todo.Punctuality.TimeDifference.Hours);
		Assert.Equal(minutesDifference, todo.Punctuality.TimeDifference.Minutes);
		Assert.Equal(secondsDifference, todo.Punctuality.TimeDifference.Seconds);
	}
	
	[Fact]
	public void TodoTask_ShouldHaveReadonly_ListOfStatusChangelogs()
	{
		Assert.NotNull(typeof(TodoTask).GetProperty("StatusChangeLogs"));
		Assert.False(typeof(TodoTask).GetProperty("StatusChangeLogs").CanWrite);
		Assert.True(typeof(TodoTask).GetProperty("StatusChangeLogs").PropertyType == typeof(IReadOnlyList<StatusChangelog>));
	}

	[Fact]
	public void TodoTask_ShouldAddStatusCreated_ToStatusChangelogWhenCreated()
	{
		TodoTask todo = new();
		TaskState expectedStatus = TaskState.Created;
		StatusChangelog log = todo.StatusChangeLogs.First();
		
		Assert.True(todo.StatusChangeLogs.Count != 0);
		Assert.True(todo.StatusChangeLogs.Count == 1);
		Assert.Equal(expectedStatus, log.NewState);
	}
	
	[Fact]
	public void ChangeStatus_ShouldAddStatusChangelog()
	{
		TodoTask todo = new();
		TaskState expectedStatus = TaskState.Backlog;
		
		todo.SetStatus(expectedStatus);
		StatusChangelog log = todo.StatusChangeLogs.Last();
		
		Assert.True(todo.StatusChangeLogs.Count != 0);
		Assert.True(todo.StatusChangeLogs.Count == 2);
		Assert.Equal(expectedStatus, log.NewState);
	}

	[Fact]
	public void TodoTask_ShouldHave_MultipleParents()
	{
		Assert.NotNull(typeof(TodoTask).GetProperty("Parents"));
		Assert.True(typeof(TodoTask).GetProperty("Parents").PropertyType == typeof(IReadOnlyList<TodoTask>));
	}

	[Fact]
	public void Parents_ShouldBeReadOnly()
	{
		Assert.False(typeof(TodoTask).GetProperty("Parents").CanWrite);
	}
	
	[Fact]
	public void Parents_OnlyAddedVia_AddParents()
	{
		TodoTask parent1 = new("Parent 1");
		TodoTask parent2 = new("Parent 2");
		TodoTask parent3 = new("Parent 3");
		TodoTask task = new();

		task.AddParents([ parent1, parent2, parent3 ]);
		
		Assert.True(task.Parents.Count != 0);
		Assert.Contains(parent1, task.Parents);
		Assert.Contains(parent2, task.Parents);
		Assert.Contains(parent3, task.Parents);
	}

	[Fact]
	public void TodoTask_ShouldHave_MultipleChildren()
	{
		Assert.NotNull(typeof(TodoTask).GetProperty("Children"));
		Assert.True(typeof(TodoTask).GetProperty("Children").PropertyType == typeof(IReadOnlyList<TodoTask>));
	}
	
	[Fact]
	public void Children_ShouldBeReadOnly()
	{
		Assert.False(typeof(TodoTask).GetProperty("Children").CanWrite);
	}

	[Fact]
	public void Children_OnlyAddedVia_AddChildren()
	{
		TodoTask child1 = new("Parent 1");
		TodoTask child2 = new("Parent 2");
		TodoTask child3 = new("Parent 3");
		TodoTask task = new();

		task.AddChildren([ child1, child2, child3 ]);
		
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
	public void ElapsedTime_ShouldBe_Calculated_FromSum_DoingStates_TillNextState()
	{
		TodoTask task = new();
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

	[Theory]
	[InlineData(2)]
	[InlineData(5)]
	[InlineData(10)]
	[InlineData(50)]
	[InlineData(100)]
	[InlineData(1000)]
	public void Progress_ShouldBe_Calculated_From_CompletedChildren(int maxChildren)
	{
		TodoTask task = new();
		List<TodoTask> children = [];

		for (int i = 1; i <= maxChildren; i++)
		{
			TodoTask child = new($"Child {i}");

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
		
		task.AddChildren(children);
		
		Assert.Equal(expected, task.Progress);
	}
	
	[Fact]
	public void Progress_ShouldBe99_WhenAllChildrenAreCompletedOrDropped_ButCurrentTaskNotMarkedAsFinishedOrDropped()
	{
		TodoTask task = new();
		List<TodoTask> children = [];

		for (int i = 1; i <= 10; i++)
		{
			TodoTask child = new($"Child {i}");
			
			Random random = new();

			child.SetStatus(random.Next(0, 2) == 1 ? TaskState.Finished : TaskState.Discarded);

			children.Add(child);
		}
		
		task.AddChildren(children);
		
		Assert.Equal(99, task.Progress);
	}
	
	[Fact]
	public void Progress_ShouldBe100_When_CurrentTaskIsCompletedOrDropped()
	{
		TodoTask completed = new();
		TodoTask dropped = new();
		List<TodoTask> children = [];

		for (int i = 1; i <= 10; i++)
		{
			TodoTask child = new($"Child {i}");
			child.SetStatus(TaskState.ReadyToStart, DateTimeOffset.Now);
			children.Add(child);
		}
		
		completed.AddChildren(children);
		dropped.AddChildren(children);
		completed.SetStatus(TaskState.Finished);
		dropped.SetStatus(TaskState.Discarded);
		
		Assert.Equal(100, completed.Progress);
		Assert.Equal(100, dropped.Progress);
	}

	[Fact]
	public void Progress_ShouldBe0_When_NoChildrenAndTaskIsNotCompletedNorDropped()
	{
		TodoTask task = new();
		
		Assert.Equal(0, task.Progress);
	}

	[Fact]
	public void TodoTask_ShouldHave_MultipleLifeAreas()
	{
		Assert.NotNull(typeof(TodoTask).GetProperty("LifeAreas"));
		Assert.True(typeof(TodoTask).GetProperty("LifeAreas").PropertyType == typeof(IReadOnlyList<LifeArea>));
	}
	
	[Fact]
	public void LifeAreas_ShouldBeReadOnly()
	{
		Assert.False(typeof(TodoTask).GetProperty("LifeAreas").CanWrite);
	}

	[Fact]
	public void LifeAreas_OnlyAddedVia_AddLifeArea()
	{
		LifeArea la1 = new("Life Area 1");
		LifeArea la2 = new("Life Area 2");
		TodoTask task = new();
		
		task.AddLifeAreas([ la1, la2 ]);
		
		Assert.True(task.LifeAreas.Count != 0);
		Assert.Contains(la1, task.LifeAreas);
		Assert.Contains(la2, task.LifeAreas);
	}
}
