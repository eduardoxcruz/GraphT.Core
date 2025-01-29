using System.Linq.Expressions;

using GraphT.Model.Aggregates;
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
        IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        IRepository<TodoTask> taskRepository = Substitute.For<IRepository<TodoTask>>();
        IRepository<TaskLog> taskLogRepository = Substitute.For<IRepository<TaskLog>>();

        unitOfWork.Repository<TodoTask>().Returns(taskRepository);
        unitOfWork.Repository<TaskLog>().Returns(taskLogRepository);

        InputDto input = new()
        {
            Name = "Test Task",
            Status = Status.Created,
            IsFun = true,
            IsProductive = true,
            Complexity = Complexity.High,
            Priority = Priority.DoItNow
        };

        UseCase useCase = new(outputPort, unitOfWork);

        // Act
        await useCase.Handle(input);

        // Assert
        await taskRepository.Received(1).AddAsync(Arg.Is<TodoTask>(t => 
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
        IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        IRepository<TodoTask> taskRepository = Substitute.For<IRepository<TodoTask>>();
        IRepository<TaskLog> taskLogRepository = Substitute.For<IRepository<TaskLog>>();

        Guid providedId = Guid.NewGuid();
        
        taskRepository.ContainsAsync(Arg.Any<Expression<Func<TodoTask, bool>>>()).Returns(false);
        unitOfWork.Repository<TodoTask>().Returns(taskRepository);
        unitOfWork.Repository<TaskLog>().Returns(taskLogRepository);
        
        InputDto input = new()
        {
            Id = providedId,
            Name = "Test Task"
        };

        UseCase useCase = new(outputPort, unitOfWork);

        // Act
        await useCase.Handle(input);

        // Assert
        await taskRepository.Received(1).AddAsync(Arg.Is<TodoTask>(t => t.Id == input.Id && t.Name == input.Name));
        await taskLogRepository.Received(1).AddAsync(Arg.Any<TaskLog>());
        await unitOfWork.Received(1).SaveChangesAsync();
        await outputPort.Received(1).Handle(Arg.Is<OutputDto>(o => o.Id == input.Id));
    }

    [Fact]
    public async Task Handle_WhenDateTimesProvided_ShouldSetAllDates()
    {
        // Arrange
        IOutputPort outputPort = Substitute.For<IOutputPort>();
        IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        IRepository<TodoTask> taskRepository = Substitute.For<IRepository<TodoTask>>();
        IRepository<TaskLog> taskLogRepository = Substitute.For<IRepository<TaskLog>>();

        unitOfWork.Repository<TodoTask>().Returns(taskRepository);
        unitOfWork.Repository<TaskLog>().Returns(taskLogRepository);

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

        UseCase useCase = new(outputPort, unitOfWork);

        // Act
        await useCase.Handle(input);

        // Assert
        await taskRepository.Received(1).AddAsync(Arg.Is<TodoTask>(t => 
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
        IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        IRepository<TodoTask> taskRepository = Substitute.For<IRepository<TodoTask>>();
        IRepository<TaskLog> taskLogRepository = Substitute.For<IRepository<TaskLog>>();

        unitOfWork.Repository<TodoTask>().Returns(taskRepository);
        unitOfWork.Repository<TaskLog>().Returns(taskLogRepository);

        InputDto input = new();

        UseCase useCase = new(outputPort, unitOfWork);

        // Act
        await useCase.Handle(input);

        // Assert
        await taskRepository.Received(1).AddAsync(Arg.Is<TodoTask>(t => 
            t.Name == "New Task"
        ));
    }
}
