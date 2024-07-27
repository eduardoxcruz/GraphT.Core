using GraphT.Model.Entities;

namespace GraphT.EfCore.Repositories.Models;

public class TaskRelationModel
{
	public Guid UpstreamTaskId { get; set; }
	public Guid DownstreamTaskId { get; set; }

	public TodoTask UpstreamTask { get; set; }
	public TodoTask DownstreamTask { get; set; }
}
