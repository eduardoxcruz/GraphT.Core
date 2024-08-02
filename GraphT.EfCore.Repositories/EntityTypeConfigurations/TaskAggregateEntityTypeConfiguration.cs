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
		builder.Property(taskAggregate => taskAggregate.DateTimeInfo.CreationDateTime).HasColumnOrder(8);
		builder.Property(taskAggregate => taskAggregate.DateTimeInfo.StartDateTime).HasColumnOrder(9);
		builder.Property(taskAggregate => taskAggregate.DateTimeInfo.FinishDateTime).HasColumnOrder(10);
		builder.Property(taskAggregate => taskAggregate.DateTimeInfo.LimitDateTime).HasColumnOrder(11);
		builder.Property(taskAggregate => taskAggregate.DateTimeInfo.TimeSpend).HasColumnOrder(12);
		builder.Property(taskAggregate => taskAggregate.DateTimeInfo.Punctuality).HasColumnOrder(13);
		
		builder.HasMany(ta => ta.Upstreams).WithMany();
		builder.HasMany(ta => ta.Downstreams).WithMany();
		
		builder.Ignore(taskAggregate => taskAggregate.Progress);
	}
}
