using GraphT.Model.Entities;
using GraphT.Model.Exceptions;
using GraphT.Model.Services.Repositories;

using SeedWork;

namespace GraphT.UseCases.FindTaskById;

public interface IInputPort : IPort<InputDto> { }

public interface IOutputPort : IPort<OutputDto> { }

public class UseCase : IInputPort
{
	private readonly IOutputPort _outputPort;
	private readonly ITodoTaskRepository _todoTaskRepository;

	public UseCase(IOutputPort outputPort, ITodoTaskRepository todoTaskRepository)
	{
		_outputPort = outputPort;
		_todoTaskRepository = todoTaskRepository;
	}

	public async ValueTask Handle(InputDto dto)
	{
		OldTodoTask? task = await _todoTaskRepository.FindByIdAsync(dto.Id);

		if (task is null) throw new TaskNotFoundException(dto.Id);
		
		await _outputPort.Handle(new OutputDto() { Task = task });
	}
}

public class InputDto
{
	public Guid Id { get; set; }
}

public class OutputDto
{
	public OldTodoTask Task { get; set; }
}
