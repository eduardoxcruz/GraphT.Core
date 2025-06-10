using GraphT.Model.Entities;

namespace GraphT.EfCore.Repositories.Models;

public class TaskLifeArea
{
	public Guid LifeAreaId { get; set; }
	public LifeArea LifeArea { get; set; }
	public Guid TaskId { get; set; }
	public TodoTask Task { get; set; }
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
