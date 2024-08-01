using GraphT.UseCases;
using GraphT.UseCases.GetFinishedAndUnfinishedTasks;

using SeedWork;

namespace GraphT.Controllers.GetFinishedOrUnfinishedTasks;

public interface IGetUnfinishedTasksController : IControllerTOutTIn<OnlyTodoTaskPagedListDto, GetTasksFromNameDto> {}

public class GetUnfinishedTasksController : IGetUnfinishedTasksController
{
	private readonly IGetUnfinishedTasksInputPort _inputPort;
	private readonly IGetUnfinishedTasksOutputPort _outputPort;

	public GetUnfinishedTasksController(IGetUnfinishedTasksInputPort inputPort, IGetUnfinishedTasksOutputPort outputPort)
	{
		_inputPort = inputPort;
		_outputPort = outputPort;
	}
    
	public async ValueTask<OnlyTodoTaskPagedListDto> RunUseCase(GetTasksFromNameDto inputDto)
	{
		await _inputPort.Handle(inputDto);
		return ((IPresenter<OnlyTodoTaskPagedListDto>)_outputPort).Content;
	}
}
