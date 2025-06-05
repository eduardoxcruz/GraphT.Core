using GraphT.UseCases.AddUpstream;

using SeedWork;

namespace GraphT.Controllers.AddUpstream;

public interface IAddUpstreamController : IControllerTIn<InputDto> {}

public class Controller : IAddUpstreamController
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

