using GraphT.Model.Services.Specifications;
using GraphT.Model.ValueObjects;

using SeedWork;

namespace GraphT.Model.Services;

public static class TimeSpendCalculatorService
{
	public static async ValueTask<(string, TimeSpan)> GetTimeSpend(
		Guid taskId, 
		Status newStatus, 
		DateTimeOffset newDateTime, 
		IUnitOfWork unitOfWork)
	{
		//Generic last log when none found
		TaskLog? lastLog = (await unitOfWork
			.Repository<TaskLog>()
			.FindAsync(new LastTaskLogSpecification(taskId)))
			.FirstOrDefault() ?? new TaskLog(Guid.Empty, newDateTime, Status.Created, TimeSpan.Zero);
		TimeSpan timeSpend = lastLog.TimeSpentOnTask!.Value;
		
		if ((lastLog.Status is Status.InProgress) && (newStatus is not Status.InProgress))
		{
			timeSpend = lastLog.TimeSpentOnTask!.Value + (newDateTime - lastLog.DateTime);
		}
		
		string emoji = timeSpend.TotalMinutes > 59 ? "\u23f0" : "\u26a1";
		
		return ($"{emoji} {timeSpend.Days} day(s) - {timeSpend.Hours} hours - {timeSpend.Minutes} minutes", timeSpend);
	}
}
