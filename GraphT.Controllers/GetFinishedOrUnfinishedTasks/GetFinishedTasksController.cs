using GraphT.UseCases;
using GraphT.UseCases.GetFinishedAndUnfinishedTasks;

using SeedWork;

namespace GraphT.Controllers.GetFinishedOrUnfinishedTasks;

public interface IGetFinishedTasksController : IControllerTOutTIn<OnlyTodoTaskPagedListDto, GetTasksFromNameDto> {}

public class GetFinishedTasksController : IGetFinishedTasksController
{
	private readonly IGetFinishedTasksInputPort _inputPort;
	private readonly IGetFinishedTasksOutputPort _outputPort;

	public GetFinishedTasksController(IGetFinishedTasksInputPort inputPort, IGetFinishedTasksOutputPort outputPort)
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
