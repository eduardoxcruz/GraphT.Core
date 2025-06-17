using GraphT.UseCases.FindInProgressTasks;

using SeedWork;

namespace GraphT.Controllers.FindInProgressTasks;

public interface IFindInProgressTasksController : IControllerTOutTIn<OutputDto, InputDto> {}

public class Controller : IFindInProgressTasksController
{
	private readonly IInputPort _inputPort;
	private readonly IOutputPort _outputPort;

	public Controller(IInputPort inputPort, IOutputPort outputPort)
	{
		_inputPort = inputPort;
		_outputPort = outputPort;
	}
    
	public async ValueTask<OutputDto> RunUseCase(InputDto inputDto)
	{
		await _inputPort.Handle(inputDto);
		return ((IPresenter<OutputDto>)_outputPort).Content;
	}
}

