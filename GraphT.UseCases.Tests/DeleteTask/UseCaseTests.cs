using System.Linq.Expressions;

using GraphT.Model.Aggregates;
using GraphT.Model.Entities;
using GraphT.Model.Exceptions;
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
        IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        IRepository<TodoTask> repository = Substitute.For<IRepository<TodoTask>>();
        
        Guid taskId = Guid.NewGuid();
        TodoTask existingTask = new("Test Task", id: taskId);
        InputDto input = new() { Id = taskId };

        repository.FindByIdAsync(taskId).Returns(existingTask);
        unitOfWork.Repository<TodoTask>().Returns(repository);

        UseCase useCase = new(outputPort, unitOfWork);

        // Act
        await useCase.Handle(input);

        await repository.Received(1).RemoveAsync(existingTask);
        await unitOfWork.Received(1).SaveChangesAsync();
        await outputPort.Received(1).Handle();
    }

    [Fact]
    public async Task Handle_WhenTaskDoesNotExist_ThrowsTaskNotFoundException()
    {
        // Arrange
        IOutputPort outputPort = Substitute.For<IOutputPort>();
        IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        IRepository<TodoTask> repository = Substitute.For<IRepository<TodoTask>>();
        
        Guid taskId = Guid.NewGuid();
        InputDto input = new() { Id = taskId };

        unitOfWork.Repository<TodoTask>().Returns(repository);
        repository.ContainsAsync(Arg.Any<Expression<Func<TodoTask, bool>>>()).Returns(false);

        UseCase useCase = new(outputPort, unitOfWork);

        // Act & Assert
        TaskNotFoundException exception = await Assert.ThrowsAsync<TaskNotFoundException>(
            async () => await useCase.Handle(input)
        );
        
        Assert.Equal(taskId, exception.Id);
        await repository.DidNotReceive().RemoveAsync(Arg.Any<TodoTask>());
        await unitOfWork.DidNotReceive().SaveChangesAsync();
        await outputPort.DidNotReceive().Handle();
    }
}
