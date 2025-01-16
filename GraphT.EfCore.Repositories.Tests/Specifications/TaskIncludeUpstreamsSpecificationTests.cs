using GraphT.Model.Aggregates;
using GraphT.Model.Services.Specifications;

using Microsoft.EntityFrameworkCore;

using SeedWork;

namespace GraphT.EfCore.Repositories.Tests.Specifications;

public class TaskIncludeUpstreamsSpecificationTests : IClassFixture<TestDatabaseFixture>
{
	private readonly TestDatabaseFixture _fixture;

    public TaskIncludeUpstreamsSpecificationTests(TestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public async Task Find_ReturnsTaskWithUpstreams()
    {
        // Arrange
        EfDbContext context = _fixture.CreateContext();
        Repository<TodoTask> repository = new(context);
        TodoTask mainTask = new("Main Task");
        TodoTask upstream = new("Name 1");
        TodoTask upstream2 = new("Name 2");
        TaskIncludeUpstreamsSpecification spec = new(mainTask.Id);

        // Act
        mainTask.AddUpstreams([upstream, upstream2]);
        await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskStreams]");
        await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
        await context.TodoTasks.AddRangeAsync(mainTask, upstream, upstream2);
        await context.SaveChangesAsync();
        PagedList<TodoTask> results = await repository.FindAsync(spec);
        TodoTask resultTask = results.First();

        // Assert
        Assert.NotNull(results);
        Assert.Single(results);
        Assert.Equal(mainTask.Id, resultTask.Id);
        Assert.Equal(2, resultTask.Upstreams.Count);
        Assert.Contains(resultTask.Upstreams, t => t.Id == upstream.Id);
        Assert.Contains(resultTask.Upstreams, t => t.Id == upstream2.Id);
    }

    [Fact]
    public async Task Find_WhenTaskHasNoUpstreams_ReturnsTaskWithEmptyUpstreams()
    {
        // Arrange
        EfDbContext context = _fixture.CreateContext();
        Repository<TodoTask> repository = new(context);
        TodoTask mainTask = new("Main Task");
        TaskIncludeUpstreamsSpecification spec = new(mainTask.Id);

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
        Assert.Empty(resultTask.Upstreams);
    }

    [Fact]
    public async Task Find_WhenTaskDoesNotExist_ReturnsEmptyList()
    {
        // Arrange
        EfDbContext context = _fixture.CreateContext();
        Repository<TodoTask> repository = new(context);
        Guid nonExistentId = Guid.NewGuid();
        TaskIncludeUpstreamsSpecification spec = new(nonExistentId);

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
