using GraphT.UseCases.AddNewTask;

using SeedWork;

namespace GraphT.Controllers.AddNewTask;

public interface IAddNewTaskController : IControllerTOutTIn<OutputDto,InputDto>;

public class Controller : IAddNewTaskController
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
