using GraphT.EfCore.Repositories.Models;
using GraphT.EfCore.Repositories.Repositories;
using GraphT.Model.Entities;

using Microsoft.EntityFrameworkCore;

using SeedWork;

namespace GraphT.EfCore.Repositories.Tests.Repositories;

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
}
