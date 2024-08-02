using GraphT.UseCases;
using GraphT.UseCases.GetFinishedTasks;

using SeedWork;

namespace GraphT.Controllers.GetFinishedOrUnfinishedTasks;

public interface IGetFinishedTasksController : IControllerTOutTIn<TaskIdAndNamePagedListDto, GetTasksFromNameDto> {}

public class GetFinishedTasksController : IGetFinishedTasksController
{
	private readonly IInputPort _inputPort;
	private readonly IOutputPort _outputPort;

	public GetFinishedTasksController(IInputPort inputPort, IOutputPort outputPort)
	{
		_inputPort = inputPort;
		_outputPort = outputPort;
	}
    
	public async ValueTask<TaskIdAndNamePagedListDto> RunUseCase(GetTasksFromNameDto inputDto)
	{
		await _inputPort.Handle(inputDto);
		return ((IPresenter<TaskIdAndNamePagedListDto>)_outputPort).Content;
	}
}
