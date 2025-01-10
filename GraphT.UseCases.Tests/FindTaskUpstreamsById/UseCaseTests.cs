using GraphT.Model.Aggregates;
using GraphT.Model.Services.Specifications;
using GraphT.UseCases.FindTaskUpstreamsById;

using NSubstitute;

using SeedWork;

namespace GraphT.UseCases.Tests.FindTaskUpstreamsById;

public class UseCaseTests
{
	[Fact]
	public async Task Handle_WhenTaskHasUpstreams_ReturnsPagedListOfUpstreams()
	{
		// Arrange
		IOutputPort outputPort = Substitute.For<IOutputPort>();
		IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
		IRepository<TaskAggregate> repository = Substitute.For<IRepository<TaskAggregate>>();

		Guid taskId = Guid.NewGuid();
		TaskAggregate testTask = new("Test Task", id: taskId);
		TaskAggregate taskUpstream1 = new("Test Upstream 1");
		TaskAggregate taskUpstream2 = new("Test Upstream 2");
		testTask.AddUpstreams([taskUpstream1, taskUpstream2]);
		PagingParams pagingParams = new() { PageNumber = 1, PageSize = 10 };
		InputDto input = new() { Id = taskId, PagingParams = pagingParams };

		unitOfWork.Repository<TaskAggregate>().Returns(repository);
		repository.FindByIdAsync(taskId).Returns(testTask);
		repository.FindAsync(Arg.Any<FindUpstreamsByTaskIdSpecification>()).Returns(new PagedList<TaskAggregate>([ testTask ], 1, 1, 10));

		UseCase useCase = new(outputPort, unitOfWork);

		// Act
		await useCase.Handle(input);

		// Assert
		await repository.Received(1).FindAsync(Arg.Any<FindUpstreamsByTaskIdSpecification>());
		await outputPort.Received(1).Handle(Arg.Is<OutputDto>(dto => 
			dto.Upstreams.Count == 2 &&
			dto.Upstreams.TotalCount == 2 &&
			dto.Upstreams.CurrentPage == 1 &&
			dto.Upstreams.PageSize == 10 &&
			dto.Upstreams.Any(t => t.Id.Equals(taskUpstream1.Id)) &&
			dto.Upstreams.Any(t => t.Id.Equals(taskUpstream2.Id))
		));
	}

	[Fact]
	public async Task Handle_WhenTaskHasNoUpstreams_ReturnsEmptyPagedList()
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
		repository.FindAsync(Arg.Any<FindUpstreamsByTaskIdSpecification>()).Returns(new PagedList<TaskAggregate>([ testTask ], 1, 1, 10));

		UseCase useCase = new(outputPort, unitOfWork);

		// Act
		await useCase.Handle(input);

		// Assert
		await repository.Received(1).FindAsync(Arg.Any<FindUpstreamsByTaskIdSpecification>());
		await outputPort.Received(1).Handle(Arg.Is<OutputDto>(dto => 
			dto.Upstreams.Count == 0 &&
			dto.Upstreams.TotalCount == 0
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
		repository.FindAsync(Arg.Any<FindUpstreamsByTaskIdSpecification>()).Returns(new PagedList<TaskAggregate>([ testTask ], 1, 2, 5));

		UseCase useCase = new(outputPort, unitOfWork);

		// Act
		await useCase.Handle(input);

		// Assert
		await repository.Received(1).FindAsync(Arg.Is<FindUpstreamsByTaskIdSpecification>(spec =>
			spec.PageNumber == pagingParams.PageNumber &&
			spec.PageSize == pagingParams.PageSize
		));
	}
}
