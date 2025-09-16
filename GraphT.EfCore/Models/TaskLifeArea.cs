using GraphT.Model.Entities;

namespace GraphT.EfCore.Models;

public class TaskLifeArea
{
	public Guid LifeAreaId { get; set; }
	public LifeArea LifeArea { get; set; }
	public Guid TaskId { get; set; }
	public OldTodoTask Task { get; set; }
	public DateTimeOffset CreatedAt { get; set; }
    
	public TaskLifeArea()
	{
		CreatedAt = DateTimeOffset.UtcNow;
	}
    
	public TaskLifeArea(Guid lifeAreaId, Guid taskId) : this()
	{
		LifeAreaId = lifeAreaId;
		TaskId = taskId;
	}
}
