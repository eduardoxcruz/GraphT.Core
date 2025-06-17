using GraphT.UseCases.AddDownstream;

using SeedWork;

namespace GraphT.Controllers.AddDownstream;

public interface IAddDownstreamController : IControllerTIn<InputDto> {}

public class Controller : IAddDownstreamController
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

