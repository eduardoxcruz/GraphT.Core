using GraphT.Model.Aggregates;
using GraphT.Model.Services.Specifications;

using SeedWork;

namespace GraphT.UseCases.GetUnfinishedTasks;

public interface IInputPort : IPort<GetTasksFromNameDto> { }

public interface IOutputPort : IPort<TaskIdAndNamePagedListDto> { }

internal class UseCase : IInputPort
{
	private readonly IOutputPort _outputPort;
	private readonly IUnitOfWork _unitOfWork;

	public UseCase(IOutputPort outputPort, IUnitOfWork unitOfWork)
	{
		_outputPort = outputPort;
		_unitOfWork = unitOfWork;
	}

	public async ValueTask Handle(GetTasksFromNameDto dto)
	{
		UnfinishedTasksSpecification specification = new(dto.TaskName, dto.PagingParams);
		PagedList<TaskAggregate> tasksFromDb = await _unitOfWork.Repository<TaskAggregate>().FindAsync(specification);
		PagedList<TaskIdAndName> tasks = new(
			tasksFromDb.Select(TaskIdAndName.MapFrom).ToList(), 
			tasksFromDb.TotalCount, 
			tasksFromDb.CurrentPage,
			tasksFromDb.PageSize);
		TaskIdAndNamePagedListDto outDto = new(tasks);
		await _outputPort.Handle(outDto);
	}
}
