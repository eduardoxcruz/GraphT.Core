using GraphT.Model.Aggregates;
using GraphT.Model.Services.Specifications;

using SeedWork;

namespace GraphT.UseCases.FindTasksWithoutUpstreams;

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
		TasksWithoutUpstreamsSpecification specification = new(dto.PagingParams);
		PagedList<TaskAggregate> tasks = await _unitOfWork.Repository<TaskAggregate>().FindAsync(specification);
		
		await _outputPort.Handle(new OutputDto(tasks));
	}
}

public record struct InputDto(PagingParams PagingParams) {}

public record struct OutputDto(PagedList<TaskAggregate> Tasks) {}

