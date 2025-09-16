using GraphT.Model.Entities;
using GraphT.Model.Services.Repositories;

using SeedWork;

namespace GraphT.UseCases.FindTasksWithoutUpstreams;

public interface IInputPort : IPort<InputDto> { }

public interface IOutputPort : IPort<OutputDto> { }

public class UseCase : IInputPort
{
	private readonly IOutputPort _outputPort;
	private readonly ITaskUpstreamsRepository _taskUpstreamsRepository;

	public UseCase(IOutputPort outputPort, ITaskUpstreamsRepository taskUpstreamsRepository)
	{
		_outputPort = outputPort;
		_taskUpstreamsRepository = taskUpstreamsRepository;
	}

	public async ValueTask Handle(InputDto dto)
	{
		PagedList<OldTodoTask> tasks = await _taskUpstreamsRepository.FindTasksWithoutUpstreams(dto.PagingParams);
		
		await _outputPort.Handle(new OutputDto(tasks));
	}
}

public record struct InputDto(PagingParams PagingParams) {}

public record struct OutputDto(PagedList<OldTodoTask> Tasks) {}
