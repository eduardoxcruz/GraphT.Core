using GraphT.Model.Services.Specifications;
using GraphT.Model.ValueObjects;

using Microsoft.EntityFrameworkCore;

using SeedWork;

namespace GraphT.EfCore.Repositories.Tests.Specifications;

public class TaskLastLogSpecificationTests  : IClassFixture<TestDatabaseFixture>
{
    private readonly TestDatabaseFixture _fixture;

    public TaskLastLogSpecificationTests(TestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Find_ReturnsLogsOrderedByDateTimeDescending()
    {
        // Arrange
        EfDbContext context = _fixture.CreateContext();
        Repository<TaskLog> repository = new(context);
        Guid taskId = Guid.NewGuid();
        DateTimeOffset now = DateTimeOffset.UtcNow;
        List<TaskLog> logs = [
            new(taskId, now.AddHours(-2), Status.Created),
            new(taskId, now.AddHours(-1), Status.Doing),
            new(taskId, now, Status.Paused)
        ];
        TaskLastLogSpecification spec = new(taskId);

        // Act
        await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskLogs]");
        await context.TaskLogs.AddRangeAsync(logs);
        await context.SaveChangesAsync();
        PagedList<TaskLog> results = await repository.FindAsync(spec);

        // Assert
        Assert.NotNull(results);
        Assert.Equal(3, results.TotalCount);
        Assert.Single(results);
        Assert.Equal(now, results.First().DateTime);
        Assert.Equal(Status.Paused, results.First().Status);
    }

    [Fact]
    public async Task Find_WhenNoLogsExist_ReturnsEmptyList()
    {
        // Arrange
        EfDbContext context = _fixture.CreateContext();
        Repository<TaskLog> repository = new(context);
        Guid taskId = Guid.NewGuid();
        TaskLastLogSpecification spec = new(taskId);

        // Act
        await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskLogs]");
        PagedList<TaskLog> results = await repository.FindAsync(spec);

        // Assert
        Assert.NotNull(results);
        Assert.Empty(results);
        Assert.Equal(0, results.TotalCount);
    }

    [Fact]
    public async Task Find_FiltersByTaskId()
    {
        // Arrange
        EfDbContext context = _fixture.CreateContext();
        Repository<TaskLog> repository = new(context);
        Guid targetTaskId = Guid.NewGuid();
        Guid otherTaskId = Guid.NewGuid();
        DateTimeOffset now = DateTimeOffset.UtcNow;
        List<TaskLog> logs = [
            new(targetTaskId, now, Status.Created),
            new(otherTaskId, now.AddMinutes(1), Status.Created),
            new(targetTaskId, now.AddHours(-1), Status.Doing),
            new(otherTaskId, now.AddHours(-2), Status.Doing)
        ];
        TaskLastLogSpecification spec = new(targetTaskId);

        // Act
        await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskLogs]");
        await context.TaskLogs.AddRangeAsync(logs);
        await context.SaveChangesAsync();
        PagedList<TaskLog> results = await repository.FindAsync(spec);

        // Assert
        Assert.NotNull(results);
        Assert.Equal(2, results.TotalCount);
        Assert.All(results, log => Assert.Equal(targetTaskId, log.TaskId));
    }
}
