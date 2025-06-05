using GraphT.Model.Services;
using GraphT.Model.Services.Repositories;

using SeedWork;

namespace GraphT.UseCases.AddUpstream;

public interface IInputPort : IPort<InputDto> { }

public interface IOutputPort : IPort { }

public class UseCase : IInputPort
{
	private readonly IOutputPort _outputPort;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ITaskUpstreamsRepository _taskUpstreamRepository;

	public UseCase(IOutputPort outputPort, IUnitOfWork unitOfWork, ITaskUpstreamsRepository taskUpstreamRepository)
	{
		_outputPort = outputPort;
		_unitOfWork = unitOfWork;
		_taskUpstreamRepository = taskUpstreamRepository;
	}

	public async ValueTask Handle(InputDto dto)
	{
		if (dto.TaskId.Equals(Guid.Empty)) throw new ArgumentException("Task Id cannot be empty Id");

		if (TaskStreamValidatorService.IsStreamValid(dto.UpstreamId))
		{
			await _taskUpstreamRepository.AddUpstreamAsync(dto.TaskId, dto.UpstreamId);
			await _unitOfWork.SaveChangesAsync();
		}
		
		await _outputPort.Handle();
	}
}

public record struct InputDto()
{
	public Guid TaskId { get; set; }
	public Guid UpstreamId { get; set; }
}
