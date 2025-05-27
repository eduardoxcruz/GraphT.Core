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
        Repository<TaskAggregate> repository = new(context);
        TaskAggregate mainTask = new("Main Task");
        TaskAggregate downstream1 = new("Downstream 1");
        TaskAggregate downstream2 = new("Downstream 2");
        TaskAggregate downstream3 = new("Downstream 3");
        TaskAggregate downstream4 = new("Downstream 4");
        TaskIncludeDownstreamsSpecification spec = new(mainTask.Id);

        // Act
        mainTask.AddDownstreams([downstream1, downstream2]);
        downstream3.AddUpstream(mainTask);
        downstream4.AddUpstream(mainTask);
        await context.TaskAggregates.AddRangeAsync(mainTask, downstream1, downstream2, downstream3, downstream4);
        await context.SaveChangesAsync();
        PagedList<TaskAggregate> results = await repository.FindAsync(spec);
        TaskAggregate resultTask = results.First();

        // Assert
        Assert.NotNull(results);
        Assert.Single(results);
        Assert.Equal(mainTask.Id, resultTask.Id);
        Assert.Equal(4, resultTask.Downstreams.Count);
        Assert.Contains(resultTask.Downstreams, t => t.Id == downstream1.Id);
        Assert.Contains(resultTask.Downstreams, t => t.Id == downstream2.Id);
        Assert.Contains(resultTask.Downstreams, t => t.Id == downstream3.Id);
        Assert.Contains(resultTask.Downstreams, t => t.Id == downstream4.Id);
        await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskStreams]");
    }

    [Fact]
    public async Task Find_WhenTaskHasNoDownstreams_ReturnsTaskWithEmptyDownstreams()
    {
        // Arrange
        EfDbContext context = _fixture.CreateContext();
        Repository<TaskAggregate> repository = new(context);
        TaskAggregate mainTask = new("Main Task");
        TaskIncludeDownstreamsSpecification spec = new(mainTask.Id);

        // Act
        await context.TaskAggregates.AddAsync(mainTask);
        await context.SaveChangesAsync();
        PagedList<TaskAggregate> results = await repository.FindAsync(spec);
        TaskAggregate resultTask = results.First();

        // Assert
        Assert.NotNull(results);
        Assert.Single(results);
        Assert.Equal(mainTask.Id, resultTask.Id);
        Assert.Empty(resultTask.Downstreams);
        await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskStreams]");
    }

    [Fact]
    public async Task Find_WhenTaskDoesNotExist_ReturnsEmptyList()
    {
        // Arrange
        EfDbContext context = _fixture.CreateContext();
        Repository<TaskAggregate> repository = new(context);
        Guid nonExistentId = Guid.NewGuid();
        TaskIncludeDownstreamsSpecification spec = new(nonExistentId);

        // Act
        PagedList<TaskAggregate> results = await repository.FindAsync(spec);

        // Assert
        Assert.NotNull(results);
        Assert.Empty(results);
        Assert.Equal(0, results.TotalCount);
        await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskStreams]");
    }
}
