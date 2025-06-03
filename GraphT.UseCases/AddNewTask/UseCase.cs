using GraphT.Model.Aggregates;
using GraphT.Model.Entities;
using GraphT.Model.Services.Repositories;
using GraphT.Model.ValueObjects;

using SeedWork;

namespace GraphT.UseCases.AddNewTask;

public interface IInputPort : IPort<InputDto> { }

public interface IOutputPort : IPort<OutputDto> { }

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
		Guid id = Guid.NewGuid();
		
		if ((dto.Id.HasValue) && !(await _todoTaskRepository.ContainsAsync(dto.Id.Value)))
		{
			id = dto.Id.Value;
		}
		
		TodoTask task = new(dto.Name ?? "New Task", dto.Status, dto.IsFun, dto.IsProductive, dto.Complexity, dto.Priority, id);
		
		if (dto.StartDateTime.HasValue) task.SetStartDate(dto.StartDateTime.Value);

		if (dto.FinishDateTime.HasValue) task.SetFinishDate(dto.FinishDateTime.Value);
		
		if (dto.LimitDateTime.HasValue) task.SetLimitDate(dto.LimitDateTime.Value);
		
		TaskLog createdTaskLog = new(id, DateTimeOffset.UtcNow, Status.Created, TimeSpan.Zero);
		
		if (dto.Status is not null)
		{
			TaskLog taskLog = new(id, DateTimeOffset.UtcNow.AddSeconds(2), dto.Status.Value, TimeSpan.Zero);
			await _taskLogRepository.AddAsync(taskLog);
		}
		
		await _taskLogRepository.AddAsync(createdTaskLog);
		await _todoTaskRepository.AddAsync(task);
		await _unitOfWork.SaveChangesAsync();
		await _outputPort.Handle(new OutputDto(id));
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

public class OutputDto
{
	public Guid Id { get; set; }

	public OutputDto(Guid id)
	{
		Id = id;
	}
}
