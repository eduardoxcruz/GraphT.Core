using GraphT.Model.Entities;
using GraphT.Model.Services.Repositories;
using GraphT.Model.ValueObjects;
using GraphT.UseCases.AddNewTask;

using NSubstitute;

using SeedWork;

namespace GraphT.UseCases.Tests.AddNewTask;

public class UseCaseTests
{
	[Fact]
    public async Task Handle_WhenIdNotProvided_ShouldCreateNewTask()
    {
        // Arrange
        IOutputPort outputPort = Substitute.For<IOutputPort>();
        ITodoTaskRepository todoTaskRepository = Substitute.For<ITodoTaskRepository>();
        ITaskLogRepository taskLogRepository = Substitute.For<ITaskLogRepository>();
        IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        
        InputDto input = new()
        {
            Name = "Test Task",
            Status = Status.Created,
            IsFun = true,
            IsProductive = true,
            Complexity = OldComplexity.High,
            Priority = OldPriority.Urgent
        };

        UseCase useCase = new(outputPort, todoTaskRepository, taskLogRepository, unitOfWork);

        // Act
        await useCase.Handle(input);

        // Assert
        await todoTaskRepository.Received(1).AddAsync(Arg.Is<TodoTask>(t => 
            t.Name == input.Name && 
            t.IsFun == input.IsFun && 
            t.IsProductive == input.IsProductive && 
            t.Complexity == input.Complexity && 
            t.Priority == input.Priority
        ));
        await taskLogRepository.Received(2).AddAsync(Arg.Any<TaskLog>());
        await unitOfWork.Received(1).SaveChangesAsync();
        await outputPort.Received(1).Handle(Arg.Is<OutputDto>(o => o.Id != Guid.Empty));
    }

    [Fact]
    public async Task Handle_WhenIdProvided_AndTaskNotExists_ShouldUseProvidedId()
    {
        // Arrange
        IOutputPort outputPort = Substitute.For<IOutputPort>();
        ITodoTaskRepository todoTaskRepository = Substitute.For<ITodoTaskRepository>();
        ITaskLogRepository taskLogRepository = Substitute.For<ITaskLogRepository>();
        IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();

        Guid providedId = Guid.NewGuid();
        
        todoTaskRepository.ContainsAsync(providedId).Returns(false);
        
        InputDto input = new()
        {
            Id = providedId,
            Name = "Test Task"
        };

        UseCase useCase = new(outputPort, todoTaskRepository, taskLogRepository, unitOfWork);

        // Act
        await useCase.Handle(input);

        // Assert
        await todoTaskRepository.Received(1).AddAsync(Arg.Is<TodoTask>(t => t.Id == input.Id && t.Name == input.Name));
        await taskLogRepository.Received(1).AddAsync(Arg.Any<TaskLog>());
        await unitOfWork.Received(1).SaveChangesAsync();
        await outputPort.Received(1).Handle(Arg.Is<OutputDto>(o => o.Id == input.Id));
    }

    [Fact]
    public async Task Handle_WhenDateTimesProvided_ShouldSetAllDates()
    {
        // Arrange
        IOutputPort outputPort = Substitute.For<IOutputPort>();
        ITodoTaskRepository todoTaskRepository = Substitute.For<ITodoTaskRepository>();
        ITaskLogRepository taskLogRepository = Substitute.For<ITaskLogRepository>();
        IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();

        DateTimeOffset startDate = DateTimeOffset.UtcNow;
        DateTimeOffset finishDate = startDate.AddDays(1);
        DateTimeOffset limitDate = startDate.AddDays(2);

        InputDto input = new()
        {
            Name = "Test Task",
            StartDateTime = startDate,
            FinishDateTime = finishDate,
            LimitDateTime = limitDate
        };

        UseCase useCase = new(outputPort, todoTaskRepository, taskLogRepository, unitOfWork);

        // Act
        await useCase.Handle(input);

        // Assert
        await todoTaskRepository.Received(1).AddAsync(Arg.Is<TodoTask>(t => 
            t.Name == input.Name &&
            t.DateTimeInfo.StartDateTime == input.StartDateTime &&
            t.DateTimeInfo.FinishDateTime == input.FinishDateTime &&
            t.DateTimeInfo.LimitDateTime == input.LimitDateTime
        ));
        await taskLogRepository.Received(1).AddAsync(Arg.Any<TaskLog>());
        await unitOfWork.Received(1).SaveChangesAsync();
        await outputPort.Received(1).Handle(Arg.Is<OutputDto>(o => o.Id != Guid.Empty));
    }

    [Fact]
    public async Task Handle_WhenNameNotProvided_ShouldUseDefaultName()
    {
        // Arrange
        IOutputPort outputPort = Substitute.For<IOutputPort>();
        ITodoTaskRepository todoTaskRepository = Substitute.For<ITodoTaskRepository>();
        ITaskLogRepository taskLogRepository = Substitute.For<ITaskLogRepository>();
        IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();

        InputDto input = new();

        UseCase useCase = new(outputPort, todoTaskRepository, taskLogRepository, unitOfWork);

        // Act
        await useCase.Handle(input);

        // Assert
        await todoTaskRepository.Received(1).AddAsync(Arg.Is<TodoTask>(t => 
            t.Name == "New Task"
        ));
        await unitOfWork.Received(1).SaveChangesAsync();
    }
}
