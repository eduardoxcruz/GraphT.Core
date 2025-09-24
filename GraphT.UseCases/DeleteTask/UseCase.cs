using GraphT.Model.Entities;
using GraphT.Model.Exceptions;
using GraphT.Model.Services.Repositories;

using SeedWork;

namespace GraphT.UseCases.DeleteTask;

public interface IInputPort : IPort<InputDto> { }

public interface IOutputPort : IPort { }

public class UseCase : IInputPort
{
	private readonly IOutputPort _outputPort;
	private readonly ITodoTaskRepository _todoTaskRepository;
	private readonly IUnitOfWork _unitOfWork;

	public UseCase(
		IOutputPort outputPort, 
		ITodoTaskRepository todoTaskRepository,
		IUnitOfWork unitOfWork)
	{
		_outputPort = outputPort;
		_todoTaskRepository = todoTaskRepository;
		_unitOfWork = unitOfWork;
	}

	public async ValueTask Handle(InputDto dto)
	{
		OldTodoTask? task = (await _todoTaskRepository.FindByIdAsync(dto.Id));
		
		if (task is null)
		{
			throw new TaskNotFoundException("Task not found.", dto.Id);
		}
		
		await _todoTaskRepository.RemoveAsync(task);
		await _unitOfWork.SaveChangesAsync();
		await _outputPort.Handle();
	}
}

public class InputDto
{
	public Guid Id { get; set; }
}

