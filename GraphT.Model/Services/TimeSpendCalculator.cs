using GraphT.Model.Services.Specifications;
using GraphT.Model.ValueObjects;

using SeedWork;

namespace GraphT.Model.Services;

public class TimeSpendCalculator
{
	private readonly IUnitOfWork _unitOfWork;

	public TimeSpendCalculator(IUnitOfWork unitOfWork)
	{
		_unitOfWork = unitOfWork;
	}

	public async ValueTask<TimeSpan?> GetTimeSpend(Guid taskId, Status newStatus, DateTimeOffset newDateTime)
	{
		if (newStatus is Status.Created or Status.Backlog or Status.ReadyToStart or Status.InProgress) return null;
		
		TaskLog? lastLog = (await _unitOfWork.Repository<TaskLog>().FindAsync(new LastTaskLogSpecification(taskId)))
			.FirstOrDefault();

		if (lastLog is null) return null;
		
		if (lastLog.Status is Status.Paused or Status.Dropped or Status.Completed) return null;
		
		return newDateTime - lastLog.DateTime;
	}
}
