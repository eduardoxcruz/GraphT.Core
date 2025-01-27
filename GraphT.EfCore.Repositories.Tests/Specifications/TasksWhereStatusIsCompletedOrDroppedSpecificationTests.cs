using GraphT.Model.Aggregates;
using GraphT.Model.Services.Specifications;
using GraphT.Model.ValueObjects;

using Microsoft.EntityFrameworkCore;

using SeedWork;

namespace GraphT.EfCore.Repositories.Tests.Specifications;

public class TasksWhereStatusIsCompletedOrDroppedSpecificationTests : IClassFixture<TestDatabaseFixture>
{
	private readonly TestDatabaseFixture _fixture;

    public TasksWhereStatusIsCompletedOrDroppedSpecificationTests(TestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public async Task Find_ReturnsTasksWithCompletedOrDroppedStatus()
    {
        // Arrange
        EfDbContext context = _fixture.CreateContext();
        Repository<TodoTask> repository = new(context);
        PagingParams pagingParams = new() { PageNumber = 1, PageSize = 10 };
        TasksWhereStatusIsCompletedOrDroppedSpecification spec = new(pagingParams);
        List<TodoTask> tasks = [
            new("Task 1", Status.Completed),
            new("Task 2", Status.Dropped),
            new("Task 3", Status.InProgress),
            new("Task 4", Status.ReadyToStart),
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
        Assert.All(results, task => Assert.True(task.Status is Status.Completed or Status.Dropped));
    }

    [Fact]
    public async Task Find_OrdersByFinishDateTimeDescending()
    {
        // Arrange
        EfDbContext context = _fixture.CreateContext();
        Repository<TodoTask> repository = new(context);
        PagingParams pagingParams = new() { PageNumber = 1, PageSize = 10 };
        TasksWhereStatusIsCompletedOrDroppedSpecification spec = new(pagingParams);
        DateTimeOffset now = DateTimeOffset.UtcNow;
        TodoTask firstTask = new("Task 1", Status.Completed);
        TodoTask secondTask = new("Task 2", Status.Dropped);
        TodoTask thirdTask = new("Task 3", Status.Completed);
        TodoTask fourthTask = new("Task 4", Status.Dropped);
        List<TodoTask> tasks = [
            thirdTask,
            firstTask,
            fourthTask,
            secondTask
        ];

        firstTask.SetFinishDate(now.AddDays(-1));
        secondTask.SetFinishDate(now.AddDays(-2));
        thirdTask.SetFinishDate(now);
        fourthTask.SetFinishDate(now.AddDays(-3));

        await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
        await context.TodoTasks.AddRangeAsync(tasks);
        await context.SaveChangesAsync();
        PagedList<TodoTask> results = await repository.FindAsync(spec);

        // Assert
        Assert.NotNull(results);
        Assert.Equal(4, results.TotalCount);
        Assert.Equal(thirdTask.Id, results[0].Id);
        Assert.Equal(firstTask.Id, results[1].Id);
        Assert.Equal(secondTask.Id, results[2].Id);
        Assert.Equal(fourthTask.Id, results[3].Id);
    }

    [Fact]
    public async Task Find_AppliesPagination()
    {
        // Arrange
        EfDbContext context = _fixture.CreateContext();
        Repository<TodoTask> repository = new(context);
        PagingParams pagingParams = new() { PageNumber = 2, PageSize = 2 };
        TasksWhereStatusIsCompletedOrDroppedSpecification spec = new(pagingParams);
        List<TodoTask> tasks = [
            new("Task 1", Status.Completed),
            new("Task 2", Status.Dropped),
            new("Task 3", Status.Completed),
            new("Task 4", Status.Dropped),
            new("Task 5", Status.Completed)
        ];

        await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
        await context.TodoTasks.AddRangeAsync(tasks);
        await context.SaveChangesAsync();
        PagedList<TodoTask> results = await repository.FindAsync(spec);

        // Assert
        Assert.NotNull(results);
        Assert.Equal(5, results.TotalCount);
        Assert.Equal(2, results.Count);
        Assert.Equal(2, results.CurrentPage);
        Assert.Equal(3, results.TotalPages);
        Assert.True(results.HasNext);
        Assert.True(results.HasPrevious);
    }
}
