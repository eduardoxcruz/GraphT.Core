using GraphT.Model.Aggregates;
using GraphT.Model.Exceptions;
using GraphT.UseCases.FindTaskById;

using NSubstitute;

using SeedWork;

namespace GraphT.UseCases.Tests.FindTaskById;

public class UseCaseTests
{
	[Fact]
    public async Task Handle_WhenTaskExists_ShouldCallOutputPortWithTask()
    {
        // Arrange
        IOutputPort outputPort = Substitute.For<IOutputPort>();
        IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        IRepository<TaskAggregate> repository = Substitute.For<IRepository<TaskAggregate>>();

        Guid taskId = Guid.NewGuid();
        TaskAggregate existingTask = new("Test task", id: taskId);
        InputDto input = new() { Id = taskId };

        repository.FindByIdAsync(taskId).Returns(existingTask);
        unitOfWork.Repository<TaskAggregate>().Returns(repository);

        UseCase useCase = new(outputPort, unitOfWork);

        // Act
        await useCase.Handle(input);

        // Assert
        unitOfWork.Received(1).Repository<TaskAggregate>();
        await repository.Received(1).FindByIdAsync(taskId);
        await outputPort.Received(1).Handle(Arg.Is<OutputDto>(dto => dto.Task.Id.Equals(taskId)));
    }

    [Fact]
    public async Task Handle_WhenTaskDoesNotExist_ShouldThrowTaskNotFoundException()
    {
        // Arrange
        IOutputPort outputPort = Substitute.For<IOutputPort>();
        IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        IRepository<TaskAggregate> repository = Substitute.For<IRepository<TaskAggregate>>();
        
        Guid taskId = Guid.NewGuid();
        InputDto input = new() { Id = taskId };
        
        unitOfWork.Repository<TaskAggregate>().Returns(repository);
        repository.FindByIdAsync(taskId).Returns((TaskAggregate)null!);
        
        UseCase useCase = new(outputPort, unitOfWork);

        // Act & Assert
        TaskNotFoundException exception = await Assert.ThrowsAsync<TaskNotFoundException>(
            async () => await useCase.Handle(input)
        );
        
        Assert.Equal(taskId, exception.Id);
    }
}
