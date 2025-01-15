using GraphT.Model.Aggregates;
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
		TodoTask? task = await _unitOfWork.Repository<TodoTask>().FindByIdAsync(dto.Id);

		if (task is null) throw new TaskNotFoundException("Task not found", dto.Id);

		TaskIncludeDownstreamsSpecification specification = new(dto.Id);
		task = (await _unitOfWork.Repository<TodoTask>().FindAsync(specification)).First();
		PagedList<TaskIdAndName> downstreams = new(
			task.Downstreams.Select(TaskIdAndName.MapFrom).ToList(),
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
	public PagedList<TaskIdAndName> Downstreams { get; set; }
}
