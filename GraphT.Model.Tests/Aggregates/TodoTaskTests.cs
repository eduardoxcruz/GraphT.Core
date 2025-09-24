using System.Reflection;

using GraphT.Model.Aggregates;
using GraphT.Model.ValueObjects;

namespace GraphT.Model.Tests.Aggregates;

public class TodoTaskTests
{
	[Fact]
	public void CreateTodo_ShouldCreateWithDefaultName_WhenNameNotProvided()
	{
		TodoTask todo = new();
		
		Assert.Equal("New Todo Task", todo.Name);
		Assert.NotEqual(Guid.Empty, todo.Id);
	}
	
	[Fact]
	public void Todo_ShouldHaveName_WhenCreated()
	{
		string name = "Test";
		
		TodoTask todo = new(name);

		Assert.NotNull(todo);
		Assert.Equal(name, todo.Name);
	}
	
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
	public void CreateTodo_ShouldHaveUniqueId_WhenCreated()
	{
		string name = "Test";
    
		TodoTask todo = new(name);

		Assert.NotEqual(Guid.Empty, todo.Id);
	}
	
	[Fact]
	public void CreateTodo_ShouldGenerateUniqueIds_ForLargeNumberOfTodos()
	{
		const int totalTodos = 1000;
		var todos = new List<TodoTask>();

		for (int i = 0; i < totalTodos; i++)
		{
			todos.Add(new TodoTask($"Todo {i}"));
		}

		List<Guid> allIds = todos.Select(TodoTask => TodoTask.Id).ToList();
		List<Guid> distinctIds = allIds.Distinct().ToList();

		Assert.Equal(totalTodos, distinctIds.Count);
	}
	
	[Fact]
	public void Id_ShouldBeReadOnly()
	{
		TodoTask todo = new("Test");

		PropertyInfo? idProperty = typeof(TodoTask).GetProperty("Id");
    
		Assert.NotNull(idProperty);
		Assert.True(idProperty.CanRead, "Id should be readable");
		Assert.False(idProperty.CanWrite, "Id should not be publicly writable");
	}

	[Fact]
	public void TodoTask_ShouldHaveRelevance_WhenCreated()
	{
		TodoTask todo = new("Test");
		
		Assert.Equal(todo.Relevance, Relevance.Superfluous);
	}

	[Fact]
	public void TodoTask_Relevance_ShouldBeReadOnly()
	{
		var todo = new TodoTask("Test");

		var prop = typeof(TodoTask).GetProperty("Relevance");

		Assert.NotNull(prop);
		Assert.True(prop.CanRead, "Relevance should be readable");
		Assert.False(prop.CanWrite, "Relevance should not be publicly writable");
	}

	[Fact]
	public void Relevance_ShouldUpdate_WhenIsFunOrIsProductiveChanges()
	{
		TodoTask todo = new TodoTask("Test")
		{
			IsFun = false,
			IsProductive = false
		};

		Assert.Equal("\ud83d\ude12", todo.Relevance.Emoji);

		todo.IsFun = true;
		todo.IsProductive = false;
		Assert.Equal("\ud83e\udd24", todo.Relevance.Emoji);

		todo.IsFun = false;
		todo.IsProductive = true;
		Assert.Equal("\ud83e\uddd0", todo.Relevance.Emoji);
		
		todo.IsFun = true;
		todo.IsProductive = true;
		Assert.Equal("\ud83d\ude0e", todo.Relevance.Emoji);
	}
	
	[Fact]
	public void TodoTask_ShouldHave_ComplexityIndefinite_WhenCreated()
	{
		TodoTask item = new("Test");
		
		Assert.Equal(item.Complexity, Complexity.Indefinite);
	}
	
	[Fact]
	public void TodoTask_ShouldHave_PriorityDistraction_WhenCreated()
	{
		TodoTask item = new("Test");
		
		Assert.Equal(item.Priority, Priority.Distraction);
	}
	
	[Fact]
	public void TodoTask_ShouldHave_StatusCreated_WhenCreated()
	{
		TodoTask item = new("Test");
		
		Assert.Equal(item.Status, Status.Created);
	}

	[Fact]
	public void Status_OnlyChangedVia_SetStatus()
	{
		TodoTask todo = new();
		Status expected = Status.Backlog;
		
		todo.SetStatus(expected);
		
		Assert.Equal(expected, todo.Status);
		Assert.True(typeof(TodoTask).GetProperty("Status").SetMethod.IsPrivate);
	}

	[Fact]
	public void TodoTask_ShouldHave_LimitDateTime()
	{
		Assert.NotNull(typeof(TodoTask).GetProperty("LimitDateTime"));
	}

	[Fact]
	public void LimitDateTime_OnlyChangedVia_SetLimitDateTime()
	{
		TodoTask todo = new("Test");
		DateTimeOffset expected = DateTimeOffset.Now;
		todo.SetLimitDateTime(expected);
		
		Assert.Equal(expected, todo.LimitDateTime);
		Assert.True(typeof(TodoTask).GetProperty("LimitDateTime").SetMethod.IsPrivate);
		Assert.NotNull(typeof(TodoTask).GetMethod("SetLimitDateTime"));
	}

	[Fact]
	public void TodoTask_ShouldHave_ReadonlyPunctuality()
	{
		Assert.NotNull(typeof(TodoTask).GetProperty("Punctuality"));
		Assert.False(typeof(TodoTask).GetProperty("Punctuality").CanWrite);
	}

	[Fact]
	public void Punctuality_ReturnsNoTargetWhenLimitDateTimeNotSet()
	{
		TodoTask todo = new();
		
		Assert.Equal("\u26a0 No Target", todo.Punctuality);
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
		
		todo.SetLimitDateTime(limitDateTime);
		todo.SetStatus(
			limitDateTime
				.AddDays(daysDifference)
				.AddHours(hoursDifference)
				.AddMinutes(minutesDifference)
				.AddSeconds(secondsDifference), 
			Status.Completed);
		
		Assert.Equal($"\u2b50 Early {Math.Abs(daysDifference)} day(s) - {Math.Abs(hoursDifference)} hours(s) - {Math.Abs(minutesDifference)} minute(s) - {Math.Abs(secondsDifference)} second(s)!", todo.Punctuality);
	}

	[Theory]
	[InlineData(-1, 0, 0, 0)]
	[InlineData(0, -1, 0, 0)]
	[InlineData(0, 0, -1, 0)]
	[InlineData(0, 0, 0, -1)]
	[InlineData(0, 0, 0, 0)]
	public void Punctuality_ReturnsOnTimeWhenFinishedOnLimitDateOrWithin24Hours(double daysDifference, double hoursDifference, double minutesDifference = 0, double secondsDifference = 0)
	{
		TodoTask todo = new("Test");
		DateTimeOffset limitDateTime = DateTimeOffset.Now;
		
		todo.SetLimitDateTime(limitDateTime);
		todo.SetStatus(
			limitDateTime
				.AddDays(daysDifference)
				.AddHours(hoursDifference)
				.AddMinutes(minutesDifference)
				.AddSeconds(secondsDifference), 
			Status.Completed);
		
		Assert.Equal("\u2705 On Time!", todo.Punctuality);
	}

	[Theory]
	[InlineData(0, 0, 0, 1)]
	[InlineData(0, 0, 1, 0)]
	[InlineData(0, 1, 0, 0)]
	[InlineData(1, 0, 0, 0)]
	public void Punctuality_ReturnsLateDaysWhenFinishedAfterLimitDate(double daysDifference, double hoursDifference, double minutesDifference = 0, double secondsDifference = 0)
	{
		TodoTask todo = new("Test");
		DateTimeOffset limitDateTime = DateTimeOffset.Now;
		
		todo.SetLimitDateTime(limitDateTime);
		todo.SetStatus(
			limitDateTime
				.AddDays(daysDifference)
				.AddHours(hoursDifference)
				.AddMinutes(minutesDifference)
				.AddSeconds(secondsDifference), 
			Status.Completed);
		
		Assert.Equal($"\ud83d\udea8 Late {Math.Abs(daysDifference)} day(s) - {Math.Abs(hoursDifference)} hours(s) - {Math.Abs(minutesDifference)} minute(s) - {Math.Abs(secondsDifference)} second(s)!", todo.Punctuality);
	}

	[Theory]
	[InlineData(2, 0, 0, 0)]
	[InlineData(1, 1, 0, 0)]
	[InlineData(1, 0, 1, 0)]
	[InlineData(1, 0, 0, 1)]
	public void Punctuality_ReturnsDaysToGoWhenCurrentDateIsBeforeLimitDate(double daysDifference, double hoursDifference, double minutesDifference = 0, double secondsDifference = 0)
	{
		string expected =
			$"\u23f1 {Math.Abs(daysDifference)} day(s) - {Math.Abs(hoursDifference)} hours(s) - {Math.Abs(minutesDifference)} minute(s) - {Math.Abs(secondsDifference)} second(s) To Go!";
		TodoTask todo = new();
		DateTimeOffset limitDateTime = DateTimeOffset.Now;
		
		todo.SetLimitDateTime(limitDateTime
			.AddDays(daysDifference)
			.AddHours(hoursDifference)
			.AddMinutes(minutesDifference)
			.AddSeconds(secondsDifference));
		
		Assert.Equal(expected, todo.Punctuality);
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
		
		todo.SetLimitDateTime(limitDateTime
			.AddDays(daysDifference)
			.AddHours(hoursDifference)
			.AddMinutes(minutesDifference)
			.AddSeconds(secondsDifference));
		
		Assert.Equal($"\u26a0 Finish Today!", todo.Punctuality);
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
		
		todo.SetLimitDateTime(limitDateTime
			.AddDays(daysDifference)
			.AddHours(hoursDifference)
			.AddMinutes(minutesDifference)
			.AddSeconds(secondsDifference));
		
		Assert.Equal(expected, todo.Punctuality);
	}
	
	[Fact]
	public void TodoTask_ShouldHaveReadonly_LinkedListOfStatusChangelogs()
	{
		Assert.NotNull(typeof(TodoTask).GetProperty("StatusChangeLogs"));
		Assert.True(typeof(TodoTask).GetProperty("StatusChangeLogs").SetMethod.IsPrivate);
		Assert.True(typeof(TodoTask).GetProperty("StatusChangeLogs").PropertyType == typeof(LinkedList<StatusChangelog>));
	}

	[Fact]
	public void TodoTask_ShouldAddStatusCreated_ToStatusChangelogWhenCreated()
	{
		TodoTask todo = new();
		Status expectedStatus = Status.Created;
		StatusChangelog log = todo.StatusChangeLogs.First();
		
		Assert.True(todo.StatusChangeLogs.Count != 0);
		Assert.True(todo.StatusChangeLogs.Count == 1);
		Assert.Equal(expectedStatus, log.Status);
	}
	
	[Fact]
	public void ChangeStatus_ShouldAddStatusChangelog()
	{
		TodoTask todo = new();
		Status expectedStatus = Status.Backlog;
		
		todo.SetStatus(expectedStatus);
		StatusChangelog log = todo.StatusChangeLogs.Last();
		
		Assert.True(todo.StatusChangeLogs.Count != 0);
		Assert.True(todo.StatusChangeLogs.Count == 2);
		Assert.Equal(expectedStatus, log.Status);
	}

	[Fact]
	public void TodoTask_ShouldHave_MultipleParents()
	{
		Assert.NotNull(typeof(TodoTask).GetProperty("Parents"));
		Assert.True(typeof(TodoTask).GetProperty("Parents").PropertyType == typeof(IReadOnlySet<TodoTask>));
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
		Assert.True(typeof(TodoTask).GetProperty("Children").PropertyType == typeof(IReadOnlySet<TodoTask>));
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
	public void ElapsedTime_ShouldBeReadOnly()
	{
		Assert.False(typeof(TodoTask).GetProperty("ElapsedTime").CanWrite);
	}

	[Fact]
	public void ElapsedTime_ShouldBe_StringFormated_With_Days_Hours_Minutes_Seconds_WhenCalledToString()
	{
		TodoTask task = new();
		
		Assert.Equal("\u23f0 0 day(s) - 0 hour(s) - 0 minute(s) - 0 second(s)", task.ElapsedTimeFormatted);
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
		
		task.SetStatus(dateTimeLog1, Status.Backlog);
		task.SetStatus(dateTimeLog2, Status.ReadyToStart);
		task.SetStatus(currentlyDoingLog1, Status.CurrentlyDoing);
		task.SetStatus(dateTimeLog4, Status.Paused);
		task.SetStatus(dateTimeLog5, Status.ReadyToStart);
		task.SetStatus(currentlyDoingLog2, Status.CurrentlyDoing);
		task.SetStatus(dateTimeLog7, Status.Dropped);
		task.SetStatus(currentlyDoingLog3, Status.CurrentlyDoing);
		task.SetStatus(dateTimeLog9, Status.Backlog);
		task.SetStatus(currentlyDoingLog4, Status.CurrentlyDoing);
		task.SetStatus(dateTimeLog11, Status.Completed);
		task.SetStatus(currentlyDoingLog5, Status.CurrentlyDoing);
		task.SetStatus(dateTimeLog13, Status.Completed);
		
		Assert.Equal("\u23f0 3 day(s) - 4 hour(s) - 16 minute(s) - 5 second(s)", task.ElapsedTimeFormatted);
	}

	[Fact]
	public void TodoTask_ShouldHave_Progress()
	{
		Assert.NotNull(typeof(TodoTask).GetProperty("Progress"));
	}
	
	[Fact]
	public void Progress_ShouldBeReadOnly()
	{
		Assert.False(typeof(TodoTask).GetProperty("Progress").CanWrite);
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
		HashSet<TodoTask> children = [];

		for (int i = 1; i <= maxChildren; i++)
		{
			TodoTask child = new($"Child {i}");

			if (i < maxChildren)
			{
				Random random = new();
			
				if (random.Next(0, 2) == 1)
				{
					child.SetStatus(random.Next(0, 2) == 1 ? Status.Completed : Status.Dropped);
				}
			}
			
			children.Add(child);
		}
		
		int childrenCompleted = children.Count(t => t.Status.Index > 4);
		double expected = ((childrenCompleted * 100) / maxChildren);
		
		task.AddChildren(children);
		
		Assert.Equal(expected, task.Progress);
	}
	
	[Fact]
	public void Progress_ShouldBe99_WhenAllChildrenAreCompletedOrDropped_ButCurrentTaskNotMarkedAsFinishedOrDropped()
	{
		TodoTask task = new();
		HashSet<TodoTask> children = [];

		for (int i = 1; i <= 10; i++)
		{
			TodoTask child = new($"Child {i}");
			
			Random random = new();

			child.SetStatus(random.Next(0, 2) == 1 ? Status.Completed : Status.Dropped);

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
		HashSet<TodoTask> children = [];

		for (int i = 1; i <= 10; i++)
		{
			TodoTask child = new($"Child {i}");
			child.SetStatus(DateTimeOffset.Now, Status.ReadyToStart);
			children.Add(child);
		}
		
		completed.AddChildren(children);
		dropped.AddChildren(children);
		completed.SetStatus(Status.Completed);
		dropped.SetStatus(Status.Dropped);
		
		Assert.Equal(100, completed.Progress);
		Assert.Equal(100, dropped.Progress);
	}

	[Fact]
	public void Progress_ShouldBe0_When_NoChildrenAndTaskIsNotCompletedNorDropped()
	{
		TodoTask task = new();
		
		Assert.Equal(0, task.Progress);
	}
}
