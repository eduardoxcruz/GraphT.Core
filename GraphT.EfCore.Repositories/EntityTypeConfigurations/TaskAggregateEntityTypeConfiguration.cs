using GraphT.Model.Aggregates;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphT.EfCore.Repositories.EntityTypeConfigurations;

public class TaskAggregateEntityTypeConfiguration : IEntityTypeConfiguration<TaskAggregate>
{
	public void Configure(EntityTypeBuilder<TaskAggregate> builder)
	{
		builder.HasIndex(taskAggregate => taskAggregate.Id);
		builder.Property(taskAggregate => taskAggregate.Id).HasColumnName("Id").HasColumnOrder(0);
		builder.Property(taskAggregate => taskAggregate.Name).HasColumnOrder(1);
		builder.Property(taskAggregate => taskAggregate.IsFun).HasColumnOrder(2);
		builder.Property(taskAggregate => taskAggregate.IsProductive).HasColumnOrder(3);
		builder.Property(taskAggregate => taskAggregate.Status).HasColumnOrder(4);
		builder.Property(taskAggregate => taskAggregate.Complexity).HasColumnOrder(5);
		builder.Property(taskAggregate => taskAggregate.Priority).HasColumnOrder(6);
		builder.Property(taskAggregate => taskAggregate.Relevance).HasColumnOrder(7);

		builder.Ignore(taskAggregate => taskAggregate.Upstreams);
		builder.Ignore(taskAggregate => taskAggregate.Downstreams);
		builder.Ignore(taskAggregate => taskAggregate.Progress);
		builder.Ignore(taskAggregate => taskAggregate.DateTimeInfo);
	}
}
