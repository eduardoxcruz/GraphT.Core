using GraphT.Model.Aggregates;
using GraphT.Model.ValueObjects;

using Microsoft.EntityFrameworkCore;

using SeedWork;

namespace GraphT.EfCore.Repositories.Tests.Repositories;

public class TodoTaskRepository : IClassFixture<TestDatabaseFixture>
{
	private TestDatabaseFixture Fixture { get; }

	public TodoTaskRepository(TestDatabaseFixture fixture) => Fixture = fixture;

	[Fact]
	public async Task FindByIdAsync_ReturnsCorrectEntity()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<TodoTask> repository = new(context);
		TodoTask task = new("Test Task");
		
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
		await context.TodoTasks.AddAsync(task);
		await context.SaveChangesAsync();
		TodoTask? result = await repository.FindByIdAsync(task.Id);
		
		Assert.NotNull(result);
		Assert.Equal(task.Id, result.Id);
		Assert.Equal("Test Task", result.Name);
	}

	[Fact]
	public async Task Find_WithSpecification_ReturnsPagedList()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<TodoTask> repository = new(context);
		Status expectedStatus = Status.Doing;
		List<TodoTask> tasks =
		[
			new("Task 1"), new("Task 2", expectedStatus), new("Task 3", expectedStatus)
			, new("Task 4"), new("Task 5", expectedStatus), new("Task 6", expectedStatus)
			, new("Task 7"), new("Task 8", expectedStatus), new("Task 9", expectedStatus)
		];
		
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
		await context.TodoTasks.AddRangeAsync(tasks);
		await context.SaveChangesAsync();
		PagedList<TodoTask> results = await repository.FindAsync(new BaseSpecification<TodoTask>(t => t.Status == expectedStatus));
		
		Assert.NotNull(results);
		Assert.NotEmpty(results);
		Assert.All(results, todoTask => Assert.Equal(expectedStatus, todoTask.Status));
		Assert.Equal(6, results.TotalCount);
		Assert.Equal(1, results.CurrentPage);
		Assert.Equal(1, results.TotalPages);
		Assert.False(results.HasNext);
		Assert.False(results.HasPrevious);
	}
	
	[Fact]
	public async Task Find_ReturnsAllEntities()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<TodoTask> repository = new(context);
		List<TodoTask> tasks =
		[
			new("Task 1"), new("Task 2", Status.Ready), new("Task 3", Status.Paused)
			, new("Task 4", Status.Dropped), new("Task 5", Status.Completed)
		];
		
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
		await context.TodoTasks.AddRangeAsync(tasks);
		await context.SaveChangesAsync();
		PagedList<TodoTask> results = await repository.FindAsync();
		
		Assert.NotNull(results);
		Assert.NotEmpty(results);
		Assert.Equal(5, results.TotalCount);
		Assert.Equal(1, results.CurrentPage);
		Assert.Equal(1, results.TotalPages);
		Assert.False(results.HasNext);
		Assert.False(results.HasPrevious);
	}

	[Fact]
	public async Task AddAsync_AddsEntityToContext()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<TodoTask> repository = new(context);
		TodoTask task = new("New Task");
		
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
		await repository.AddAsync(task);
		await context.SaveChangesAsync();
		TodoTask? addedTask = await context.TodoTasks.FindAsync(task.Id);
		
		Assert.NotNull(addedTask);
		Assert.Equal(task.Id, addedTask.Id);
	}
	
	[Fact]
	public async Task AddRangeAsync_AddsEntitiesToContext()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<TodoTask> repository = new(context);
		List<TodoTask> tasks =
		[
			new("Task 1"), new("Task 2", Status.Ready), new("Task 3", Status.Paused)
			, new("Task 4", Status.Dropped), new("Task 5", Status.Completed)
		];
		
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
		await repository.AddRangeAsync(tasks);
		await context.SaveChangesAsync();

		foreach (TodoTask todoTask in tasks)
		{
			TodoTask? addedTask = await context.TodoTasks.FindAsync(todoTask.Id);
			
			Assert.NotNull(addedTask);
			Assert.Equal(todoTask.Id, addedTask.Id);
		}
	}

	[Fact]
	public async Task RemoveAsync_RemovesEntityFromContext()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<TodoTask> repository = new(context);
		TodoTask task = new("Task to Remove");
		
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
		await context.TodoTasks.AddAsync(task);
		await context.SaveChangesAsync();
		await repository.RemoveAsync(task);
		await context.SaveChangesAsync();
		TodoTask? removedTask = await context.TodoTasks.FindAsync(task.Id);
		
		Assert.Null(removedTask);
	}
	
	[Fact]
	public async Task RemoveRangeAsync_RemovesEntitiesFromContext()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<TodoTask> repository = new(context);
		List<TodoTask> tasks =
		[
			new("Task 1"), new("Task 2", Status.Ready), new("Task 3", Status.Paused)
			, new("Task 4", Status.Dropped), new("Task 5", Status.Completed)
		];
		
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
		await context.TodoTasks.AddRangeAsync(tasks);
		await context.SaveChangesAsync();
		await repository.RemoveRangeAsync(tasks);
		await context.SaveChangesAsync();
		
		foreach (TodoTask todoTask in tasks)
		{
			TodoTask? removedTask = await context.TodoTasks.FindAsync(todoTask.Id);
			
			Assert.Null(removedTask);
		}
	}

	[Fact]
	public async Task UpdateAsync_UpdatesEntityInContext()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<TodoTask> repository = new(context);
		TodoTask task = new("Task to Update");
		string newName = "New task name";
		
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
		await context.TodoTasks.AddAsync(task);
		await context.SaveChangesAsync();
		task.Name = newName;
		await repository.UpdateAsync(task);
		await context.SaveChangesAsync();
		TodoTask? updatedTask = await context.TodoTasks.FindAsync(task.Id);

		Assert.NotNull(updatedTask);
		Assert.Equal(newName, updatedTask.Name);
	}
	
	[Fact]
	public async Task UpdateRangeAsync_UpdatesEntitiesInContex()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<TodoTask> repository = new(context);
		List<TodoTask> tasks = new()
		{
			new TodoTask("Task 1"), 
			new TodoTask("Task 2", Status.Ready), 
			new TodoTask("Task 3", Status.Paused)
		};
		
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
		await context.TodoTasks.AddRangeAsync(tasks);
		await context.SaveChangesAsync();
		tasks[0].Name = "Task 4";
		tasks[1].Name = "Task 5";
		tasks[2].Name = "Task 6";
		await repository.UpdateRangeAsync(tasks);
		await context.SaveChangesAsync();
		TodoTask? firstUpdated = await context.TodoTasks.FindAsync(tasks[0].Id);
		TodoTask? secondUpdated = await context.TodoTasks.FindAsync(tasks[1].Id);
		TodoTask? thirdUpdated = await context.TodoTasks.FindAsync(tasks[2].Id);
		
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
		EfDbContext context = Fixture.CreateContext();
		Repository<TodoTask> repository = new(context);
		Status expectedStatus = Status.Paused;
		TodoTask task = new("Backlog Task", expectedStatus);
		
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
		await context.TodoTasks.AddAsync(task);
		await context.SaveChangesAsync();
		bool result = await repository.ContainsAsync(new BaseSpecification<TodoTask>(t => t.Id.Equals(task.Id)));

		Assert.True(result);
	}
	
	[Fact]
	public async Task ContainsAsync_WithPredicate_ReturnsCorrectResult()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<TodoTask> repository = new(context);
		Status expectedStatus = Status.Dropped;
		TodoTask task = new("Backlog Task", expectedStatus);
		
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
		await context.TodoTasks.AddAsync(task);
		await context.SaveChangesAsync();
		bool result = await repository.ContainsAsync(todoTask => todoTask.Id.Equals(task.Id));

		Assert.True(result);
	}

	[Fact]
	public async Task CountAsync_WithSpecification_ReturnsCorrectCount()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<TodoTask> repository = new(context);
		Status expectedStatus = Status.Completed;
		List<TodoTask> tasks = new()
		{
			new TodoTask("Task 1"), 
			new TodoTask("Task 2"), 
			new TodoTask("Task 3", expectedStatus)
		};
		
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
		await context.TodoTasks.AddRangeAsync(tasks);
		await context.SaveChangesAsync();
		int count = await repository.CountAsync(new BaseSpecification<TodoTask>(t => t.Status == expectedStatus));

		Assert.Equal(1, count);
	}
	
	[Fact]
	public async Task CountAsync_WithPredicate_ReturnsCorrectCount()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<TodoTask> repository = new(context);
		Status expectedStatus = Status.Ready;
		List<TodoTask> tasks = new()
		{
			new TodoTask("Task 1", expectedStatus), 
			new TodoTask("Task 2", expectedStatus), 
			new TodoTask("Task 3", expectedStatus),
			new TodoTask("Task 4")
		};
		
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
		await context.TodoTasks.AddRangeAsync(tasks);
		await context.SaveChangesAsync();
		int count = await repository.CountAsync(todoTask => todoTask.Status == expectedStatus);

		Assert.Equal(3, count);
	}
}
