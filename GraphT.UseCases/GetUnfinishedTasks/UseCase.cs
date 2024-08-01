using GraphT.Model.Entities;
using GraphT.Model.Services.Specifications;

using SeedWork;

namespace GraphT.UseCases.GetUnfinishedTasks;

public interface IInputPort : IPort<GetTasksFromNameDto> { }

public interface IOutputPort : IPort<OnlyTodoTaskPagedListDto> { }

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
		PagedList<TodoTask> tasks = await _unitOfWork.Repository<TodoTask>().FindAsync(specification);
		OnlyTodoTaskPagedListDto onlyTodoTaskPagedListDto = new(tasks);
		await _outputPort.Handle(onlyTodoTaskPagedListDto);
	}
}
