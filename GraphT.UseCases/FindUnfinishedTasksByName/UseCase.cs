using GraphT.Model.Aggregates;
using GraphT.Model.Services.Specifications;

using SeedWork;

namespace GraphT.UseCases.FindUnfinishedTasksByName;

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
		UnfinishedTasksSpecification specification = new(dto.TaskName, dto.PagingParams);
		PagedList<TaskAggregate> tasksFromDb = await _unitOfWork.Repository<TaskAggregate>().FindAsync(specification);
		PagedList<TaskIdAndName> tasks = new(
			tasksFromDb.Select(TaskIdAndName.MapFrom).ToList(), 
			tasksFromDb.TotalCount, 
			tasksFromDb.CurrentPage,
			tasksFromDb.PageSize);
		OutputDto outDto = new(){ Tasks = tasks };
		await _outputPort.Handle(outDto);
	}
}

public class InputDto
{
	public PagingParams PagingParams { get; set; }
	public string? TaskName { get; set; }
}

public class OutputDto
{
	public PagedList<TaskIdAndName> Tasks { get; set; }
}
