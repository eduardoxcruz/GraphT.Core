using GraphT.Model.Entities;
using GraphT.Model.Services.Repositories;

using SeedWork;

namespace GraphT.UseCases.FindInProgressTasks;

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
		PagedList<TodoTask> tasks = await _todoTaskRepository.FindTasksInProgress(dto.PagingParams);
		
		await _outputPort.Handle(new OutputDto { Tasks = tasks });
	}
}

public class InputDto
{
	public PagingParams PagingParams { get; set; }
}

public class OutputDto
{
	public PagedList<TodoTask> Tasks { get; set; }
}
