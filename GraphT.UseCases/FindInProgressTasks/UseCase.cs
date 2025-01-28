using GraphT.Model.Aggregates;
using GraphT.Model.Services.Specifications;

using SeedWork;

namespace GraphT.UseCases.FindInProgressTasks;

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
		TasksWhereStatusIsInProgressSpecification specification = new(dto.PagingParams);
		PagedList<TodoTask> tasks = await _unitOfWork.Repository<TodoTask>().FindAsync(specification);
		
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
