using GraphT.Model.ValueObjects;

using Microsoft.EntityFrameworkCore;

using SeedWork;

namespace GraphT.EfCore.Repositories.Tests.Repositories;

public class TaskLogRepository : IClassFixture<TestDatabaseFixture>
{
	private TestDatabaseFixture Fixture { get; }

	public TaskLogRepository(TestDatabaseFixture fixture)
	{
		Fixture = fixture;
	}

	[Fact]
	public async Task Find_WithSpecification_ReturnsPagedList()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<TaskLog> repository = new(context);
		Guid taskId = Guid.NewGuid();
		Guid uselessId = Guid.NewGuid();
		DateTimeOffset expectedTime = DateTimeOffset.Now;
		Status expectedStatus = Status.Paused;
		List<TaskLog> logs =
		[
			new(taskId, expectedTime, expectedStatus), 
			new(uselessId, expectedTime.AddHours(-1), Status.Completed), 
			new(taskId, expectedTime.AddHours(-2), Status.Doing), 
			new(uselessId, expectedTime.AddHours(-3), Status.Created), 
			new(taskId, expectedTime.AddHours(-4), Status.Dropped)
		];

		await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskLogs]");
		await context.TaskLogs.AddRangeAsync(logs);
		await context.SaveChangesAsync();
		PagedList<TaskLog> results =
			await repository.FindAsync(new BaseSpecification<TaskLog>(t => t.TaskId.Equals(taskId)));

		Assert.NotNull(results);
		Assert.NotEmpty(results);
		Assert.Equal(3, results.TotalCount);
		Assert.Equal(1, results.CurrentPage);
		Assert.Equal(1, results.TotalPages);
		Assert.False(results.HasNext);
		Assert.False(results.HasPrevious);
		Assert.True(results.Any(t => t.TaskId.Equals(taskId)) &&
		            results.Any(t => t.DateTime.Equals(expectedTime)) &&
		            results.Any(t => t.Status == expectedStatus));
	}

	[Fact]
	public async Task Find_ReturnsAllEntities()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<TaskLog> repository = new(context);
		List<TaskLog> logs =
		[
			new(Guid.NewGuid(), DateTimeOffset.UtcNow.AddMinutes(30), Status.Created), 
			new(Guid.NewGuid(), DateTimeOffset.UtcNow.AddHours(-5), Status.Ready), 
			new(Guid.NewGuid(), DateTimeOffset.UtcNow.AddDays(2), Status.Doing)
		];

		await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskLogs]");
		await context.TaskLogs.AddRangeAsync(logs);
		await context.SaveChangesAsync();
		PagedList<TaskLog> results = await repository.FindAsync();

		Assert.NotNull(results);
		Assert.NotEmpty(results);
		Assert.Equal(3, results.TotalCount);
		Assert.Equal(1, results.CurrentPage);
		Assert.Equal(1, results.TotalPages);
		Assert.False(results.HasNext);
		Assert.False(results.HasPrevious);
	}

	[Fact]
	public async Task AddAsync_AddsEntityToContext()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<TaskLog> repository = new(context);
		TaskLog log = new(Guid.NewGuid(), DateTimeOffset.UtcNow, Status.Created);

		await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskLogs]");
		await repository.AddAsync(log);
		await context.SaveChangesAsync();
		PagedList<TaskLog> results = await repository.FindAsync();

		Assert.NotNull(results);
		Assert.NotEmpty(results);
		Assert.Equal(1, results.TotalCount);
		Assert.Equal(1, results.CurrentPage);
		Assert.Equal(1, results.TotalPages);
		Assert.False(results.HasNext);
		Assert.False(results.HasPrevious);
	}

	[Fact]
	public async Task AddRangeAsync_AddsEntitiesToContext()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<TaskLog> repository = new(context);
		Guid taskId = Guid.NewGuid();
		List<TaskLog> logs =
		[
			new(taskId, DateTimeOffset.UtcNow, Status.Created), 
			new(taskId, DateTimeOffset.UtcNow, Status.Ready), 
			new(taskId, DateTimeOffset.UtcNow, Status.Doing)
		];

		await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskLogs]");
		await repository.AddRangeAsync(logs);
		await context.SaveChangesAsync();
		PagedList<TaskLog> results = await repository.FindAsync();

		Assert.NotNull(results);
		Assert.NotEmpty(results);
		Assert.Equal(3, results.TotalCount);
		Assert.Equal(1, results.CurrentPage);
		Assert.Equal(1, results.TotalPages);
		Assert.False(results.HasNext);
		Assert.False(results.HasPrevious);
	}

	[Fact]
	public async Task RemoveAsync_RemovesEntityFromContext()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<TaskLog> repository = new(context);
		TaskLog log = new(Guid.NewGuid(), DateTimeOffset.UtcNow, Status.Created);

		await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskLogs]");
		await context.TaskLogs.AddAsync(log);
		await context.SaveChangesAsync();
		await repository.RemoveAsync(log);
		await context.SaveChangesAsync();
		PagedList<TaskLog> results = await repository.FindAsync(new BaseSpecification<TaskLog>(t => t.Equals(log)));

		Assert.Empty(results);
		Assert.Equal(0, results.TotalCount);
		Assert.Equal(1, results.CurrentPage);
		Assert.Equal(1, results.TotalPages);
		Assert.False(results.HasNext);
		Assert.False(results.HasPrevious);
	}

	[Fact]
	public async Task RemoveRangeAsync_RemovesEntitiesFromContext()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<TaskLog> repository = new(context);
		Guid taskId = Guid.NewGuid();
		List<TaskLog> logs =
		[
			new(taskId, DateTimeOffset.UtcNow, Status.Created), 
			new(taskId, DateTimeOffset.UtcNow, Status.Ready), 
			new(taskId, DateTimeOffset.UtcNow, Status.Doing)
		];

		await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskLogs]");
		await context.TaskLogs.AddRangeAsync(logs);
		await context.SaveChangesAsync();
		await repository.RemoveRangeAsync(logs);
		await context.SaveChangesAsync();
		PagedList<TaskLog> results = await repository.FindAsync();

		Assert.Empty(results);
		Assert.Equal(0, results.TotalCount);
		Assert.Equal(1, results.CurrentPage);
		Assert.Equal(1, results.TotalPages);
		Assert.False(results.HasNext);
		Assert.False(results.HasPrevious);
	}

	[Fact]
	public async Task ContainsAsync_WithSpecification_ReturnsCorrectResult()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<TaskLog> repository = new(context);
		Guid taskId = Guid.NewGuid();
		Guid uselessId = Guid.NewGuid();
		DateTimeOffset expectedTime = DateTimeOffset.Now;
		Status expectedStatus = Status.Paused;
		List<TaskLog> logs =
		[
			new(taskId, expectedTime, expectedStatus), 
			new(uselessId, expectedTime.AddHours(-1), Status.Completed), 
			new(taskId, expectedTime.AddHours(-2), Status.Doing), 
			new(uselessId, expectedTime.AddHours(-3), Status.Created), 
			new(taskId, expectedTime.AddHours(-4), Status.Dropped)
		];

		await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskLogs]");
		await context.TaskLogs.AddRangeAsync(logs);
		await context.SaveChangesAsync();
		bool results = await repository.ContainsAsync(new BaseSpecification<TaskLog>(t => t.TaskId.Equals(taskId)));

		Assert.True(results);
	}

	[Fact]
	public async Task ContainsAsync_WithPredicate_ReturnsCorrectResult()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<TaskLog> repository = new(context);
		Guid taskId = Guid.NewGuid();
		Guid uselessId = Guid.NewGuid();
		DateTimeOffset expectedTime = DateTimeOffset.Now;
		Status expectedStatus = Status.Paused;
		List<TaskLog> logs =
		[
			new(taskId, expectedTime, expectedStatus), 
			new(uselessId, expectedTime.AddHours(-1), Status.Completed), 
			new(taskId, expectedTime.AddHours(-2), Status.Doing), 
			new(uselessId, expectedTime.AddHours(-3), Status.Created), 
			new(taskId, expectedTime.AddHours(-4), Status.Dropped)
		];

		await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskLogs]");
		await context.TaskLogs.AddRangeAsync(logs);
		await context.SaveChangesAsync();
		bool result = await repository.ContainsAsync(taskLog => taskLog.Status == expectedStatus);

		Assert.True(result);
	}

	[Fact]
	public async Task CountAsync_WithSpecification_ReturnsCorrectCount()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<TaskLog> repository = new(context);
		Guid taskId = Guid.NewGuid();
		Guid uselessId = Guid.NewGuid();
		DateTimeOffset expectedTime = DateTimeOffset.Now;
		Status expectedStatus = Status.Paused;
		List<TaskLog> logs =
		[
			new(taskId, expectedTime, expectedStatus), 
			new(uselessId, expectedTime.AddHours(-1), Status.Completed), 
			new(taskId, expectedTime.AddHours(-2), Status.Doing), 
			new(uselessId, expectedTime.AddHours(-3), Status.Created), 
			new(taskId, expectedTime.AddHours(-4), Status.Dropped)
		];

		await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskLogs]");
		await context.TaskLogs.AddRangeAsync(logs);
		await context.SaveChangesAsync();
		int result = await repository.CountAsync(new BaseSpecification<TaskLog>(t => t.TaskId.Equals(taskId)));

		Assert.Equal(3, result);
	}

	[Fact]
	public async Task CountAsync_WithPredicate_ReturnsCorrectCount()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<TaskLog> repository = new(context);
		Guid taskId = Guid.NewGuid();
		Guid uselessId = Guid.NewGuid();
		DateTimeOffset expectedTime = DateTimeOffset.Now;
		Status expectedStatus = Status.Paused;
		List<TaskLog> logs =
		[
			new(taskId, expectedTime, expectedStatus), 
			new(uselessId, expectedTime.AddHours(-1), Status.Completed), 
			new(taskId, expectedTime.AddHours(-2), Status.Doing), 
			new(uselessId, expectedTime.AddHours(-3), Status.Created), 
			new(taskId, expectedTime.AddHours(-4), Status.Dropped)
		];

		await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskLogs]");
		await context.TaskLogs.AddRangeAsync(logs);
		await context.SaveChangesAsync();
		int result = await repository.CountAsync(t => t.TaskId.Equals(taskId));

		Assert.Equal(3, result);
	}
}
