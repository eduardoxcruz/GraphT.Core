using GraphT.Model.Services.Repositories;
using GraphT.Model.Exceptions;

using SeedWork;

namespace GraphT.UseCases.DeleteTask;

public interface IInputPort : IPort<InputDto> { }

public interface IOutputPort : IPort { }

public class UseCase : IInputPort
{
	private readonly IOutputPort _outputPort;
	private readonly ITodoTaskRepository _repository;

	public UseCase(IOutputPort outputPort, ITodoTaskRepository repository)
	{
		_outputPort = outputPort;
		_repository = repository;
	}

	public async ValueTask Handle(InputDto dto)
	{
		try
		{
			await _repository.RemoveAsync(dto.Id);
			await _outputPort.Handle();
		}
		catch (Exception ex)
		{
			throw new ExternalRepositoryException("Error removing task from repository", ex);
		}
	}
}

public record struct InputDto
{
	public Guid Id { get; set; }
}

