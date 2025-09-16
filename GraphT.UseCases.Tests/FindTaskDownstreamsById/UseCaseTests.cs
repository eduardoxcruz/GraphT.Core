using GraphT.Model.Entities;
using GraphT.Model.Services.Repositories;
using GraphT.Model.ValueObjects;
using GraphT.UseCases.FindTaskDownstreamsById;

using NSubstitute;
using NSubstitute.ReceivedExtensions;

using SeedWork;

namespace GraphT.UseCases.Tests.FindTaskDownstreamsById;

public class UseCaseTests
{
	[Fact]
	public async Task Handle_WhenTaskHasDownstreams_ReturnsPagedListOfDownstreams()
	{
		// Arrange
		IOutputPort outputPort = Substitute.For<IOutputPort>();
		ITodoTaskRepository todoTaskRepository = Substitute.For<ITodoTaskRepository>();
		ITaskDownstreamsRepository taskDownstreamsRepository = Substitute.For<ITaskDownstreamsRepository>();

		Guid taskId = Guid.NewGuid();
		OldTodoTask testTask = new("Test Task", id: taskId);
		OldTodoTask taskDownstream1 = new("Test Downstream 1");
		OldTodoTask taskDownstream2 = new("Test Downstream 2");
		PagedList<OldTodoTask> downstreams = new(
			[ taskDownstream1, taskDownstream2 ], 
			2, 
			1, 
			10
		);
		
		PagingParams pagingParams = new() { PageNumber = 1, PageSize = 10 };
		InputDto input = new() { Id = taskId, PagingParams = pagingParams };

		todoTaskRepository.ContainsAsync(taskId).Returns(true);
		taskDownstreamsRepository.FindTaskDownstreamsById(taskId).Returns(downstreams);

		UseCase useCase = new(outputPort, todoTaskRepository, taskDownstreamsRepository);

		// Act
		await useCase.Handle(input);

		// Assert
		await todoTaskRepository.Received(1).ContainsAsync(taskId);
		await taskDownstreamsRepository.Received(1).FindTaskDownstreamsById(taskId);
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
		ITodoTaskRepository todoTaskRepository = Substitute.For<ITodoTaskRepository>();
		ITaskDownstreamsRepository taskDownstreamsRepository = Substitute.For<ITaskDownstreamsRepository>();

		Guid taskId = Guid.NewGuid();
		PagedList<OldTodoTask> emptyDownstreams = new(
			[], 
			0, 
			0, 
			10
		);
		
		PagingParams pagingParams = new() { PageNumber = 1, PageSize = 10 };
		InputDto input = new() { Id = taskId, PagingParams = pagingParams };

		todoTaskRepository.ContainsAsync(taskId).Returns(true);
		taskDownstreamsRepository.FindTaskDownstreamsById(taskId).Returns(emptyDownstreams);

		UseCase useCase = new(outputPort, todoTaskRepository, taskDownstreamsRepository);

		// Act
		await useCase.Handle(input);

		// Assert
		await todoTaskRepository.Received(1).ContainsAsync(taskId);
		await taskDownstreamsRepository.Received(1).FindTaskDownstreamsById(taskId);
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
		ITodoTaskRepository todoTaskRepository = Substitute.For<ITodoTaskRepository>();
		ITaskDownstreamsRepository taskDownstreamsRepository = Substitute.For<ITaskDownstreamsRepository>();

		Guid taskId = Guid.NewGuid();
		PagingParams pagingParams = new() { PageNumber = 2, PageSize = 5 };
		InputDto input = new() { Id = taskId, PagingParams = pagingParams };

		PagedList<OldTodoTask> downstreams = new(
			[], 
			2, 
			2, 
			5
		);

		todoTaskRepository.ContainsAsync(taskId).Returns(true);
		taskDownstreamsRepository.FindTaskDownstreamsById(taskId).Returns(downstreams);

		UseCase useCase = new(outputPort, todoTaskRepository, taskDownstreamsRepository);

		// Act
		await useCase.Handle(input);

		// Assert
		await todoTaskRepository.Received(1).ContainsAsync(taskId);
		await taskDownstreamsRepository.Received(1).FindTaskDownstreamsById(taskId);
		await outputPort.Received(1).Handle(Arg.Is<OutputDto>(dto => 
			dto.Downstreams.PageSize == pagingParams.PageSize &&
			dto.Downstreams.CurrentPage == pagingParams.PageNumber
		));
	}
}
