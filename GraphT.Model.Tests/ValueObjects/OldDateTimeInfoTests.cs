using GraphT.Model.ValueObjects;

namespace GraphT.Model.Tests.ValueObjects;

public class OldDateTimeInfoTests
{
	[Fact]
	public void Punctuality_ReturnsNoTargetWhenLimitDateTimeNotSet()
	{
		OldDateTimeInfo oldDateTimeInfo = new();

		Assert.Equal("\u26a0 No Target", oldDateTimeInfo.Punctuality);
	}

	[Fact]
	public void Punctuality_ReturnsOnTimeWhenFinishedOnLimitDate()
	{
		DateTimeOffset limitDate = DateTimeOffset.Now;
		OldDateTimeInfo sameOldDate = new() { LimitDateTime = limitDate, FinishDateTime = limitDate };
		OldDateTimeInfo beforeLimit = new() { LimitDateTime = limitDate, FinishDateTime = limitDate.AddSeconds(-1) };

		Assert.Equal("\u2705 On Time!", sameOldDate.Punctuality);
		Assert.Equal("\u2705 On Time!", beforeLimit.Punctuality);
	}

	[Fact]
	public void Punctuality_ReturnsLateWhenFinishedAfterLimitDate()
	{
		DateTimeOffset limitDate = DateTimeOffset.Now.Date;
		OldDateTimeInfo oldDateTimeInfo = new() { LimitDateTime = limitDate, FinishDateTime = limitDate.AddDays(2) };

		Assert.Equal("\ud83d\udea8 Late 2 day(s)!", oldDateTimeInfo.Punctuality);
	}

	[Fact]
	public void Punctuality_ReturnsEarlyWhenFinishedBeforeLimitDate()
	{
		DateTimeOffset limitDate = DateTimeOffset.Now;
		OldDateTimeInfo oldDateTimeInfo = new() { LimitDateTime = limitDate, FinishDateTime = limitDate.AddDays(-2) };

		Assert.Equal("\u2b50 Early 2 day(s)!", oldDateTimeInfo.Punctuality);
	}

	[Fact]
	public void Punctuality_ReturnsFinishTodayWhenLimitDateIsToday()
	{
		OldDateTimeInfo oldDateTimeInfo = new() { LimitDateTime = DateTimeOffset.Now };

		Assert.Equal("\u26a0 Finish Today!", oldDateTimeInfo.Punctuality);
	}

	[Fact]
	public void Punctuality_ReturnsLateWhenCurrentDateIsAfterLimitDate()
	{
		OldDateTimeInfo oldDateTimeInfo = new() { LimitDateTime = DateTimeOffset.Now.AddDays(-2) };

		Assert.Equal("\ud83d\udea8 Late 2 day(s)!", oldDateTimeInfo.Punctuality);
	}

	[Fact]
	public void Punctuality_ReturnsDaysToGoWhenCurrentDateIsBeforeLimitDate()
	{
		OldDateTimeInfo oldDateTimeInfo = new() { LimitDateTime = DateTimeOffset.Now.AddDays(3) };

		Assert.Equal("\u23f1 4 day(s) to go!", oldDateTimeInfo.Punctuality);
	}
}
