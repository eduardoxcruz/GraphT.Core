using GraphT.Model.Services.Specifications;
using GraphT.Model.ValueObjects;

using SeedWork;

namespace GraphT.Model.Services;

public static class TimeSpendCalculatorService
{
	public static async ValueTask<TimeSpan> GetTimeSpend(
		Guid taskId, 
		Status newStatus, 
		DateTimeOffset newDateTime, 
		IUnitOfWork unitOfWork)
	{
		if (newStatus is Status.Created or Status.Backlog or Status.ReadyToStart or Status.InProgress) return TimeSpan.Zero;
		
		TaskLog? lastLog = (await unitOfWork.Repository<TaskLog>().FindAsync(new LastTaskLogSpecification(taskId)))
			.FirstOrDefault();

		if (lastLog is null) return TimeSpan.Zero;
		
		if (lastLog.Status is Status.Paused or Status.Dropped or Status.Completed) return TimeSpan.Zero;
		
		return newDateTime - lastLog.DateTime;
	}
}
