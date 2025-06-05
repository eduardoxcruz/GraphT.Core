using GraphT.Model.Entities;
using GraphT.Model.Services.Repositories;

using SeedWork;

namespace GraphT.UseCases.GetTasksOrderedByCreationDateDesc;

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
		PagedList<TodoTask> tasks = await _todoTaskRepository.GetTasksOrderedByCreationDateDescAsync(dto.PagingParams);
		
		await _outputPort.Handle(new OutputDto() { Tasks = tasks });
	}
}

public record struct InputDto()
{
	public PagingParams PagingParams { get; set; }
}

public record struct OutputDto()
{
	public PagedList<TodoTask> Tasks { get; set; }
}

