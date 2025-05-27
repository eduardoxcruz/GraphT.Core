using GraphT.Model.Aggregates;
using GraphT.Model.ValueObjects;

using Microsoft.EntityFrameworkCore;

using SeedWork;

namespace GraphT.EfCore.Repositories.Tests.Repositories;

public class TaskAggregateRepository : IClassFixture<TestDatabaseFixture>
{
	private TestDatabaseFixture Fixture { get; }

	public TaskAggregateRepository(TestDatabaseFixture fixture) => Fixture = fixture;
	
	[Fact]
	public async Task FindByIdAsync_ReturnsCorrectEntity()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<TaskAggregate> repository = new(context);
		TaskAggregate task = new("Test Task");

		await context.TaskAggregates.AddAsync(task);
		await context.SaveChangesAsync();
		TaskAggregate? result = await repository.FindByIdAsync(task.Id);

		Assert.NotNull(result);
		Assert.Equal(task.Id, result.Id);
		Assert.Equal("Test Task", result.Name);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
	}

	[Fact]
	public async Task Find_WithSpecification_ReturnsPagedList()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<TaskAggregate> repository = new(context);
		Status expectedStatus = Status.Doing;
		List<TaskAggregate> tasks =
		[
			new("Task 1"), new("Task 2", expectedStatus), new("Task 3", expectedStatus)
			, new("Task 4"), new("Task 5", expectedStatus), new("Task 6", expectedStatus)
			, new("Task 7"), new("Task 8", expectedStatus), new("Task 9", expectedStatus)
		];

		await context.TaskAggregates.AddRangeAsync(tasks);
		await context.SaveChangesAsync();
		PagedList<TaskAggregate> results = await repository.FindAsync(new BaseSpecification<TaskAggregate>(t => t.Status == expectedStatus));

		Assert.NotNull(results);
		Assert.NotEmpty(results);
		Assert.All(results, TaskAggregate => Assert.Equal(expectedStatus, TaskAggregate.Status));
		Assert.Equal(6, results.TotalCount);
		Assert.Equal(1, results.CurrentPage);
		Assert.Equal(1, results.TotalPages);
		Assert.False(results.HasNext);
		Assert.False(results.HasPrevious);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
	}
	
	[Fact]
	public async Task Find_ReturnsAllEntities()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<TaskAggregate> repository = new(context);
		List<TaskAggregate> tasks =
		[
			new("Task 1"), new("Task 2", Status.Ready), new("Task 3", Status.Paused)
			, new("Task 4", Status.Dropped), new("Task 5", Status.Completed)
		];

		await context.TaskAggregates.AddRangeAsync(tasks);
		await context.SaveChangesAsync();
		PagedList<TaskAggregate> results = await repository.FindAsync();

		Assert.NotNull(results);
		Assert.NotEmpty(results);
		Assert.Equal(5, results.TotalCount);
		Assert.Equal(1, results.CurrentPage);
		Assert.Equal(1, results.TotalPages);
		Assert.False(results.HasNext);
		Assert.False(results.HasPrevious);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
	}

	[Fact]
	public async Task AddAsync_AddsEntityToContext()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<TaskAggregate> repository = new(context);
		TaskAggregate task = new("New Task");

		await repository.AddAsync(task);
		await context.SaveChangesAsync();
		TaskAggregate? addedTask = await context.TaskAggregates.FindAsync(task.Id);

		Assert.NotNull(addedTask);
		Assert.Equal(task.Id, addedTask.Id);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
	}
	
	[Fact]
	public async Task AddRangeAsync_AddsEntitiesToContext()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<TaskAggregate> repository = new(context);
		List<TaskAggregate> tasks =
		[
			new("Task 1"), new("Task 2", Status.Ready), new("Task 3", Status.Paused)
			, new("Task 4", Status.Dropped), new("Task 5", Status.Completed)
		];

		await repository.AddRangeAsync(tasks);
		await context.SaveChangesAsync();

		foreach (TaskAggregate TaskAggregate in tasks)
		{
			TaskAggregate? addedTask = await context.TaskAggregates.FindAsync(TaskAggregate.Id);
			
			Assert.NotNull(addedTask);
			Assert.Equal(TaskAggregate.Id, addedTask.Id);
		}

		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
	}

	[Fact]
	public async Task RemoveAsync_RemovesEntityFromContext()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<TaskAggregate> repository = new(context);
		TaskAggregate task = new("Task to Remove");

		await context.TaskAggregates.AddAsync(task);
		await context.SaveChangesAsync();
		await repository.RemoveAsync(task);
		await context.SaveChangesAsync();
		TaskAggregate? removedTask = await context.TaskAggregates.FindAsync(task.Id);

		Assert.Null(removedTask);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
	}
	
	[Fact]
	public async Task RemoveRangeAsync_RemovesEntitiesFromContext()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<TaskAggregate> repository = new(context);
		List<TaskAggregate> tasks =
		[
			new("Task 1"), new("Task 2", Status.Ready), new("Task 3", Status.Paused)
			, new("Task 4", Status.Dropped), new("Task 5", Status.Completed)
		];

		await context.TaskAggregates.AddRangeAsync(tasks);
		await context.SaveChangesAsync();
		await repository.RemoveRangeAsync(tasks);
		await context.SaveChangesAsync();

		foreach (TaskAggregate TaskAggregate in tasks)
		{
			TaskAggregate? removedTask = await context.TaskAggregates.FindAsync(TaskAggregate.Id);
			
			Assert.Null(removedTask);
		}

		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
	}

	[Fact]
	public async Task UpdateAsync_UpdatesEntityInContext()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<TaskAggregate> repository = new(context);
		TaskAggregate task = new("Task to Update");
		string newName = "New task name";

		await context.TaskAggregates.AddAsync(task);
		await context.SaveChangesAsync();
		task.Name = newName;
		await repository.UpdateAsync(task);
		await context.SaveChangesAsync();
		TaskAggregate? updatedTask = await context.TaskAggregates.FindAsync(task.Id);

		Assert.NotNull(updatedTask);
		Assert.Equal(newName, updatedTask.Name);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
	}
	
	[Fact]
	public async Task UpdateRangeAsync_UpdatesEntitiesInContex()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<TaskAggregate> repository = new(context);
		List<TaskAggregate> tasks = new()
		{
			new TaskAggregate("Task 1"), 
			new TaskAggregate("Task 2", Status.Ready), 
			new TaskAggregate("Task 3", Status.Paused)
		};

		await context.TaskAggregates.AddRangeAsync(tasks);
		await context.SaveChangesAsync();
		tasks[0].Name = "Task 4";
		tasks[1].Name = "Task 5";
		tasks[2].Name = "Task 6";
		await repository.UpdateRangeAsync(tasks);
		await context.SaveChangesAsync();
		TaskAggregate? firstUpdated = await context.TaskAggregates.FindAsync(tasks[0].Id);
		TaskAggregate? secondUpdated = await context.TaskAggregates.FindAsync(tasks[1].Id);
		TaskAggregate? thirdUpdated = await context.TaskAggregates.FindAsync(tasks[2].Id);

		Assert.NotNull(firstUpdated);
		Assert.NotNull(secondUpdated);
		Assert.NotNull(thirdUpdated);
		Assert.Equal("Task 4", firstUpdated.Name);
		Assert.Equal("Task 5", secondUpdated.Name);
		Assert.Equal("Task 6", thirdUpdated.Name);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
	}

	[Fact]
	public async Task ContainsAsync_WithSpecification_ReturnsCorrectResult()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<TaskAggregate> repository = new(context);
		Status expectedStatus = Status.Paused;
		TaskAggregate task = new("Backlog Task", expectedStatus);

		await context.TaskAggregates.AddAsync(task);
		await context.SaveChangesAsync();
		bool result = await repository.ContainsAsync(new BaseSpecification<TaskAggregate>(t => t.Id.Equals(task.Id)));

		Assert.True(result);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
	}
	
	[Fact]
	public async Task ContainsAsync_WithPredicate_ReturnsCorrectResult()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<TaskAggregate> repository = new(context);
		Status expectedStatus = Status.Dropped;
		TaskAggregate task = new("Backlog Task", expectedStatus);

		await context.TaskAggregates.AddAsync(task);
		await context.SaveChangesAsync();
		bool result = await repository.ContainsAsync(TaskAggregate => TaskAggregate.Id.Equals(task.Id));

		Assert.True(result);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
	}

	[Fact]
	public async Task CountAsync_WithSpecification_ReturnsCorrectCount()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<TaskAggregate> repository = new(context);
		Status expectedStatus = Status.Completed;
		List<TaskAggregate> tasks = new()
		{
			new TaskAggregate("Task 1"), 
			new TaskAggregate("Task 2"), 
			new TaskAggregate("Task 3", expectedStatus)
		};

		await context.TaskAggregates.AddRangeAsync(tasks);
		await context.SaveChangesAsync();
		int count = await repository.CountAsync(new BaseSpecification<TaskAggregate>(t => t.Status == expectedStatus));

		Assert.Equal(1, count);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
	}
	
	[Fact]
	public async Task CountAsync_WithPredicate_ReturnsCorrectCount()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<TaskAggregate> repository = new(context);
		Status expectedStatus = Status.Ready;
		List<TaskAggregate> tasks = new()
		{
			new TaskAggregate("Task 1", expectedStatus), 
			new TaskAggregate("Task 2", expectedStatus), 
			new TaskAggregate("Task 3", expectedStatus),
			new TaskAggregate("Task 4")
		};

		await context.TaskAggregates.AddRangeAsync(tasks);
		await context.SaveChangesAsync();
		int count = await repository.CountAsync(TaskAggregate => TaskAggregate.Status == expectedStatus);

		Assert.Equal(3, count);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
	}
}
