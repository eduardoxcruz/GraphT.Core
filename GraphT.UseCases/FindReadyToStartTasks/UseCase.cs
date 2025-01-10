using GraphT.Model.Aggregates;
using GraphT.Model.Services.Specifications;
using GraphT.Model.ValueObjects;

using SeedWork;

namespace GraphT.UseCases.FindReadyToStartTasks;

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
		TasksWhereStatusIsReadyToStartSpecification specification = new(dto.PagingParams);
		PagedList<TaskAggregate> tasks = await _unitOfWork.Repository<TaskAggregate>().FindAsync(specification);
		
		await _outputPort.Handle(new OutputDto { Tasks = tasks });
	}
}

public class InputDto
{
	public PagingParams PagingParams { get; set; }
}

public class OutputDto
{
	public PagedList<TaskAggregate> Tasks { get; set; }
}

