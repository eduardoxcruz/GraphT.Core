using GraphT.UseCases;
using GraphT.UseCases.GetUnfinishedTasks;

using SeedWork;

namespace GraphT.Controllers.GetFinishedOrUnfinishedTasks;

public interface IGetUnfinishedTasksController : IControllerTOutTIn<OnlyTodoTaskPagedListDto, GetTasksFromNameDto> {}

public class GetUnfinishedTasksController : IGetUnfinishedTasksController
{
	private readonly IInputPort _inputPort;
	private readonly IOutputPort _outputPort;

	public GetUnfinishedTasksController(IInputPort inputPort, IOutputPort outputPort)
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
