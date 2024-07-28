using GraphT.Model.Aggregates;
using GraphT.Model.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphT.EfCore.Repositories.EntityTypeConfigurations;

public class TodoTaskEntityTypeConfiguration : IEntityTypeConfiguration<TodoTask>
{
	public void Configure(EntityTypeBuilder<TodoTask> builder)
	{
		builder
			.HasDiscriminator<string>("TaskType")
			.HasValue<TodoTask>("TodoTask")
			.HasValue<TaskAggregate>("TaskAggregate");

		builder.HasIndex(todoTask => todoTask.Id);
		builder.Property(todoTask => todoTask.Id).HasColumnName("Id").HasColumnOrder(0);
		builder.Property(todoTask => todoTask.Name).HasColumnOrder(1);
		builder.Property(todoTask => todoTask.Status).HasColumnOrder(4);
	}
}
