using GraphT.Model.Services.Specifications;
using GraphT.Model.ValueObjects;

using SeedWork;

namespace GraphT.Model.Services;

public static class TimeSpendCalculatorService
{
	public static async ValueTask<string> GetTimeSpendString(
		Guid taskId, 
		Status newStatus, 
		DateTimeOffset newDateTime, 
		IUnitOfWork unitOfWork)
	{
		string zeroString = $"\u23f0 0 day(s) - 0 hours - 0 minutes";
		
		if (newStatus is Status.Created or Status.Backlog or Status.ReadyToStart or Status.InProgress) return zeroString;
		
		TaskLog? lastLog = (await unitOfWork.Repository<TaskLog>().FindAsync(new LastTaskLogSpecification(taskId)))
			.FirstOrDefault();
		
		if (lastLog is null) return zeroString;
		
		if (lastLog.Status is Status.Paused or Status.Dropped or Status.Completed) return zeroString;
		
		TimeSpan timeSpend = newDateTime - lastLog.DateTime;
		
		if (timeSpend.Ticks < 1) return zeroString;
		
		string emoji = timeSpend.TotalHours > 1 ? "\u23f0" : "\u26a1";
		
		return $"{emoji} {timeSpend.Days} day(s) - {timeSpend.Hours} hours - {timeSpend.Minutes} minutes";
	}
}
