using GraphT.Model.Entities;

namespace GraphT.EfCore.Repositories.Models;

public class TaskStream
{
	public Guid UpstreamId { get; set; }
	public TodoTask Upstream { get; set; }
	public Guid DownstreamId { get; set; }
	public TodoTask Downstream { get; set; }
	public DateTimeOffset CreatedAt { get; set; }
    
	public TaskStream()
	{
		CreatedAt = DateTimeOffset.UtcNow;
	}
    
	public TaskStream(Guid upstreamTaskId, Guid downstreamTaskId) : this()
	{
		UpstreamId = upstreamTaskId;
		DownstreamId = downstreamTaskId;
	}
}
