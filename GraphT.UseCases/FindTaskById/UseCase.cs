using GraphT.Model.Aggregates;
using GraphT.Model.Exceptions;
using GraphT.Model.Services.Specifications;

using SeedWork;

namespace GraphT.UseCases.FindTaskById;

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
		TaskAggregate? task = await _unitOfWork.Repository<TaskAggregate>().FindByIdAsync(dto.Id);

		if (task is null) throw new TaskNotFoundException(dto.Id);
		
		await _outputPort.Handle(new OutputDto() { Task = task });
	}
}

public class InputDto
{
	public Guid Id { get; set; }
}

public class OutputDto
{
	public TaskAggregate Task { get; set; }
}
