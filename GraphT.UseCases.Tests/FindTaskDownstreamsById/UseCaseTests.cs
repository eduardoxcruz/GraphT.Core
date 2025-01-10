using GraphT.Model.Aggregates;
using GraphT.Model.Services.Specifications;
using GraphT.UseCases.FindTaskDownstreamsById;

using NSubstitute;

using SeedWork;

namespace GraphT.UseCases.Tests.FindTaskDownstreamsById;

public class UseCaseTests
{
	[Fact]
	public async Task Handle_WhenTaskHasDownstreams_ReturnsPagedListOfDownstreams()
	{
		// Arrange
		IOutputPort outputPort = Substitute.For<IOutputPort>();
		IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
		IRepository<TaskAggregate> repository = Substitute.For<IRepository<TaskAggregate>>();

		Guid taskId = Guid.NewGuid();
		TaskAggregate testTask = new("Test Task", id: taskId);
		TaskAggregate taskDownstream1 = new("Test Downstream 1");
		TaskAggregate taskDownstream2 = new("Test Downstream 2");
		testTask.AddDownstreams([taskDownstream1, taskDownstream2]);
		PagingParams pagingParams = new() { PageNumber = 1, PageSize = 10 };
		InputDto input = new() { Id = taskId, PagingParams = pagingParams };

		unitOfWork.Repository<TaskAggregate>().Returns(repository);
		repository.FindByIdAsync(taskId).Returns(testTask);
		repository.FindAsync(Arg.Any<TaskIncludeDownstreamsSpecification>()).Returns(new PagedList<TaskAggregate>([ testTask ], 1, 1, 10));

		UseCase useCase = new(outputPort, unitOfWork);

		// Act
		await useCase.Handle(input);

		// Assert
		await repository.Received(1).FindAsync(Arg.Any<TaskIncludeDownstreamsSpecification>());
		await outputPort.Received(1).Handle(Arg.Is<OutputDto>(dto => 
			dto.Downstreams.Count == 2 &&
			dto.Downstreams.TotalCount == 2 &&
			dto.Downstreams.CurrentPage == 1 &&
			dto.Downstreams.PageSize == 10 &&
			dto.Downstreams.Any(t => t.Id.Equals(taskDownstream1.Id)) &&
			dto.Downstreams.Any(t => t.Id.Equals(taskDownstream2.Id))
		));
	}

	[Fact]
	public async Task Handle_WhenTaskHasNoDownstreams_ReturnsEmptyPagedList()
	{
		// Arrange
		IOutputPort outputPort = Substitute.For<IOutputPort>();
		IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
		IRepository<TaskAggregate> repository = Substitute.For<IRepository<TaskAggregate>>();

		Guid taskId = Guid.NewGuid();
		TaskAggregate testTask = new("Test Task", id: taskId);
		PagingParams pagingParams = new() { PageNumber = 1, PageSize = 10 };
		InputDto input = new() { Id = taskId, PagingParams = pagingParams };

		unitOfWork.Repository<TaskAggregate>().Returns(repository);
		repository.FindByIdAsync(taskId).Returns(testTask);
		repository.FindAsync(Arg.Any<TaskIncludeDownstreamsSpecification>()).Returns(new PagedList<TaskAggregate>([ testTask ], 1, 1, 10));

		UseCase useCase = new(outputPort, unitOfWork);

		// Act
		await useCase.Handle(input);

		// Assert
		await repository.Received(1).FindAsync(Arg.Any<TaskIncludeDownstreamsSpecification>());
		await outputPort.Received(1).Handle(Arg.Is<OutputDto>(dto => 
			dto.Downstreams.Count == 0 &&
			dto.Downstreams.TotalCount == 0
		));
	}

	[Fact]
	public async Task Handle_ValidatesPagingParameters()
	{
		// Arrange
		IOutputPort outputPort = Substitute.For<IOutputPort>();
		IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
		IRepository<TaskAggregate> repository = Substitute.For<IRepository<TaskAggregate>>();

		Guid taskId = Guid.NewGuid();
		TaskAggregate testTask = new("Test Task", id: taskId);
		PagingParams pagingParams = new() { PageNumber = 2, PageSize = 5 };
		InputDto input = new() { Id = taskId, PagingParams = pagingParams };

		unitOfWork.Repository<TaskAggregate>().Returns(repository);
		repository.FindByIdAsync(taskId).Returns(testTask);
		repository.FindAsync(Arg.Any<TaskIncludeDownstreamsSpecification>()).Returns(new PagedList<TaskAggregate>([ testTask ], 1, 2, 5));

		UseCase useCase = new(outputPort, unitOfWork);

		// Act
		await useCase.Handle(input);

		// Assert
		await repository.Received(1).FindAsync(Arg.Is<TaskIncludeDownstreamsSpecification>(spec =>
			spec.PageNumber == pagingParams.PageNumber &&
			spec.PageSize == pagingParams.PageSize
		));
	}
}
