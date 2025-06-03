using GraphT.EfCore.Repositories.Models;
using GraphT.EfCore.Repositories.Repositories;
using GraphT.Model.Entities;

using Microsoft.EntityFrameworkCore;

using SeedWork;

namespace GraphT.EfCore.Repositories.Tests.Repositories;

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
}
