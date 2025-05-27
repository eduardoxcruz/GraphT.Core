using GraphT.Model.Aggregates;
using GraphT.Model.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphT.EfCore.Repositories.EntityTypeConfigurations;

public class LifeAreaAggregateEntityTypeConfiguration : IEntityTypeConfiguration<LifeAreaAggregate>
{
    public void Configure(EntityTypeBuilder<LifeAreaAggregate> builder)
    {
        builder
            .HasMany(la => la.Tasks)
            .WithMany("_lifeAreas")
            .UsingEntity("TaskLifeAreas");
    }
}
