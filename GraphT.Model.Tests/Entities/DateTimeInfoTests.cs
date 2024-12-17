using GraphT.Model.Services;
using GraphT.Model.Services.Specifications;
using GraphT.Model.ValueObjects;

using NSubstitute;

using SeedWork;

namespace GraphT.Model.Tests.Entities;

public class DateTimeInfoTests
{
	[Fact]
	public void Punctuality_ReturnsNoTargetWhenLimitDateTimeNotSet()
	{
		DateTimeInfo dateTimeInfo = new();

		Assert.Equal("\u26a0 No Target", dateTimeInfo.Punctuality);
	}

	[Fact]
	public void Punctuality_ReturnsOnTimeWhenFinishedOnLimitDate()
	{
		DateTimeOffset limitDate = DateTimeOffset.Now;
		DateTimeInfo sameDate = new() { LimitDateTime = limitDate, FinishDateTime = limitDate };
		DateTimeInfo beforeLimit = new() { LimitDateTime = limitDate, FinishDateTime = limitDate.AddSeconds(-1) };

		Assert.Equal("\u2705 On Time!", sameDate.Punctuality);
		Assert.Equal("\u2705 On Time!", beforeLimit.Punctuality);
	}

	[Fact]
	public void Punctuality_ReturnsLateWhenFinishedAfterLimitDate()
	{
		DateTimeOffset limitDate = DateTimeOffset.Now.Date;
		DateTimeInfo dateTimeInfo = new() { LimitDateTime = limitDate, FinishDateTime = limitDate.AddDays(2) };

		Assert.Equal("\ud83d\udea8 Late 2 day(s)!", dateTimeInfo.Punctuality);
	}

	[Fact]
	public void Punctuality_ReturnsEarlyWhenFinishedBeforeLimitDate()
	{
		DateTimeOffset limitDate = DateTimeOffset.Now;
		DateTimeInfo dateTimeInfo = new() { LimitDateTime = limitDate, FinishDateTime = limitDate.AddDays(-2) };

		Assert.Equal("\u2b50 Early 2 day(s)!", dateTimeInfo.Punctuality);
	}

	[Fact]
	public void Punctuality_ReturnsFinishTodayWhenLimitDateIsToday()
	{
		DateTimeInfo dateTimeInfo = new() { LimitDateTime = DateTimeOffset.Now };

		Assert.Equal("\u26a0 Finish Today!", dateTimeInfo.Punctuality);
	}

	[Fact]
	public void Punctuality_ReturnsLateWhenCurrentDateIsAfterLimitDate()
	{
		DateTimeInfo dateTimeInfo = new() { LimitDateTime = DateTimeOffset.Now.AddDays(-2) };

		Assert.Equal("\ud83d\udea8 Late 2 day(s)!", dateTimeInfo.Punctuality);
	}

	[Fact]
	public void Punctuality_ReturnsDaysToGoWhenCurrentDateIsBeforeLimitDate()
	{
		DateTimeInfo dateTimeInfo = new() { LimitDateTime = DateTimeOffset.Now.AddDays(3) };

		Assert.Equal("\u23f1 4 day(s) to go!", dateTimeInfo.Punctuality);
	}

	[Fact]
	public async Task GetTimeSpendString_WhenStatusIsNotCompletable_ReturnsZeroTimeSpend()
	{
		// Arrange
		IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
		Guid taskId = Guid.NewGuid();
		DateTimeOffset newDateTime = DateTimeOffset.UtcNow;

		// Pruebas con estados que no calculan tiempo
		Status[] nonCalculableStatuses = new[]
		{
			Status.Created, Status.Backlog, Status.ReadyToStart, Status.InProgress
		};

		foreach (Status status in nonCalculableStatuses)
		{
			// Act
			(string, TimeSpan) result = await TimeSpendCalculatorService.GetTimeSpend(taskId, status, newDateTime, unitOfWork);

			// Assert
			Assert.Equal("\u23f0 0 day(s) - 0 hours - 0 minutes", result.Item1);
			Assert.Equal(TimeSpan.Zero, result.Item2);
		}
	}

	[Fact]
	public async Task GetTimeSpendString_WhenNoLastLogExists_ReturnsZeroTimeSpend()
	{
		// Arrange
		IUnitOfWork? unitOfWork = Substitute.For<IUnitOfWork>();
		Guid taskId = Guid.NewGuid();
		DateTimeOffset newDateTime = DateTimeOffset.UtcNow;
		Status newStatus = Status.Completed;

		IRepository<TaskLog>? repository = Substitute.For<IRepository<TaskLog>>();
		repository.FindAsync(Arg.Any<LastTaskLogSpecification>())
			.Returns(new PagedList<TaskLog>(new List<TaskLog>(), 0, 1, 1));

		unitOfWork.Repository<TaskLog>().Returns(repository);

		// Act
		(string, TimeSpan) result = await TimeSpendCalculatorService.GetTimeSpend(taskId, newStatus, newDateTime, unitOfWork);

		// Assert
		Assert.Equal("\u23f0 0 day(s) - 0 hours - 0 minutes", result.Item1);
		Assert.Equal(TimeSpan.Zero, result.Item2);
	}

	[Fact]
	public async Task GetTimeSpendString_WhenLastLogHasNonWorkingStatus_ReturnsZeroTimeSpend()
	{
		// Arrange
		IUnitOfWork? unitOfWork = Substitute.For<IUnitOfWork>();
		Guid taskId = Guid.NewGuid();
		DateTimeOffset newDateTime = DateTimeOffset.UtcNow;
		Status newStatus = Status.Completed;

		Status[] nonWorkingStatuses = new[] { Status.Paused, Status.Dropped, Status.Completed };

		foreach (Status lastLogStatus in nonWorkingStatuses)
		{
			TaskLog lastLog = new(taskId, DateTimeOffset.UtcNow.AddDays(-1), lastLogStatus);
			IRepository<TaskLog>? repository = Substitute.For<IRepository<TaskLog>>();
			repository.FindAsync(Arg.Any<LastTaskLogSpecification>())
				.Returns(new PagedList<TaskLog>(new List<TaskLog> { lastLog }, 1, 1, 1));

			unitOfWork.Repository<TaskLog>().Returns(repository);

			// Act
			(string, TimeSpan) result = await TimeSpendCalculatorService.GetTimeSpend(taskId, newStatus, newDateTime, unitOfWork);

			// Assert
			Assert.Equal("\u23f0 0 day(s) - 0 hours - 0 minutes", result.Item1);
			Assert.Equal(TimeSpan.Zero, result.Item2);
		}
	}

	[Fact]
	public async Task GetTimeSpendString_CalculatesTimeSpendCorrectly()
	{
		// Arrange
		var testCases = new List<(int days, int hours, int minutes, string expectedResult, TimeSpan expectedTimeSpan)>
		{
			(1, 0, 0, "\u23f0 1 day(s) - 0 hours - 0 minutes", TimeSpan.FromDays(1)),
			(31, 0, 0, "\u23f0 31 day(s) - 0 hours - 0 minutes", TimeSpan.FromDays(31)),
			(2, 3, 0, "\u23f0 2 day(s) - 3 hours - 0 minutes", TimeSpan.FromDays(2) + TimeSpan.FromHours(3)),
			(0, 0, 30, "\u26a1 0 day(s) - 0 hours - 30 minutes", TimeSpan.FromMinutes(30))
		};

		foreach (var testCase in testCases)
		{
			IUnitOfWork? unitOfWork = Substitute.For<IUnitOfWork>();
			Guid taskId = Guid.NewGuid();
			DateTimeOffset newDateTime = DateTimeOffset.UtcNow;
			DateTimeOffset lastLogDateTime = newDateTime.Subtract(TimeSpan.FromDays(testCase.days) + TimeSpan.FromHours(testCase.hours) + TimeSpan.FromMinutes(testCase.minutes));
			TaskLog lastLog = new(taskId, lastLogDateTime, Status.InProgress);

			IRepository<TaskLog>? repository = Substitute.For<IRepository<TaskLog>>();
			repository.FindAsync(Arg.Any<LastTaskLogSpecification>())
				.Returns(new PagedList<TaskLog>(new List<TaskLog> { lastLog }, 1, 1, 1));

			unitOfWork.Repository<TaskLog>().Returns(repository);

			// Act
			(string, TimeSpan) result = await TimeSpendCalculatorService.GetTimeSpend(taskId, Status.Completed, newDateTime, unitOfWork);

			// Assert
			Assert.Equal(testCase.expectedResult, result.Item1);
			Assert.Equal(testCase.expectedTimeSpan, result.Item2);
		}
	}

	[Fact]
	public async Task GetTimeSpendString_WhenTimeSpanIsZero_ReturnsZeroTimeSpend()
	{
		// Arrange
		IUnitOfWork? unitOfWork = Substitute.For<IUnitOfWork>();
		Guid taskId = Guid.NewGuid();
		DateTimeOffset newDateTime = DateTimeOffset.UtcNow;
		DateTimeOffset lastLogDateTime = newDateTime;
		TaskLog lastLog = new(taskId, lastLogDateTime, Status.InProgress);

		IRepository<TaskLog>? repository = Substitute.For<IRepository<TaskLog>>();
		repository.FindAsync(Arg.Any<LastTaskLogSpecification>())
			.Returns(new PagedList<TaskLog>(new List<TaskLog> { lastLog }, 1, 1, 1));

		unitOfWork.Repository<TaskLog>().Returns(repository);

		// Act
		(string, TimeSpan) result = await TimeSpendCalculatorService.GetTimeSpend(taskId, Status.Completed, newDateTime, unitOfWork);

		// Assert
		Assert.Equal("\u23f0 0 day(s) - 0 hours - 0 minutes", result.Item1);
		Assert.Equal(TimeSpan.Zero, result.Item2);
	}
}
