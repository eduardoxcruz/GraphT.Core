using GraphT.Model.Services.Repositories;
using GraphT.UseCases.AddNewTask;
using GraphT.Model.Aggregates;
using GraphT.Model.Exceptions;
using GraphT.Model.ValueObjects;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace GraphT.UseCases.Tests.AddNewTask;

public class UseCaseTests
{
	private readonly ITodoTaskRepository _repository;
	private readonly IOutputPort _outputPort;
	private readonly UseCase _useCase;

	public UseCaseTests()
	{
		_repository = Substitute.For<ITodoTaskRepository>();
		_outputPort = Substitute.For<IOutputPort>();
		_useCase = new UseCase(_outputPort, _repository);
	}

	[Fact]
	public async Task Handle_ShouldCreateGuid_ForTaskNewTask()
	{
		// Arrange
		InputDto inputDto = new InputDto { Name = "Test Task" };
		TodoTask capturedTask = null;
		
		_repository.AddAsync(Arg.Do<TodoTask>(task => capturedTask = task))
			.Returns(ValueTask.CompletedTask);
		
		// Act
		await _useCase.Handle(inputDto);
		
		// Assert
		Assert.NotEqual(Guid.Empty, capturedTask.Id);
	}
	
	[Fact]
	public async Task Handle_ShouldCallRepository_AddAsync()
	{
		// Arrange
		InputDto inputDto = new InputDto { Name = "Test Task" };
		
		// Act
		await _useCase.Handle(inputDto);
		
		// Assert
		await _repository.Received(1).AddAsync(Arg.Any<TodoTask>());
	}
	
	[Fact]
	public async Task Handle_ShouldAssignDtoPropertiesToNewTask()
	{
		// Arrange
		string expectedName = "Test Task";
		InputDto inputDto = new InputDto { Name = expectedName };
		TodoTask capturedTask = null;
		
		_repository.AddAsync(Arg.Do<TodoTask>(task => capturedTask = task))
			.Returns(ValueTask.CompletedTask);
		
		// Act
		await _useCase.Handle(inputDto);
		
		// Assert
		Assert.Equal(expectedName, capturedTask.Name);
	}
	
	[Fact]
	public async Task Handle_ShouldThrowExternalRepositoryException_WhenRepositoryThrows()
	{
		// Arrange
		InputDto inputDto = new InputDto { Name = "Test Task" };
		_repository.AddAsync(Arg.Any<TodoTask>())
			.Throws(new Exception("Database error"));
		
		// Act & Assert
		await Assert.ThrowsAsync<ExternalRepositoryException>(async () => await _useCase.Handle(inputDto));
	}

	[Fact]
	public async Task Handle_ShouldAssignDtoPropertiesToNewTask_IfDtoPropertyIsNotNull()
	{
		// Arrange
		InputDto inputDto = new InputDto 
		{ 
			Name = "Test Task",
			IsFun = true,
			IsProductive = true,
			Complexity = Complexity.High,
			Priority = Priority.Critical,
			Status = Status.Backlog,
			LimitDateTime = DateTimeOffset.Now.AddDays(1)
		};
		
		TodoTask? capturedTask = null;
		
		_repository.AddAsync(Arg.Do<TodoTask>(task => capturedTask = task))
			.Returns(ValueTask.CompletedTask);
		
		// Act
		await _useCase.Handle(inputDto);
		
		// Assert
		Assert.Equal(inputDto.Name, capturedTask.Name);
		Assert.Equal(inputDto.IsFun.Value, capturedTask.IsFun);
		Assert.Equal(inputDto.IsProductive.Value, capturedTask.IsProductive);
		Assert.Equal(inputDto.Complexity.Value, capturedTask.Complexity);
		Assert.Equal(inputDto.Priority.Value, capturedTask.Priority);
		Assert.Equal(inputDto.Status.Value, capturedTask.Status);
		Assert.Equal(inputDto.LimitDateTime.Value, capturedTask.LimitDateTime);
	}
}
