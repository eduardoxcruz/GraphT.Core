using GraphT.UseCases.RemoveUpstream;

using SeedWork;

namespace GraphT.Controllers.RemoveUpstream;

public interface IRemoveUpstreamController : IControllerTIn<InputDto> {}

public class Controller : IRemoveUpstreamController
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

