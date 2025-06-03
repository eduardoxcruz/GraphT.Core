using GraphT.Model.Entities;
using GraphT.Model.Exceptions;
using GraphT.Model.Services.Repositories;
using GraphT.Model.ValueObjects;

using SeedWork;

namespace GraphT.UseCases.FindTaskLifeAreasById;

public interface IInputPort : IPort<InputDto> { }

public interface IOutputPort : IPort<OutputDto> { }

public class UseCase : IInputPort
{
	private readonly IOutputPort _outputPort;
	private readonly ITodoTaskRepository _todoTaskRepository;
	private readonly ILifeAreasRepository _lifeAreasRepository;

	public UseCase(IOutputPort outputPort, ITodoTaskRepository todoTaskRepository, ILifeAreasRepository lifeAreasRepository)
	{
		_outputPort = outputPort;
		_todoTaskRepository = todoTaskRepository;
		_lifeAreasRepository = lifeAreasRepository;
	}

	public async ValueTask Handle(InputDto dto)
	{
		if (!await _todoTaskRepository.ContainsAsync(dto.Id)) throw new TaskNotFoundException("Task not found", dto.Id);
		
		PagedList<LifeArea> lifeAreas = await _lifeAreasRepository.FindTaskLifeAreasById(dto.Id);
		
		await _outputPort.Handle(new OutputDto { LifeAreas = lifeAreas });
	}
}

public class InputDto
{
	public PagingParams PagingParams { get; set; }
	public Guid Id { get; set; }
}

public class OutputDto
{
	public PagedList<LifeArea> LifeAreas { get; set; }
}
