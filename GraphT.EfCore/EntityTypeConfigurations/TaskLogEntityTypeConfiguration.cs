using GraphT.Model.ValueObjects;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphT.EfCore.EntityTypeConfigurations;

public class TaskLogEntityTypeConfiguration : IEntityTypeConfiguration<OldTaskLog>
{
	public void Configure(EntityTypeBuilder<OldTaskLog> builder)
	{
		builder.HasKey(t => new { t.TaskId, t.DateTime, Status = t.OldStatus});
		builder.Property(t => t.TaskId).HasColumnOrder(0);
		builder.Property(t => t.DateTime).HasColumnOrder(1);
		builder.Property(t => t.OldStatus).HasColumnOrder(2);
		builder.Property(t => t.TimeSpentOnTask).HasColumnOrder(3);
	}
}
