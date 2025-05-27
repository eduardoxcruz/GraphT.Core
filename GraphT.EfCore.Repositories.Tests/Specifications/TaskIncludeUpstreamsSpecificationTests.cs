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
        Repository<TaskAggregate> repository = new(context);
        TaskAggregate mainTask = new("Main Task");
        TaskAggregate upstream = new("Name 1");
        TaskAggregate upstream2 = new("Name 2");
        TaskAggregate upstream3 = new("Name 3");
        TaskAggregate upstream4 = new("Name 4");
        TaskIncludeUpstreamsSpecification spec = new(mainTask.Id);

        // Act
        mainTask.AddUpstreams([upstream, upstream2]);
        upstream3.AddDownstream(mainTask);
        upstream4.AddDownstream(mainTask);
        await context.TaskAggregates.AddRangeAsync(mainTask, upstream, upstream2, upstream3, upstream4);
        await context.SaveChangesAsync();
        PagedList<TaskAggregate> results = await repository.FindAsync(spec);
        TaskAggregate resultTask = results.First();

        // Assert
        Assert.NotNull(results);
        Assert.Single(results);
        Assert.Equal(mainTask.Id, resultTask.Id);
        Assert.Equal(4, resultTask.Upstreams.Count);
        Assert.Contains(resultTask.Upstreams, t => t.Id == upstream.Id);
        Assert.Contains(resultTask.Upstreams, t => t.Id == upstream2.Id);
        Assert.Contains(resultTask.Upstreams, t => t.Id == upstream3.Id);
        Assert.Contains(resultTask.Upstreams, t => t.Id == upstream4.Id);
        await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskStreams]");
    }

    [Fact]
    public async Task Find_WhenTaskHasNoUpstreams_ReturnsTaskWithEmptyUpstreams()
    {
        // Arrange
        EfDbContext context = _fixture.CreateContext();
        Repository<TaskAggregate> repository = new(context);
        TaskAggregate mainTask = new("Main Task");
        TaskIncludeUpstreamsSpecification spec = new(mainTask.Id);

        // Act
        await context.TaskAggregates.AddAsync(mainTask);
        await context.SaveChangesAsync();
        PagedList<TaskAggregate> results = await repository.FindAsync(spec);
        TaskAggregate resultTask = results.First();

        // Assert
        Assert.NotNull(results);
        Assert.Single(results);
        Assert.Equal(mainTask.Id, resultTask.Id);
        Assert.Empty(resultTask.Upstreams);
        await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskStreams]");
    }

    [Fact]
    public async Task Find_WhenTaskDoesNotExist_ReturnsEmptyList()
    {
        // Arrange
        EfDbContext context = _fixture.CreateContext();
        Repository<TaskAggregate> repository = new(context);
        Guid nonExistentId = Guid.NewGuid();
        TaskIncludeUpstreamsSpecification spec = new(nonExistentId);

        // Act
        PagedList<TaskAggregate> results = await repository.FindAsync(spec);

        // Assert
        Assert.NotNull(results);
        Assert.Empty(results);
        Assert.Equal(0, results.TotalCount);
        await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskStreams]");
    }
}
