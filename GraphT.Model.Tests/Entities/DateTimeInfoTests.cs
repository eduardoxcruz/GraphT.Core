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
			string result =
				await TimeSpendCalculatorService.GetTimeSpendString(taskId, status, newDateTime, unitOfWork);

			// Assert
			Assert.Equal("\u23f0 0 day(s) - 0 hours - 0 minutes", result);
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
		string result = await TimeSpendCalculatorService.GetTimeSpendString(taskId, newStatus, newDateTime, unitOfWork);

		// Assert
		Assert.Equal("\u23f0 0 day(s) - 0 hours - 0 minutes", result);
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
			string result =
				await TimeSpendCalculatorService.GetTimeSpendString(taskId, newStatus, newDateTime, unitOfWork);

			// Assert
			Assert.Equal("\u23f0 0 day(s) - 0 hours - 0 minutes", result);
		}
	}

	[Theory]
	[InlineData(1, 0, 0, "\u23f0 1 day(s) - 0 hours - 0 minutes")]
	[InlineData(31, 0, 0, "\u23f0 31 day(s) - 0 hours - 0 minutes")]
	[InlineData(2, 3, 0, "\u23f0 2 day(s) - 3 hours - 0 minutes")]
	[InlineData(0, 0, 30, "\u26a1 0 day(s) - 0 hours - 30 minutes")]
	public async Task GetTimeSpendString_CalculatesTimeSpendCorrectly(int days, int hours, int minutes,
		string expectedResult)
	{
		// Arrange
		IUnitOfWork? unitOfWork = Substitute.For<IUnitOfWork>();
		Guid taskId = Guid.NewGuid();
		DateTimeOffset newDateTime = DateTimeOffset.UtcNow;
		DateTimeOffset lastLogDateTime =
			newDateTime.Subtract(TimeSpan.FromDays(days) + TimeSpan.FromHours(hours) + TimeSpan.FromMinutes(minutes));
		TaskLog lastLog = new(taskId, lastLogDateTime, Status.InProgress);

		IRepository<TaskLog>? repository = Substitute.For<IRepository<TaskLog>>();
		repository.FindAsync(Arg.Any<LastTaskLogSpecification>())
			.Returns(new PagedList<TaskLog>(new List<TaskLog> { lastLog }, 1, 1, 1));

		unitOfWork.Repository<TaskLog>().Returns(repository);

		// Act
		string result =
			await TimeSpendCalculatorService.GetTimeSpendString(taskId, Status.Completed, newDateTime, unitOfWork);

		// Assert
		Assert.Equal(expectedResult, result);
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
		string result =
			await TimeSpendCalculatorService.GetTimeSpendString(taskId, Status.Completed, newDateTime, unitOfWork);

		// Assert
		Assert.Equal("\u23f0 0 day(s) - 0 hours - 0 minutes", result);
	}
}
