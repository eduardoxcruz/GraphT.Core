using GraphT.EfCore.Repositories.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphT.EfCore.Repositories.EntityTypeConfigurations;

public class TaskRelationEntityTypeConfiguration : IEntityTypeConfiguration<TaskRelationModel>
{
	public void Configure(EntityTypeBuilder<TaskRelationModel> builder)
	{
		builder.HasKey(taskRelation => new { taskRelation.UpstreamTaskId, taskRelation.DownstreamTaskId });
	}
}
