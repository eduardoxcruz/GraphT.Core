using GraphT.Model.Entities;
using GraphT.Model.Services.Repositories;
using GraphT.Model.ValueObjects;
using GraphT.UseCases.FindFinishedTasks;

using NSubstitute;

using SeedWork;

namespace GraphT.UseCases.Tests.FindFinishedTasks;

public class UseCaseTests
{
    [Fact]
    public async Task Handle_WhenTasksExist_ReturnsPagedListOfFinishedOrDroppedTasks()
    {
        // Arrange
        IOutputPort outputPort = Substitute.For<IOutputPort>();
        ITodoTaskRepository todoTaskRepository = Substitute.For<ITodoTaskRepository>();

        TodoTask task1 = new("Task 1", OldStatus.Completed);
        TodoTask task2 = new("Task 2", OldStatus.Dropped);
        List<TodoTask> tasks = [task1, task2];
        PagingParams pagingParams = new() { PageNumber = 1, PageSize = 10 };
        InputDto input = new() { PagingParams = pagingParams };

        todoTaskRepository.FindTasksCompletedOrDropped(pagingParams)
            .Returns(new PagedList<TodoTask>(tasks, tasks.Count, pagingParams.PageNumber, pagingParams.PageSize));

        UseCase useCase = new(outputPort, todoTaskRepository);

        // Act
        await useCase.Handle(input);

        // Assert
        await todoTaskRepository.Received(1).FindTasksCompletedOrDropped(Arg.Is<PagingParams>(p => 
            p.PageNumber == pagingParams.PageNumber &&
            p.PageSize == pagingParams.PageSize
        ));
        await outputPort.Received(1).Handle(Arg.Is<OutputDto>(dto =>
            dto.Tasks.Count == 2 &&
            dto.Tasks.TotalCount == 2 &&
            dto.Tasks.CurrentPage == 1 &&
            dto.Tasks.PageSize == 10 &&
            dto.Tasks.All(task => task.OldStatus == OldStatus.Completed || task.OldStatus == OldStatus.Dropped)
        ));
    }

    [Fact]
    public async Task Handle_WhenNoTasksExist_ReturnsEmptyPagedList()
    {
        // Arrange
        IOutputPort outputPort = Substitute.For<IOutputPort>();
        ITodoTaskRepository todoTaskRepository = Substitute.For<ITodoTaskRepository>();

        PagingParams pagingParams = new() { PageNumber = 1, PageSize = 10 };
        InputDto input = new() { PagingParams = pagingParams };

        todoTaskRepository.FindTasksCompletedOrDropped(pagingParams)
            .Returns(new PagedList<TodoTask>([], 0, pagingParams.PageNumber, pagingParams.PageSize));

        UseCase useCase = new(outputPort, todoTaskRepository);

        // Act
        await useCase.Handle(input);

        // Assert
        await todoTaskRepository.Received(1).FindTasksCompletedOrDropped(Arg.Any<PagingParams>());
        await outputPort.Received(1).Handle(Arg.Is<OutputDto>(dto =>
            dto.Tasks.Count == 0 &&
            dto.Tasks.TotalCount == 0 &&
            dto.Tasks.CurrentPage == 1 &&
            dto.Tasks.PageSize == 10
        ));
    }

    [Fact]
    public async Task Handle_ValidatesPagingParameters()
    {
        // Arrange
        IOutputPort outputPort = Substitute.For<IOutputPort>();
        ITodoTaskRepository todoTaskRepository = Substitute.For<ITodoTaskRepository>();

        PagingParams pagingParams = new() { PageNumber = 2, PageSize = 5 };
        InputDto input = new() { PagingParams = pagingParams };

        todoTaskRepository.FindTasksCompletedOrDropped(pagingParams)
            .Returns(new PagedList<TodoTask>([], 0, pagingParams.PageNumber, pagingParams.PageSize));

        UseCase useCase = new(outputPort, todoTaskRepository);

        // Act
        await useCase.Handle(input);

        // Assert
        await todoTaskRepository.Received(1).FindTasksCompletedOrDropped(Arg.Is<PagingParams>(p =>
            p.PageNumber == pagingParams.PageNumber &&
            p.PageSize == pagingParams.PageSize
        ));
    }
}
