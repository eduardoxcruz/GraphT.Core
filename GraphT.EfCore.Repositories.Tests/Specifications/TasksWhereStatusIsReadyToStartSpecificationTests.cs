using GraphT.Model.Aggregates;
using GraphT.Model.Services.Specifications;
using GraphT.Model.ValueObjects;

using Microsoft.EntityFrameworkCore;

using SeedWork;

namespace GraphT.EfCore.Repositories.Tests.Specifications;

public class TasksWhereStatusIsReadyToStartSpecificationTests: IClassFixture<TestDatabaseFixture>
{
    private readonly TestDatabaseFixture _fixture;

    public TasksWhereStatusIsReadyToStartSpecificationTests(TestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Find_ReturnsTasksWithReadyToStartOrPausedStatus()
    {
        // Arrange
        EfDbContext context = _fixture.CreateContext();
        Repository<TodoTask> repository = new(context);
        PagingParams pagingParams = new() { PageNumber = 1, PageSize = 10 };
        TasksWhereStatusIsReadyToStartSpecification spec = new(pagingParams);
        List<TodoTask> tasks = [
            new("Task 1", Status.Ready),
            new("Task 2", Status.Paused),
            new("Task 3", Status.Doing),
            new("Task 4", Status.Completed),
            new("Task 5", Status.Backlog)
        ];

        await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
        await context.TodoTasks.AddRangeAsync(tasks);
        await context.SaveChangesAsync();
        PagedList<TodoTask> results = await repository.FindAsync(spec);

        // Assert
        Assert.NotNull(results);
        Assert.Equal(2, results.TotalCount);
        Assert.Equal(2, results.Count);
        Assert.All(results, task => Assert.True(task.Status is Status.Ready or Status.Paused)); 
    }

    [Fact]
    public async Task Find_OrdersByPriorityDescending()
    {
        // Arrange
        EfDbContext context = _fixture.CreateContext();
        Repository<TodoTask> repository = new(context);
        PagingParams pagingParams = new() { PageNumber = 1, PageSize = 10 };
        TasksWhereStatusIsReadyToStartSpecification spec = new(pagingParams);
        List<TodoTask> tasks = [
            new("Task Low", Status.Ready) { Priority = Priority.Distraction },
            new("Task Medium", Status.Paused) { Priority = Priority.Consider },
            new("Task Medium", Status.Ready) { Priority = Priority.Urgent },
            new("Task High", Status.Paused) { Priority = Priority.Critical }
        ];

        await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
        await context.TodoTasks.AddRangeAsync(tasks);
        await context.SaveChangesAsync();
        PagedList<TodoTask> results = await repository.FindAsync(spec);

        // Assert
        Assert.NotNull(results);
        Assert.Equal(4, results.TotalCount);
        Assert.Equal(4, results.Count);
        Assert.Equal(Priority.Critical, results.First().Priority);
        Assert.Equal(Priority.Distraction, results.Last().Priority);
    }

    [Fact]
    public async Task Find_ThenOrdersByLimitDateTime()
    {
        // Arrange
        EfDbContext context = _fixture.CreateContext();
        Repository<TodoTask> repository = new(context);
        PagingParams pagingParams = new() { PageNumber = 1, PageSize = 10 };
        TasksWhereStatusIsReadyToStartSpecification spec = new(pagingParams);
        DateTimeOffset now = DateTimeOffset.UtcNow;
        TodoTask firstTask = new("Task Soon", Status.Ready) { Priority = Priority.Urgent };
        TodoTask secondTask = new("Task Soon", Status.Ready) { Priority = Priority.Urgent };
        TodoTask thirdTask = new("Task Later", Status.Ready) { Priority = Priority.Urgent };
        TodoTask fourthTask = new("Task Later", Status.Ready) { Priority = Priority.Urgent };
        List<TodoTask> tasks = [
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
        await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
        await context.TodoTasks.AddRangeAsync(tasks);
        await context.SaveChangesAsync();
        PagedList<TodoTask> results = await repository.FindAsync(spec);

        // Assert
        Assert.NotNull(results);
        Assert.Equal(4, results.TotalCount);
        Assert.Equal(firstTask.Id, results[0].Id);
        Assert.Equal(secondTask.Id, results[1].Id);
        Assert.Equal(thirdTask.Id, results[2].Id);
        Assert.Equal(fourthTask.Id, results[3].Id);
    }

    [Fact]
    public async Task Find_FiltersTasksWithDownstreamsCompleted()
    {
        // Arrange
        EfDbContext context = _fixture.CreateContext();
        Repository<TodoTask> repository = new(context);
        PagingParams pagingParams = new() { PageNumber = 1, PageSize = 10 };
        TasksWhereStatusIsReadyToStartSpecification spec = new(pagingParams);
        
        TodoTask taskWithIncompleteDownstreams = new("Main Task 1", Status.Ready);
        TodoTask taskWithCompleteDownstreams = new("Main Task 2", Status.Ready);
        TodoTask downstream1 = new("Downstream 1", Status.Doing);
        TodoTask downstream2 = new("Downstream 2", Status.Completed);

        taskWithIncompleteDownstreams.AddDownstream(downstream1);
        taskWithCompleteDownstreams.AddDownstream(downstream2);
        taskWithCompleteDownstreams.Progress = 99;

        // Act
        await context.Database.ExecuteSqlAsync($"DELETE FROM [TaskStreams]");
        await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
        await context.TodoTasks.AddRangeAsync([
            taskWithIncompleteDownstreams, 
            taskWithCompleteDownstreams,
            downstream1,
            downstream2
        ]);
        int result = await context.SaveChangesAsync();
        PagedList<TodoTask> results = await repository.FindAsync(spec);

        // Assert
        Assert.NotNull(results);
        Assert.Single(results);
        Assert.Equal(taskWithCompleteDownstreams.Id, results.First().Id);
    }
}
