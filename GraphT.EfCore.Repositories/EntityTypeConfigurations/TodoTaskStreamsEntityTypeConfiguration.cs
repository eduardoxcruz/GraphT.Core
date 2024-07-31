using GraphT.EfCore.Repositories.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphT.EfCore.Repositories.EntityTypeConfigurations;

public class TodoTaskStreamsEntityTypeConfiguration : IEntityTypeConfiguration<TodoTaskStream>
{
	public void Configure(EntityTypeBuilder<TodoTaskStream> builder)
	{
		builder.HasKey(todoTaskStream => new { todoTaskStream.UpstreamTaskId, todoTaskStream.DownstreamTaskId });

		builder
			.HasOne(todoTaskStream => todoTaskStream.Upstream)
			.WithMany()
			.HasForeignKey(todoTaskStream => todoTaskStream.UpstreamTaskId)
			.OnDelete(DeleteBehavior.Cascade);
		
		builder
			.HasOne(todoTaskStream => todoTaskStream.Downstream)
			.WithMany()
			.HasForeignKey(todoTaskStream => todoTaskStream.DownstreamTaskId)
			.OnDelete(DeleteBehavior.Restrict);
	}
}
