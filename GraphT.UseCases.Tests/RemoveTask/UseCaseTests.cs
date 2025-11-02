using GraphT.Model.Exceptions;
using GraphT.Model.Services.Repositories;
using GraphT.UseCases.RemoveTask;

using NSubstitute;

namespace GraphT.UseCases.Tests.RemoveTask;

public class UseCaseTests
{
	private readonly IRemoveTaskPort _removeTaskPort;
	private readonly IContainsTaskPort _containsTaskPort;
	private readonly UseCase _useCase;
	private readonly Guid _taskId;

	public UseCaseTests()
	{
		_removeTaskPort = Substitute.For<IRemoveTaskPort>();
		_containsTaskPort = Substitute.For<IContainsTaskPort>();
		_useCase = new UseCase(_removeTaskPort, _containsTaskPort);
		_taskId = Guid.NewGuid();
	}

	[Fact]
	public async Task Handle_ShouldCallRepository_RemoveAsync()
	{
		// Arrange
		InputDto inputDto = new()
		{
			Id = _taskId
		};

		// Act
		_containsTaskPort.HandleAsync(Arg.Any<Guid>()).Returns(true);
		await _useCase.HandleAsync(inputDto);

		// Assert
		await _removeTaskPort.Received(1).HandleAsync(_taskId);
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
