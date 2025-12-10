using GraphT.Model.Aggregates;
using GraphT.Model.Exceptions;
using GraphT.Model.Services.Repositories;
using GraphT.Model.ValueObjects;

using SeedWork;

namespace GraphT.UseCases.AddNewTask;

public class UseCase : IFullPort<InputDto, OutputDto>
{
	private readonly IAddTaskPort _addTaskPort;
	
	public UseCase(IAddTaskPort addTaskPort)
	{
		_addTaskPort = addTaskPort;
	}

	public async ValueTask<OutputDto> HandleAsync(InputDto dto)
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

		if (dto.StatusChange.HasValue)
		{
			switch (dto.StatusChange.Value.ChangeDateTime.HasValue)
			{
				case true:
					newTask.SetStatus(dto.StatusChange.Value.ChangeDateTime.Value, dto.StatusChange.Value.Status);
					break;
				case false:
					newTask.SetStatus(dto.StatusChange.Value.Status);
					break;
			}
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

		await _addTaskPort.HandleAsync(newTask);
			
		return new OutputDto { Task = newTask };
	}
}

public record struct InputDto
{
	public string Name { get; set; }
	public bool? IsFun { get; set; }
	public bool? IsProductive { get; set; }
	public Complexity? Complexity { get; set; }
	public Priority? Priority { get; set; }
	public StatusChangeDto? StatusChange { get; set; }
	public DateTimeOffset? LimitDateTime { get; set; }
	public List<TodoTask>? Parents { get; set; }
	public List<TodoTask>? Children { get; set; }
	public List<LifeArea>? LifeAreas { get; set; }
}

public record struct OutputDto
{
	public TodoTask Task { get; set; }
}
