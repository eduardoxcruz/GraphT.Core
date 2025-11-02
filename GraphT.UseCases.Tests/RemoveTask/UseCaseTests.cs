using GraphT.Model.Services.Repositories;
using GraphT.Model.Exceptions;
using GraphT.UseCases.RemoveTask;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace GraphT.UseCases.Tests.DeleteTask;

public class UseCaseTests
{
	private readonly IRemoveTaskPort _removeTaskPort;
	private readonly UseCase _useCase;
	private readonly Guid _taskId;

	public UseCaseTests()
	{
		_removeTaskPort = Substitute.For<IRemoveTaskPort>();
		_useCase = new UseCase(_removeTaskPort);
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
		await _useCase.HandleAsync(inputDto);

		// Assert
		await _removeTaskPort.Received(1).HandleAsync(_taskId);
	}
}
