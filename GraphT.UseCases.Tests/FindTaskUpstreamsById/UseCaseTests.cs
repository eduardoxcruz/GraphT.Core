using GraphT.Model.Entities;
using GraphT.Model.Exceptions;
using GraphT.Model.Services.Repositories;
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
		ITodoTaskRepository todoTaskRepository = Substitute.For<ITodoTaskRepository>();
		ITaskUpstreamsRepository taskUpstreamsRepository = Substitute.For<ITaskUpstreamsRepository>();

		Guid taskId = Guid.NewGuid();
		OldTodoTask taskUpstream1 = new("Test Upstream 1");
		OldTodoTask taskUpstream2 = new("Test Upstream 2");
		var upstreams = new PagedList<OldTodoTask>([taskUpstream1, taskUpstream2], 2, 1, 10);
		
		PagingParams pagingParams = new() { PageNumber = 1, PageSize = 10 };
		InputDto input = new() { Id = taskId, PagingParams = pagingParams };

		todoTaskRepository.ContainsAsync(taskId).Returns(true);
		taskUpstreamsRepository.FindTaskUpstreamsById(taskId).Returns(upstreams);

		UseCase useCase = new(outputPort, todoTaskRepository, taskUpstreamsRepository);

		// Act
		await useCase.Handle(input);

		// Assert
		await todoTaskRepository.Received(1).ContainsAsync(taskId);
		await taskUpstreamsRepository.Received(1).FindTaskUpstreamsById(taskId);
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
		ITodoTaskRepository todoTaskRepository = Substitute.For<ITodoTaskRepository>();
		ITaskUpstreamsRepository taskUpstreamsRepository = Substitute.For<ITaskUpstreamsRepository>();

		Guid taskId = Guid.NewGuid();
		var emptyUpstreams = new PagedList<OldTodoTask>([], 0, 1, 10);
		
		PagingParams pagingParams = new() { PageNumber = 1, PageSize = 10 };
		InputDto input = new() { Id = taskId, PagingParams = pagingParams };

		todoTaskRepository.ContainsAsync(taskId).Returns(true);
		taskUpstreamsRepository.FindTaskUpstreamsById(taskId).Returns(emptyUpstreams);

		UseCase useCase = new(outputPort, todoTaskRepository, taskUpstreamsRepository);

		// Act
		await useCase.Handle(input);

		// Assert
		await todoTaskRepository.Received(1).ContainsAsync(taskId);
		await taskUpstreamsRepository.Received(1).FindTaskUpstreamsById(taskId);
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
		ITodoTaskRepository todoTaskRepository = Substitute.For<ITodoTaskRepository>();
		ITaskUpstreamsRepository taskUpstreamsRepository = Substitute.For<ITaskUpstreamsRepository>();

		Guid taskId = Guid.NewGuid();
		PagingParams pagingParams = new() { PageNumber = 2, PageSize = 5 };
		InputDto input = new() { Id = taskId, PagingParams = pagingParams };

		// Create a paged list with the expected paging parameters
		var upstreams = new PagedList<OldTodoTask>([], 0, 2, 5);
		
		todoTaskRepository.ContainsAsync(taskId).Returns(true);
		taskUpstreamsRepository.FindTaskUpstreamsById(taskId).Returns(upstreams);

		UseCase useCase = new(outputPort, todoTaskRepository, taskUpstreamsRepository);

		// Act
		await useCase.Handle(input);

		// Assert
		await todoTaskRepository.Received(1).ContainsAsync(taskId);
		await taskUpstreamsRepository.Received(1).FindTaskUpstreamsById(taskId);
		await outputPort.Received(1).Handle(Arg.Is<OutputDto>(dto => 
			dto.Upstreams.PageSize == pagingParams.PageSize &&
			dto.Upstreams.CurrentPage == pagingParams.PageNumber
		));
	}
}
