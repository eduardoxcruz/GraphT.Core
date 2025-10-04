using GraphT.Model.Aggregates;
using GraphT.Model.Exceptions;
using GraphT.Model.Services.Repositories;
using GraphT.Model.ValueObjects;

using SeedWork;

namespace GraphT.UseCases.AddNewTask;

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
			TodoTask newTask = new(dto.Name);

			if (dto.IsFun.HasValue)
			{
				newTask.IsFun = dto.IsFun.Value;
			}

			if (dto.IsProductive.HasValue)
			{
				newTask.IsProductive = dto.IsProductive.Value;
			}

			if (dto.Complexity.HasValue)
			{
				newTask.Complexity = dto.Complexity.Value;
			}

			if (dto.Priority.HasValue)
			{
				newTask.Priority = dto.Priority.Value;
			}

			if (dto.Status.HasValue)
			{
				newTask.SetStatus(dto.Status.Value);
			}

			if (dto.LimitDateTime.HasValue)
			{
				newTask.SetLimitDateTime(dto.LimitDateTime.Value);
			}

			if (dto.Parents is { Count: > 0 })
			{
				newTask.AddParents([..dto.Parents]);
			}

			if (dto.Children is { Count: > 0 })
			{
				newTask.AddChildren([..dto.Children]);
			}

			if (dto.LifeAreas is { Count: > 0 })
			{
				newTask.AddLifeAreas([..dto.LifeAreas]);
			}

			await _repository.AddAsync(newTask);
			await _outputPort.Handle();
		}
		catch (Exception ex) when (ex is not ExternalRepositoryException)
		{
			throw new ExternalRepositoryException("Error occurred while adding a new task", ex);
		}
	}
}

public record struct InputDto
{
	public string Name { get; set; }
	public bool? IsFun { get; set; }
	public bool? IsProductive { get; set; }
	public Complexity? Complexity { get; set; }
	public Priority? Priority { get; set; }
	public Status? Status { get; set; }
	public DateTimeOffset? LimitDateTime { get; set; }
	public List<TodoTask>? Parents { get; set; }
	public List<TodoTask>? Children { get; set; }
	public List<LifeArea>? LifeAreas { get; set; }
}
