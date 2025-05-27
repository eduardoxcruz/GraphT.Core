using GraphT.Model.Aggregates;
using GraphT.Model.Entities;
using GraphT.Model.Services.Specifications;
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
        IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        IRepository<TodoTask> repository = Substitute.For<IRepository<TodoTask>>();

        TodoTask task1 = new("Task 1", Status.Completed);
        TodoTask task2 = new("Task 2", Status.Dropped);
        List<TodoTask> tasks = [task1, task2];
        PagingParams pagingParams = new() { PageNumber = 1, PageSize = 10 };
        InputDto input = new() { PagingParams = pagingParams };

        unitOfWork.Repository<TodoTask>().Returns(repository);
        repository.FindAsync(Arg.Any<TasksWhereStatusIsCompletedOrDroppedSpecification>()).Returns(new PagedList<TodoTask>(tasks, tasks.Count, pagingParams.PageNumber, pagingParams.PageSize));

        UseCase useCase = new(outputPort, unitOfWork);

        // Act
        await useCase.Handle(input);

        // Assert
        await repository.Received(1).FindAsync(Arg.Is<TasksWhereStatusIsCompletedOrDroppedSpecification>(spec =>
            spec.PageNumber == pagingParams.PageNumber &&
            spec.PageSize == pagingParams.PageSize
        ));
        await outputPort.Received(1).Handle(Arg.Is<OutputDto>(dto =>
            dto.Tasks.Count == 2 &&
            dto.Tasks.TotalCount == 2 &&
            dto.Tasks.CurrentPage == 1 &&
            dto.Tasks.PageSize == 10 &&
            dto.Tasks.All(task => task.Status == Status.Completed || task.Status == Status.Dropped)
        ));
    }

    [Fact]
    public async Task Handle_WhenNoTasksExist_ReturnsEmptyPagedList()
    {
        // Arrange
        IOutputPort outputPort = Substitute.For<IOutputPort>();
        IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        IRepository<TodoTask> repository = Substitute.For<IRepository<TodoTask>>();

        PagingParams pagingParams = new() { PageNumber = 1, PageSize = 10 };
        InputDto input = new() { PagingParams = pagingParams };

        unitOfWork.Repository<TodoTask>().Returns(repository);
        repository.FindAsync(Arg.Any<TasksWhereStatusIsCompletedOrDroppedSpecification>())
            .Returns(new PagedList<TodoTask>([], 0, pagingParams.PageNumber, pagingParams.PageSize));

        UseCase useCase = new(outputPort, unitOfWork);

        // Act
        await useCase.Handle(input);

        // Assert
        await repository.Received(1).FindAsync(Arg.Any<TasksWhereStatusIsCompletedOrDroppedSpecification>());
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
        IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        IRepository<TodoTask> repository = Substitute.For<IRepository<TodoTask>>();

        PagingParams pagingParams = new() { PageNumber = 2, PageSize = 5 };
        InputDto input = new() { PagingParams = pagingParams };

        unitOfWork.Repository<TodoTask>().Returns(repository);
        repository.FindAsync(Arg.Any<TasksWhereStatusIsCompletedOrDroppedSpecification>())
            .Returns(new PagedList<TodoTask>([], 0, pagingParams.PageNumber, pagingParams.PageSize));

        UseCase useCase = new(outputPort, unitOfWork);

        // Act
        await useCase.Handle(input);

        // Assert
        await repository.Received(1).FindAsync(Arg.Is<TasksWhereStatusIsCompletedOrDroppedSpecification>(spec =>
            spec.PageNumber == pagingParams.PageNumber &&
            spec.PageSize == pagingParams.PageSize
        ));
    }
}
