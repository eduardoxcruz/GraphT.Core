using GraphT.Model.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphT.EfCore.EntityTypeConfigurations;

public class LifeAreaEntityTypeConfiguration : IEntityTypeConfiguration<LifeArea>
{
	public void Configure(EntityTypeBuilder<LifeArea> builder)
	{
		builder.HasIndex(lifeArea => lifeArea.Id);
		builder.Property(lifeArea => lifeArea.Id).HasColumnOrder(0);
		builder.Property(todoTask => todoTask.Name).HasColumnOrder(1);
	}
}
