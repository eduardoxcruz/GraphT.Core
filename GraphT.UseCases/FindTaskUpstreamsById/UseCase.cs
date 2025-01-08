using GraphT.Model.Aggregates;
using GraphT.Model.Services.Specifications;

using SeedWork;

namespace GraphT.UseCases.FindTaskUpstreamsById;

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
		FindUpstreamsByTaskIdSpecification specification = new(dto.Id, dto.PagingParams);
		PagedList<TaskAggregate> entitiesFromDb = await _unitOfWork.Repository<TaskAggregate>().FindAsync(specification);
		PagedList<TaskIdAndName> upstreams = new(
			entitiesFromDb.Select(TaskIdAndName.MapFrom).ToList(),
			entitiesFromDb.TotalCount,
			entitiesFromDb.CurrentPage,
			entitiesFromDb.PageSize);
		
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
	public PagedList<TaskIdAndName> Upstreams { get; set; }
}

