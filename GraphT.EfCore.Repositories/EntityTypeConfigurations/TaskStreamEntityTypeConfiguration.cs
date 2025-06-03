using GraphT.EfCore.Repositories.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphT.EfCore.Repositories.EntityTypeConfigurations;

public class TaskStreamEntityTypeConfiguration : IEntityTypeConfiguration<TaskStream>
{
	public void Configure(EntityTypeBuilder<TaskStream> builder)
	{
		builder.ToTable("TaskStreams",
			t => t.HasCheckConstraint("CK_TaskStreams_NoSelfReference", "[UpstreamId] <> [DownstreamId]"));
		
		builder.HasKey(td => new { td.UpstreamId, td.DownstreamId });

		builder.HasOne(td => td.Upstream)
			.WithMany()
			.HasForeignKey(td => td.UpstreamId)
			.OnDelete(DeleteBehavior.NoAction);

		builder.HasOne(td => td.Downstream)
			.WithMany()
			.HasForeignKey(td => td.DownstreamId)
			.OnDelete(DeleteBehavior.NoAction);

		builder.Property(td => td.CreatedAt);
		builder.HasIndex(td => td.UpstreamId).HasDatabaseName("IX_TaskStreams_UpstreamId");
		builder.HasIndex(td => td.DownstreamId).HasDatabaseName("IX_TaskStreams_DownstreamId");
	}
}
