using GraphT.Model.Aggregates;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphT.EfCore.Repositories.EntityTypeConfigurations;

public class TodoTaskEntityTypeConfiguration : IEntityTypeConfiguration<TodoTask>
{
	public void Configure(EntityTypeBuilder<TodoTask> builder)
	{
		builder.HasIndex(todoTask => todoTask.Id);
		builder.Property(todoTask => todoTask.Id).HasColumnOrder(0);
		builder.Property(todoTask => todoTask.Name).HasColumnOrder(1);
		builder.Property(todoTask => todoTask.IsFun).HasColumnOrder(2);
		builder.Property(todoTask => todoTask.IsProductive).HasColumnOrder(3);
		builder.Property(todoTask => todoTask.Status).HasColumnOrder(4);
		builder.Property(todoTask => todoTask.Complexity).HasColumnOrder(5);
		builder.Property(todoTask => todoTask.Priority).HasColumnOrder(6);
		builder.Property(todoTask => todoTask.Relevance).HasColumnOrder(7);
		builder.Property(todoTask => todoTask.Progress).HasColumnOrder(8);
		builder.ComplexProperty(todoTask => todoTask.DateTimeInfo);

		builder
			.HasMany(ta => ta.Upstreams)
			.WithMany(ta => ta.Downstreams)
			.UsingEntity("TaskStreams",
				right => right.HasOne(typeof(TodoTask)).WithMany().HasForeignKey("UpstreamId").HasPrincipalKey(nameof(TodoTask.Id)),
				left => left.HasOne(typeof(TodoTask)).WithMany().HasForeignKey("DownstreamId").HasPrincipalKey(nameof(TodoTask.Id)),
				join => join.HasKey("UpstreamId", "DownstreamId"));

		builder.Ignore(todoTask => todoTask.StatusLabel);
		builder.Ignore(todoTask => todoTask.ComplexityLabel);
		builder.Ignore(todoTask => todoTask.PriorityLabel);
		builder.Ignore(todoTask => todoTask.RelevanceLabel);
	}
}
