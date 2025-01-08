using GraphT.Model.Aggregates;
using GraphT.Model.Services.Specifications;
using GraphT.UseCases.FindTaskUpstreamsById;

using NSubstitute;

using SeedWork;

namespace GraphT.UseCases.Tests.FindTaskUpstreamsById;

public class UseCaseTests
{
	[Fact]
    public async Task Handle_ShouldCreateSpecificationWithCorrectParameters()
    {
        // Arrange
        IOutputPort outputPort = Substitute.For<IOutputPort>();
        IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        IRepository<TaskAggregate> repository = Substitute.For<IRepository<TaskAggregate>>();
        
        Guid taskId = Guid.NewGuid();
        PagingParams pagingParams = new() { Page = 1, PageSize = 10 };
        InputDto input = new() { Id = taskId, PagingParams = pagingParams };
        
        unitOfWork.Repository<TaskAggregate>().Returns(repository);
        repository.FindAsync(Arg.Any<FindUpstreamsByTaskIdSpecification>()).Returns(new PagedList<TaskAggregate>(new List<TaskAggregate>(), 0, 1, 10));
        
        UseCase useCase = new UseCase(outputPort, unitOfWork);

        // Act
        await useCase.Handle(input);

        // Assert
        await repository.Received(1).FindAsync(
            Arg.Is<FindUpstreamsByTaskIdSpecification>(spec => 
                spec.TaskId == taskId && 
                spec.PagingParams.Page == pagingParams.Page && 
                spec.PagingParams.PageSize == pagingParams.PageSize
            )
        );
    }

    [Fact]
    public async Task Handle_WhenTasksExist_ShouldMapAndReturnCorrectly()
    {
        // Arrange
        IOutputPort outputPort = Substitute.For<IOutputPort>();
        IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        IRepository<TaskAggregate> repository = Substitute.For<IRepository<TaskAggregate>>();
        
        List<TaskAggregate> tasks = new()
        {
            new TaskAggregate() { Id = Guid.NewGuid(), Name = "Task 1" },
            new TaskAggregate() { Id = Guid.NewGuid(), Name = "Task 2" }
        };
        
        int totalCount = 10;
        int currentPage = 1;
        int pageSize = 2;
        
        PagedList<TaskAggregate> pagedTasks = new(tasks, totalCount, currentPage, pageSize);
        
        unitOfWork.Repository<TaskAggregate>().Returns(repository);
        repository.FindAsync(Arg.Any<FindUpstreamsByTaskIdSpecification>())
            .Returns(pagedTasks);
        
        UseCase useCase = new UseCase(outputPort, unitOfWork);
        InputDto input = new() 
        { 
            Id = Guid.NewGuid(), 
            PagingParams = new PagingParams { Page = currentPage, PageSize = pageSize } 
        };

        // Act
        await useCase.Handle(input);

        // Assert
        await outputPort.Received(1).Handle(Arg.Is<OutputDto>(dto => 
            dto.Upstreams.Count == tasks.Count &&
            dto.Upstreams.TotalCount == totalCount &&
            dto.Upstreams.CurrentPage == currentPage &&
            dto.Upstreams.PageSize == pageSize
        ));
    }

    [Fact]
    public async Task Handle_WhenNoTasks_ShouldReturnEmptyPagedList()
    {
        // Arrange
        IOutputPort outputPort = Substitute.For<IOutputPort>();
        IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        IRepository<TaskAggregate> repository = Substitute.For<IRepository<TaskAggregate>>();
        
        List<TaskAggregate> emptyTasks = new();
        PagedList<TaskAggregate> emptyPagedList = new(emptyTasks, 0, 1, 10);
        
        unitOfWork.Repository<TaskAggregate>().Returns(repository);
        repository.FindAsync(Arg.Any<FindUpstreamsByTaskIdSpecification>())
            .Returns(emptyPagedList);
        
        UseCase useCase = new UseCase(outputPort, unitOfWork);
        InputDto input = new() 
        { 
            Id = Guid.NewGuid(), 
            PagingParams = new PagingParams { Page = 1, PageSize = 10 } 
        };

        // Act
        await useCase.Handle(input);

        // Assert
        await outputPort.Received(1).Handle(Arg.Is<OutputDto>(dto => 
            dto.Upstreams.Count == 0 &&
            dto.Upstreams.TotalCount == 0 &&
            dto.Upstreams.CurrentPage == 1 &&
            dto.Upstreams.PageSize == 10
        ));
    }
}
