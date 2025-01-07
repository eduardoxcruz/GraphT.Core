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
        IRepository<TaskAggregate> taskRepository = Substitute.For<IRepository<TaskAggregate>>();
        IRepository<TaskLog> taskLogRepository = Substitute.For<IRepository<TaskLog>>();

        unitOfWork.Repository<TaskAggregate>().Returns(taskRepository);
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
        await taskRepository.Received(1).AddAsync(Arg.Is<TaskAggregate>(t => 
            t.Name == "Test Task" && 
            t.IsFun == true && 
            t.IsProductive == true && 
            t.Complexity == Complexity.High && 
            t.Priority == Priority.DoItNow
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
        IRepository<TaskAggregate> taskRepository = Substitute.For<IRepository<TaskAggregate>>();
        IRepository<TaskLog> taskLogRepository = Substitute.For<IRepository<TaskLog>>();

        Guid providedId = Guid.NewGuid();
        
        taskRepository.FindByIdAsync(providedId).Returns((TaskAggregate)null);
        unitOfWork.Repository<TaskAggregate>().Returns(taskRepository);
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
        await taskRepository.Received(1).AddAsync(Arg.Is<TaskAggregate>(t => 
            t.Id == providedId &&
            t.Name == "Test Task"
        ));
    }

    [Fact]
    public async Task Handle_WhenDateTimesProvided_ShouldSetAllDates()
    {
        // Arrange
        IOutputPort outputPort = Substitute.For<IOutputPort>();
        IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        IRepository<TaskAggregate> taskRepository = Substitute.For<IRepository<TaskAggregate>>();
        IRepository<TaskLog> taskLogRepository = Substitute.For<IRepository<TaskLog>>();

        unitOfWork.Repository<TaskAggregate>().Returns(taskRepository);
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
        await taskRepository.Received(1).AddAsync(Arg.Is<TaskAggregate>(t => 
            t.Name == "Test Task"
        ));
        
        await taskLogRepository.Received(1).AddAsync(Arg.Any<TaskLog>());
        await unitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task Handle_WhenNameNotProvided_ShouldUseDefaultName()
    {
        // Arrange
        IOutputPort outputPort = Substitute.For<IOutputPort>();
        IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        IRepository<TaskAggregate> taskRepository = Substitute.For<IRepository<TaskAggregate>>();
        IRepository<TaskLog> taskLogRepository = Substitute.For<IRepository<TaskLog>>();

        unitOfWork.Repository<TaskAggregate>().Returns(taskRepository);
        unitOfWork.Repository<TaskLog>().Returns(taskLogRepository);

        InputDto input = new();

        UseCase useCase = new(outputPort, unitOfWork);

        // Act
        await useCase.Handle(input);

        // Assert
        await taskRepository.Received(1).AddAsync(Arg.Is<TaskAggregate>(t => 
            t.Name == "New Task"
        ));
    }
}
