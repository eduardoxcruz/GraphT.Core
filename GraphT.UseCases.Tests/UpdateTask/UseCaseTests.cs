using GraphT.Model.Aggregates;
using GraphT.Model.Exceptions;
using GraphT.Model.Services.Specifications;
using GraphT.Model.ValueObjects;
using GraphT.UseCases.DTOs;
using GraphT.UseCases.UpdateTask;

using NSubstitute;

using SeedWork;

namespace GraphT.UseCases.Tests.UpdateTask;

public class UseCaseTests
{
	[Fact]
	public async Task Handle_WhenIdIsNull_ThrowsArgumentException()
	{
		// Arrange
		IOutputPort? outputPort = Substitute.For<IOutputPort>();
		IUnitOfWork? unitOfWork = Substitute.For<IUnitOfWork>();
		UseCase useCase = new(outputPort, unitOfWork);
		TaskInfo dto = new() { Id = null };

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentException>(() => useCase.Handle(dto).AsTask());
	}

	[Fact]
	public async Task Handle_WhenTaskNotFound_ThrowsTaskNotFoundException()
	{
		// Arrange
		IOutputPort? outputPort = Substitute.For<IOutputPort>();
		IUnitOfWork? unitOfWork = Substitute.For<IUnitOfWork>();
		IRepository<TaskAggregate>? repository = Substitute.For<IRepository<TaskAggregate>>();
		Guid taskId = Guid.NewGuid();

		unitOfWork.Repository<TaskAggregate>().Returns(repository);
		repository.FindByIdAsync(taskId).Returns((TaskAggregate?)null);

		UseCase useCase = new(outputPort, unitOfWork);
		TaskInfo dto = new() { Id = taskId };

		// Act & Assert
		await Assert.ThrowsAsync<TaskNotFoundException>(() => useCase.Handle(dto).AsTask());
	}

	[Fact]
	public async Task Handle_WhenOnlyBasicPropertiesUpdated_UpdatesTaskSuccessfully()
	{
		// Arrange
		IOutputPort? outputPort = Substitute.For<IOutputPort>();
		IUnitOfWork? unitOfWork = Substitute.For<IUnitOfWork>();
		IRepository<TaskAggregate>? repository = Substitute.For<IRepository<TaskAggregate>>();
		Guid taskId = Guid.NewGuid();
		TaskAggregate existingTask = new("Old Name", id: taskId);
		TaskInfo dto = new()
		{
			Id = taskId
			, Name = "New Name"
			, IsFun = true
			, IsProductive = true
			, Complexity = Complexity.High
			, Priority = Priority.DropEverythingElse
		};

		unitOfWork.Repository<TaskAggregate>().Returns(repository);
		repository.FindByIdAsync(taskId).Returns(existingTask);
		UseCase useCase = new(outputPort, unitOfWork);

		// Act
		await useCase.Handle(dto);

		// Assert
		await repository.Received(1).UpdateAsync(Arg.Is<TaskAggregate>(t =>
			t.Name == dto.Name &&
			t.IsFun == dto.IsFun &&
			t.IsProductive == dto.IsProductive &&
			t.Complexity == dto.Complexity &&
			t.Priority == dto.Priority
		));
		await unitOfWork.Received(1).SaveChangesAsync();
		await outputPort.Received(1).Handle();
	}

	[Theory]
	[InlineData(Status.InProgress, Status.Completed, 1, 5, 45, "\u23f0 1 day(s) - 7 hours - 45 minutes")]
	public async Task Handle_WhenPreviousLogExist_UpdatesTimeSpendCorrectly(
		Status initialStatus,
		Status newStatus,
		int expectedDays,
		int expectedHours,
		int expectedMinutes,
		string expectedTimeSpendString)
	{
		// Arrange
		IOutputPort outputPort = Substitute.For<IOutputPort>();
		IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
		IRepository<TaskAggregate> taskRepository = Substitute.For<IRepository<TaskAggregate>>();
		IRepository<TaskLog> logRepository = Substitute.For<IRepository<TaskLog>>();
		Guid taskId = Guid.NewGuid();
		DateTimeOffset currentTime = DateTimeOffset.UtcNow;
		TaskAggregate existingTask = new("Test Task", id: taskId, status: initialStatus);
		TaskInfo dto = new() { Id = taskId, Status = newStatus };
		TimeSpan previousTimeSpent = TimeSpan.FromDays(expectedDays) + TimeSpan.FromHours(expectedHours) + TimeSpan.FromMinutes(expectedMinutes);
		TaskLog lastLog = new(taskId, currentTime.AddHours(-2), initialStatus, previousTimeSpent);
		PagedList<TaskLog> logList = new([ lastLog ], 1, 1, 1);
		
		logRepository.FindAsync(Arg.Any<LastTaskLogSpecification>()).Returns(logList);
		taskRepository.FindByIdAsync(taskId).Returns(existingTask);
		unitOfWork.Repository<TaskAggregate>().Returns(taskRepository);
		unitOfWork.Repository<TaskLog>().Returns(logRepository);
		UseCase useCase = new(outputPort, unitOfWork);
		
		// Act
		await useCase.Handle(dto);
		
		// Assert
		await taskRepository.Received(1).UpdateAsync(Arg.Is<TaskAggregate>(t =>
			t.Status == newStatus &&
			t.DateTimeInfo.TimeSpend == expectedTimeSpendString
		));

		await logRepository.Received(1).AddAsync(Arg.Is<TaskLog>(l =>
			l.TaskId == taskId &&
			l.Status == newStatus &&
			l.TimeSpentOnTask!.Value.Days == (previousTimeSpent +  (currentTime - lastLog.DateTime)).Days &&
			l.TimeSpentOnTask!.Value.Hours == (previousTimeSpent +  (currentTime - lastLog.DateTime)).Hours && 
			l.TimeSpentOnTask!.Value.Minutes == (previousTimeSpent +  (currentTime - lastLog.DateTime)).Minutes &&
			l.TimeSpentOnTask!.Value.Seconds == (previousTimeSpent +  (currentTime - lastLog.DateTime)).Seconds
			)
		);

		await unitOfWork.Received(1).SaveChangesAsync();
		await outputPort.Received(1).Handle();
	}

	[Fact]
	public async Task Handle_WhenNoPreviousLogs_CreatesFirstLogWithZeroTime()
	{
		// Arrange
		IOutputPort outputPort = Substitute.For<IOutputPort>();
		IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
		IRepository<TaskAggregate> taskRepository = Substitute.For<IRepository<TaskAggregate>>();
		IRepository<TaskLog> logRepository = Substitute.For<IRepository<TaskLog>>();

		Guid taskId = Guid.NewGuid();
		Status newStatus = Status.Completed;
		TaskAggregate existingTask = new("New Task", id: taskId, status: Status.ReadyToStart);
		TaskInfo dto = new() { Id = taskId, Status = newStatus };
		string expectedTimeSpendString = "\u26a1 0 day(s) - 0 hours - 0 minutes";
		
		logRepository.FindAsync(Arg.Any<LastTaskLogSpecification>()).Returns(new PagedList<TaskLog>([], 0, 1, 1));
		taskRepository.FindByIdAsync(taskId).Returns(existingTask);
		unitOfWork.Repository<TaskAggregate>().Returns(taskRepository);
		unitOfWork.Repository<TaskLog>().Returns(logRepository);
		UseCase useCase = new(outputPort, unitOfWork);

		// Act
		await useCase.Handle(dto);

		// Assert
		await taskRepository.Received(1).UpdateAsync(Arg.Is<TaskAggregate>(t =>
			t.Status == newStatus &&
			t.DateTimeInfo.TimeSpend == expectedTimeSpendString
		));

		await logRepository.Received(1).AddAsync(Arg.Is<TaskLog>(l =>
			l.TaskId == taskId &&
			l.Status == newStatus &&
			l.TimeSpentOnTask == TimeSpan.Zero
		));

		await unitOfWork.Received(1).SaveChangesAsync();
		await outputPort.Received(1).Handle();
	}

	[Fact]
	public async Task Handle_WhenDateTimeFieldsUpdated_UpdatesTaskDatesSuccessfully()
	{
		// Arrange
		IOutputPort? outputPort = Substitute.For<IOutputPort>();
		IUnitOfWork? unitOfWork = Substitute.For<IUnitOfWork>();
		IRepository<TaskAggregate>? repository = Substitute.For<IRepository<TaskAggregate>>();
		Guid taskId = Guid.NewGuid();

		DateTimeOffset startDate = DateTimeOffset.UtcNow;
		DateTimeOffset finishDate = DateTimeOffset.UtcNow.AddDays(1);
		DateTimeOffset limitDate = DateTimeOffset.UtcNow.AddDays(2);

		TaskAggregate existingTask = new("Old Name", id: taskId);

		unitOfWork.Repository<TaskAggregate>().Returns(repository);
		repository.FindByIdAsync(taskId).Returns(existingTask);

		UseCase useCase = new(outputPort, unitOfWork);
		TaskInfo dto = new()
		{
			Id = taskId, StartDateTime = startDate, FinishDateTime = finishDate, LimitDateTime = limitDate
		};

		// Act
		await useCase.Handle(dto);

		// Assert
		await repository.Received(1).UpdateAsync(Arg.Is<TaskAggregate>(t =>
			t.Id == taskId &&
			t.DateTimeInfo.StartDateTime != null &&
			t.DateTimeInfo.StartDateTime == dto.StartDateTime &&
			t.DateTimeInfo.FinishDateTime != null &&
			t.DateTimeInfo.FinishDateTime == dto.FinishDateTime &&
			t.DateTimeInfo.LimitDateTime != null &&
			t.DateTimeInfo.LimitDateTime == dto.LimitDateTime
		));
		await unitOfWork.Received(1).SaveChangesAsync();
		await outputPort.Received(1).Handle();
	}

	[Fact]
	public async Task Handle_WhenNoChangesProvided_OnlyUpdatesUnchangedTask()
	{
		// Arrange
		IOutputPort? outputPort = Substitute.For<IOutputPort>();
		IUnitOfWork? unitOfWork = Substitute.For<IUnitOfWork>();
		IRepository<TaskAggregate>? repository = Substitute.For<IRepository<TaskAggregate>>();
		Guid taskId = Guid.NewGuid();

		TaskAggregate existingTask = new("Original Name", id: taskId, status: Status.Paused);

		unitOfWork.Repository<TaskAggregate>().Returns(repository);
		repository.FindByIdAsync(taskId).Returns(existingTask);

		UseCase useCase = new(outputPort, unitOfWork);
		TaskInfo dto = new() { Id = taskId };

		// Act
		await useCase.Handle(dto);

		// Assert
		await repository.Received(1).UpdateAsync(Arg.Is<TaskAggregate>(t =>
			t.Name == "Original Name" &&
			t.Status == Status.Paused
		));
		await unitOfWork.Received(1).SaveChangesAsync();
		await outputPort.Received(1).Handle();
	}
}
