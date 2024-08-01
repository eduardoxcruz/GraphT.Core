using GraphT.Model.Entities;
using GraphT.Model.Services.Specifications;

using SeedWork;

namespace GraphT.UseCases.GetFinishedAndUnfinishedTasks;

public interface IGetFinishedTasksInputPort : IPort<GetTasksFromNameDto> { }

public interface IGetFinishedTasksOutputPort : IPort<OnlyTodoTaskPagedListDto> { }

internal class GetFinishedTasks : IGetFinishedTasksInputPort
{
	private readonly IGetFinishedTasksOutputPort _outputPort;
	private readonly IUnitOfWork _unitOfWork;

	public GetFinishedTasks(IGetFinishedTasksOutputPort outputPort, IUnitOfWork unitOfWork)
	{
		_outputPort = outputPort;
		_unitOfWork = unitOfWork;
	}

	public async ValueTask Handle(GetTasksFromNameDto dto)
	{
		FinishedTasksSpecification specification = new(dto.TaskName, dto.PagingParams);
		PagedList<TodoTask> tasks = await _unitOfWork.Repository<TodoTask>().FindAsync(specification);
		OnlyTodoTaskPagedListDto onlyTodoTaskPagedListDto = new(tasks);
		await _outputPort.Handle(onlyTodoTaskPagedListDto);
	}
}
