using GraphT.Model.Aggregates;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphT.EfCore.Repositories.EntityTypeConfigurations;

public class TaskAggregateEntityTypeConfiguration : IEntityTypeConfiguration<TaskAggregate>
{
	public void Configure(EntityTypeBuilder<TaskAggregate> builder)
	{
		builder.HasIndex(taskAggregate => taskAggregate.Id);
		builder.Property(taskAggregate => taskAggregate.Id).HasColumnOrder(0);
		builder.Property(taskAggregate => taskAggregate.Name).HasColumnOrder(1);
		builder.Property(taskAggregate => taskAggregate.IsFun).HasColumnOrder(2);
		builder.Property(taskAggregate => taskAggregate.IsProductive).HasColumnOrder(3);
		builder.Property(taskAggregate => taskAggregate.Status).HasColumnOrder(4);
		builder.Property(taskAggregate => taskAggregate.Complexity).HasColumnOrder(5);
		builder.Property(taskAggregate => taskAggregate.Priority).HasColumnOrder(6);
		builder.Property(taskAggregate => taskAggregate.Relevance).HasColumnOrder(7);
		builder.ComplexProperty(taskAggregate => taskAggregate.DateTimeInfo);

		builder
			.HasMany(ta => ta.Upstreams)
			.WithMany(ta => ta.Downstreams)
			.UsingEntity("TaskStreams",
				right => right.HasOne(typeof(TaskAggregate)).WithMany().HasForeignKey("UpstreamId").HasPrincipalKey(nameof(TaskAggregate.Id)),
				left => left.HasOne(typeof(TaskAggregate)).WithMany().HasForeignKey("DownstreamId").HasPrincipalKey(nameof(TaskAggregate.Id)),
				join => join.HasKey("UpstreamId", "DownstreamId"));
		
		builder.Ignore(taskAggregate => taskAggregate.Progress);
	}
}
