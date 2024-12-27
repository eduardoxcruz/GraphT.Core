using GraphT.Model.Services;
using GraphT.Model.Services.Specifications;
using GraphT.Model.ValueObjects;

using NSubstitute;

using SeedWork;

namespace GraphT.Model.Tests.Services;

public class TimeSpendCalculatorServiceTests
{
	[Fact]
	public async Task GetTimeSpend_TaskHasNoLogs_ReturnsDefaultTime()
	{
		// Arrange
		Guid taskId = Guid.NewGuid();
		Status newStatus = Status.Completed;
		DateTimeOffset newDateTime = DateTimeOffset.UtcNow;
		IUnitOfWork? unitOfWork = Substitute.For<IUnitOfWork>();
		IRepository<TaskLog>? repository = Substitute.For<IRepository<TaskLog>>();
		
		repository.FindAsync(Arg.Any<LastTaskLogSpecification>()).Returns(new PagedList<TaskLog>(new List<TaskLog>(), 0, 1, 1));
		unitOfWork.Repository<TaskLog>().Returns(repository);
		
		(string, TimeSpan Zero) expected = ("\u26a1 0 day(s) - 0 hours - 0 minutes", TimeSpan.Zero);

		// Act
		(string, TimeSpan) result = await TimeSpendCalculatorService.GetTimeSpend(taskId, newStatus, newDateTime, unitOfWork);

		// Assert
		Assert.Equal(expected, result);
	}

	[Theory]
	[InlineData(30, "0 day(s) - 0 hours - 30 minutes")]
	[InlineData(60, "0 day(s) - 1 hours - 0 minutes")]
	[InlineData(90, "0 day(s) - 1 hours - 30 minutes")]
	[InlineData(120, "0 day(s) - 2 hours - 0 minutes")]
	public async Task GetTimeSpend_TaskInProgressThenChangedToAnotherState_ReturnsCorrectTimeSpend(int timeDifferenceMinutes, string expectedDuration)
	{
		// Arrange
		Guid taskId = Guid.NewGuid();
		Status initialStatus = Status.InProgress;
		Status finalStatus = Status.Completed;
		DateTimeOffset initialDateTime = DateTimeOffset.UtcNow.AddMinutes(-timeDifferenceMinutes);
		DateTimeOffset finalDateTime = DateTimeOffset.UtcNow;
		IUnitOfWork? unitOfWork = Substitute.For<IUnitOfWork>();
		IRepository<TaskLog>? repository = Substitute.For<IRepository<TaskLog>>();

		TaskLog lastLog = new(taskId, initialDateTime, initialStatus, TimeSpan.Zero);
		TimeSpan expectedTimeSpan = finalDateTime - initialDateTime;
		string expectedEmoji = "\u26a1";

		if (timeDifferenceMinutes > 59) expectedEmoji = "\u23f0";
			
		(string, TimeSpan) expectedResult = ($"{expectedEmoji} {expectedDuration}", expectedTimeSpan);

		repository.FindAsync(Arg.Any<LastTaskLogSpecification>()).Returns(new PagedList<TaskLog>([lastLog], 1, 1, 1));
		unitOfWork.Repository<TaskLog>().Returns(repository);

		// Act
		(string, TimeSpan) result = await TimeSpendCalculatorService.GetTimeSpend(taskId, finalStatus, finalDateTime, unitOfWork);

		// Assert
		Assert.Equal(expectedResult, result);
	}
	
	[Theory]
	[InlineData(50, "0 day(s) - 0 hours - 1 minute")]
	[InlineData(100, "0 day(s) - 0 hours - 2 minutes")]
	[InlineData(80000, "2 day(s) - 4 hours - 16 minutes")]
	public async Task GetTimeSpend_TaskInProgressThenChangedToAnotherState_WithNonZeroTimeSpentOnTask_ReturnsCorrectTimeSpend(long timeSpentOnTask, string expectedDuration)
	{
		// Arrange
		Guid taskId = Guid.NewGuid();
		Status initialStatus = Status.InProgress;
		Status finalStatus = Status.Completed;
		DateTimeOffset initialDateTime = DateTimeOffset.UtcNow.AddMinutes(-1);
		DateTimeOffset finalDateTime = DateTimeOffset.UtcNow;
		IUnitOfWork? unitOfWork = Substitute.For<IUnitOfWork>();
		IRepository<TaskLog>? repository = Substitute.For<IRepository<TaskLog>>();

		TaskLog lastLog = new(taskId, initialDateTime, initialStatus, TimeSpan.FromMilliseconds(timeSpentOnTask));
		TimeSpan expectedTimeSpan = (finalDateTime - initialDateTime) + lastLog.TimeSpentOnTask!.Value;

		string expectedEmoji = "\u23f0";
		if (expectedTimeSpan.TotalHours <= 1)
			expectedEmoji = "\u26a1";

		(string, TimeSpan) expectedResult = ($"{expectedEmoji} {expectedTimeSpan.Days} day(s) - {expectedTimeSpan.Hours} hours - {expectedTimeSpan.Minutes} minutes", expectedTimeSpan);

		repository.FindAsync(Arg.Any<LastTaskLogSpecification>()).Returns(new PagedList<TaskLog>([lastLog], 1, 1, 1));
		unitOfWork.Repository<TaskLog>().Returns(repository);

		// Act
		(string, TimeSpan) result = await TimeSpendCalculatorService.GetTimeSpend(taskId, finalStatus, finalDateTime, unitOfWork);

		// Assert
		Assert.Equal(expectedResult, result);
	}

	[Fact]
	public async Task GetTimeSpend_TimeSpentGreaterThanOneHour_ReturnsCorrectEmojiAndTimeSpend()
	{
		// Arrange
		Guid taskId = Guid.NewGuid();
		Status initialStatus = Status.InProgress;
		Status finalStatus = Status.Completed;
		DateTimeOffset initialDateTime = DateTimeOffset.UtcNow.AddHours(-2);
		DateTimeOffset finalDateTime = DateTimeOffset.UtcNow;
		IUnitOfWork? unitOfWork = Substitute.For<IUnitOfWork>();
		IRepository<TaskLog>? repository = Substitute.For<IRepository<TaskLog>>();

		TaskLog lastLog = new(taskId, initialDateTime, initialStatus, TimeSpan.Zero);
		TimeSpan expectedTimeSpan = finalDateTime - initialDateTime;
		string expectedEmoji = "\u23f0";
		(string, TimeSpan expectedTimeSpan) expectedResult = ($"{expectedEmoji} 0 day(s) - {expectedTimeSpan.Hours} hours - {expectedTimeSpan.Minutes} minutes", expectedTimeSpan);
		
		repository.FindAsync(Arg.Any<LastTaskLogSpecification>()).Returns(new PagedList<TaskLog>([lastLog], 1, 1, 1));
		unitOfWork.Repository<TaskLog>().Returns(repository);

		// Act
		(string, TimeSpan) result = await TimeSpendCalculatorService.GetTimeSpend(taskId, finalStatus, finalDateTime, unitOfWork);

		// Assert
		Assert.Equal(expectedResult, result);
	}

	[Fact]
	public async Task GetTimeSpend_TimeSpentLessThanOrEqualToOneHour_ReturnsCorrectEmojiAndTimeSpend()
	{
		// Arrange
		Guid taskId = Guid.NewGuid();
		Status initialStatus = Status.InProgress;
		Status finalStatus = Status.Completed;
		DateTimeOffset initialDateTime = DateTimeOffset.UtcNow.AddMinutes(-59);
		DateTimeOffset finalDateTime = DateTimeOffset.UtcNow;
		IUnitOfWork? unitOfWork = Substitute.For<IUnitOfWork>();
		IRepository<TaskLog>? repository = Substitute.For<IRepository<TaskLog>>();

		TaskLog lastLog = new(taskId, initialDateTime, initialStatus, TimeSpan.Zero);
		TimeSpan expectedTimeSpan = finalDateTime - initialDateTime;
		string expectedEmoji = "\u23f0";
		(string, TimeSpan expectedTimeSpan) expectedResult = ($"{expectedEmoji} 0 day(s) - {expectedTimeSpan.Hours} hours - {expectedTimeSpan.Minutes} minutes", expectedTimeSpan);
		
		repository.FindAsync(Arg.Any<LastTaskLogSpecification>()).Returns(new PagedList<TaskLog>([lastLog], 1, 1, 1));
		unitOfWork.Repository<TaskLog>().Returns(repository);

		// Act
		(string, TimeSpan) result = await TimeSpendCalculatorService.GetTimeSpend(taskId, finalStatus, finalDateTime, unitOfWork);

		// Assert
		Assert.Equal(expectedResult, result);
	}
}
