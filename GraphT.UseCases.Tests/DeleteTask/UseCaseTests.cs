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

	[Fact]
	public async Task Handle_ShouldThrowExternalRepositoryException_WhenRepositoryThrows()
	{
		// Arrange
		InputDto inputDto = new()
		{
			Id = _taskId
		};
		ExternalRepositoryException exception = new("Repository error");

		// Act & Assert
		_removeTaskPort.HandleAsync(Arg.Any<Guid>()).Throws(exception);
		
		ExternalRepositoryException thrownException = await Assert.ThrowsAsync<ExternalRepositoryException>(
			async () => await _useCase.HandleAsync(inputDto));
		
		Assert.Equal("Error removing task from repository.", thrownException.Message);
		Assert.Same(exception, thrownException.InnerException);
	}
}
