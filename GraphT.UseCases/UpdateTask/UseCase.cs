using GraphT.Model.Entities;
using GraphT.Model.Exceptions;
using GraphT.Model.Services;
using GraphT.Model.Services.Repositories;
using GraphT.Model.ValueObjects;

using SeedWork;

namespace GraphT.UseCases.UpdateTask;

public interface IInputPort : IPort<InputDto> { }

public interface IOutputPort : IPort { }

public class UseCase : IInputPort
{
	private readonly IOutputPort _outputPort;
	private readonly ITodoTaskRepository _todoTaskRepository;
	private readonly ITaskLogRepository _taskLogRepository;
	private readonly IUnitOfWork _unitOfWork;

	public UseCase(
		IOutputPort outputPort, 
		ITodoTaskRepository todoTaskRepository,
		ITaskLogRepository taskLogRepository,
		IUnitOfWork unitOfWork)
	{
		_outputPort = outputPort;
		_todoTaskRepository = todoTaskRepository;
		_taskLogRepository = taskLogRepository;
		_unitOfWork = unitOfWork;
	}

	public async ValueTask Handle(InputDto dto)
	{
		if (dto.Id is null) throw new ArgumentException("Task id cannot be null", nameof(dto.Id));
		
		TodoTask? task = await _todoTaskRepository.FindByIdAsync(dto.Id.Value);
		
		if (task is null) throw new TaskNotFoundException("Task not found", dto.Id.Value);

		task.Name = dto.Name ?? task.Name;
		task.IsFun = dto.IsFun ?? task.IsFun;
		task.IsProductive = dto.IsProductive ?? task.IsProductive;
		task.Complexity = dto.Complexity ?? task.Complexity;
		task.Priority = dto.Priority ?? task.Priority;
		
		if (dto.Status is not null)
		{
			DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
			OldTaskLog? lastLog = (await _taskLogRepository.FindTaskLastLog(dto.Id.Value) ?? 
				new OldTaskLog(Guid.Empty, dateTimeOffset, OldStatus.Created, TimeSpan.Zero));
			(string, TimeSpan) timeSpend = TimeSpendCalculatorService.GetTimeSpend(dto.Status.Value, dateTimeOffset, lastLog);
			task.OldStatus = dto.Status.Value;
			task.SetTimeSpend(timeSpend.Item1);
			OldTaskLog oldTaskLog = new(task.Id, dateTimeOffset, task.OldStatus, timeSpend.Item2);
			
			await _taskLogRepository.AddAsync(oldTaskLog);
		}
		
		if (dto.StartDateTime.HasValue) task.SetStartDate(dto.StartDateTime.Value);

		if (dto.FinishDateTime.HasValue) task.SetFinishDate(dto.FinishDateTime.Value);
		
		if (dto.LimitDateTime.HasValue) task.SetLimitDate(dto.LimitDateTime.Value);
		
		await _todoTaskRepository.UpdateAsync(task);
		await _unitOfWork.SaveChangesAsync();
		await _outputPort.Handle();
	}
}

public class InputDto
{
	public Guid? Id { get; set; }
	public string? Name { get; set; }
	public OldStatus? Status { get; set; }
	public bool? IsFun { get; set; }
	public bool? IsProductive { get; set; }
	public OldComplexity? Complexity { get; set; }
	public OldPriority? Priority { get; set; }
	public DateTimeOffset? StartDateTime { get; set; }
	public DateTimeOffset? FinishDateTime { get; set; }
	public DateTimeOffset? LimitDateTime { get; set; }
}
