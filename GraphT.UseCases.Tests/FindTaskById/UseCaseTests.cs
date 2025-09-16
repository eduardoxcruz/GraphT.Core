using GraphT.Model.Entities;
using GraphT.Model.Exceptions;
using GraphT.Model.Services.Repositories;
using GraphT.UseCases.FindTaskById;

using NSubstitute;

namespace GraphT.UseCases.Tests.FindTaskById;

public class UseCaseTests
{
    [Fact]
    public async Task Handle_WhenTaskExists_ShouldCallOutputPortWithTask()
    {
        // Arrange
        IOutputPort outputPort = Substitute.For<IOutputPort>();
        ITodoTaskRepository todoTaskRepository = Substitute.For<ITodoTaskRepository>();

        Guid taskId = Guid.NewGuid();
        OldTodoTask existingTask = new("Test task", id: taskId);
        InputDto input = new() { Id = taskId };

        todoTaskRepository.FindByIdAsync(taskId).Returns(existingTask);

        UseCase useCase = new(outputPort, todoTaskRepository);

        // Act
        await useCase.Handle(input);

        // Assert
        await todoTaskRepository.Received(1).FindByIdAsync(taskId);
        await outputPort.Received(1).Handle(Arg.Is<OutputDto>(dto => dto.Task.Id.Equals(taskId)));
    }

    [Fact]
    public async Task Handle_WhenTaskDoesNotExist_ShouldThrowTaskNotFoundException()
    {
        // Arrange
        IOutputPort outputPort = Substitute.For<IOutputPort>();
        ITodoTaskRepository todoTaskRepository = Substitute.For<ITodoTaskRepository>();
        
        Guid taskId = Guid.NewGuid();
        InputDto input = new() { Id = taskId };
        
        todoTaskRepository.FindByIdAsync(taskId).Returns((OldTodoTask)null!);
        
        UseCase useCase = new(outputPort, todoTaskRepository);

        // Act & Assert
        TaskNotFoundException exception = await Assert.ThrowsAsync<TaskNotFoundException>(
            async () => await useCase.Handle(input)
        );
        
        Assert.Equal(taskId, exception.Id);
    }
}
