using GraphT.Model.Aggregates;
using GraphT.Model.Exceptions;
using GraphT.Model.Services.Specifications;

using SeedWork;

namespace GraphT.UseCases.FindTaskLifeAreasById;

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
		TaskIncludeLifeAreasSpecification specification = new(dto.Id);
		TodoTask? task = (await _unitOfWork.Repository<TodoTask>().FindAsync(specification)).FirstOrDefault();

		if (task is null) throw new TaskNotFoundException("Task not found", dto.Id);
		
		await _outputPort.Handle(new OutputDto() { LifeAreas = new PagedList<LifeArea>(
			task.LifeAreas.ToList(),
			task.LifeAreas.Count,
			dto.PagingParams.PageNumber,
			dto.PagingParams.PageSize) });
	}
}

public class InputDto
{
	public PagingParams PagingParams { get; set; }
	public Guid Id { get; set; }
}

public class OutputDto
{
	public PagedList<LifeArea> LifeAreas { get; set; }
}
