using GraphT.EfCore.Models;
using GraphT.EfCore.Repositories;
using GraphT.Model.Entities;

using Microsoft.EntityFrameworkCore;

using SeedWork;

namespace GraphT.EfCore.Tests.Repositories;

public class TaskUpstreamsRepositoryTests: IClassFixture<TestDatabaseFixture>
{
	private readonly TestDatabaseFixture _fixture;

	public TaskUpstreamsRepositoryTests(TestDatabaseFixture fixture) => _fixture = fixture;
	
	[Fact]
    public async Task FindTaskUpstreamsById_ReturnsUpstreams()
    {
	    // Arrange
	    EfDbContext context = _fixture.CreateContext();
	    TaskUpstreamsRepository repository = new(context);
	    TodoTask mainTask = new("Main Task");
	    TodoTask downstream1 = new("Downstream 1");
	    TodoTask downstream2 = new("Downstream 2");
	    TodoTask downstream3 = new("Downstream 3");
	    TodoTask downstream4 = new("Downstream 4");

	    // Act
	    TaskStream stream = new(downstream1.Id, mainTask.Id);
	    TaskStream stream2 = new(downstream2.Id,mainTask.Id);
	    TaskStream stream3 = new(downstream3.Id, mainTask.Id);
	    TaskStream stream4 = new(downstream4.Id, mainTask.Id);
	    await context.TodoTasks.AddRangeAsync(mainTask, downstream1, downstream2, downstream3, downstream4);
	    await context.TaskStreams.AddRangeAsync(stream, stream2, stream3, stream4);
	    await context.SaveChangesAsync();
	    PagedList<TodoTask> results = await repository.FindTaskUpstreamsById(mainTask.Id);

	    // Assert
	    Assert.NotNull(results);
	    Assert.NotEmpty(results);
	    Assert.Equal(4, results.Count);
	    Assert.Contains(results, t => t.Id == downstream1.Id);
	    Assert.Contains(results, t => t.Id == downstream2.Id);
	    Assert.Contains(results, t => t.Id == downstream3.Id);
	    Assert.Contains(results, t => t.Id == downstream4.Id);
	    await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskStreams]");
	    await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
    }

    [Fact]
    public async Task FindTaskUpstreamsById_WhenTaskHasNoUpstreams_ReturnsEmptyPagedList()
    {
	    // Arrange
	    EfDbContext context = _fixture.CreateContext();
	    TaskUpstreamsRepository repository = new(context);
	    TodoTask mainTask = new("Main Task");

	    // Act
	    await context.TodoTasks.AddAsync(mainTask);
	    await context.SaveChangesAsync();
	    PagedList<TodoTask> results = await repository.FindTaskUpstreamsById(mainTask.Id);

	    // Assert
	    Assert.NotNull(results);
	    Assert.Empty(results);
	    await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskStreams]");
	    await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
    }

    [Fact]
    public async Task FindTaskUpstreamsById_WhenTaskDoesNotExist_ReturnsEmptyPagedList()
    {
	    // Arrange
	    EfDbContext context = _fixture.CreateContext();
	    TaskUpstreamsRepository repository = new(context);
	    Guid nonExistentId = Guid.NewGuid();

	    // Act
	    PagedList<TodoTask> results = await repository.FindTaskUpstreamsById(nonExistentId);

	    // Assert
	    Assert.NotNull(results);
	    Assert.Empty(results);
	    await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskStreams]");
	    await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
    }
    
    [Fact]
	public async Task AddUpstreamAsync_AddsTaskStreamToContext()
	{
		// Arrange
		EfDbContext context = _fixture.CreateContext();
		TaskUpstreamsRepository repository = new(context);
		TodoTask mainTask = new("Main Task");
		TodoTask upstreamTask = new("Upstream Task");
		
		// Act
		await context.TodoTasks.AddRangeAsync(mainTask, upstreamTask);
		await context.SaveChangesAsync();
		await repository.AddUpstreamAsync(mainTask.Id, upstreamTask.Id);
		await context.SaveChangesAsync();

		// Assert
		TaskStream? addedStream = await context.TaskStreams
			.FirstOrDefaultAsync(ts => ts.UpstreamId == upstreamTask.Id && ts.DownstreamId == mainTask.Id);
		
		Assert.NotNull(addedStream);
		Assert.Equal(upstreamTask.Id, addedStream.UpstreamId);
		Assert.Equal(mainTask.Id, addedStream.DownstreamId);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskStreams]");
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
	}

	[Fact]
	public async Task AddUpstreamAsync_WithSameTaskIds_AddsMultipleStreams()
	{
		// Arrange
		EfDbContext context = _fixture.CreateContext();
		TaskUpstreamsRepository repository = new(context);
		TodoTask mainTask = new("Main Task");
		TodoTask upstreamTask1 = new("Upstream Task 1");
		TodoTask upstreamTask2 = new("Upstream Task 2");

		// Act
		await context.TodoTasks.AddRangeAsync(mainTask, upstreamTask1, upstreamTask2);
		await context.SaveChangesAsync();
		await repository.AddUpstreamAsync(mainTask.Id, upstreamTask1.Id);
		await repository.AddUpstreamAsync(mainTask.Id, upstreamTask2.Id);
		await context.SaveChangesAsync();

		// Assert
		int streamCount = await context.TaskStreams.CountAsync(ts => ts.DownstreamId == mainTask.Id);
		
		Assert.Equal(2, streamCount);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskStreams]");
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
	}

	[Fact]
	public async Task RemoveUpstreamAsync_RemovesTaskStreamFromContext()
	{
		// Arrange
		EfDbContext context = _fixture.CreateContext();
		TaskUpstreamsRepository repository = new(context);
		TodoTask mainTask = new("Main Task");
		TodoTask upstreamTask = new("Upstream Task");
		TaskStream stream = new(upstreamTask.Id, mainTask.Id);

		// Act
		await context.TodoTasks.AddRangeAsync(mainTask, upstreamTask);
		await context.TaskStreams.AddAsync(stream);
		await context.SaveChangesAsync();
		context.Entry(stream).State = EntityState.Detached;
		await repository.RemoveUpstreamAsync(mainTask.Id, upstreamTask.Id);
		await context.SaveChangesAsync();

		// Assert
		TaskStream? removedStream = await context.TaskStreams.FirstOrDefaultAsync(ts => ts.UpstreamId == upstreamTask.Id && ts.DownstreamId == mainTask.Id);
		Assert.Null(removedStream);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskStreams]");
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
	}

	[Fact]
	public async Task RemoveUpstreamAsync_WhenStreamDoesNotExist_DoesNotThrowException()
	{
		// Arrange
		EfDbContext context = _fixture.CreateContext();
		TaskUpstreamsRepository repository = new(context);
		Guid nonExistentTaskId = Guid.NewGuid();
		Guid nonExistentUpstreamId = Guid.NewGuid();

		// Act & Assert
		await repository.RemoveUpstreamAsync(nonExistentTaskId, nonExistentUpstreamId);
		await context.SaveChangesAsync();
		int streamCount = await context.TaskStreams.CountAsync();
		Assert.Equal(0, streamCount);
		
		// Cleanup
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskStreams]");
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
	}

	[Fact]
	public async Task RemoveUpstreamAsync_WithMultipleStreams_RemovesOnlySpecifiedStream()
	{
		// Arrange
		EfDbContext context = _fixture.CreateContext();
		TaskUpstreamsRepository repository = new(context);
		TodoTask mainTask = new("Main Task");
		TodoTask upstreamTask1 = new("Upstream Task 1");
		TodoTask upstreamTask2 = new("Upstream Task 2");
		TaskStream stream1 = new(upstreamTask1.Id, mainTask.Id);
		TaskStream stream2 = new(upstreamTask2.Id, mainTask.Id);

		// Act
		await context.TodoTasks.AddRangeAsync(mainTask, upstreamTask1, upstreamTask2);
		await context.TaskStreams.AddRangeAsync(stream1, stream2);
		await context.SaveChangesAsync();
		context.Entry(stream1).State = EntityState.Detached;
		context.Entry(stream2).State = EntityState.Detached;
		await repository.RemoveUpstreamAsync(mainTask.Id, upstreamTask1.Id);
		await context.SaveChangesAsync();

		// Assert
		TaskStream? removedStream = await context.TaskStreams.FirstOrDefaultAsync(ts => ts.UpstreamId == upstreamTask1.Id && ts.DownstreamId == mainTask.Id);
		TaskStream? remainingStream = await context.TaskStreams.FirstOrDefaultAsync(ts => ts.UpstreamId == upstreamTask2.Id && ts.DownstreamId == mainTask.Id);
		int totalStreams = await context.TaskStreams.CountAsync();

		Assert.Null(removedStream);
		Assert.NotNull(remainingStream);
		Assert.Equal(1, totalStreams);
		
		// Cleanup
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskStreams]");
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
	}
}
