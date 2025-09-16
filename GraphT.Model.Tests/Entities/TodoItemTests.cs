using System.Reflection;

using GraphT.Model.Entities;
using GraphT.Model.ValueObjects;

namespace GraphT.Model.Tests.Entities;

public class TodoItemTests
{
	[Fact]
	public void CreateTodo_ShouldCreateWithDefaultName_WhenNameNotProvided()
	{
		TodoItem todo = new();
		
		Assert.Equal("New Todo Task", todo.Name);
		Assert.NotEqual(Guid.Empty, todo.Id);
	}
	
	[Fact]
	public void Todo_ShouldHaveName_WhenCreated()
	{
		string name = "Test";
		
		TodoItem todo = new(name);

		Assert.NotNull(todo);
		Assert.Equal(name, todo.Name);
	}
	
	[Fact]
	public void CreateTodo_ShouldCreateTodo_WhenNameIsExtremelyLong()
	{
		string name = new('A', 10000);

		TodoItem todo = new(name);

		Assert.NotNull(todo);
		Assert.Equal(name, todo.Name);
	}
	
	[Fact]
	public void CreateTodo_ShouldThrowException_WhenNameIsEmpty()
	{
		string name = string.Empty;

		ArgumentException exception = Assert.Throws<ArgumentException>(() => new TodoItem(name));
		Assert.Equal("Name cannot be empty", exception.Message);
	}
	
	[Fact]
	public void CreateTodo_ShouldThrowException_WhenNameIsWhitespace()
	{
		string name = "   ";

		ArgumentException exception = Assert.Throws<ArgumentException>(() => new TodoItem(name));
		Assert.Equal("Name cannot be empty", exception.Message);
	}
	
	[Fact]
	public void CreateTodo_ShouldHaveUniqueId_WhenCreated()
	{
		string name = "Test";
    
		TodoItem todo = new(name);

		Assert.NotEqual(Guid.Empty, todo.Id);
	}
	
	[Fact]
	public void CreateTodo_ShouldGenerateUniqueIds_ForLargeNumberOfTodos()
	{
		const int totalTodos = 1000;
		var todos = new List<TodoItem>();

		for (int i = 0; i < totalTodos; i++)
		{
			todos.Add(new TodoItem($"Todo {i}"));
		}

		List<Guid> allIds = todos.Select(todoItem => todoItem.Id).ToList();
		List<Guid> distinctIds = allIds.Distinct().ToList();

		Assert.Equal(totalTodos, distinctIds.Count);
	}
	
	[Fact]
	public void Id_ShouldBeReadOnly()
	{
		TodoItem todo = new("Test");

		PropertyInfo? idProperty = typeof(TodoItem).GetProperty("Id");
    
		Assert.NotNull(idProperty);
		Assert.True(idProperty.CanRead, "Id should be readable");
		Assert.False(idProperty.CanWrite, "Id should not be publicly writable");
	}

	[Fact]
	public void TodoItem_ShouldHaveRelevance_WhenCreated()
	{
		TodoItem todo = new("Test");
		
		Assert.Equal(todo.Relevance, Relevance.Superfluous);
	}

	[Fact]
	public void TodoItem_Relevance_ShouldBeReadOnly()
	{
		var todo = new TodoItem("Test");

		var prop = typeof(TodoItem).GetProperty("Relevance");

		Assert.NotNull(prop);
		Assert.True(prop.CanRead, "Relevance should be readable");
		Assert.False(prop.CanWrite, "Relevance should not be publicly writable");
	}

	[Fact]
	public void Relevance_ShouldUpdate_WhenIsFunOrIsProductiveChanges()
	{
		TodoItem todo = new TodoItem("Test")
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
	public void TodoItem_ShouldHave_ComplexityIndefinite_WhenCreated()
	{
		TodoItem item = new("Test");
		
		Assert.Equal(item.Complexity, Complexity.Indefinite);
	}
	
	[Fact]
	public void TodoItem_ShouldHave_PriorityDistraction_WhenCreated()
	{
		TodoItem item = new("Test");
		
		Assert.Equal(item.Priority, Priority.Distraction);
	}
	
	[Fact]
	public void TodoItem_ShouldHave_StatusCreated_WhenCreated()
	{
		TodoItem item = new("Test");
		
		Assert.Equal(item.Status, Status.Created);
	}

	[Fact]
	public void Status_OnlyChangedVia_SetStatus()
	{
		TodoItem todo = new();
		Status expected = Status.Backlog;
		
		todo.SetStatus(expected);
		
		Assert.Equal(expected, todo.Status);
		Assert.True(typeof(TodoItem).GetProperty("Status").SetMethod.IsPrivate);
	}

	[Fact]
	public void TodoItem_ShouldHave_LimitDateTime()
	{
		Assert.NotNull(typeof(TodoItem).GetProperty("LimitDateTime"));
	}

	[Fact]
	public void LimitDateTime_OnlyChangedVia_SetLimitDateTime()
	{
		TodoItem todo = new("Test");
		DateTimeOffset expected = DateTimeOffset.Now;
		todo.SetLimitDateTime(expected);
		
		Assert.Equal(expected, todo.LimitDateTime);
		Assert.True(typeof(TodoItem).GetProperty("LimitDateTime").SetMethod.IsPrivate);
		Assert.NotNull(typeof(TodoItem).GetMethod("SetLimitDateTime"));
	}

	[Fact]
	public void TodoItem_ShouldHave_ReadonlyPunctuality()
	{
		Assert.NotNull(typeof(TodoItem).GetProperty("Punctuality"));
		Assert.False(typeof(TodoItem).GetProperty("Punctuality").CanWrite);
	}

	[Fact]
	public void Punctuality_ReturnsNoTargetWhenLimitDateTimeNotSet()
	{
		TodoItem todo = new();
		
		Assert.Equal("\u26a0 No Target", todo.Punctuality);
	}

	[Theory]
	[InlineData(-2, 0, 0, 0)]
	[InlineData(-1, -1, 0, 0)]
	[InlineData(-1, 0, -1, 0)]
	[InlineData(-1, 0, 0, -1)]
	public void Punctuality_ReturnsEarlyDaysWhenFinishedBeforeLimitDate(double daysDifference, double hoursDifference, double minutesDifference = 0, double secondsDifference = 0)
	{
		TodoItem todo = new("Test");
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
		TodoItem todo = new("Test");
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
		TodoItem todo = new("Test");
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
		TodoItem todo = new();
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
		TodoItem todo = new();
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
		TodoItem todo = new();
		DateTimeOffset limitDateTime = DateTimeOffset.Now;
		
		todo.SetLimitDateTime(limitDateTime
			.AddDays(daysDifference)
			.AddHours(hoursDifference)
			.AddMinutes(minutesDifference)
			.AddSeconds(secondsDifference));
		
		Assert.Equal(expected, todo.Punctuality);
	}
	
	[Fact]
	public void TodoItem_ShouldHaveReadonly_LinkedListOfStatusChangelogs()
	{
		Assert.NotNull(typeof(TodoItem).GetProperty("StatusChangeLogs"));
		Assert.True(typeof(TodoItem).GetProperty("StatusChangeLogs").SetMethod.IsPrivate);
		Assert.True(typeof(TodoItem).GetProperty("StatusChangeLogs").PropertyType == typeof(LinkedList<StatusChangelog>));
	}

	[Fact]
	public void TodoItem_ShouldAddStatusCreated_ToStatusChangelogWhenCreated()
	{
		TodoItem todo = new();
		Status expectedStatus = Status.Created;
		StatusChangelog log = todo.StatusChangeLogs.First();
		
		Assert.True(todo.StatusChangeLogs.Count != 0);
		Assert.True(todo.StatusChangeLogs.Count == 1);
		Assert.Equal(expectedStatus, log.Status);
	}
	
	[Fact]
	public void ChangeStatus_ShouldAddStatusChangelog()
	{
		TodoItem todo = new();
		Status expectedStatus = Status.Backlog;
		
		todo.SetStatus(expectedStatus);
		StatusChangelog log = todo.StatusChangeLogs.Last();
		
		Assert.True(todo.StatusChangeLogs.Count != 0);
		Assert.True(todo.StatusChangeLogs.Count == 2);
		Assert.Equal(expectedStatus, log.Status);
	}
}
