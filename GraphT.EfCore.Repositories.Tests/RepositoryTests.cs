using GraphT.Model.Aggregates;
using GraphT.Model.ValueObjects;

using SeedWork;

namespace GraphT.EfCore.Repositories.Tests;

public class RepositoryTests : TestBase
{
	private readonly Repository<TaskAggregate> _repository;

	public RepositoryTests()
	{
		_repository = new Repository<TaskAggregate>(_context);
	}

	[Fact]
	public async Task FindByIdAsync_ReturnsCorrectEntity()
	{
		TaskAggregate task = new("Test Task");
		
		await _context.TaskAggregates.AddAsync(task);
		await _context.SaveChangesAsync();
		TaskAggregate? result = await _repository.FindByIdAsync(task.Id);
		
		Assert.NotNull(result);
		Assert.Equal(task.Id, result.Id);
		Assert.Equal("Test Task", result.Name);
	}

	[Fact]
	public async Task Find_WithSpecification_ReturnsPagedList()
	{
		Status expectedStatus = Status.InProgress;
		List<TaskAggregate> tasks = new()
		{
			new TaskAggregate("Task 1"), 
			new TaskAggregate("Task 2", expectedStatus), 
			new TaskAggregate("Task 3", expectedStatus),
			new TaskAggregate("Task 4"), 
			new TaskAggregate("Task 5", expectedStatus), 
			new TaskAggregate("Task 6", expectedStatus),
			new TaskAggregate("Task 7"), 
			new TaskAggregate("Task 8", expectedStatus), 
			new TaskAggregate("Task 9", expectedStatus)
		};
		TasksWithSpecificStatusSpecification spec = new(expectedStatus, 2, 2);
		
		await _context.TaskAggregates.AddRangeAsync(tasks);
		await _context.SaveChangesAsync();
		PagedList<TaskAggregate> results = await _repository.FindAsync(spec);
		
		Assert.NotNull(results);
		Assert.NotEmpty(results);
		Assert.All(results, TaskAggregate => Assert.Equal(expectedStatus, TaskAggregate.Status));
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
		List<TaskAggregate> tasks = new()
		{
			new TaskAggregate("Task 1"), 
			new TaskAggregate("Task 2", Status.ReadyToStart), 
			new TaskAggregate("Task 3", Status.Paused),
			new TaskAggregate("Task 4", Status.Dropped),
			new TaskAggregate("Task 5", Status.Completed)
		};
		
		await _context.TaskAggregates.AddRangeAsync(tasks);
		await _context.SaveChangesAsync();
		PagedList<TaskAggregate> results = await _repository.FindAsync();
		
		Assert.Equal(5, results.Count);
	}

	[Fact]
	public async Task AddAsync_AddsEntityToContext()
	{
		TaskAggregate task = new("New Task");
		
		await _repository.AddAsync(task);
		await _context.SaveChangesAsync();
		TaskAggregate? addedTask = await _context.TaskAggregates.FindAsync(task.Id);
		
		Assert.NotNull(addedTask);
		Assert.Equal(task.Id, addedTask.Id);
	}
	
	[Fact]
	public async Task AddRangeAsync_AddsEntitiesToContext()
	{
		List<TaskAggregate> tasks = new()
		{
			new TaskAggregate("Task 1"), 
			new TaskAggregate("Task 2", Status.ReadyToStart), 
			new TaskAggregate("Task 3", Status.Paused),
			new TaskAggregate("Task 4", Status.Dropped),
			new TaskAggregate("Task 5", Status.Completed)
		};
		
		await _repository.AddRangeAsync(tasks);
		await _context.SaveChangesAsync();

		foreach (TaskAggregate TaskAggregate in tasks)
		{
			TaskAggregate? addedTask = await _context.TaskAggregates.FindAsync(TaskAggregate.Id);
			
			Assert.NotNull(addedTask);
			Assert.Equal(TaskAggregate.Id, addedTask.Id);
		}
	}

	[Fact]
	public async Task RemoveAsync_RemovesEntityFromContext()
	{
		TaskAggregate task = new("Task to Remove");
		
		await _context.TaskAggregates.AddAsync(task);
		await _context.SaveChangesAsync();
		await _repository.RemoveAsync(task);
		await _context.SaveChangesAsync();
		TaskAggregate? removedTask = await _context.TaskAggregates.FindAsync(task.Id);
		
		Assert.Null(removedTask);
	}
	
	[Fact]
	public async Task RemoveRangeAsync_RemovesEntitiesFromContext()
	{
		List<TaskAggregate> tasks = new()
		{
			new TaskAggregate("Task 1"), 
			new TaskAggregate("Task 2", Status.ReadyToStart), 
			new TaskAggregate("Task 3", Status.Paused),
			new TaskAggregate("Task 4", Status.Dropped),
			new TaskAggregate("Task 5", Status.Completed)
		};
		
		await _context.TaskAggregates.AddRangeAsync(tasks);
		await _context.SaveChangesAsync();
		await _repository.RemoveRangeAsync(tasks);
		await _context.SaveChangesAsync();
		
		foreach (TaskAggregate TaskAggregate in tasks)
		{
			TaskAggregate? removedTask = await _context.TaskAggregates.FindAsync(TaskAggregate.Id);
			
			Assert.Null(removedTask);
		}
	}

	[Fact]
	public async Task UpdateAsync_UpdatesEntityInContext()
	{
		TaskAggregate task = new("Task to Update");
		string newName = "New task name";
		
		await _context.TaskAggregates.AddAsync(task);
		await _context.SaveChangesAsync();
		task.Name = newName;
		await _repository.UpdateAsync(task);
		await _context.SaveChangesAsync();
		TaskAggregate? updatedTask = await _context.TaskAggregates.FindAsync(task.Id);

		Assert.NotNull(updatedTask);
		Assert.Equal(newName, updatedTask.Name);
	}
	
	[Fact]
	public async Task UpdateRangeAsync_UpdatesEntitiesInContex()
	{
		List<TaskAggregate> tasks = new()
		{
			new TaskAggregate("Task 1"), 
			new TaskAggregate("Task 2", Status.ReadyToStart), 
			new TaskAggregate("Task 3", Status.Paused)
		};
		
		await _context.TaskAggregates.AddRangeAsync(tasks);
		await _context.SaveChangesAsync();
		tasks[0].Name = "Task 4";
		tasks[1].Name = "Task 5";
		tasks[2].Name = "Task 6";
		await _repository.UpdateRangeAsync(tasks);
		await _context.SaveChangesAsync();
		TaskAggregate? firstUpdated = await _context.TaskAggregates.FindAsync(tasks[0].Id);
		TaskAggregate? secondUpdated = await _context.TaskAggregates.FindAsync(tasks[1].Id);
		TaskAggregate? thirdUpdated = await _context.TaskAggregates.FindAsync(tasks[2].Id);
		
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
		TaskAggregate task = new("Backlog Task", expectedStatus);
		TasksWithSpecificStatusSpecification spec = new(expectedStatus);
		
		await _context.TaskAggregates.AddAsync(task);
		await _context.SaveChangesAsync();
		bool result = await _repository.ContainsAsync(spec);

		Assert.True(result);
	}
	
	[Fact]
	public async Task ContainsAsync_WithPredicate_ReturnsCorrectResult()
	{
		Status expectedStatus = Status.Dropped;
		TaskAggregate task = new("Backlog Task", expectedStatus);
		
		await _context.TaskAggregates.AddAsync(task);
		await _context.SaveChangesAsync();
		bool result = await _repository.ContainsAsync(TaskAggregate => TaskAggregate.Status == expectedStatus);

		Assert.True(result);
	}

	[Fact]
	public async Task CountAsync_WithSpecification_ReturnsCorrectCount()
	{
		Status expectedStatus = Status.Completed;
		List<TaskAggregate> tasks = new()
		{
			new TaskAggregate("Task 1"), 
			new TaskAggregate("Task 2"), 
			new TaskAggregate("Task 3", expectedStatus)
		};
		TasksWithSpecificStatusSpecification spec = new(expectedStatus);
		
		await _context.TaskAggregates.AddRangeAsync(tasks);
		await _context.SaveChangesAsync();
		int count = await _repository.CountAsync(spec);

		Assert.Equal(1, count);
	}
	
	[Fact]
	public async Task CountAsync_WithPredicate_ReturnsCorrectCount()
	{
		Status expectedStatus = Status.ReadyToStart;
		List<TaskAggregate> tasks = new()
		{
			new TaskAggregate("Task 1", expectedStatus), 
			new TaskAggregate("Task 2", expectedStatus), 
			new TaskAggregate("Task 3", expectedStatus),
			new TaskAggregate("Task 4")
		};
		
		await _context.TaskAggregates.AddRangeAsync(tasks);
		await _context.SaveChangesAsync();
		int count = await _repository.CountAsync(TaskAggregate => TaskAggregate.Status == expectedStatus);

		Assert.Equal(3, count);
	}
}
