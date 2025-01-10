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
		List<TaskAggregate> upstreamTasks = new()
		{
			new TaskAggregate("Upstream 1"),
			new TaskAggregate("Upstream 2")
		};
		PagingParams pagingParams = new() { PageNumber = 1, PageSize = 10 };
		InputDto input = new() { Id = taskId, PagingParams = pagingParams };
		PagedList<TaskAggregate> pagedUpstreams = new(upstreamTasks, 2, 1, 10);

		unitOfWork.Repository<TaskAggregate>().Returns(repository);
		repository.FindAsync(Arg.Any<FindUpstreamsByTaskIdSpecification>()).Returns(pagedUpstreams);

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
			dto.Upstreams.Any(t => t.Id.Equals(upstreamTasks[0].Id)) &&
			dto.Upstreams.Any(t => t.Id.Equals(upstreamTasks[1].Id))
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
		PagingParams pagingParams = new() { PageNumber = 1, PageSize = 10 };
		InputDto input = new() { Id = taskId, PagingParams = pagingParams };
		PagedList<TaskAggregate> emptyPagedList = new(new List<TaskAggregate>(), 0, 1, 10);

		unitOfWork.Repository<TaskAggregate>().Returns(repository);
		repository.FindAsync(Arg.Any<FindUpstreamsByTaskIdSpecification>()).Returns(emptyPagedList);

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
		PagingParams pagingParams = new() { PageNumber = 2, PageSize = 5 };
		InputDto input = new() { Id = taskId, PagingParams = pagingParams };

		unitOfWork.Repository<TaskAggregate>().Returns(repository);
		repository.FindAsync(Arg.Any<FindUpstreamsByTaskIdSpecification>())
			.Returns(new PagedList<TaskAggregate>(new List<TaskAggregate>(), 0, 2, 5));

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
