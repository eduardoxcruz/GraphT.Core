using GraphT.Model.Aggregates;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphT.EfCore.Repositories.EntityTypeConfigurations;

public class LifeAreaEntityTypeConfiguration : IEntityTypeConfiguration<LifeAreaAggregate>
{
	public void Configure(EntityTypeBuilder<LifeAreaAggregate> builder)
	{
		builder.HasIndex(t => t.Id);
		builder.Property(t => t.Name).HasColumnOrder(0);

		builder
			.HasMany(t => t.Tasks)
			.WithMany(t => t.LifeAreas);
	}
}
