using GraphT.UseCases.GetTaskEnumsItems;

using SeedWork;

namespace GraphT.Controllers.GetTaskEnumsItems;

public interface IGetTaskEnumsItemsController : IControllerTOut<OutputDto> {}

public class Controller : IGetTaskEnumsItemsController
{
	private readonly IInputPort _inputPort;
	private readonly IOutputPort _outputPort;

	public Controller(IInputPort inputPort, IOutputPort outputPort)
	{
		_inputPort = inputPort;
		_outputPort = outputPort;
	}
    
	public async ValueTask<OutputDto> RunUseCase()
	{
		await _inputPort.Handle();
		return ((IPresenter<OutputDto>)_outputPort).Content;
	}
}

