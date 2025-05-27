using GraphT.Model.Aggregates;
using GraphT.Model.Entities;
using GraphT.Model.Exceptions;
using GraphT.Model.Services.Specifications;

using SeedWork;

namespace GraphT.UseCases.FindTaskDownstreamsById;

public interface IInputPort : IPort<InputDto> { }

public interface IOutputPort : IPort<OutputDto> { }

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
		TaskIncludeDownstreamsSpecification specification = new(dto.Id);
		TaskAggregate? task = (await _unitOfWork.Repository<TaskAggregate>().FindAsync(specification)).FirstOrDefault();

		if (task is null) throw new TaskNotFoundException("Task not found", dto.Id);

		PagedList<TodoTask> downstreams = new(
			task.Downstreams.ToList(),
			task.Downstreams.Count,
			dto.PagingParams.PageNumber,
			dto.PagingParams.PageSize);

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
	public PagedList<TodoTask> Downstreams { get; set; }
}
