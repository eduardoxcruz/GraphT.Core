using GraphT.Model.Entities;
using GraphT.Model.Exceptions;
using GraphT.Model.Services.Repositories;
using GraphT.Model.ValueObjects;
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
		IOutputPort outputPort = Substitute.For<IOutputPort>();
		ITodoTaskRepository todoTaskRepository = Substitute.For<ITodoTaskRepository>();
		ITaskLogRepository taskLogRepository = Substitute.For<ITaskLogRepository>();
		IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
		UseCase useCase = new(outputPort, todoTaskRepository, taskLogRepository, unitOfWork);
		InputDto dto = new() { Id = null };

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentException>(() => useCase.Handle(dto).AsTask());
	}

	[Fact]
	public async Task Handle_WhenTaskNotFound_ThrowsTaskNotFoundException()
	{
		// Arrange
		IOutputPort outputPort = Substitute.For<IOutputPort>();
		ITodoTaskRepository todoTaskRepository = Substitute.For<ITodoTaskRepository>();
		ITaskLogRepository taskLogRepository = Substitute.For<ITaskLogRepository>();
		IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
		Guid taskId = Guid.NewGuid();

		todoTaskRepository.FindByIdAsync(taskId).Returns((TodoTask?)null);

		UseCase useCase = new(outputPort, todoTaskRepository, taskLogRepository, unitOfWork);
		InputDto dto = new() { Id = taskId };

		// Act & Assert
		await Assert.ThrowsAsync<TaskNotFoundException>(() => useCase.Handle(dto).AsTask());
	}

	[Fact]
	public async Task Handle_WhenOnlyBasicPropertiesUpdated_UpdatesTaskSuccessfully()
	{
		// Arrange
		IOutputPort outputPort = Substitute.For<IOutputPort>();
		ITodoTaskRepository todoTaskRepository = Substitute.For<ITodoTaskRepository>();
		ITaskLogRepository taskLogRepository = Substitute.For<ITaskLogRepository>();
		IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
		Guid taskId = Guid.NewGuid();
		TodoTask existingTask = new("Old Name", id: taskId);
		InputDto dto = new()
		{
			Id = taskId
			, Name = "New Name"
			, IsFun = true
			, IsProductive = true
			, Complexity = OldComplexity.High
			, Priority = OldPriority.Critical
		};

		todoTaskRepository.FindByIdAsync(taskId).Returns(existingTask);
		UseCase useCase = new(outputPort, todoTaskRepository, taskLogRepository, unitOfWork);

		// Act
		await useCase.Handle(dto);

		// Assert
		await todoTaskRepository.Received(1).UpdateAsync(Arg.Is<TodoTask>(t =>
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
	[InlineData(OldStatus.Doing, OldStatus.Completed, 1, 5, 45, "\u23f0 1 day(s) - 7 hours - 45 minutes")]
	public async Task Handle_WhenPreviousLogExist_UpdatesTimeSpendCorrectly(
		OldStatus initialOldStatus,
		OldStatus newOldStatus,
		int expectedDays,
		int expectedHours,
		int expectedMinutes,
		string expectedTimeSpendString)
	{
		// Arrange
		IOutputPort outputPort = Substitute.For<IOutputPort>();
		ITodoTaskRepository todoTaskRepository = Substitute.For<ITodoTaskRepository>();
		ITaskLogRepository taskLogRepository = Substitute.For<ITaskLogRepository>();
		IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
		Guid taskId = Guid.NewGuid();
		DateTimeOffset currentTime = DateTimeOffset.UtcNow;
		TodoTask existingTask = new("Test Task", id: taskId, status: initialOldStatus);
		InputDto dto = new() { Id = taskId, Status = newOldStatus };
		TimeSpan previousTimeSpent = TimeSpan.FromDays(expectedDays) + TimeSpan.FromHours(expectedHours) + TimeSpan.FromMinutes(expectedMinutes);
		TaskLog lastLog = new(taskId, currentTime.AddHours(-2), initialOldStatus, previousTimeSpent);
		
		taskLogRepository.FindTaskLastLog(taskId).Returns(lastLog);
		todoTaskRepository.FindByIdAsync(taskId).Returns(existingTask);
		UseCase useCase = new(outputPort, todoTaskRepository, taskLogRepository, unitOfWork);
		
		// Act
		await useCase.Handle(dto);
		
		// Assert
		await todoTaskRepository.Received(1).UpdateAsync(Arg.Is<TodoTask>(t =>
			t.OldStatus == newOldStatus &&
			t.DateTimeInfo.TimeSpend == expectedTimeSpendString
		));

		await taskLogRepository.Received(1).AddAsync(Arg.Is<TaskLog>(l =>
			l.TaskId == taskId &&
			l.OldStatus == newOldStatus &&
			l.TimeSpentOnTask!.Value.Days == (previousTimeSpent + (currentTime - lastLog.DateTime)).Days &&
			l.TimeSpentOnTask!.Value.Hours == (previousTimeSpent + (currentTime - lastLog.DateTime)).Hours && 
			l.TimeSpentOnTask!.Value.Minutes == (previousTimeSpent + (currentTime - lastLog.DateTime)).Minutes &&
			l.TimeSpentOnTask!.Value.Seconds == (previousTimeSpent + (currentTime - lastLog.DateTime)).Seconds
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
		ITodoTaskRepository todoTaskRepository = Substitute.For<ITodoTaskRepository>();
		ITaskLogRepository taskLogRepository = Substitute.For<ITaskLogRepository>();
		IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();

		Guid taskId = Guid.NewGuid();
		OldStatus newOldStatus = OldStatus.Completed;
		TodoTask existingTask = new("New Task", id: taskId, status: OldStatus.Ready);
		InputDto dto = new() { Id = taskId, Status = newOldStatus };
		string expectedTimeSpendString = "\u26a1 0 day(s) - 0 hours - 0 minutes";
		
		taskLogRepository.FindTaskLastLog(taskId).Returns((TaskLog?)null);
		todoTaskRepository.FindByIdAsync(taskId).Returns(existingTask);
		UseCase useCase = new(outputPort, todoTaskRepository, taskLogRepository, unitOfWork);

		// Act
		await useCase.Handle(dto);

		// Assert
		await todoTaskRepository.Received(1).UpdateAsync(Arg.Is<TodoTask>(t =>
			t.OldStatus == newOldStatus &&
			t.DateTimeInfo.TimeSpend == expectedTimeSpendString
		));

		await taskLogRepository.Received(1).AddAsync(Arg.Is<TaskLog>(l =>
			l.TaskId == taskId &&
			l.OldStatus == newOldStatus &&
			l.TimeSpentOnTask == TimeSpan.Zero
		));

		await unitOfWork.Received(1).SaveChangesAsync();
		await outputPort.Received(1).Handle();
	}

	[Fact]
	public async Task Handle_WhenDateTimeFieldsUpdated_UpdatesTaskDatesSuccessfully()
	{
		// Arrange
		IOutputPort outputPort = Substitute.For<IOutputPort>();
		ITodoTaskRepository todoTaskRepository = Substitute.For<ITodoTaskRepository>();
		ITaskLogRepository taskLogRepository = Substitute.For<ITaskLogRepository>();
		IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
		Guid taskId = Guid.NewGuid();

		DateTimeOffset startDate = DateTimeOffset.UtcNow;
		DateTimeOffset finishDate = DateTimeOffset.UtcNow.AddDays(1);
		DateTimeOffset limitDate = DateTimeOffset.UtcNow.AddDays(2);

		TodoTask existingTask = new("Old Name", id: taskId);

		todoTaskRepository.FindByIdAsync(taskId).Returns(existingTask);

		UseCase useCase = new(outputPort, todoTaskRepository, taskLogRepository, unitOfWork);
		InputDto dto = new()
		{
			Id = taskId, StartDateTime = startDate, FinishDateTime = finishDate, LimitDateTime = limitDate
		};

		// Act
		await useCase.Handle(dto);

		// Assert
		await todoTaskRepository.Received(1).UpdateAsync(Arg.Is<TodoTask>(t =>
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
		IOutputPort outputPort = Substitute.For<IOutputPort>();
		ITodoTaskRepository todoTaskRepository = Substitute.For<ITodoTaskRepository>();
		ITaskLogRepository taskLogRepository = Substitute.For<ITaskLogRepository>();
		IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
		Guid taskId = Guid.NewGuid();

		TodoTask existingTask = new("Original Name", id: taskId, status: OldStatus.Paused);

		todoTaskRepository.FindByIdAsync(taskId).Returns(existingTask);

		UseCase useCase = new(outputPort, todoTaskRepository, taskLogRepository, unitOfWork);
		InputDto dto = new() { Id = taskId };

		// Act
		await useCase.Handle(dto);

		// Assert
		await todoTaskRepository.Received(1).UpdateAsync(Arg.Is<TodoTask>(t =>
			t.Name == "Original Name" &&
			t.OldStatus == OldStatus.Paused
		));
		await unitOfWork.Received(1).SaveChangesAsync();
		await outputPort.Received(1).Handle();
	}
}
