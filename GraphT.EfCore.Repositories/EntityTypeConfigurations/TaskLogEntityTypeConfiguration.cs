using GraphT.Model.ValueObjects;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphT.EfCore.Repositories.EntityTypeConfigurations;

public class TaskLogEntityTypeConfiguration : IEntityTypeConfiguration<TaskLog>
{
	public void Configure(EntityTypeBuilder<TaskLog> builder)
	{
		builder.HasKey(t => new { t.TaskId, t.DateTime, t.Status});
		builder.Property(t => t.TaskId).HasColumnOrder(0);
		builder.Property(t => t.DateTime).HasColumnOrder(1);
		builder.Property(t => t.Status).HasColumnOrder(2);
		builder.Property(t => t.TimeSpentOnTask).HasColumnOrder(3);
	}
}
