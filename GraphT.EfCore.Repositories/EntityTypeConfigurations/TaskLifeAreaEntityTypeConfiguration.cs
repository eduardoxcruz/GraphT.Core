using GraphT.EfCore.Repositories.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphT.EfCore.Repositories.EntityTypeConfigurations;

public class TaskLifeAreaEntityTypeConfiguration : IEntityTypeConfiguration<TaskLifeArea>
{
	public void Configure(EntityTypeBuilder<TaskLifeArea> builder)
	{
		builder.ToTable("TaskLifeAreas",
			t => t.HasCheckConstraint("CK_TaskLifeAreas_NoSelfReference", "[LifeAreaId] <> [TaskId]"));
		
		builder.HasKey(lifeArea => new { lifeArea.LifeAreaId, lifeArea.TaskId });

		builder.HasOne(lifeArea => lifeArea.LifeArea)
			.WithMany()
			.HasForeignKey(td => td.LifeAreaId)
			.OnDelete(DeleteBehavior.SetNull);
		
		builder.HasOne(lifeArea => lifeArea.Task)
			.WithMany()
			.HasForeignKey(td => td.TaskId)
			.OnDelete(DeleteBehavior.SetNull);

		builder.Property(td => td.CreatedAt);
		builder.HasIndex(td => td.LifeAreaId).HasDatabaseName("IX_TaskLifeArea_LifeAreaId");
		builder.HasIndex(td => td.TaskId).HasDatabaseName("IX_TaskLifeArea_TaskId");
	}
}
