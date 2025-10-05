using GraphT.Model.Services.Repositories;
using GraphT.UseCases.DeleteTask;
using GraphT.Model.Exceptions;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace GraphT.UseCases.Tests.DeleteTask;

public class UseCaseTests
{
	private readonly ITodoTaskRepository _repository;
	private readonly IOutputPort _outputPort;
	private readonly UseCase _useCase;
	private readonly Guid _taskId;

	public UseCaseTests()
	{
		_repository = Substitute.For<ITodoTaskRepository>();
		_outputPort = Substitute.For<IOutputPort>();
		_useCase = new(_outputPort, _repository);
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
		await _useCase.Handle(inputDto);

		// Assert
		await _repository.Received(1).RemoveAsync(_taskId);
	}

	[Fact]
	public async Task Handle_ShouldCallOutputPort()
	{
		// Arrange
		InputDto inputDto = new()
		{
			Id = _taskId
		};

		// Act
		await _useCase.Handle(inputDto);

		// Assert
		await _outputPort.Received(1).Handle();
	}
	
	[Fact]
	public async Task Handle_ShouldThrowExternalRepositoryException_WhenRepositoryThrows()
	{
		// Arrange
		InputDto inputDto = new()
		{
			Id = _taskId
		};
		Exception exception = new("Repository error");

		// Act & Assert
		_repository.RemoveAsync(_taskId).Throws(exception);
		ExternalRepositoryException thrownException = await Assert.ThrowsAsync<ExternalRepositoryException>(
			async () => await _useCase.Handle(inputDto)
		);
		
		Assert.Equal("Error removing task from repository", thrownException.Message);
		Assert.Same(exception, thrownException.InnerException);
	}
}
