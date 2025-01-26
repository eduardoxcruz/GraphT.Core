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
            new("Task 1", Status.ReadyToStart),
            new("Task 2", Status.Paused),
            new("Task 3", Status.InProgress),
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
        Assert.All(results, task => Assert.True(task.Status is Status.ReadyToStart or Status.Paused)); 
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
            new("Task Low", Status.ReadyToStart) { Priority = Priority.MentalClutter },
            new("Task Medium", Status.Paused) { Priority = Priority.ThinkAboutIt },
            new("Task Medium", Status.ReadyToStart) { Priority = Priority.DoItNow },
            new("Task High", Status.Paused) { Priority = Priority.DropEverythingElse }
        ];

        await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
        await context.TodoTasks.AddRangeAsync(tasks);
        await context.SaveChangesAsync();
        PagedList<TodoTask> results = await repository.FindAsync(spec);

        // Assert
        Assert.NotNull(results);
        Assert.Equal(4, results.TotalCount);
        Assert.Equal(4, results.Count);
        Assert.Equal(Priority.DropEverythingElse, results.First().Priority);
        Assert.Equal(Priority.MentalClutter, results.Last().Priority);
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
        TodoTask firstTask = new("Task Soon", Status.ReadyToStart) { Priority = Priority.DoItNow };
        TodoTask secondTask = new("Task Soon", Status.ReadyToStart) { Priority = Priority.DoItNow };
        TodoTask thirdTask = new("Task Later", Status.ReadyToStart) { Priority = Priority.DoItNow };
        TodoTask fourthTask = new("Task Later", Status.ReadyToStart) { Priority = Priority.DoItNow };
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
        
        TodoTask taskWithIncompleteDownstreams = new("Main Task 1", Status.ReadyToStart);
        TodoTask taskWithCompleteDownstreams = new("Main Task 2", Status.ReadyToStart);
        TodoTask downstream1 = new("Downstream 1", Status.InProgress);
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
