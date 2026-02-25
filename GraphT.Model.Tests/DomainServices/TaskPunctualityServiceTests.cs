using GraphT.Model.Services;
using GraphT.Model.Enums;

namespace GraphT.Model.Tests.DomainServices;

public class TaskPunctualityServiceTests
{
	[Fact]
	public void Punctuality_ReturnsNoLimitWhenLimitDateTimeNotSet()
	{
		Assert.Equal(PunctualityType.NoLimit, TaskPunctualityService.Calculate(null, null).Type);
	}

	[Theory]
	[InlineData(-2, 0, 0, 0)]
	[InlineData(-1, -1, 0, 0)]
	[InlineData(-1, 0, -1, 0)]
	[InlineData(-1, 0, 0, -1)]
	public void Punctuality_ReturnsEarlyDaysWhenFinishedBeforeLimitDate(double daysDifference, double hoursDifference, double minutesDifference = 0, double secondsDifference = 0)
	{
		DateTimeOffset limitDateTime = DateTimeOffset.UtcNow;
		DateTimeOffset finishDateTime = limitDateTime
			.AddDays(daysDifference)
			.AddHours(hoursDifference)
			.AddMinutes(minutesDifference)
			.AddSeconds(secondsDifference);

		Assert.True(TaskPunctualityService.Calculate(limitDateTime, finishDateTime).Type == PunctualityType.Early);
		Assert.Equal(daysDifference, TaskPunctualityService.Calculate(limitDateTime, finishDateTime).TimeDifference.Days);
		Assert.Equal(hoursDifference, TaskPunctualityService.Calculate(limitDateTime, finishDateTime).TimeDifference.Hours);
		Assert.Equal(minutesDifference, TaskPunctualityService.Calculate(limitDateTime, finishDateTime).TimeDifference.Minutes);
		Assert.Equal(secondsDifference, TaskPunctualityService.Calculate(limitDateTime, finishDateTime).TimeDifference.Seconds);
	}

	[Theory]
	[InlineData(-1, 0, 0, 0)]
	[InlineData(0, -1, 0, 0)]
	[InlineData(0, 0, -1, 0)]
	[InlineData(0, 0, 0, -1)]
	[InlineData(0, 0, 0, 0)]
	public void Punctuality_ReturnsOnTimeWhenFinishedOnLimitDateOrWithin24Hours(double daysDifference, double hoursDifference, double minutesDifference = 0, double secondsDifference = 0)
	{
		DateTimeOffset limitDateTime = DateTimeOffset.UtcNow;
		DateTimeOffset finishDateTime =
			limitDateTime
				.AddDays(daysDifference)
				.AddHours(hoursDifference)
				.AddMinutes(minutesDifference)
				.AddSeconds(secondsDifference);
		
		Assert.True(TaskPunctualityService.Calculate(limitDateTime, finishDateTime).Type == PunctualityType.OnTime);
	}

	[Theory]
	[InlineData(0, 0, 0, 1)]
	[InlineData(0, 0, 1, 0)]
	[InlineData(0, 1, 0, 0)]
	[InlineData(1, 0, 0, 0)]
	public void Punctuality_ReturnsLateDaysWhenFinishedAfterLimitDate(double daysDifference, double hoursDifference, double minutesDifference = 0, double secondsDifference = 0)
	{
		DateTimeOffset limitDateTime = DateTimeOffset.UtcNow;
		DateTimeOffset finishDateTime =
			limitDateTime
				.AddDays(daysDifference)
				.AddHours(hoursDifference)
				.AddMinutes(minutesDifference)
				.AddSeconds(secondsDifference);
		
		Assert.True(TaskPunctualityService.Calculate(limitDateTime, finishDateTime).Type == PunctualityType.Late);
		Assert.Equal(daysDifference, TaskPunctualityService.Calculate(limitDateTime, finishDateTime).TimeDifference.Days);
		Assert.Equal(hoursDifference, TaskPunctualityService.Calculate(limitDateTime, finishDateTime).TimeDifference.Hours);
		Assert.Equal(minutesDifference, TaskPunctualityService.Calculate(limitDateTime, finishDateTime).TimeDifference.Minutes);
		Assert.Equal(secondsDifference, TaskPunctualityService.Calculate(limitDateTime, finishDateTime).TimeDifference.Seconds);
	}

	[Theory]
	[InlineData(2, 0, 0, 0)]
	[InlineData(1, 1, 0, 0)]
	[InlineData(1, 0, 1, 0)]
	[InlineData(1, 0, 0, 1)]
	public void Punctuality_ReturnsDaysToGoWhenCurrentDateIsBeforeLimitDate(double daysDifference, double hoursDifference, double minutesDifference, double secondsDifference)
	{
		DateTimeOffset limitDateTime = DateTimeOffset.UtcNow
			.AddDays(daysDifference)
			.AddHours(hoursDifference)
			.AddMinutes(minutesDifference)
			.AddSeconds(secondsDifference);
		
		Assert.True(TaskPunctualityService.Calculate(limitDateTime, null).Type == PunctualityType.TimeRemaining);
		Assert.Equal(daysDifference, TaskPunctualityService.Calculate(limitDateTime, null).TimeDifference.Days);
		Assert.Equal(hoursDifference, TaskPunctualityService.Calculate(limitDateTime, null).TimeDifference.Hours);
		Assert.Equal(minutesDifference, TaskPunctualityService.Calculate(limitDateTime, null).TimeDifference.Minutes);
		Assert.Equal(secondsDifference, TaskPunctualityService.Calculate(limitDateTime, null).TimeDifference.Seconds);
	}

	[Theory]
	[InlineData(1, 0, 0, 0)]
	[InlineData(0, 1, 0, 0)]
	[InlineData(0, 0, 1, 0)]
	[InlineData(0, 0, 0, 1)]
	[InlineData(0, 0, 0, 0)]
	public void Punctuality_ReturnsFinishTodayWhenLimitDateIsTodayOrWithin24Hours(double daysDifference,
		double hoursDifference, double minutesDifference = 0, double secondsDifference = 0)
	{
		DateTimeOffset limitDateTime = DateTimeOffset.UtcNow.AddDays(daysDifference)
			.AddHours(hoursDifference)
			.AddMinutes(minutesDifference)
			.AddSeconds(secondsDifference);
		
		Assert.True(TaskPunctualityService.Calculate(limitDateTime, null).Type == PunctualityType.FinishToday);
	}

	[Theory]
	[InlineData(0, 0, 0, -1)]
	[InlineData(0, 0, -1, 0)]
	[InlineData(0, -1, 0, 0)]
	[InlineData(-1, 0, 0, 0)]
	public void Punctuality_ReturnsDaysLateWhenCurrentDateIsAfterLimitDate(double daysDifference, double hoursDifference, double minutesDifference = 0, double secondsDifference = 0)
	{
		DateTimeOffset limitDateTime = DateTimeOffset.UtcNow
			.AddDays(daysDifference)
			.AddHours(hoursDifference)
			.AddMinutes(minutesDifference)
			.AddSeconds(secondsDifference);
		
		Assert.True(TaskPunctualityService.Calculate(limitDateTime, null).Type == PunctualityType.Late);
		Assert.Equal(daysDifference, TaskPunctualityService.Calculate(limitDateTime, null).TimeDifference.Days);
		Assert.Equal(hoursDifference, TaskPunctualityService.Calculate(limitDateTime, null).TimeDifference.Hours);
		Assert.Equal(minutesDifference, TaskPunctualityService.Calculate(limitDateTime, null).TimeDifference.Minutes);
		Assert.Equal(secondsDifference, TaskPunctualityService.Calculate(limitDateTime, null).TimeDifference.Seconds);
	}
}
