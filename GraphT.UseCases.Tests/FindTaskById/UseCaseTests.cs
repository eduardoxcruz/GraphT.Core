using GraphT.Model.Exceptions;
using GraphT.Model.Services.Repositories;
using GraphT.UseCases.FindTaskById;

using NSubstitute;

namespace GraphT.UseCases.Tests.FindTaskById;

public class UseCaseTests
{
	private readonly UseCase _useCase;
	private readonly IFindTaskByIdPort _findTaskByIdPort;
	private readonly IContainsTaskPort _containsTaskPort;

	public UseCaseTests()
	{
		_findTaskByIdPort = Substitute.For<IFindTaskByIdPort>();
		_containsTaskPort = Substitute.For<IContainsTaskPort>();
		_useCase = new UseCase(_findTaskByIdPort, _containsTaskPort);
	}

	[Fact]
	public async Task Handle_ShouldCallFindById_Port()
	{
		// Arrange
		Guid taskId = Guid.NewGuid();
		InputDto inputDto = new()
		{
			Id = taskId
		};

		// Act
		_containsTaskPort.HandleAsync(Arg.Any<Guid>()).Returns(true);
		await _useCase.HandleAsync(inputDto);

		// Assert
		await _findTaskByIdPort.Received(1).HandleAsync(inputDto.Id);
	}
	
	[Fact]
	public async Task Handle_ShouldCallContainsTask_Port()
	{
		// Arrange
		Guid taskId = Guid.NewGuid();
		InputDto inputDto = new()
		{
			Id = taskId
		};

		// Act
		_containsTaskPort.HandleAsync(Arg.Any<Guid>()).Returns(true);
		await _useCase.HandleAsync(inputDto);

		// Assert
		await _containsTaskPort.Received(1).HandleAsync(inputDto.Id);
	}
	
	[Fact]
	public async Task Handle_ShouldThrowTaskNotFoundException_WhenContainsTaskPortReturnsFalse()
	{
		// Arrange
		Guid taskId = Guid.NewGuid();
		InputDto inputDto = new()
		{
			Id = taskId
		};

		// Act
		_containsTaskPort.HandleAsync(Arg.Any<Guid>()).Returns(false);
		
		TaskNotFoundException exception = 
			await Assert.ThrowsAsync<TaskNotFoundException>(async () => await _useCase.HandleAsync(inputDto));

		// Assert
		Assert.Equal(taskId, exception.Id);
		Assert.Equal("Task not found.", exception.Message);
	}
}

