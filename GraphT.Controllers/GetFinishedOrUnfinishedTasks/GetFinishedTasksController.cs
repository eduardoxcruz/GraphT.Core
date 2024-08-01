using GraphT.UseCases;
using GraphT.UseCases.GetFinishedTasks;

using SeedWork;

namespace GraphT.Controllers.GetFinishedOrUnfinishedTasks;

public interface IGetFinishedTasksController : IControllerTOutTIn<OnlyTodoTaskPagedListDto, GetTasksFromNameDto> {}

public class GetFinishedTasksController : IGetFinishedTasksController
{
	private readonly IInputPort _inputPort;
	private readonly IOutputPort _outputPort;

	public GetFinishedTasksController(IInputPort inputPort, IOutputPort outputPort)
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
