using GraphT.EfCore.Models;
using GraphT.EfCore.Repositories;
using GraphT.Model.Entities;
using GraphT.Model.ValueObjects;

using Microsoft.EntityFrameworkCore;

using SeedWork;

namespace GraphT.EfCore.Tests.Repositories;

public class TodoTaskRepositoryTests : IClassFixture<TestDatabaseFixture>
{
	private readonly TestDatabaseFixture _fixture;

	public TodoTaskRepositoryTests(TestDatabaseFixture fixture) => _fixture = fixture;

	[Fact]
	public async Task FindByIdAsync_ReturnsCorrectEntity()
	{
		EfDbContext context = _fixture.CreateContext();
		TodoTaskRepository repository = new(context);
		OldTodoTask task = new("Test Task");

		await context.TodoTasks.AddAsync(task);
		await context.SaveChangesAsync();
		OldTodoTask? result = await repository.FindByIdAsync(task.Id);

		Assert.NotNull(result);
		Assert.Equal(task.Id, result.Id);
		Assert.Equal("Test Task", result.Name);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
	}

	[Fact]
	public async Task AddAsync_AddsEntityToContext()
	{
		EfDbContext context = _fixture.CreateContext();
		TodoTaskRepository repository = new(context);
		OldTodoTask task = new("New Task");

		await repository.AddAsync(task);
		await context.SaveChangesAsync();
		OldTodoTask? addedTask = await context.TodoTasks.FindAsync(task.Id);

		Assert.NotNull(addedTask);
		Assert.Equal(task.Id, addedTask.Id);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
	}
	
	[Fact]
	public async Task AddRangeAsync_AddsEntitiesToContext()
	{
		EfDbContext context = _fixture.CreateContext();
		TodoTaskRepository repository = new(context);
		List<OldTodoTask> tasks =
		[
			new("Task 1"), 
			new("Task 2", OldStatus.Ready), 
			new("Task 3", OldStatus.Paused), 
			new("Task 4", OldStatus.Dropped), 
			new("Task 5", OldStatus.Completed)
		];

		await repository.AddRangeAsync(tasks);
		await context.SaveChangesAsync();

		foreach (OldTodoTask todoTask in tasks)
		{
			OldTodoTask? addedTask = await context.TodoTasks.FindAsync(todoTask.Id);
			
			Assert.NotNull(addedTask);
			Assert.Equal(todoTask.Id, addedTask.Id);
		}

		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
	}

	[Fact]
	public async Task RemoveAsync_RemovesEntityFromContext()
	{
		EfDbContext context = _fixture.CreateContext();
		TodoTaskRepository repository = new(context);
		OldTodoTask task = new("Task to Remove");

		await context.TodoTasks.AddAsync(task);
		await context.SaveChangesAsync();
		await repository.RemoveAsync(task);
		await context.SaveChangesAsync();
		OldTodoTask? removedTask = await context.TodoTasks.FindAsync(task.Id);

		Assert.Null(removedTask);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
	}
	
	[Fact]
	public async Task RemoveAsync_RemovesRelatedStreamsAndTodoTaskFromContext()
	{
		EfDbContext context = _fixture.CreateContext();
		TodoTaskRepository repository = new(context);
		OldTodoTask task = new("Task to Remove");
		OldTodoTask upstreamTask = new("Task to Remove");
		OldTodoTask downstreamTask = new("Task to Remove");
		TaskStream upstream = new(upstreamTask.Id, task.Id);
		TaskStream downstream = new(task.Id, downstreamTask.Id);
		
		await context.TodoTasks.AddRangeAsync(task, upstreamTask, downstreamTask);
		await context.TaskStreams.AddRangeAsync(upstream, downstream);
		await context.SaveChangesAsync();
		await repository.RemoveAsync(task);
		await context.SaveChangesAsync();
		OldTodoTask? removedTask = await context.TodoTasks.FindAsync(task.Id);

		Assert.Null(removedTask);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskStreams]");
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
	}
	
	[Fact]
	public async Task RemoveRangeAsync_RemovesEntitiesFromContext()
	{
		EfDbContext context = _fixture.CreateContext();
		TodoTaskRepository repository = new(context);
		List<OldTodoTask> tasks =
		[
			new("Task 1"), 
			new("Task 2", OldStatus.Ready), 
			new("Task 3", OldStatus.Paused), 
			new("Task 4", OldStatus.Dropped), 
			new("Task 5", OldStatus.Completed)
		];

		await context.TodoTasks.AddRangeAsync(tasks);
		await context.SaveChangesAsync();
		await repository.RemoveRangeAsync(tasks);
		await context.SaveChangesAsync();

		foreach (OldTodoTask todoTask in tasks)
		{
			OldTodoTask? removedTask = await context.TodoTasks.FindAsync(todoTask.Id);
			
			Assert.Null(removedTask);
		}

		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
	}

	[Fact]
public async Task RemoveRangeAsync_RemovesRelatedStreamsAndTaskRangeFromContext()
{
    EfDbContext context = _fixture.CreateContext();
    TodoTaskRepository repository = new(context);
    OldTodoTask task1 = new("Task 1 to Remove");
    OldTodoTask task2 = new("Task 2 to Remove");
    OldTodoTask upstreamTask = new("Upstream Task");
    OldTodoTask downstreamTask = new("Downstream Task");
    OldTodoTask independentTask = new("Independent Task");
    TaskStream upstream1 = new(upstreamTask.Id, task1.Id);
    TaskStream downstream1 = new(task1.Id, downstreamTask.Id);
    TaskStream upstream2 = new(upstreamTask.Id, task2.Id);
    TaskStream downstream2 = new(task2.Id, downstreamTask.Id);
    TaskStream betweenTasks = new(task1.Id, task2.Id);
    TaskStream independentStream = new(upstreamTask.Id, independentTask.Id);
    
    await context.TodoTasks.AddRangeAsync(task1, task2, upstreamTask, downstreamTask, independentTask);
    await context.TaskStreams.AddRangeAsync(upstream1, downstream1, upstream2, downstream2, betweenTasks, independentStream);
    await context.SaveChangesAsync();
    await repository.RemoveRangeAsync([task1, task2]);
    await context.SaveChangesAsync();
    
    bool task1Exits = await context.TodoTasks.ContainsAsync(task1);
    bool task2Exist = await context.TodoTasks.ContainsAsync(task2);
    bool upstreamTaskStillExists = await context.TodoTasks.ContainsAsync(upstreamTask);
    bool downstreamTaskStillExists = await context.TodoTasks.ContainsAsync(downstreamTask);
    bool independentTaskStillExists = await context.TodoTasks.ContainsAsync(independentTask);
    
    bool relatedStreamsExist = await context.TaskStreams
        .AnyAsync(ts => ts.UpstreamId == task1.Id || ts.DownstreamId == task1.Id || 
                       ts.UpstreamId == task2.Id || ts.DownstreamId == task2.Id);
    
    bool independentStreamExists = await context.TaskStreams
        .AnyAsync(ts => (ts.UpstreamId == independentStream.UpstreamId) && ts.DownstreamId == independentStream.DownstreamId);
    
    // Assertions
    Assert.False(task1Exits);
    Assert.False(task2Exist);
    Assert.True(upstreamTaskStillExists);
    Assert.True(downstreamTaskStillExists);
    Assert.True(independentTaskStillExists);
    Assert.False(relatedStreamsExist);
    Assert.True(independentStreamExists);
    
    await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskStreams]");
    await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
}
	
	[Fact]
	public async Task UpdateAsync_UpdatesEntityInContext()
	{
		EfDbContext context = _fixture.CreateContext();
		TodoTaskRepository repository = new(context);
		OldTodoTask task = new("Task to Update");
		string newName = "New task name";

		await context.TodoTasks.AddAsync(task);
		await context.SaveChangesAsync();
		task.Name = newName;
		await repository.UpdateAsync(task);
		await context.SaveChangesAsync();
		OldTodoTask? updatedTask = await context.TodoTasks.FindAsync(task.Id);

		Assert.NotNull(updatedTask);
		Assert.Equal(newName, updatedTask.Name);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
	}
	
	[Fact]
	public async Task UpdateRangeAsync_UpdatesEntitiesInContex()
	{
		EfDbContext context = _fixture.CreateContext();
		TodoTaskRepository repository = new(context);
		List<OldTodoTask> tasks = new()
		{
			new OldTodoTask("Task 1"), 
			new OldTodoTask("Task 2", OldStatus.Ready), 
			new OldTodoTask("Task 3", OldStatus.Paused)
		};

		await context.TodoTasks.AddRangeAsync(tasks);
		await context.SaveChangesAsync();
		tasks[0].Name = "Task 4";
		tasks[1].Name = "Task 5";
		tasks[2].Name = "Task 6";
		await repository.UpdateRangeAsync(tasks);
		await context.SaveChangesAsync();
		OldTodoTask? firstUpdated = await context.TodoTasks.FindAsync(tasks[0].Id);
		OldTodoTask? secondUpdated = await context.TodoTasks.FindAsync(tasks[1].Id);
		OldTodoTask? thirdUpdated = await context.TodoTasks.FindAsync(tasks[2].Id);

		Assert.NotNull(firstUpdated);
		Assert.NotNull(secondUpdated);
		Assert.NotNull(thirdUpdated);
		Assert.Equal("Task 4", firstUpdated.Name);
		Assert.Equal("Task 5", secondUpdated.Name);
		Assert.Equal("Task 6", thirdUpdated.Name);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
	}
	
	[Fact]
    public async Task FindTasksCompletedOrDropped_ReturnsTasksWithCompletedOrDroppedStatus()
    {
        // Arrange
        EfDbContext context = _fixture.CreateContext();
        TodoTaskRepository repository = new(context);
        PagingParams pagingParams = new() { PageNumber = 1, PageSize = 10 };
        List<OldTodoTask> tasks = [
            new("Task 1", OldStatus.Completed),
            new("Task 2", OldStatus.Dropped),
            new("Task 3", OldStatus.Doing),
            new("Task 4", OldStatus.Ready),
            new("Task 5", OldStatus.Backlog)
        ];

        await context.TodoTasks.AddRangeAsync(tasks);
        await context.SaveChangesAsync();
        PagedList<OldTodoTask> results = await repository.FindTasksCompletedOrDropped(pagingParams);

        // Assert
        Assert.NotNull(results);
        Assert.Equal(2, results.TotalCount);
        Assert.Equal(2, results.Count);
        Assert.All(results, task => Assert.True(task.OldStatus is OldStatus.Completed or OldStatus.Dropped));
        await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
    }

    [Fact]
    public async Task FindTasksCompletedOrDropped_OrdersByFinishDateTimeDescending()
    {
        // Arrange
        EfDbContext context = _fixture.CreateContext();
        TodoTaskRepository repository = new(context);
        PagingParams pagingParams = new() { PageNumber = 1, PageSize = 10 };
        DateTimeOffset now = DateTimeOffset.UtcNow;
        OldTodoTask firstTask = new("Task 1", OldStatus.Completed);
        OldTodoTask secondTask = new("Task 2", OldStatus.Dropped);
        OldTodoTask thirdTask = new("Task 3", OldStatus.Completed);
        OldTodoTask fourthTask = new("Task 4", OldStatus.Dropped);
        List<OldTodoTask> tasks = [
            thirdTask,
            firstTask,
            fourthTask,
            secondTask
        ];

        firstTask.SetFinishDate(now.AddDays(-1));
        secondTask.SetFinishDate(now.AddDays(-2));
        thirdTask.SetFinishDate(now);
        fourthTask.SetFinishDate(now.AddDays(-3));

        await context.TodoTasks.AddRangeAsync(tasks);
        await context.SaveChangesAsync();
        PagedList<OldTodoTask> results = await repository.FindTasksCompletedOrDropped(pagingParams);

        // Assert
        Assert.NotNull(results);
        Assert.Equal(4, results.TotalCount);
        Assert.Equal(thirdTask.Id, results[0].Id);
        Assert.Equal(firstTask.Id, results[1].Id);
        Assert.Equal(secondTask.Id, results[2].Id);
        Assert.Equal(fourthTask.Id, results[3].Id);
        await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
    }

    [Fact]
    public async Task FindTasksCompletedOrDropped_AppliesPagination()
    {
        // Arrange
        EfDbContext context = _fixture.CreateContext();
        TodoTaskRepository repository = new(context);
        PagingParams pagingParams = new() { PageNumber = 2, PageSize = 2 };
        List<OldTodoTask> tasks = [
            new("Task 1", OldStatus.Completed),
            new("Task 2", OldStatus.Dropped),
            new("Task 3", OldStatus.Completed),
            new("Task 4", OldStatus.Dropped),
            new("Task 5", OldStatus.Completed)
        ];

        await context.TodoTasks.AddRangeAsync(tasks);
        await context.SaveChangesAsync();
        PagedList<OldTodoTask> results = await repository.FindTasksCompletedOrDropped(pagingParams);

        // Assert
        Assert.NotNull(results);
        Assert.Equal(5, results.TotalCount);
        Assert.Equal(2, results.Count);
        Assert.Equal(2, results.CurrentPage);
        Assert.Equal(3, results.TotalPages);
        Assert.True(results.HasNext);
        Assert.True(results.HasPrevious);
        await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
    }
    
    [Fact]
	public async Task FindTasksInProgress_ReturnsTasksWithInProgressStatus()
	{
		// Arrange
		EfDbContext context = _fixture.CreateContext();
		TodoTaskRepository repository = new(context);
		PagingParams pagingParams = new() { PageNumber = 1, PageSize = 10 };
		List<OldTodoTask> tasks =
		[
			new("Task 1", OldStatus.Completed), new("Task 2", OldStatus.Dropped), new("Task 3", OldStatus.Doing)
			, new("Task 4", OldStatus.Ready), new("Task 5", OldStatus.Backlog)
		];

		await context.TodoTasks.AddRangeAsync(tasks);
		await context.SaveChangesAsync();
		PagedList<OldTodoTask> results = await repository.FindTasksInProgress(pagingParams);

		// Assert
		Assert.NotNull(results);
		Assert.Equal(1, results.TotalCount);
		Assert.Single(results);
		Assert.All(results, task => Assert.True(task.OldStatus is OldStatus.Doing));
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
	}

	[Fact]
    public async Task FindTasksInProgress_OrdersByPriorityDescending()
    {
        // Arrange
        EfDbContext context = _fixture.CreateContext();
        TodoTaskRepository repository = new(context);
        PagingParams pagingParams = new() { PageNumber = 1, PageSize = 10 };
        List<OldTodoTask> tasks = [
            new("Task Low", OldStatus.Doing) { Priority = OldPriority.Distraction },
            new("Task Medium", OldStatus.Doing) { Priority = OldPriority.Consider },
            new("Task Medium", OldStatus.Doing) { Priority = OldPriority.Urgent },
            new("Task High", OldStatus.Doing) { Priority = OldPriority.Critical }
        ];

        await context.TodoTasks.AddRangeAsync(tasks);
        await context.SaveChangesAsync();
        PagedList<OldTodoTask> results = await repository.FindTasksInProgress(pagingParams);

        // Assert
        Assert.NotNull(results);
        Assert.Equal(4, results.TotalCount);
        Assert.Equal(4, results.Count);
        Assert.Equal(OldPriority.Critical, results.First().Priority);
        Assert.Equal(OldPriority.Distraction, results.Last().Priority);
        await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
    }

    [Fact]
    public async Task FindTasksInProgress_ThenOrdersByLimitDateTime()
    {
        // Arrange
        EfDbContext context = _fixture.CreateContext();
        TodoTaskRepository repository = new(context);
        PagingParams pagingParams = new() { PageNumber = 1, PageSize = 10 };
        DateTimeOffset now = DateTimeOffset.UtcNow;
        OldTodoTask firstTask = new("Task Soon", OldStatus.Doing) { Priority = OldPriority.Urgent };
        OldTodoTask secondTask = new("Task Soon", OldStatus.Doing) { Priority = OldPriority.Urgent };
        OldTodoTask thirdTask = new("Task Later", OldStatus.Doing) { Priority = OldPriority.Urgent };
        OldTodoTask fourthTask = new("Task Later", OldStatus.Doing) { Priority = OldPriority.Urgent };
        List<OldTodoTask> tasks = [
	        thirdTask,
	        firstTask,
            fourthTask,
	        secondTask
        ];
        
        firstTask.SetLimitDate(now.AddDays(2));
        secondTask.SetLimitDate(now.AddDays(3));
        thirdTask.SetLimitDate(now.AddDays(4));
        fourthTask.SetLimitDate(now.AddDays(5));

        // Act
        await context.TodoTasks.AddRangeAsync(tasks);
        await context.SaveChangesAsync();
        PagedList<OldTodoTask> results = await repository.FindTasksInProgress(pagingParams);

        // Assert
        Assert.NotNull(results);
        Assert.Equal(4, results.TotalCount);
        Assert.Equal(firstTask.Id, results[0].Id);
        Assert.Equal(secondTask.Id, results[1].Id);
        Assert.Equal(thirdTask.Id, results[2].Id);
        Assert.Equal(fourthTask.Id, results[3].Id);
        await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
    }

	[Fact]
	public async Task FindTasksInProgress_AppliesPagination()
	{
		// Arrange
		EfDbContext context = _fixture.CreateContext();
		TodoTaskRepository repository = new(context);
		PagingParams pagingParams = new() { PageNumber = 2, PageSize = 2 };
		List<OldTodoTask> tasks =
		[
			new("Task 1", OldStatus.Doing), new("Task 2", OldStatus.Doing), new("Task 3", OldStatus.Doing)
			, new("Task 4", OldStatus.Doing), new("Task 5", OldStatus.Doing)
		];

		await context.TodoTasks.AddRangeAsync(tasks);
		await context.SaveChangesAsync();
		PagedList<OldTodoTask> results = await repository.FindTasksInProgress(pagingParams);

		// Assert
		Assert.NotNull(results);
		Assert.Equal(5, results.TotalCount);
		Assert.Equal(2, results.Count);
		Assert.Equal(2, results.CurrentPage);
		Assert.Equal(3, results.TotalPages);
		Assert.True(results.HasNext);
		Assert.True(results.HasPrevious);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
	}
	
	[Fact]
    public async Task FindTasksReadyToStart_ReturnsTasksWithReadyToStartOrPausedStatus()
    {
        // Arrange
        EfDbContext context = _fixture.CreateContext();
        TodoTaskRepository repository = new(context);
        PagingParams pagingParams = new() { PageNumber = 1, PageSize = 10 };
        List<OldTodoTask> tasks = [
            new("Task 1", OldStatus.Ready),
            new("Task 2", OldStatus.Paused),
            new("Task 3", OldStatus.Doing),
            new("Task 4", OldStatus.Completed),
            new("Task 5", OldStatus.Backlog)
        ];

        await context.TodoTasks.AddRangeAsync(tasks);
        await context.SaveChangesAsync();
        PagedList<OldTodoTask> results = await repository.FindTasksReadyToStart(pagingParams);

        // Assert
        Assert.NotNull(results);
        Assert.Equal(2, results.TotalCount);
        Assert.Equal(2, results.Count);
        Assert.All(results, task => Assert.True(task.OldStatus is OldStatus.Ready or OldStatus.Paused));
        await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskStreams]");
        await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
    }

    [Fact]
    public async Task FindTasksReadyToStart_OrdersByPriorityDescending()
    {
        // Arrange
        EfDbContext context = _fixture.CreateContext();
        TodoTaskRepository repository = new(context);
        PagingParams pagingParams = new() { PageNumber = 1, PageSize = 10 };
        DateTimeOffset now = DateTimeOffset.UtcNow;
        OldTodoTask firstTask = new("Task Soon", OldStatus.Ready) { Priority = OldPriority.Distraction };
        OldTodoTask secondTask = new("Task Soon", OldStatus.Ready) { Priority = OldPriority.Consider };
        OldTodoTask thirdTask = new("Task Later", OldStatus.Ready) { Priority = OldPriority.Urgent };
        OldTodoTask fourthTask = new("Task Later", OldStatus.Ready) { Priority = OldPriority.Critical };
        List<OldTodoTask> tasks = [
	        thirdTask,
	        firstTask,
	        fourthTask,
	        secondTask
        ];
        
        firstTask.SetLimitDate(now);
        secondTask.SetLimitDate(now);
        thirdTask.SetLimitDate(now);
        fourthTask.SetLimitDate(now);

        await context.TodoTasks.AddRangeAsync(tasks);
        await context.SaveChangesAsync();
        PagedList<OldTodoTask> results = await repository.FindTasksReadyToStart(pagingParams);

        // Assert
        Assert.NotNull(results);
        Assert.Equal(4, results.TotalCount);
        Assert.Equal(4, results.Count);
        Assert.Equal(OldPriority.Critical, results.First().Priority);
        Assert.Equal(OldPriority.Distraction, results.Last().Priority);
        await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskStreams]");
        await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
    }

    [Fact]
    public async Task FindTasksReadyToStart_ThenOrdersByLimitDateTime()
    {
        // Arrange
        EfDbContext context = _fixture.CreateContext();
        TodoTaskRepository repository = new(context);
        PagingParams pagingParams = new() { PageNumber = 1, PageSize = 10 };
        DateTimeOffset now = DateTimeOffset.UtcNow;
        OldTodoTask firstTask = new("Task Soon", OldStatus.Ready) { Priority = OldPriority.Urgent };
        OldTodoTask secondTask = new("Task Soon", OldStatus.Ready) { Priority = OldPriority.Urgent };
        OldTodoTask thirdTask = new("Task Later", OldStatus.Ready) { Priority = OldPriority.Urgent };
        OldTodoTask fourthTask = new("Task Later", OldStatus.Ready) { Priority = OldPriority.Urgent };
        List<OldTodoTask> tasks = [
	        thirdTask,
	        firstTask,
            fourthTask,
	        secondTask
        ];
        
        firstTask.SetLimitDate(now.AddDays(2));
        secondTask.SetLimitDate(now.AddDays(3));
        thirdTask.SetLimitDate(now.AddDays(4));
        fourthTask.SetLimitDate(now.AddDays(5));

        // Act
        await context.TodoTasks.AddRangeAsync(tasks);
        await context.SaveChangesAsync();
        PagedList<OldTodoTask> results = await repository.FindTasksReadyToStart(pagingParams);

        // Assert
        Assert.NotNull(results);
        Assert.Equal(4, results.TotalCount);
        Assert.Equal(firstTask.Id, results[0].Id);
        Assert.Equal(secondTask.Id, results[1].Id);
        Assert.Equal(thirdTask.Id, results[2].Id);
        Assert.Equal(fourthTask.Id, results[3].Id);
        await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskStreams]");
        await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
    }

    [Fact]
    public async Task FindTasksReadyToStart_FiltersTasksWithDownstreamsCompleted()
    {
        // Arrange
        EfDbContext context = _fixture.CreateContext();
        TodoTaskRepository repository = new(context);
        PagingParams pagingParams = new() { PageNumber = 1, PageSize = 10 };
        
        OldTodoTask taskWithIncompleteDownstreams = new("Main Task 1", OldStatus.Ready);
        OldTodoTask taskWithCompleteDownstreams = new("Main Task 2", OldStatus.Ready);
        OldTodoTask downstream1 = new("Downstream 1", OldStatus.Doing);
        OldTodoTask downstream2 = new("Downstream 2", OldStatus.Completed);
        TaskStream stream1 = new(taskWithIncompleteDownstreams.Id, downstream1.Id);
        TaskStream stream2 = new(taskWithCompleteDownstreams.Id, downstream2.Id);;
        
        // Act
        taskWithCompleteDownstreams.Progress = 99;
        await context.TodoTasks.AddRangeAsync(taskWithIncompleteDownstreams, taskWithCompleteDownstreams, downstream1, downstream2);
        await context.TaskStreams.AddRangeAsync(stream1, stream2);
        await context.SaveChangesAsync();
        PagedList<OldTodoTask> results = await repository.FindTasksReadyToStart(pagingParams);

		// Assert
		Assert.NotNull(results);
		Assert.Single(results);
		Assert.Equal(taskWithCompleteDownstreams.Id, results.First().Id);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskStreams]");
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
	}

	[Fact]
	public async Task GetTasksOrderedByCreationDateDescAsync_ReturnsTasksOrderedByCreationDateDescending()
	{
		// Arrange
		EfDbContext context = _fixture.CreateContext();
		TodoTaskRepository repository = new(context);
		PagingParams pagingParams = new() { PageNumber = 1, PageSize = 10 };
		List<OldTodoTask> tasks = [];
		
		// Act
		for (int i = 1; i <= 5; i++)
		{
			tasks.Add(new OldTodoTask($"Task {i}"));
			await Task.Delay(1000);
		}

		await context.TodoTasks.AddRangeAsync(tasks);
		await context.SaveChangesAsync();
		PagedList<OldTodoTask> result = await repository.GetTasksOrderedByCreationDateDescAsync(pagingParams);

		// Assert
		for (int i = 0; i < result.Count - 1; i++)
		{
			Assert.True(result[i].OldDateTimeInfo.CreationDateTime > result[i + 1].OldDateTimeInfo.CreationDateTime);
		}

		Assert.NotNull(result);
		Assert.Equal(5, result.TotalCount);
		Assert.Equal(5, result.Count);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
	}

	[Fact]
	public async Task GetTasksOrderedByCreationDateDescAsync_ReturnsCorrectPagedResults()
	{
		// Arrange
		EfDbContext context = _fixture.CreateContext();
		TodoTaskRepository repository = new(context);
		List<OldTodoTask> tasks = [];
		PagingParams pagingParams = new() { PageNumber = 1, PageSize = 3 };

		// Act
		for (int i = 1; i <= 5; i++)
		{
			tasks.Add(new OldTodoTask($"Task {i}"));
			await Task.Delay(1);
		}

		await context.TodoTasks.AddRangeAsync(tasks);
		await context.SaveChangesAsync();
		PagedList<OldTodoTask> result = await repository.GetTasksOrderedByCreationDateDescAsync(pagingParams);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(5, result.TotalCount);
		Assert.Equal(3, result.Count);
		Assert.Equal(1, result.CurrentPage);
		Assert.Equal(3, result.PageSize);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
	}

	[Fact]
	public async Task GetTasksOrderedByCreationDateDescAsync_ReturnsCorrectSecondPage()
	{
		// Arrange
		EfDbContext context = _fixture.CreateContext();
		TodoTaskRepository repository = new(context);
		List<OldTodoTask> tasks = [];
		PagingParams pagingParams = new() { PageNumber = 2, PageSize = 3 };

		// Act
		for (int i = 1; i <= 5; i++)
		{
			tasks.Add(new OldTodoTask($"Task {i}"));
			await Task.Delay(1);
		}

		await context.TodoTasks.AddRangeAsync(tasks);
		await context.SaveChangesAsync();
		PagedList<OldTodoTask> result = await repository.GetTasksOrderedByCreationDateDescAsync(pagingParams);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(5, result.TotalCount);
		Assert.Equal(2, result.Count);
		Assert.Equal(2, result.CurrentPage);
		Assert.Equal(3, result.PageSize);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
	}

	[Fact]
	public async Task GetTasksOrderedByCreationDateDescAsync_ReturnsEmptyWhenNoTasks()
	{
		// Arrange
		EfDbContext context = _fixture.CreateContext();
		TodoTaskRepository repository = new(context);
		PagingParams pagingParams = new() { PageNumber = 1, PageSize = 10 };

		// Act
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
		PagedList<OldTodoTask> result = await repository.GetTasksOrderedByCreationDateDescAsync(pagingParams);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(0, result.TotalCount);
		Assert.Empty(result);
		Assert.Equal(1, result.CurrentPage);
		Assert.Equal(10, result.PageSize);
	}

	[Fact]
	public async Task GetTasksOrderedByCreationDateDescAsync_ReturnsEmptyWhenPageExceedsTotalPages()
	{
		// Arrange
		EfDbContext context = _fixture.CreateContext();
		TodoTaskRepository repository = new(context);
		OldTodoTask task1 = new("Task 1");
		OldTodoTask task2 = new("Task 2");
		PagingParams pagingParams = new() { PageNumber = 3, PageSize = 10 };

		// Act
		await context.TodoTasks.AddRangeAsync(task1, task2);
		await context.SaveChangesAsync();
		PagedList<OldTodoTask> result = await repository.GetTasksOrderedByCreationDateDescAsync(pagingParams);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(2, result.TotalCount);
		Assert.Empty(result);
		Assert.Equal(3, result.CurrentPage);
		Assert.Equal(10, result.PageSize);
		await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
	}
}
