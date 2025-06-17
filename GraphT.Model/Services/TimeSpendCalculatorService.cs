using GraphT.Model.ValueObjects;

using SeedWork;

namespace GraphT.Model.Services;

public static class TimeSpendCalculatorService
{
	public static (string, TimeSpan) GetTimeSpend(
		Status newStatus, 
		DateTimeOffset newDateTime, 
		TaskLog lastLog)
	{
		TimeSpan timeSpend = lastLog.TimeSpentOnTask!.Value;
		
		if ((lastLog.Status is Status.Doing) && (newStatus is not Status.Doing))
		{
			timeSpend = lastLog.TimeSpentOnTask!.Value + (newDateTime - lastLog.DateTime);
		}
		
		string emoji = timeSpend.TotalMinutes > 59 ? "\u23f0" : "\u26a1";
		
		return ($"{emoji} {timeSpend.Days} day(s) - {timeSpend.Hours} hours - {timeSpend.Minutes} minutes", timeSpend);
	}
}
