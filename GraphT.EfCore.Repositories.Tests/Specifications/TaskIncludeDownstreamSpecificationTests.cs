using GraphT.Model.Aggregates;
using GraphT.Model.Services.Specifications;

using Microsoft.EntityFrameworkCore;

using SeedWork;

namespace GraphT.EfCore.Repositories.Tests.Specifications;

public class TaskIncludeDownstreamSpecificationTests: IClassFixture<TestDatabaseFixture>
{
	private readonly TestDatabaseFixture _fixture;

    public TaskIncludeDownstreamSpecificationTests(TestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }
    [Fact]
    public async Task Find_ReturnsTaskWithDownstreams()
    {
        // Arrange
        EfDbContext context = _fixture.CreateContext();
        Repository<TodoTask> repository = new(context);
        TodoTask mainTask = new("Main Task");
        TodoTask downstream1 = new("Downstream 1");
        TodoTask downstream2 = new("Downstream 2");
        TaskIncludeDownstreamsSpecification spec = new(mainTask.Id);

        // Act
        mainTask.AddDownstreams([downstream1, downstream2]);
        await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskStreams]");
        await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
        await context.TodoTasks.AddRangeAsync(mainTask, downstream1, downstream2);
        await context.SaveChangesAsync();
        PagedList<TodoTask> results = await repository.FindAsync(spec);
        TodoTask resultTask = results.First();

        // Assert
        Assert.NotNull(results);
        Assert.Single(results);
        Assert.Equal(mainTask.Id, resultTask.Id);
        Assert.Equal(2, resultTask.Downstreams.Count);
        Assert.Contains(resultTask.Downstreams, t => t.Id == downstream1.Id);
        Assert.Contains(resultTask.Downstreams, t => t.Id == downstream2.Id);
    }

    [Fact]
    public async Task Find_WhenTaskHasNoDownstreams_ReturnsTaskWithEmptyDownstreams()
    {
        // Arrange
        EfDbContext context = _fixture.CreateContext();
        Repository<TodoTask> repository = new(context);
        TodoTask mainTask = new("Main Task");
        TaskIncludeDownstreamsSpecification spec = new(mainTask.Id);

        // Act
        await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskStreams]");
        await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
        await context.TodoTasks.AddAsync(mainTask);
        await context.SaveChangesAsync();
        PagedList<TodoTask> results = await repository.FindAsync(spec);
        TodoTask resultTask = results.First();

        // Assert
        Assert.NotNull(results);
        Assert.Single(results);
        Assert.Equal(mainTask.Id, resultTask.Id);
        Assert.Empty(resultTask.Downstreams);
    }

    [Fact]
    public async Task Find_WhenTaskDoesNotExist_ReturnsEmptyList()
    {
        // Arrange
        EfDbContext context = _fixture.CreateContext();
        Repository<TodoTask> repository = new(context);
        Guid nonExistentId = Guid.NewGuid();
        TaskIncludeDownstreamsSpecification spec = new(nonExistentId);

        // Act
        await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskStreams]");
        await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
        PagedList<TodoTask> results = await repository.FindAsync(spec);

        // Assert
        Assert.NotNull(results);
        Assert.Empty(results);
        Assert.Equal(0, results.TotalCount);
    }
}
