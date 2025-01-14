using GraphT.Model.ValueObjects;

namespace GraphT.EfCore.Repositories.Tests.Entities;

public class TaskLogRepository : IClassFixture<TestDatabaseFixture>
{
	public TestDatabaseFixture Fixture { get; }

	public TaskLogRepository(TestDatabaseFixture fixture) => Fixture = fixture;

    [Fact]
    public async Task Find_WithSpecification_ReturnsPagedList()
    {
        EfDbContext context = Fixture.CreateContext();
        Repository<TaskLog> repository = new(context);
        await repository.RemoveRangeAsync(context.TaskLogs);
        Guid taskId = Guid.NewGuid();
        Status expectedStatus = Status.InProgress;
        List<TaskLog> logs = new()
        {
            new TaskLog(taskId, DateTimeOffset.UtcNow, Status.Created),
            new TaskLog(taskId, DateTimeOffset.UtcNow, expectedStatus),
            new TaskLog(taskId, DateTimeOffset.UtcNow, expectedStatus)
        };
        TaskLogsWithSpecificStatusSpecification spec = new(expectedStatus, 1, 2);
        
        await context.TaskLogs.AddRangeAsync(logs);
        await context.SaveChangesAsync();
        PagedList<TaskLog> results = await repository.FindAsync(spec);
        
        Assert.NotNull(results);
        Assert.NotEmpty(results);
        Assert.All(results, log => Assert.Equal(expectedStatus, log.Status));
        Assert.Equal(2, results.Count);
        Assert.Equal(2, results.TotalCount);
        Assert.Equal(1, results.CurrentPage);
        Assert.Equal(1, results.TotalPages);
        Assert.False(results.HasNext);
        Assert.False(results.HasPrevious);
    }
    
    [Fact]
    public async Task Find_ReturnsAllEntities()
    {
        EfDbContext context = Fixture.CreateContext();
        Repository<TaskLog> repository = new(context);
        await repository.RemoveRangeAsync(context.TaskLogs);
        Guid taskId = Guid.NewGuid();
        List<TaskLog> logs = new()
        {
            new TaskLog(taskId, DateTimeOffset.UtcNow, Status.Created),
            new TaskLog(taskId, DateTimeOffset.UtcNow, Status.ReadyToStart),
            new TaskLog(taskId, DateTimeOffset.UtcNow, Status.InProgress)
        };
        
        await context.TaskLogs.AddRangeAsync(logs);
        await context.SaveChangesAsync();
        PagedList<TaskLog> results = await repository.FindAsync();
        
        Assert.Equal(3, results.Count);
    }

    [Fact]
    public async Task AddAsync_AddsEntityToContext()
    {
        EfDbContext context = Fixture.CreateContext();
        Repository<TaskLog> repository = new(context);
        await repository.RemoveRangeAsync(context.TaskLogs);
        TaskLog log = new(Guid.NewGuid(), DateTimeOffset.UtcNow, Status.Created);
        
        await repository.AddAsync(log);
        await context.SaveChangesAsync();
        TaskLog? addedLog = await context.TaskLogs.FindAsync(log.Id);
        
        Assert.NotNull(addedLog);
        Assert.Equal(log.Id, addedLog.Id);
    }
    
    [Fact]
    public async Task AddRangeAsync_AddsEntitiesToContext()
    {
        EfDbContext context = Fixture.CreateContext();
        Repository<TaskLog> repository = new(context);
        await repository.RemoveRangeAsync(context.TaskLogs);
        Guid taskId = Guid.NewGuid();
        List<TaskLog> logs = new()
        {
            new TaskLog(taskId, DateTimeOffset.UtcNow, Status.Created),
            new TaskLog(taskId, DateTimeOffset.UtcNow, Status.ReadyToStart),
            new TaskLog(taskId, DateTimeOffset.UtcNow, Status.InProgress)
        };
        
        await repository.AddRangeAsync(logs);
        await context.SaveChangesAsync();

        foreach (TaskLog log in logs)
        {
            TaskLog? addedLog = await context.TaskLogs.FindAsync(log.Id);
            
            Assert.NotNull(addedLog);
            Assert.Equal(log.Id, addedLog.Id);
        }
    }

    [Fact]
    public async Task RemoveAsync_RemovesEntityFromContext()
    {
        EfDbContext context = Fixture.CreateContext();
        Repository<TaskLog> repository = new(context);
        await repository.RemoveRangeAsync(context.TaskLogs);
        TaskLog log = new(Guid.NewGuid(), DateTimeOffset.UtcNow, Status.Created);
        
        await context.TaskLogs.AddAsync(log);
        await context.SaveChangesAsync();
        await repository.RemoveAsync(log);
        await context.SaveChangesAsync();
        TaskLog? removedLog = await context.TaskLogs.FindAsync(log.Id);
        
        Assert.Null(removedLog);
    }
    
    [Fact]
    public async Task RemoveRangeAsync_RemovesEntitiesFromContext()
    {
        EfDbContext context = Fixture.CreateContext();
        Repository<TaskLog> repository = new(context);
        await repository.RemoveRangeAsync(context.TaskLogs);
        Guid taskId = Guid.NewGuid();
        List<TaskLog> logs = new()
        {
            new TaskLog(taskId, DateTimeOffset.UtcNow, Status.Created),
            new TaskLog(taskId, DateTimeOffset.UtcNow, Status.ReadyToStart),
            new TaskLog(taskId, DateTimeOffset.UtcNow, Status.InProgress)
        };
        
        await context.TaskLogs.AddRangeAsync(logs);
        await context.SaveChangesAsync();
        await repository.RemoveRangeAsync(logs);
        await context.SaveChangesAsync();
        
        foreach (TaskLog log in logs)
        {
            TaskLog? removedLog = await context.TaskLogs.FindAsync(log.Id);
            
            Assert.Null(removedLog);
        }
    }

    [Fact]
    public async Task UpdateAsync_UpdatesEntityInContext()
    {
        EfDbContext context = Fixture.CreateContext();
        Repository<TaskLog> repository = new(context);
        await repository.RemoveRangeAsync(context.TaskLogs);
        TaskLog log = new(Guid.NewGuid(), DateTimeOffset.UtcNow, Status.Created);
        Status newStatus = Status.InProgress;
        
        await context.TaskLogs.AddAsync(log);
        await context.SaveChangesAsync();
        log.Status = newStatus;
        await repository.UpdateAsync(log);
        await context.SaveChangesAsync();
        TaskLog? updatedLog = await context.TaskLogs.FindAsync(log.Id);

        Assert.NotNull(updatedLog);
        Assert.Equal(newStatus, updatedLog.Status);
    }
    
    [Fact]
    public async Task UpdateRangeAsync_UpdatesEntitiesInContext()
    {
        EfDbContext context = Fixture.CreateContext();
        Repository<TaskLog> repository = new(context);
        await repository.RemoveRangeAsync(context.TaskLogs);
        Guid taskId = Guid.NewGuid();
        List<TaskLog> logs = new()
        {
            new TaskLog(taskId, DateTimeOffset.UtcNow, Status.Created),
            new TaskLog(taskId, DateTimeOffset.UtcNow, Status.ReadyToStart),
            new TaskLog(taskId, DateTimeOffset.UtcNow, Status.InProgress)
        };
        
        await context.TaskLogs.AddRangeAsync(logs);
        await context.SaveChangesAsync();
        logs[0].Status = Status.Completed;
        logs[1].Status = Status.Dropped;
        logs[2].Status = Status.Paused;
        await repository.UpdateRangeAsync(logs);
        await context.SaveChangesAsync();
        TaskLog? firstUpdated = await context.TaskLogs.FindAsync(logs[0].Id);
        TaskLog? secondUpdated = await context.TaskLogs.FindAsync(logs[1].Id);
        TaskLog? thirdUpdated = await context.TaskLogs.FindAsync(logs[2].Id);
        
        Assert.NotNull(firstUpdated);
        Assert.NotNull(secondUpdated);
        Assert.NotNull(thirdUpdated);
        Assert.Equal(Status.Completed, firstUpdated.Status);
        Assert.Equal(Status.Dropped, secondUpdated.Status);
        Assert.Equal(Status.Paused, thirdUpdated.Status);
    }

	[Fact]
	public async Task ContainsAsync_WithSpecification_ReturnsCorrectResult()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<TodoTask> repository = new(context);
		await repository.RemoveRangeAsync(context.TodoTasks);
		Status expectedStatus = Status.Paused;
		TodoTask task = new("Backlog Task", expectedStatus);
		TasksWithSpecificStatusSpecification spec = new(expectedStatus);
		
		await context.TodoTasks.AddAsync(task);
		await context.SaveChangesAsync();
		bool result = await repository.ContainsAsync(spec);

		Assert.True(result);
	}
	
	[Fact]
	public async Task ContainsAsync_WithPredicate_ReturnsCorrectResult()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<TodoTask> repository = new(context);
		await repository.RemoveRangeAsync(context.TodoTasks);
		Status expectedStatus = Status.Dropped;
		TodoTask task = new("Backlog Task", expectedStatus);
		
		await context.TodoTasks.AddAsync(task);
		await context.SaveChangesAsync();
		bool result = await repository.ContainsAsync(todoTask => todoTask.Status == expectedStatus);

		Assert.True(result);
	}

	[Fact]
	public async Task CountAsync_WithSpecification_ReturnsCorrectCount()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<TodoTask> repository = new(context);
		await repository.RemoveRangeAsync(context.TodoTasks);
		Status expectedStatus = Status.Completed;
		List<TodoTask> tasks = new()
		{
			new TodoTask("Task 1"), 
			new TodoTask("Task 2"), 
			new TodoTask("Task 3", expectedStatus)
		};
		TasksWithSpecificStatusSpecification spec = new(expectedStatus);
		
		await context.TodoTasks.AddRangeAsync(tasks);
		await context.SaveChangesAsync();
		int count = await repository.CountAsync(spec);

		Assert.Equal(1, count);
	}
	
	[Fact]
	public async Task CountAsync_WithPredicate_ReturnsCorrectCount()
	{
		EfDbContext context = Fixture.CreateContext();
		Repository<TodoTask> repository = new(context);
		await repository.RemoveRangeAsync(context.TodoTasks);
		Status expectedStatus = Status.ReadyToStart;
		List<TodoTask> tasks = new()
		{
			new TodoTask("Task 1", expectedStatus), 
			new TodoTask("Task 2", expectedStatus), 
			new TodoTask("Task 3", expectedStatus),
			new TodoTask("Task 4")
		};
		
		await context.TodoTasks.AddRangeAsync(tasks);
		await context.SaveChangesAsync();
		PagedList<TodoTask> dbTasks =
			await repository.FindAsync(new TasksWithSpecificStatusSpecification(expectedStatus));
		int count = await repository.CountAsync(todoTask => todoTask.Status == expectedStatus);

		Assert.Equal(3, count);
	}
}
