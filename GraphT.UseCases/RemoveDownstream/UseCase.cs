using GraphT.Model.Services;
using GraphT.Model.Services.Repositories;

using SeedWork;

namespace GraphT.UseCases.RemoveDownstream;

public interface IInputPort : IPort<InputDto> { }

public interface IOutputPort : IPort { }

public class UseCase : IInputPort
{
	private readonly IOutputPort _outputPort;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ITaskDownstreamsRepository _taskDownstreamsRepository;

	public UseCase(IOutputPort outputPort, IUnitOfWork unitOfWork, ITaskDownstreamsRepository taskDownstreamsRepository)
	{
		_outputPort = outputPort;
		_unitOfWork = unitOfWork;
		_taskDownstreamsRepository = taskDownstreamsRepository;
	}

	public async ValueTask Handle(InputDto dto)
	{
		if (dto.TaskId.Equals(Guid.Empty)) throw new ArgumentException("Task Id cannot be empty Id");

		if (TaskStreamValidatorService.IsStreamValid(dto.DownstreamId))
		{
			await _taskDownstreamsRepository.RemoveDownstreamAsync(dto.TaskId, dto.DownstreamId);
			await _unitOfWork.SaveChangesAsync();
		}
		
		await _outputPort.Handle();
	}
}

public record struct InputDto()
{
	public Guid TaskId { get; set; }
	public Guid DownstreamId { get; set; }
}
