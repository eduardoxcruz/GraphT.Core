using GraphT.Model.Aggregates;
using GraphT.Model.Exceptions;
using GraphT.Model.Services;
using GraphT.Model.ValueObjects;
using GraphT.UseCases.DTOs;

using SeedWork;

namespace GraphT.UseCases.UpdateTask;

public interface IInputPort : IPort<TaskInfo> { }

public interface IOutputPort : IPort { }

public class UseCase : IInputPort
{
	private readonly IOutputPort _outputPort;
	private readonly IUnitOfWork _unitOfWork;

	public UseCase(IOutputPort outputPort, IUnitOfWork unitOfWork)
	{
		_outputPort = outputPort;
		_unitOfWork = unitOfWork;
	}

	public async ValueTask Handle(TaskInfo dto)
	{
		if (dto.Id is null) throw new ArgumentException("Task id cannot be null", nameof(dto.Id));
		
		TaskAggregate? task = await _unitOfWork.Repository<TaskAggregate>().FindByIdAsync(dto.Id!);
		
		if (task is null) throw new TaskNotFoundException("Task not found", (Guid)dto.Id);

		task.Name = dto.Name ?? task.Name;
		task.IsFun = dto.IsFun ?? task.IsFun;
		task.IsProductive = dto.IsProductive ?? task.IsProductive;
		task.Complexity = dto.Complexity ?? task.Complexity;
		task.Priority = dto.Priority ?? task.Priority;
		
		if (dto.Status is not null)
		{
			DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
			(string, TimeSpan) timeSpend = await TimeSpendCalculatorService.GetTimeSpend((Guid)dto.Id, (Status)dto.Status, dateTimeOffset, _unitOfWork);
			task.Status = (Status)dto.Status;
			task.SetTimeSpend(timeSpend.Item1);
			TaskLog taskLog = new(task.Id, dateTimeOffset, task.Status, timeSpend.Item2);
			
			await _unitOfWork.Repository<TaskLog>().AddAsync(taskLog);
		}
		
		if (dto.StartDateTime.HasValue) task.SetStartDate(dto.StartDateTime.Value);

		if (dto.FinishDateTime.HasValue) task.SetFinishDate(dto.FinishDateTime.Value);
		
		if (dto.LimitDateTime.HasValue) task.SetLimitDate(dto.LimitDateTime.Value);
		
		await _unitOfWork.Repository<TaskAggregate>().UpdateAsync(task);
		await _unitOfWork.SaveChangesAsync();
		await _outputPort.Handle();
	}
}
