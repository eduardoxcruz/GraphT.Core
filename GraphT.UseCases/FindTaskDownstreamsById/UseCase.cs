using GraphT.Model.Entities;
using GraphT.Model.Exceptions;
using GraphT.Model.Services.Repositories;

using SeedWork;

namespace GraphT.UseCases.FindTaskDownstreamsById;

public interface IInputPort : IPort<InputDto> { }

public interface IOutputPort : IPort<OutputDto> { }

public class UseCase : IInputPort
{
    private readonly IOutputPort _outputPort;
    private readonly ITodoTaskRepository _todoTaskRepository;
    private readonly ITaskDownstreamsRepository _taskDownstreamsRepository;

    public UseCase(IOutputPort outputPort, ITodoTaskRepository todoTaskRepository, ITaskDownstreamsRepository taskDownstreamsRepository)
    {
        _outputPort = outputPort;
        _todoTaskRepository = todoTaskRepository;
        _taskDownstreamsRepository = taskDownstreamsRepository;
    }

    public async ValueTask Handle(InputDto dto)
    {
        if (!await _todoTaskRepository.ContainsAsync(dto.Id))
            throw new TaskNotFoundException("Task not found", dto.Id);

        PagedList<OldTodoTask> downstreams = await _taskDownstreamsRepository.FindTaskDownstreamsById(dto.Id);

        await _outputPort.Handle(new OutputDto() { Downstreams = downstreams });
    }
}

public class InputDto
{
    public PagingParams PagingParams { get; set; }
    public Guid Id { get; set; }
}

public class OutputDto
{
    public PagedList<OldTodoTask> Downstreams { get; set; }
}
