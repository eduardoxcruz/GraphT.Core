using GraphT.UseCases;
using GraphT.UseCases.GetUnfinishedTasks;

using SeedWork;

namespace GraphT.Controllers.GetFinishedOrUnfinishedTasks;

public interface IGetUnfinishedTasksController : IControllerTOutTIn<TaskIdAndNamePagedListDto, GetTasksFromNameDto> {}

public class GetUnfinishedTasksController : IGetUnfinishedTasksController
{
	private readonly IInputPort _inputPort;
	private readonly IOutputPort _outputPort;

	public GetUnfinishedTasksController(IInputPort inputPort, IOutputPort outputPort)
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
