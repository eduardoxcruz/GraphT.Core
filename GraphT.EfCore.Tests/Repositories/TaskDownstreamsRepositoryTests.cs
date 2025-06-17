using GraphT.EfCore.Models;
using GraphT.EfCore.Repositories;
using GraphT.Model.Entities;

using Microsoft.EntityFrameworkCore;

using SeedWork;

namespace GraphT.EfCore.Tests.Repositories;

public class TaskDownstreamsRepositoryTests : IClassFixture<TestDatabaseFixture>
{
	private readonly TestDatabaseFixture _fixture;

	public TaskDownstreamsRepositoryTests(TestDatabaseFixture fixture) => _fixture = fixture;
	
	[Fact]
	public async Task FindTaskDownstreamsById_ReturnsDownstreams()
	{
		// Arrange
		EfDbContext context = _fixture.CreateContext();
		TaskDownstreamsRepository repository = new(context);
		TodoTask mainTask = new("Main Task");
		TodoTask downstream1 = new("Downstream 1");
		TodoTask downstream2 = new("Downstream 2");
		TodoTask downstream3 = new("Downstream 3");
		TodoTask downstream4 = new("Downstream 4");

		// Act
		TaskStream stream = new(mainTask.Id, downstream1.Id);
		TaskStream stream2 = new(mainTask.Id, downstream2.Id);
		TaskStream stream3 = new(mainTask.Id, downstream3.Id);
		TaskStream stream4 = new(mainTask.Id, downstream4.Id);
		await context.TodoTasks.AddRangeAsync(mainTask, downstream1, downstream2, downstream3, downstream4);
		await context.TaskStreams.AddRangeAsync(stream, stream2, stream3, stream4);
		await context.SaveChangesAsync();
		PagedList<TodoTask> results = await repository.FindTaskDownstreamsById(mainTask.Id);

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
	public async Task FindTaskDownstreamsById_WhenTaskHasNoDownstreams_ReturnsEmptyPagedList()
	{
		// Arrange
		EfDbContext context = _fixture.CreateContext();
		TaskDownstreamsRepository repository = new(context);
		TodoTask mainTask = new("Main Task");

		// Act
		await context.TodoTasks.AddRangeAsync(mainTask);
		await context.SaveChangesAsync();
		PagedList<TodoTask> results = await repository.FindTaskDownstreamsById(mainTask.Id);

		// Assert
		Assert.NotNull(results);
		Assert.Empty(results);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskStreams]");
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
	}

	[Fact]
	public async Task FindTaskDownstreamsById_WhenTaskDoesNotExist_ReturnsEmptyList()
	{
		// Arrange
		EfDbContext context = _fixture.CreateContext();
		TaskDownstreamsRepository repository = new(context);
		Guid nonExistentId = Guid.NewGuid();

		// Act
		PagedList<TodoTask> results = await repository.FindTaskDownstreamsById(nonExistentId);

		// Assert
		// Assert
		Assert.NotNull(results);
		Assert.Empty(results);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskStreams]");
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
	}
	
	[Fact]
	public async Task AddDownstreamAsync_AddsTaskStreamToContext()
	{
		// Arrange
		EfDbContext context = _fixture.CreateContext();
		TaskDownstreamsRepository repository = new(context);
		TodoTask mainTask = new("Main Task");
		TodoTask downstreamTask = new("Downstream Task");
		
		// Act
		await context.TodoTasks.AddRangeAsync(mainTask, downstreamTask);
		await context.SaveChangesAsync();
		await repository.AddDownstreamAsync(mainTask.Id, downstreamTask.Id);
		await context.SaveChangesAsync();

		// Assert
		TaskStream? addedStream = await context.TaskStreams
			.FirstOrDefaultAsync(ts => ts.UpstreamId == mainTask.Id && ts.DownstreamId == downstreamTask.Id);
		
		Assert.NotNull(addedStream);
		Assert.Equal(mainTask.Id, addedStream.UpstreamId);
		Assert.Equal(downstreamTask.Id, addedStream.DownstreamId);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskStreams]");
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
	}

	[Fact]
	public async Task AddDownstreamAsync_WithSameTaskIds_AddsMultipleStreams()
	{
		// Arrange
		EfDbContext context = _fixture.CreateContext();
		TaskDownstreamsRepository repository = new(context);
		TodoTask mainTask = new("Main Task");
		TodoTask downstreamTask1 = new("Downstream Task 1");
		TodoTask downstreamTask2 = new("Downstream Task 2");

		// Act
		await context.TodoTasks.AddRangeAsync(mainTask, downstreamTask1, downstreamTask2);
		await context.SaveChangesAsync();
		await repository.AddDownstreamAsync(mainTask.Id, downstreamTask1.Id);
		await repository.AddDownstreamAsync(mainTask.Id, downstreamTask2.Id);
		await context.SaveChangesAsync();

		// Assert
		int streamCount = await context.TaskStreams.CountAsync(ts => ts.UpstreamId == mainTask.Id);
		
		Assert.Equal(2, streamCount);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskStreams]");
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
	}

	[Fact]
	public async Task RemoveDownstreamAsync_RemovesTaskStreamFromContext()
	{
		// Arrange
		EfDbContext context = _fixture.CreateContext();
		TaskDownstreamsRepository repository = new(context);
		TodoTask mainTask = new("Main Task");
		TodoTask downstreamTask = new("Downstream Task");
		TaskStream stream = new(mainTask.Id, downstreamTask.Id);

		// Act
		await context.TodoTasks.AddRangeAsync(mainTask, downstreamTask);
		await context.TaskStreams.AddAsync(stream);
		await context.SaveChangesAsync();
		context.Entry(stream).State = EntityState.Detached;
		await repository.RemoveDownstreamAsync(mainTask.Id, downstreamTask.Id);
		await context.SaveChangesAsync();

		// Assert
		TaskStream? removedStream = await context.TaskStreams.FirstOrDefaultAsync(ts => ts.UpstreamId == mainTask.Id && ts.DownstreamId == downstreamTask.Id);
		Assert.Null(removedStream);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskStreams]");
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
	}

	[Fact]
	public async Task RemoveDownstreamAsync_WhenStreamDoesNotExist_DoesNotThrowException()
	{
		// Arrange
		EfDbContext context = _fixture.CreateContext();
		TaskDownstreamsRepository repository = new(context);
		Guid nonExistentTaskId = Guid.NewGuid();
		Guid nonExistentDownstreamId = Guid.NewGuid();

		// Act & Assert
		await repository.RemoveDownstreamAsync(nonExistentTaskId, nonExistentDownstreamId);
		await context.SaveChangesAsync();
		int streamCount = await context.TaskStreams.CountAsync();
		Assert.Equal(0, streamCount);
		
		// Cleanup
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskStreams]");
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
	}

	[Fact]
	public async Task RemoveDownstreamAsync_WithMultipleStreams_RemovesOnlySpecifiedStream()
	{
		// Arrange
		EfDbContext context = _fixture.CreateContext();
		TaskDownstreamsRepository repository = new(context);
		TodoTask mainTask = new("Main Task");
		TodoTask downstreamTask1 = new("Downstream Task 1");
		TodoTask downstreamTask2 = new("Downstream Task 2");
		TaskStream stream1 = new(mainTask.Id, downstreamTask1.Id);
		TaskStream stream2 = new(mainTask.Id, downstreamTask2.Id);

		// Act
		await context.TodoTasks.AddRangeAsync(mainTask, downstreamTask1, downstreamTask2);
		await context.TaskStreams.AddRangeAsync(stream1, stream2);
		await context.SaveChangesAsync();
		context.Entry(stream1).State = EntityState.Detached;
		context.Entry(stream2).State = EntityState.Detached;
		await repository.RemoveDownstreamAsync(mainTask.Id, downstreamTask1.Id);
		await context.SaveChangesAsync();

		// Assert
		TaskStream? removedStream = await context.TaskStreams.FirstOrDefaultAsync(ts => ts.UpstreamId == mainTask.Id && ts.DownstreamId == downstreamTask1.Id);
		TaskStream? remainingStream = await context.TaskStreams.FirstOrDefaultAsync(ts => ts.UpstreamId == mainTask.Id && ts.DownstreamId == downstreamTask2.Id);
		int totalStreams = await context.TaskStreams.CountAsync();

		Assert.Null(removedStream);
		Assert.NotNull(remainingStream);
		Assert.Equal(1, totalStreams);
		
		// Cleanup
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskStreams]");
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
	}
}
