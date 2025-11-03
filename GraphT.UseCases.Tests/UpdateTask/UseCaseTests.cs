using GraphT.Model.Aggregates;
using GraphT.Model.Services.Repositories;
using GraphT.Model.ValueObjects;
using GraphT.UseCases.UpdateTask;

using NSubstitute;

using SeedWork;

namespace GraphT.UseCases.Tests.UpdateTask;

public class UseCaseTests
{
	private readonly UseCase _useCase;
	private readonly IFullPort<UseCases.FindTaskById.InputDto, UseCases.FindTaskById.OutputDto> _findTaskByIdPort;
	private readonly IUpdateTaskPort _updateTaskPort;

	public UseCaseTests()
	{
		_findTaskByIdPort = Substitute.For<IFullPort<UseCases.FindTaskById.InputDto, UseCases.FindTaskById.OutputDto>>();
		_updateTaskPort = Substitute.For<IUpdateTaskPort>();
		_useCase = new UseCase(_findTaskByIdPort, _updateTaskPort);
	}
	
	[Fact]
	public async Task Handle_ShouldCallFindById_Port()
	{
		// Arrange
		Guid taskId = Guid.NewGuid();
		TodoTask task = new();
		UseCases.FindTaskById.OutputDto findTaskByIdOutputDto = new() { Task = task };
		UseCases.FindTaskById.InputDto findTaskByIdInputDto = new() { Id = taskId };
		InputDto inputDto = new() { Id = taskId, Name = "Test Task" };

		// Act
		_findTaskByIdPort.HandleAsync(Arg.Any<UseCases.FindTaskById.InputDto>()).Returns(findTaskByIdOutputDto);
		await _useCase.HandleAsync(inputDto);

		// Assert
		await _findTaskByIdPort.Received(1).HandleAsync(findTaskByIdInputDto);
	}

	[Fact]
	public async Task Handle_ShouldThrowArgumentNullException_IfAllOptionalPropertiesAreNull()
	{
		// Arrange
		Guid taskId = Guid.NewGuid();
		InputDto inputDto = new() { Id = taskId };

		// Act
		ArgumentNullException exception = 
			await Assert.ThrowsAsync<ArgumentNullException>(async () => await _useCase.HandleAsync(inputDto));

		// Assert
		Assert.Contains("All optional properties are null. At least one property must be set.", exception.Message);
	}
	
	[Fact]
    public async Task Handle_ShouldAssignDtoPropertiesToTask_IfDtoPropertyIsNotNull()
    {
        Guid taskId = Guid.NewGuid();
        string newName = "New Name";
        bool newIsFun = true;
        Complexity newComplexity = Complexity.High;
        TodoTask existingTask = new() { Name = "Original Name" };
        UseCases.FindTaskById.OutputDto findOutput = new() { Task = existingTask };
        TodoTask addedParent = new();
        TodoTask removedParent = new();
        TodoTask addedChild = new();
        TodoTask removedChild = new();
        LifeArea addedLifeArea = new("Added");
        LifeArea removedLifeArea = new("Removed");
        
        existingTask.AddParents([ removedParent ]);
        existingTask.AddChildren([ removedChild ]);
        existingTask.AddLifeAreas([ removedLifeArea ]);
        
        InputDto input = new()
        {
	        Id = taskId,
	        Name = newName,
	        IsFun = newIsFun,
	        Complexity = newComplexity,
	        ParentsToAdd = [ addedParent ],
	        ChildrenToAdd = [ addedChild ],
	        LifeAreasToAdd = [ addedLifeArea ],
	        ParentsToRemove = [ removedParent ],
	        ChildrenToRemove = [ removedChild ],
	        LifeAreasToRemove = [ removedLifeArea]
        };

        _findTaskByIdPort.HandleAsync(Arg.Any<UseCases.FindTaskById.InputDto>()).Returns(findOutput);
        OutputDto result = await _useCase.HandleAsync(input);
        
        Assert.Equal(newName, result.Task.Name);
        Assert.Equal(newIsFun, result.Task.IsFun);
        Assert.Equal(newComplexity, result.Task.Complexity);
        Assert.Contains(addedParent, result.Task.Parents);
        Assert.DoesNotContain(removedParent, result.Task.Parents);
        Assert.Contains(addedChild, result.Task.Children);
        Assert.DoesNotContain(removedChild, result.Task.Children);
        Assert.Contains(addedLifeArea, result.Task.LifeAreas);
        Assert.DoesNotContain(removedLifeArea, result.Task.LifeAreas);
    }
    
    [Fact]
    public async Task Handle_ShouldCallUpdateTaskPort_WithUpdatedTask()
    {
	    Guid taskId = Guid.NewGuid();
	    string newName = "New Name";
	    TodoTask existingTask = new() { Name = "Original Name" };
	    UseCases.FindTaskById.OutputDto findOutput = new() { Task = existingTask };
        
	    InputDto input = new()
	    {
		    Id = taskId,
		    Name = newName,
	    };

	    _findTaskByIdPort.HandleAsync(Arg.Any<UseCases.FindTaskById.InputDto>()).Returns(findOutput);
	    await _useCase.HandleAsync(input);
	    await _updateTaskPort.Received(1).HandleAsync(Arg.Any<TodoTask>());
    }
}

