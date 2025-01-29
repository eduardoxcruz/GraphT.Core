using GraphT.UseCases.DeleteTask;

using SeedWork;

namespace GraphT.Controllers.DeleteTask;

public interface IDeleteTaskController : IControllerTIn<InputDto> {}

public class Controller : IDeleteTaskController
{
	private readonly IInputPort _inputPort;

	public Controller(IInputPort inputPort)
	{
		_inputPort = inputPort;
	}
    
	public async ValueTask RunUseCase(InputDto inputDto)
	{
		await _inputPort.Handle(inputDto);
	}
}

