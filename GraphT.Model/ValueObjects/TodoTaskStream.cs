using GraphT.Model.Entities;

namespace GraphT.Model.ValueObjects;

public class TodoTaskStream
{
	public Guid UpstreamTaskId { get; set; }
	public Guid DownstreamTaskId { get; set; }
	
	public TodoTask Upstream { get; set; }
	public TodoTask Downstream { get; set; }
}
