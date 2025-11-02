using GraphT.Model.Services.Repositories;
using GraphT.UseCases.FindTaskById;

using NSubstitute;

namespace GraphT.UseCases.Tests.FindTaskById;

public class UseCaseTests
{
	private readonly UseCase _useCase;
	private readonly IFindTaskByIdPort _findTaskByIdPort;

	public UseCaseTests()
	{
		_findTaskByIdPort = Substitute.For<IFindTaskByIdPort>();
		_useCase = new UseCase(_findTaskByIdPort);
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
		await _useCase.HandleAsync(inputDto);

		// Assert
		await _findTaskByIdPort.Received(1).HandleAsync(inputDto.Id);
	}
}

