using GraphT.Model.Aggregates;

namespace GraphT.EfCore.Repositories.Models;

public class TaskRelationModel
{
	public Guid UpstreamTaskId { get; set; }
	public Guid DownstreamTaskId { get; set; }

	public TaskAggregate UpstreamTask { get; set; }
	public TaskAggregate DownstreamTask { get; set; }
}
