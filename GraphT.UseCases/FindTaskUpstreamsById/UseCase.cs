using GraphT.Model.Entities;
using GraphT.Model.Exceptions;
using GraphT.Model.Services.Repositories;

using SeedWork;

namespace GraphT.UseCases.FindTaskUpstreamsById;

public interface IInputPort : IPort<InputDto> { }

public interface IOutputPort : IPort<OutputDto> { }

public class UseCase : IInputPort
{
	private readonly IOutputPort _outputPort;
	private readonly ITodoTaskRepository _todoTaskRepository;
	private readonly ITaskUpstreamsRepository _taskUpstreamsRepository;

	public UseCase(IOutputPort outputPort, ITodoTaskRepository todoTaskRepository, ITaskUpstreamsRepository taskUpstreamsRepository)
	{
		_outputPort = outputPort;
		_todoTaskRepository = todoTaskRepository;
		_taskUpstreamsRepository = taskUpstreamsRepository;
	}

	public async ValueTask Handle(InputDto dto)
	{
		if (!await _todoTaskRepository.ContainsAsync(dto.Id)) throw new TaskNotFoundException("Task not found", dto.Id);

		PagedList<OldTodoTask> upstreams = await _taskUpstreamsRepository.FindTaskUpstreamsById(dto.Id);

		await _outputPort.Handle(new OutputDto() { Upstreams = upstreams });
	}
}

public class InputDto
{
	public PagingParams PagingParams { get; set; }
	public Guid Id { get; set; }
}

public class OutputDto
{
	public PagedList<OldTodoTask> Upstreams { get; set; }
}
