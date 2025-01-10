using GraphT.Model.Aggregates;
using GraphT.Model.ValueObjects;

using SeedWork;

namespace GraphT.EfCore.Repositories.Tests;

public class RepositoryTests : TestBase
{
	private readonly Repository<TodoTask> _repository;

	public RepositoryTests()
	{
		_repository = new Repository<TodoTask>(_context);
	}

	[Fact]
	public async Task FindByIdAsync_ReturnsCorrectEntity()
	{
		TodoTask task = new("Test Task");
		
		await _context.TodoTasks.AddAsync(task);
		await _context.SaveChangesAsync();
		TodoTask? result = await _repository.FindByIdAsync(task.Id);
		
		Assert.NotNull(result);
		Assert.Equal(task.Id, result.Id);
		Assert.Equal("Test Task", result.Name);
	}

	[Fact]
	public async Task Find_WithSpecification_ReturnsPagedList()
	{
		Status expectedStatus = Status.InProgress;
		List<TodoTask> tasks = new()
		{
			new TodoTask("Task 1"), 
			new TodoTask("Task 2", expectedStatus), 
			new TodoTask("Task 3", expectedStatus),
			new TodoTask("Task 4"), 
			new TodoTask("Task 5", expectedStatus), 
			new TodoTask("Task 6", expectedStatus),
			new TodoTask("Task 7"), 
			new TodoTask("Task 8", expectedStatus), 
			new TodoTask("Task 9", expectedStatus)
		};
		TasksWithSpecificStatusSpecification spec = new(expectedStatus, 2, 2);
		
		await _context.TodoTasks.AddRangeAsync(tasks);
		await _context.SaveChangesAsync();
		PagedList<TodoTask> results = await _repository.FindAsync(spec);
		
		Assert.NotNull(results);
		Assert.NotEmpty(results);
		Assert.All(results, todoTask => Assert.Equal(expectedStatus, todoTask.Status));
		Assert.Equal(2, results.Count);
		Assert.Equal(6, results.TotalCount);
		Assert.Equal(2, results.CurrentPage);
		Assert.Equal(3, results.TotalPages);
		Assert.True(results.HasNext);
		Assert.True(results.HasPrevious);
	}
	
	[Fact]
	public async Task Find_ReturnsAllEntities()
	{
		List<TodoTask> tasks = new()
		{
			new TodoTask("Task 1"), 
			new TodoTask("Task 2", Status.ReadyToStart), 
			new TodoTask("Task 3", Status.Paused),
			new TodoTask("Task 4", Status.Dropped),
			new TodoTask("Task 5", Status.Completed)
		};
		
		await _context.TodoTasks.AddRangeAsync(tasks);
		await _context.SaveChangesAsync();
		PagedList<TodoTask> results = await _repository.FindAsync();
		
		Assert.Equal(5, results.Count);
	}

	[Fact]
	public async Task AddAsync_AddsEntityToContext()
	{
		TodoTask task = new("New Task");
		
		await _repository.AddAsync(task);
		await _context.SaveChangesAsync();
		TodoTask? addedTask = await _context.TodoTasks.FindAsync(task.Id);
		
		Assert.NotNull(addedTask);
		Assert.Equal(task.Id, addedTask.Id);
	}
	
	[Fact]
	public async Task AddRangeAsync_AddsEntitiesToContext()
	{
		List<TodoTask> tasks = new()
		{
			new TodoTask("Task 1"), 
			new TodoTask("Task 2", Status.ReadyToStart), 
			new TodoTask("Task 3", Status.Paused),
			new TodoTask("Task 4", Status.Dropped),
			new TodoTask("Task 5", Status.Completed)
		};
		
		await _repository.AddRangeAsync(tasks);
		await _context.SaveChangesAsync();

		foreach (TodoTask todoTask in tasks)
		{
			TodoTask? addedTask = await _context.TodoTasks.FindAsync(todoTask.Id);
			
			Assert.NotNull(addedTask);
			Assert.Equal(todoTask.Id, addedTask.Id);
		}
	}

	[Fact]
	public async Task RemoveAsync_RemovesEntityFromContext()
	{
		TodoTask task = new("Task to Remove");
		
		await _context.TodoTasks.AddAsync(task);
		await _context.SaveChangesAsync();
		await _repository.RemoveAsync(task);
		await _context.SaveChangesAsync();
		TodoTask? removedTask = await _context.TodoTasks.FindAsync(task.Id);
		
		Assert.Null(removedTask);
	}
	
	[Fact]
	public async Task RemoveRangeAsync_RemovesEntitiesFromContext()
	{
		List<TodoTask> tasks = new()
		{
			new TodoTask("Task 1"), 
			new TodoTask("Task 2", Status.ReadyToStart), 
			new TodoTask("Task 3", Status.Paused),
			new TodoTask("Task 4", Status.Dropped),
			new TodoTask("Task 5", Status.Completed)
		};
		
		await _context.TodoTasks.AddRangeAsync(tasks);
		await _context.SaveChangesAsync();
		await _repository.RemoveRangeAsync(tasks);
		await _context.SaveChangesAsync();
		
		foreach (TodoTask todoTask in tasks)
		{
			TodoTask? removedTask = await _context.TodoTasks.FindAsync(todoTask.Id);
			
			Assert.Null(removedTask);
		}
	}

	[Fact]
	public async Task UpdateAsync_UpdatesEntityInContext()
	{
		TodoTask task = new("Task to Update");
		string newName = "New task name";
		
		await _context.TodoTasks.AddAsync(task);
		await _context.SaveChangesAsync();
		task.Name = newName;
		await _repository.UpdateAsync(task);
		await _context.SaveChangesAsync();
		TodoTask? updatedTask = await _context.TodoTasks.FindAsync(task.Id);

		Assert.NotNull(updatedTask);
		Assert.Equal(newName, updatedTask.Name);
	}
	
	[Fact]
	public async Task UpdateRangeAsync_UpdatesEntitiesInContex()
	{
		List<TodoTask> tasks = new()
		{
			new TodoTask("Task 1"), 
			new TodoTask("Task 2", Status.ReadyToStart), 
			new TodoTask("Task 3", Status.Paused)
		};
		
		await _context.TodoTasks.AddRangeAsync(tasks);
		await _context.SaveChangesAsync();
		tasks[0].Name = "Task 4";
		tasks[1].Name = "Task 5";
		tasks[2].Name = "Task 6";
		await _repository.UpdateRangeAsync(tasks);
		await _context.SaveChangesAsync();
		TodoTask? firstUpdated = await _context.TodoTasks.FindAsync(tasks[0].Id);
		TodoTask? secondUpdated = await _context.TodoTasks.FindAsync(tasks[1].Id);
		TodoTask? thirdUpdated = await _context.TodoTasks.FindAsync(tasks[2].Id);
		
		Assert.NotNull(firstUpdated);
		Assert.NotNull(secondUpdated);
		Assert.NotNull(thirdUpdated);
		Assert.Equal("Task 4", firstUpdated.Name);
		Assert.Equal("Task 5", secondUpdated.Name);
		Assert.Equal("Task 6", thirdUpdated.Name);
	}

	[Fact]
	public async Task ContainsAsync_WithSpecification_ReturnsCorrectResult()
	{
		Status expectedStatus = Status.Paused;
		TodoTask task = new("Backlog Task", expectedStatus);
		TasksWithSpecificStatusSpecification spec = new(expectedStatus);
		
		await _context.TodoTasks.AddAsync(task);
		await _context.SaveChangesAsync();
		bool result = await _repository.ContainsAsync(spec);

		Assert.True(result);
	}
	
	[Fact]
	public async Task ContainsAsync_WithPredicate_ReturnsCorrectResult()
	{
		Status expectedStatus = Status.Dropped;
		TodoTask task = new("Backlog Task", expectedStatus);
		
		await _context.TodoTasks.AddAsync(task);
		await _context.SaveChangesAsync();
		bool result = await _repository.ContainsAsync(todoTask => todoTask.Status == expectedStatus);

		Assert.True(result);
	}

	[Fact]
	public async Task CountAsync_WithSpecification_ReturnsCorrectCount()
	{
		Status expectedStatus = Status.Completed;
		List<TodoTask> tasks = new()
		{
			new TodoTask("Task 1"), 
			new TodoTask("Task 2"), 
			new TodoTask("Task 3", expectedStatus)
		};
		TasksWithSpecificStatusSpecification spec = new(expectedStatus);
		
		await _context.TodoTasks.AddRangeAsync(tasks);
		await _context.SaveChangesAsync();
		int count = await _repository.CountAsync(spec);

		Assert.Equal(1, count);
	}
	
	[Fact]
	public async Task CountAsync_WithPredicate_ReturnsCorrectCount()
	{
		Status expectedStatus = Status.ReadyToStart;
		List<TodoTask> tasks = new()
		{
			new TodoTask("Task 1", expectedStatus), 
			new TodoTask("Task 2", expectedStatus), 
			new TodoTask("Task 3", expectedStatus),
			new TodoTask("Task 4")
		};
		
		await _context.TodoTasks.AddRangeAsync(tasks);
		await _context.SaveChangesAsync();
		int count = await _repository.CountAsync(todoTask => todoTask.Status == expectedStatus);

		Assert.Equal(3, count);
	}
}
