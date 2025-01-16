using GraphT.Model.Aggregates;
using GraphT.Model.Services.Specifications;
using GraphT.Model.ValueObjects;

using Microsoft.EntityFrameworkCore;

using SeedWork;

namespace GraphT.EfCore.Repositories.Tests.Specifications;

public class FinishedTasksSpecificationTests : IClassFixture<TestDatabaseFixture>
{
	private readonly TestDatabaseFixture _fixture;

    public FinishedTasksSpecificationTests(TestDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task WhenNameIsNull_ReturnsAllCompletedAndDroppedTasks()
    {
        // Arrange
        EfDbContext context = _fixture.CreateContext();
        Repository<TodoTask> repository = new(context);
        PagingParams pagingParams = new() { PageNumber = 1, PageSize = 10 };
        FinishedTasksSpecification spec = new(null, pagingParams);
        List<TodoTask> tasks = [
            new("Task 1", Status.InProgress),
            new("Task 2", Status.ReadyToStart),
            new("Task 3", Status.Completed),
            new("Task 4", Status.Paused),
            new("Task 5", Status.Dropped)
        ];

        // Act
        await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
        await context.TodoTasks.AddRangeAsync(tasks);
        await context.SaveChangesAsync();
        PagedList<TodoTask> results = await repository.FindAsync(spec);

        // Assert
        Assert.NotNull(results);
        Assert.Equal(2, results.TotalCount);
        Assert.All(results, task => Assert.True(task.Status is Status.Completed or Status.Dropped));
    }

    [Fact]
    public async Task WhenNameFilter_ReturnsMatchingFinishedTasks()
    {
        // Arrange
        EfDbContext context = _fixture.CreateContext();
        Repository<TodoTask> repository = new(context);
        string nameFilter = "Special";
        PagingParams pagingParams = new() { PageNumber = 1, PageSize = 10 };
        FinishedTasksSpecification spec = new(nameFilter, pagingParams);
        List<TodoTask> tasks = [
            new("Special Task 1", Status.Completed),
            new("Special Task 3", Status.InProgress),
            new("Regular Task", Status.Completed),
            new("Special Task 2", Status.Dropped),
            new("Another Task", Status.Dropped)
        ];

        // Act
        await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
        await context.TodoTasks.AddRangeAsync(tasks);
        await context.SaveChangesAsync();
        PagedList<TodoTask> results = await repository.FindAsync(spec);

        // Assert
        Assert.NotNull(results);
        Assert.Equal(2, results.TotalCount);
        Assert.All(results, task => Assert.Contains(nameFilter, task.Name));
    }

    [Fact]
    public async Task WhenPagingParamsApplied_ReturnsPagedList()
    {
        // Arrange
        EfDbContext context = _fixture.CreateContext();
        Repository<TodoTask> repository = new(context);
        PagingParams pagingParams = new() { PageNumber = 2, PageSize = 2 };
        FinishedTasksSpecification spec = new(null, pagingParams);
        List<TodoTask> tasks = [
            new("Task 1", Status.Completed),
            new("Task 2", Status.Completed),
            new("Task 3", Status.Completed),
            new("Task 4", Status.Dropped),
            new("Task 5", Status.Dropped)
        ];

        // Act
        await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
        await context.TodoTasks.AddRangeAsync(tasks);
        await context.SaveChangesAsync();
        PagedList<TodoTask> results = await repository.FindAsync(spec);

        // Assert
        Assert.NotNull(results);
        Assert.Equal(2, results.Count);
        Assert.Equal(5, results.TotalCount);
        Assert.Equal(pagingParams.PageNumber, results.CurrentPage);
        Assert.Equal(3, results.TotalPages);
        Assert.True(results.HasNext);
        Assert.True(results.HasPrevious);
    }

    [Fact]
    public async Task Find_WithNoFinishedTasks_ReturnsEmptyList()
    {
        // Arrange
        EfDbContext context = _fixture.CreateContext();
        Repository<TodoTask> repository = new(context);
        PagingParams pagingParams = new() { PageNumber = 1, PageSize = 10 };
        FinishedTasksSpecification spec = new(null, pagingParams);
        List<TodoTask> tasks = [
            new("Task 1", Status.InProgress),
            new("Task 2", Status.Paused),
            new("Task 3", Status.ReadyToStart)
        ];

        // Act
        await context.Database.ExecuteSqlAsync($"DELETE FROM [TodoTasks]");
        await context.TodoTasks.AddRangeAsync(tasks);
        await context.SaveChangesAsync();
        PagedList<TodoTask> results = await repository.FindAsync(spec);

        // Assert
        Assert.NotNull(results);
        Assert.Empty(results);
        Assert.Equal(0, results.TotalCount);
        Assert.Equal(1, results.CurrentPage);
        Assert.Equal(1, results.TotalPages);
        Assert.False(results.HasNext);
        Assert.False(results.HasPrevious);
    }
}
