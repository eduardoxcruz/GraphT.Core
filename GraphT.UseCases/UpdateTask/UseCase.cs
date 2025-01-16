using GraphT.Model.Aggregates;
using GraphT.Model.Exceptions;
using GraphT.Model.Services;
using GraphT.Model.Services.Specifications;
using GraphT.Model.ValueObjects;

using SeedWork;

namespace GraphT.UseCases.UpdateTask;

public interface IInputPort : IPort<InputDto> { }

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

	public async ValueTask Handle(InputDto dto)
	{
		if (dto.Id is null) throw new ArgumentException("Task id cannot be null", nameof(dto.Id));
		
		TodoTask? task = await _unitOfWork.Repository<TodoTask>().FindByIdAsync(dto.Id!);
		
		if (task is null) throw new TaskNotFoundException("Task not found", dto.Id.Value);

		task.Name = dto.Name ?? task.Name;
		task.IsFun = dto.IsFun ?? task.IsFun;
		task.IsProductive = dto.IsProductive ?? task.IsProductive;
		task.Complexity = dto.Complexity ?? task.Complexity;
		task.Priority = dto.Priority ?? task.Priority;
		
		if (dto.Status is not null)
		{
			DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
			TaskLog? lastLog = (await _unitOfWork.Repository<TaskLog>().FindAsync(new TaskLastLogSpecification(dto.Id.Value))).FirstOrDefault() ?? 
				new TaskLog(Guid.Empty, dateTimeOffset, Status.Created, TimeSpan.Zero);
			(string, TimeSpan) timeSpend = TimeSpendCalculatorService.GetTimeSpend(dto.Id.Value, dto.Status.Value, dateTimeOffset, lastLog);
			task.Status = dto.Status.Value;
			task.SetTimeSpend(timeSpend.Item1);
			TaskLog taskLog = new(task.Id, dateTimeOffset, task.Status, timeSpend.Item2);
			
			await _unitOfWork.Repository<TaskLog>().AddAsync(taskLog);
		}
		
		if (dto.StartDateTime.HasValue) task.SetStartDate(dto.StartDateTime.Value);

		if (dto.FinishDateTime.HasValue) task.SetFinishDate(dto.FinishDateTime.Value);
		
		if (dto.LimitDateTime.HasValue) task.SetLimitDate(dto.LimitDateTime.Value);
		
		await _unitOfWork.Repository<TodoTask>().UpdateAsync(task);
		await _unitOfWork.SaveChangesAsync();
		await _outputPort.Handle();
	}
}

public class InputDto
{
	public Guid? Id { get; set; }
	public string? Name { get; set; }
	public Status? Status { get; set; }
	public bool? IsFun { get; set; }
	public bool? IsProductive { get; set; }
	public Complexity? Complexity { get; set; }
	public Priority? Priority { get; set; }
	public DateTimeOffset? StartDateTime { get; set; }
	public DateTimeOffset? FinishDateTime { get; set; }
	public DateTimeOffset? LimitDateTime { get; set; }
}
