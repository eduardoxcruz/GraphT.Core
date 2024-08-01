using GraphT.Model.Entities;
using GraphT.Model.Services.Specifications;

using SeedWork;

namespace GraphT.UseCases.GetFinishedAndUnfinishedTasks;

public interface IGetUnfinishedTasksInputPort : IPort<GetTasksFromNameDto> { }

public interface IGetUnfinishedTasksOutputPort : IPort<OnlyTodoTaskPagedListDto> { }

internal class GetUnfinishedTasks : IGetUnfinishedTasksInputPort
{
	private readonly IGetUnfinishedTasksOutputPort _outputPort;
	private readonly IUnitOfWork _unitOfWork;

	public GetUnfinishedTasks(IGetUnfinishedTasksOutputPort outputPort, IUnitOfWork unitOfWork)
	{
		_outputPort = outputPort;
		_unitOfWork = unitOfWork;
	}

	public async ValueTask Handle(GetTasksFromNameDto dto)
	{
		UnfinishedTasksSpecification specification = new(dto.TaskName, dto.PagingParams);
		PagedList<TodoTask> tasks = await _unitOfWork.Repository<TodoTask>().FindAsync(specification);
		OnlyTodoTaskPagedListDto onlyTodoTaskPagedListDto = new(tasks);
		await _outputPort.Handle(onlyTodoTaskPagedListDto);
	}
}
