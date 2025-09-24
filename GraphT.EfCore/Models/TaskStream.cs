using GraphT.Model.Entities;

namespace GraphT.EfCore.Models;

public class TaskStream
{
	public Guid UpstreamId { get; set; }
	public OldTodoTask Upstream { get; set; }
	public Guid DownstreamId { get; set; }
	public OldTodoTask Downstream { get; set; }
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
