using GraphT.Model.Entities;
using GraphT.Model.Exceptions;
using GraphT.Model.Services.Repositories;
using GraphT.UseCases.DeleteTask;

using NSubstitute;

using SeedWork;

namespace GraphT.UseCases.Tests.DeleteTask;

public class UseCaseTests
{
    [Fact]
    public async Task Handle_WhenTaskExists_DeletesTaskSuccessfully()
    {
        // Arrange
        IOutputPort outputPort = Substitute.For<IOutputPort>();
        ITodoTaskRepository todoTaskRepository = Substitute.For<ITodoTaskRepository>();
        IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        
        Guid taskId = Guid.NewGuid();
        TodoTask existingTask = new("Test Task", id: taskId);
        InputDto input = new() { Id = taskId };

        todoTaskRepository.FindByIdAsync(taskId).Returns(existingTask);

        UseCase useCase = new(outputPort, todoTaskRepository, unitOfWork);

        // Act
        await useCase.Handle(input);

        // Assert
        await todoTaskRepository.Received(1).RemoveAsync(existingTask);
        await unitOfWork.Received(1).SaveChangesAsync();
        await outputPort.Received(1).Handle();
    }

    [Fact]
    public async Task Handle_WhenTaskDoesNotExist_ThrowsTaskNotFoundException()
    {
        // Arrange
        IOutputPort outputPort = Substitute.For<IOutputPort>();
        ITodoTaskRepository todoTaskRepository = Substitute.For<ITodoTaskRepository>();
        IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        
        Guid taskId = Guid.NewGuid();
        InputDto input = new() { Id = taskId };

        todoTaskRepository.ContainsAsync(taskId).Returns(false);

        UseCase useCase = new(outputPort, todoTaskRepository, unitOfWork);

        // Act & Assert
        TaskNotFoundException exception = await Assert.ThrowsAsync<TaskNotFoundException>(
            async () => await useCase.Handle(input)
        );
        
        Assert.Equal(taskId, exception.Id);
        await todoTaskRepository.DidNotReceive().RemoveAsync(Arg.Any<TodoTask>());
        await unitOfWork.DidNotReceive().SaveChangesAsync();
        await outputPort.DidNotReceive().Handle();
    }
}
