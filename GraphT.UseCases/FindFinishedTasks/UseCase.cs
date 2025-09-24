using GraphT.Model.Entities;
using GraphT.Model.Services.Repositories;

using SeedWork;

namespace GraphT.UseCases.FindFinishedTasks;

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
		PagedList<OldTodoTask> tasks = await _todoTaskRepository.FindTasksCompletedOrDropped(dto.PagingParams);
		
		await _outputPort.Handle(new OutputDto { Tasks = tasks });
	}
}

public class InputDto
{
	public PagingParams PagingParams { get; set; }
}

public class OutputDto
{
	public PagedList<OldTodoTask> Tasks { get; set; }
}
