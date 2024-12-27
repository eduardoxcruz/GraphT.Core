using GraphT.UseCases.UpdateTask;

using SeedWork;

namespace GraphT.Controllers.UpdateTask;

public interface IUpdateTaskController : IControllerTIn<InputDto>;

public class UpdateTaskController : IUpdateTaskController
{
	private readonly IInputPort _inputPort;

	UpdateTaskController(IInputPort inputPort)
	{
		_inputPort = inputPort;
	}

	public async ValueTask RunUseCase(InputDto inputDto)
	{
		await _inputPort.Handle(inputDto);
	}
}
