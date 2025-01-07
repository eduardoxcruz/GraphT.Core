using GraphT.Model.Aggregates;
using GraphT.Model.Services.Specifications;
using GraphT.UseCases.FindUnfinishedTasksByName;

using NSubstitute;

using SeedWork;

namespace GraphT.UseCases.Tests.FindUnfinishedTasksByName;

public class UseCaseTests
{
	[Fact]
    public async Task Handle_WhenCalled_ShouldRetrieveTasksFromRepository()
    {
        // Arrange
        IOutputPort outputPort = Substitute.For<IOutputPort>();
        IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        IRepository<TaskAggregate> repository = Substitute.For<IRepository<TaskAggregate>>();
        InputDto inputDto = new() { TaskName = "Test Task", PagingParams = new PagingParams { PageNumber = 1, PageSize = 10 }};
        PagedList<TaskAggregate> pagedTaskAggregates = new(
	        new()
	        {
		        new("Test Task 1"), 
		        new("Test Task 2")
	        }, 
	        2, 
	        1, 
	        10);
        UseCase useCase = new(outputPort, unitOfWork);

        repository.FindAsync(Arg.Any<UnfinishedTasksSpecification>()).Returns(pagedTaskAggregates);
        unitOfWork.Repository<TaskAggregate>().Returns(repository);

        // Act
        await useCase.Handle(inputDto);

        // Assert
        await repository.Received(1).FindAsync(Arg.Any<UnfinishedTasksSpecification>());
        await outputPort.Received(1).Handle(Arg.Is<OutputDto>(dto => 
            dto.Tasks.Count == 2 &&
            dto.Tasks.CurrentPage == 1 &&
            dto.Tasks.PageSize == 10 &&
            dto.Tasks.TotalCount == 2
        ));
    }

    [Fact]
    public async Task Handle_WithEmptyRepository_ShouldReturnEmptyPagedList()
    {
        // Arrange
        IOutputPort outputPort = Substitute.For<IOutputPort>();
        IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        IRepository<TaskAggregate> repository = Substitute.For<IRepository<TaskAggregate>>();
        InputDto inputDto = new()
        {
            TaskName = "Nonexistent Task",
            PagingParams = new PagingParams { PageNumber = 1, PageSize = 10 }
        };
        PagedList<TaskAggregate> emptyPagedList = new([], 0, 1, 10);
        UseCase useCase = new(outputPort, unitOfWork);

        repository.FindAsync(Arg.Any<UnfinishedTasksSpecification>()).Returns(emptyPagedList);
        unitOfWork.Repository<TaskAggregate>().Returns(repository);

        // Act
        await useCase.Handle(inputDto);

        // Assert
        await repository.Received(1).FindAsync(Arg.Any<UnfinishedTasksSpecification>());
        await outputPort.Received(1).Handle(Arg.Is<OutputDto>(dto => 
            dto.Tasks.Count == 0 &&
            dto.Tasks.CurrentPage == 1 &&
            dto.Tasks.PageSize == 10 &&
            dto.Tasks.TotalCount == 0
        ));
    }

    [Fact]
    public async Task Handle_WithNullDtoTaskName_ShouldQueryRepositoryAllUnfinishedFinishedTasksTasks()
    {
        // Arrange
        IOutputPort outputPort = Substitute.For<IOutputPort>();
        IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        IRepository<TaskAggregate> repository = Substitute.For<IRepository<TaskAggregate>>();
        InputDto inputDto = new() { PagingParams = new PagingParams { PageNumber = 3, PageSize = 10 } };
        PagedList<TaskAggregate> pagedTaskAggregates = new(
	        new()
	        {
		        new("Test Task 1"), 
		        new("Test Task 2"), 
		        new("Other Task")
	        }, 
	        3, 
	        1, 
	        10);
        UseCase useCase = new(outputPort, unitOfWork);

        repository.FindAsync(Arg.Any<UnfinishedTasksSpecification>()).Returns(pagedTaskAggregates);
        unitOfWork.Repository<TaskAggregate>().Returns(repository);

        // Act
        await useCase.Handle(inputDto);

        // Assert
        await repository.Received(1).FindAsync(Arg.Any<UnfinishedTasksSpecification>());
        await outputPort.Received(1).Handle(Arg.Is<OutputDto>(dto => 
	        dto.Tasks.Count == 3 &&
	        dto.Tasks.CurrentPage == 1 &&
	        dto.Tasks.PageSize == 10 &&
	        dto.Tasks.TotalCount == 3
        ));
    }
}
