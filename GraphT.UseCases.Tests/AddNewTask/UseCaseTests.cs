using GraphT.Model.Services.Repositories;
using GraphT.UseCases.AddNewTask;
using GraphT.Model.Aggregates;
using GraphT.Model.Exceptions;
using GraphT.Model.ValueObjects;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

using SeedWork;

namespace GraphT.UseCases.Tests.AddNewTask;

public class UseCaseTests
{
	private readonly IAddTaskPort _addTaskPort;
	private readonly UseCase _useCase;

	public UseCaseTests()
	{
		_addTaskPort = Substitute.For<IAddTaskPort>();
		_useCase = new UseCase(_addTaskPort);
	}

	[Fact]
	public async Task Handle_ShouldCreateGuid_ForNewTask()
	{
		// Arrange
		InputDto inputDto = new() { Name = "Test Task" };
		OutputDto outputDto;

		// Act
		outputDto = await _useCase.HandleAsync(inputDto);
		
		// Assert
		Assert.NotEqual(Guid.Empty, outputDto.Task.Id);
	}
	
	[Fact]
	public async Task Handle_ShouldCallRepository_AddAsync()
	{
		// Arrange
		InputDto inputDto = new() { Name = "Test Task" };
		
		// Act
		await _useCase.HandleAsync(inputDto);
		
		// Assert
		await _addTaskPort.Received(1).HandleAsync(Arg.Any<TodoTask>());
	}
	
	[Fact]
	public async Task Handle_ShouldAssignDtoPropertiesToNewTask()
	{
		// Arrange
		const string expectedName = "Test Task";
		InputDto inputDto = new() { Name = expectedName };
		
		// Act
		OutputDto outputDto = await _useCase.HandleAsync(inputDto);
		
		// Assert
		Assert.Equal(expectedName, outputDto.Task.Name);
	}
	
	[Fact]
	public async Task Handle_ShouldThrowExternalRepositoryException_WhenRepositoryThrows()
	{
		// Arrange
		InputDto inputDto = new InputDto { Name = "Test Task" };
		
		ExternalRepositoryException originalException = new("Original error");
        
		_addTaskPort.HandleAsync(Arg.Any<TodoTask>()).Throws(originalException);

		ExternalRepositoryException exception = await Assert.ThrowsAsync<ExternalRepositoryException>(
			async () => await _useCase.HandleAsync(inputDto));
        
		Assert.Same(originalException, exception);
	}

	[Fact]
	public async Task Handle_ShouldAssignDtoPropertiesToNewTask_IfDtoPropertyIsNotNull()
	{
		// Arrange
		InputDto inputDto = new()
		{ 
			Name = "Test Task",
			IsFun = true,
			IsProductive = true,
			Complexity = Complexity.High,
			Priority = Priority.Critical,
			Status = Status.Backlog,
			LimitDateTime = DateTimeOffset.Now.AddDays(1)
		};
		OutputDto outputDto;
		
		// Act
		outputDto = await _useCase.HandleAsync(inputDto);
		
		// Assert
		Assert.Equal(inputDto.Name, outputDto.Task.Name);
		Assert.Equal(inputDto.IsFun.Value, outputDto.Task.IsFun);
		Assert.Equal(inputDto.IsProductive.Value, outputDto.Task.IsProductive);
		Assert.Equal(inputDto.Complexity.Value, outputDto.Task.Complexity);
		Assert.Equal(inputDto.Priority.Value, outputDto.Task.Priority);
		Assert.Equal(inputDto.Status.Value, outputDto.Task.Status);
		Assert.Equal(inputDto.LimitDateTime.Value, outputDto.Task.LimitDateTime);
	}
}
