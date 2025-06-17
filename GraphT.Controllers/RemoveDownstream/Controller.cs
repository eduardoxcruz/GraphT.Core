using GraphT.UseCases.RemoveDownstream;

using SeedWork;

namespace GraphT.Controllers.RemoveDownstream;

public interface IRemoveDownstreamController : IControllerTIn<InputDto> {}

public class Controller : IRemoveDownstreamController
{
	private readonly IInputPort _inputPort;
	private readonly IOutputPort _outputPort;

	public Controller(IInputPort inputPort, IOutputPort outputPort)
	{
		_inputPort = inputPort;
		_outputPort = outputPort;
	}
    
	public async ValueTask RunUseCase(InputDto inputDto)
	{
		await _inputPort.Handle(inputDto);
	}
}

