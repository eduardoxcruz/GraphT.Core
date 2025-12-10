using GraphT.Model.Aggregates;
using GraphT.Model.Services.Repositories;
using GraphT.Model.ValueObjects;

using SeedWork;

namespace GraphT.UseCases.UpdateTask;

public class UseCase : IFullPort<InputDto, OutputDto>
{
	private readonly IFullPort<FindTaskById.InputDto, FindTaskById.OutputDto> _findTaskByIdPort;
	private readonly IUpdateTaskPort _updateTaskPort;

	public UseCase(IFullPort<FindTaskById.InputDto, FindTaskById.OutputDto> findTaskByIdPort, IUpdateTaskPort updateTaskPort)
	{
		_findTaskByIdPort = findTaskByIdPort;
		_updateTaskPort = updateTaskPort;
	}

	public async ValueTask<OutputDto> HandleAsync(InputDto input)
	{
		if (input.AreAllOptionalPropertiesNull()) 
			throw new ArgumentNullException(nameof(input), "All optional properties are null. At least one property must be set.");
        
		TodoTask task = (await _findTaskByIdPort.HandleAsync(new FindTaskById.InputDto { Id = input.Id })).Task;

		if (input.Name != null)
			task.Name = input.Name;
        
		if (input.IsFun.HasValue)
			task.IsFun = input.IsFun.Value;
        
		if (input.IsProductive.HasValue)
			task.IsProductive = input.IsProductive.Value;
        
		if (input.Complexity.HasValue)
			task.Complexity = input.Complexity.Value;
        
		if (input.Priority.HasValue)
			task.Priority = input.Priority.Value;

		if (input.StatusChange.HasValue)
		{
			switch (input.StatusChange.Value.ChangeDateTime.HasValue)
			{
				case true:
					task.SetStatus(input.StatusChange.Value.ChangeDateTime.Value, input.StatusChange.Value.Status);
					break;
				case false:
					task.SetStatus(input.StatusChange.Value.Status);
					break;
			}
		}
        
		if (input.LimitDateTime.HasValue)
			task.SetLimitDateTime(input.LimitDateTime.Value);
		
		if (input.ParentsToAdd != null)
			task.AddParents(input.ParentsToAdd);
        
		if (input.ChildrenToAdd != null)
			task.AddChildren(input.ChildrenToAdd);
        
		if (input.LifeAreasToAdd != null)
			task.AddLifeAreas(input.LifeAreasToAdd);
        
		if (input.ParentsToRemove != null)
			task.RemoveParents(input.ParentsToRemove);
        
		if (input.ChildrenToRemove != null)
			task.RemoveChildren(input.ChildrenToRemove);
        
		if (input.LifeAreasToRemove != null)
			task.RemoveLifeAreas(input.LifeAreasToRemove);

		await _updateTaskPort.HandleAsync(task);
		
		return new OutputDto { Task = task };
	}
}

public record struct InputDto()
{
	public Guid Id { get; set; }
	public string? Name { get; set; }
	public bool? IsFun { get; set; }
	public bool? IsProductive { get; set; }
	public Complexity? Complexity { get; set; }
	public Priority? Priority { get; set; }
	public StatusChangeDto? StatusChange { get; set; }
	public DateTimeOffset? LimitDateTime { get; set; }
	public List<TodoTask>? ParentsToRemove { get; set; }
	public List<TodoTask>? ChildrenToRemove { get; set; }
	public List<LifeArea>? LifeAreasToRemove { get; set; }
	public List<TodoTask>? ParentsToAdd { get; set; }
	public List<TodoTask>? ChildrenToAdd { get; set; }
	public List<LifeArea>? LifeAreasToAdd { get; set; }
}

public record struct OutputDto()
{
	public TodoTask Task { get; set; }
}

public static class InputDtoExtensions
{
	public static bool AreAllOptionalPropertiesNull(this InputDto dto)
	{
		return dto.Name == null &&
		       dto.IsFun == null &&
		       dto.IsProductive == null &&
		       dto.Complexity == null &&
		       dto.Priority == null &&
		       dto.StatusChange == null &&
		       dto.LimitDateTime == null &&
		       dto.ParentsToAdd == null &&
		       dto.ChildrenToAdd == null &&
		       dto.LifeAreasToAdd == null &&
		       dto.ParentsToRemove == null &&
		       dto.ChildrenToRemove == null &&
		       dto.LifeAreasToRemove == null;
	}
}
