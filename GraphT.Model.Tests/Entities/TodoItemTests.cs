using System.Reflection;

using GraphT.Model.Entities;
using GraphT.Model.ValueObjects;

namespace GraphT.Model.Tests.Entities;

public class TodoItemTests
{
	[Fact]
	public void CreateTodo_ShouldHaveName_WhenCreated()
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
	public void TodoItem_Id_ShouldBeReadOnly()
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
}
