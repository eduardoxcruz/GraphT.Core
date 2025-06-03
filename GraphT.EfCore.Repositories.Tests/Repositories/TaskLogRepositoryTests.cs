using GraphT.EfCore.Repositories.Repositories;
using GraphT.Model.ValueObjects;

using Microsoft.EntityFrameworkCore;

using SeedWork;

namespace GraphT.EfCore.Repositories.Tests.Repositories;

public class TaskLogRepositoryTests : IClassFixture<TestDatabaseFixture>
{
	private readonly TestDatabaseFixture _fixture;

	public TaskLogRepositoryTests(TestDatabaseFixture fixture) => _fixture = fixture;

	[Fact]
	public async Task AddAsync_AddsEntityToContext()
	{
		EfDbContext context = _fixture.CreateContext();
		TaskLogRepository repository = new(context);
		TaskLog log = new(Guid.NewGuid(), DateTimeOffset.UtcNow, Status.Created);
		
		await repository.AddAsync(log);
		await context.SaveChangesAsync();
		List<TaskLog> results = await context.TaskLogs.ToListAsync();

		Assert.NotNull(results);
		Assert.NotEmpty(results);
		Assert.Single(results);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskLogs]");
	}

	[Fact]
	public async Task AddRangeAsync_AddsEntitiesToContext()
	{
		EfDbContext context = _fixture.CreateContext();
		TaskLogRepository repository = new(context);
		Guid taskId = Guid.NewGuid();
		List<TaskLog> logs =
		[
			new(taskId, DateTimeOffset.UtcNow, Status.Created), 
			new(taskId, DateTimeOffset.UtcNow, Status.Ready), 
			new(taskId, DateTimeOffset.UtcNow, Status.Doing)
		];

		await repository.AddRangeAsync(logs);
		await context.SaveChangesAsync();
		List<TaskLog> results = await context.TaskLogs.ToListAsync();

		Assert.NotNull(results);
		Assert.NotEmpty(results);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskLogs]");
	}

	[Fact]
	public async Task RemoveAsync_RemovesEntityFromContext()
	{
		EfDbContext context = _fixture.CreateContext();
		TaskLogRepository repository = new(context);
		TaskLog log = new(Guid.NewGuid(), DateTimeOffset.UtcNow, Status.Created);

		await context.TaskLogs.AddAsync(log);
		await context.SaveChangesAsync();
		await repository.RemoveAsync(log);
		await context.SaveChangesAsync();
		List<TaskLog> results = await context.TaskLogs.ToListAsync();

		Assert.Empty(results);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskLogs]");
	}

	[Fact]
	public async Task RemoveRangeAsync_RemovesEntitiesFromContext()
	{
		EfDbContext context = _fixture.CreateContext();
		TaskLogRepository repository = new(context);
		Guid taskId = Guid.NewGuid();
		List<TaskLog> logs =
		[
			new(taskId, DateTimeOffset.UtcNow, Status.Created), 
			new(taskId, DateTimeOffset.UtcNow, Status.Ready), 
			new(taskId, DateTimeOffset.UtcNow, Status.Doing)
		];

		await context.TaskLogs.AddRangeAsync(logs);
		await context.SaveChangesAsync();
		await repository.RemoveRangeAsync(logs);
		await context.SaveChangesAsync();
		List<TaskLog> results = await context.TaskLogs.ToListAsync();

		Assert.Empty(results);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskLogs]");
	}
	
	[Fact]
    public async Task FindTaskLastLog_ReturnsLogOrderedByDateTimeDescending()
    {
        // Arrange
        EfDbContext context = _fixture.CreateContext();
        TaskLogRepository repository = new(context);
        Guid taskId = Guid.NewGuid();
        DateTimeOffset now = DateTimeOffset.UtcNow;
        List<TaskLog> logs = [
            new(taskId, now.AddHours(-2), Status.Created),
            new(taskId, now.AddHours(-1), Status.Doing),
            new(taskId, now, Status.Paused)
        ];

        // Act
        await context.TaskLogs.AddRangeAsync(logs);
        await context.SaveChangesAsync();
        TaskLog? result = await repository.FindTaskLastLog(taskId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(now, result.DateTime);
        Assert.Equal(Status.Paused, result.Status);
        await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskLogs]");
    }

    [Fact]
    public async Task FindTaskLastLog_WhenNoLogsExist_ReturnsNull()
    {
        // Arrange
        EfDbContext context = _fixture.CreateContext();
        TaskLogRepository repository = new(context);
        Guid taskId = Guid.NewGuid();

        // Act
        TaskLog? result = await repository.FindTaskLastLog(taskId);

        // Assert
        Assert.Null(result);
        await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskLogs]");
    }

    [Fact]
    public async Task FindTaskLastLog_FiltersByTaskId()
    {
        // Arrange
        EfDbContext context = _fixture.CreateContext();
        TaskLogRepository repository = new(context);
        Guid targetTaskId = Guid.NewGuid();
        Guid otherTaskId = Guid.NewGuid();
        DateTimeOffset now = DateTimeOffset.UtcNow;
        List<TaskLog> logs = [
            new(targetTaskId, now, Status.Created),
            new(otherTaskId, now.AddMinutes(1), Status.Created),
            new(targetTaskId, now.AddHours(-1), Status.Doing),
            new(otherTaskId, now.AddHours(-2), Status.Doing)
        ];

        // Act
        await context.TaskLogs.AddRangeAsync(logs);
        await context.SaveChangesAsync();
        TaskLog? result = await repository.FindTaskLastLog(targetTaskId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(targetTaskId, result.TaskId);
        await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskLogs]");
    }
}
