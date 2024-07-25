using GraphT.Model.ValueObjects;

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
	public void TimeSpend_ReturnsZeroWhenStartDateTimeNotSet()
	{
		DateTimeInfo dateTimeInfo = new();

		Assert.Equal("\u23f0 0 month(s) - 0 day(s) - 0 hours - 0 minutes", dateTimeInfo.TimeSpend);
	}

	[Fact]
	public void TimeSpend_CalculatesCorrectDuration()
	{
		DateTimeInfo dateTimeInfo = new()
		{
			StartDateTime = DateTimeOffset.Now.AddMonths(-1).AddDays(-5).AddHours(-3).AddMinutes(-30),
			FinishDateTime = DateTimeOffset.Now
		};

		Assert.Equal("\u23f0 1 month(s) - 5 day(s) - 3 hours - 30 minutes", dateTimeInfo.TimeSpend);
	}

	[Fact]
	public void TimeSpend_UsesLightningEmojiForShortDurations()
	{
		DateTimeInfo dateTimeInfo = new()
		{
			StartDateTime = DateTimeOffset.Now.AddMinutes(-59), 
			FinishDateTime = DateTimeOffset.Now
		};

		Assert.StartsWith($"\u26a1 0 month(s) - 0 day(s) - 0 hours - 59 minutes", dateTimeInfo.TimeSpend);
	}
}
