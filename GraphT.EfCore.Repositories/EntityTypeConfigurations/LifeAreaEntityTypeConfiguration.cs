using GraphT.Model.Aggregates;
using GraphT.Model.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphT.EfCore.Repositories.EntityTypeConfigurations;

public class LifeAreaEntityTypeConfiguration : IEntityTypeConfiguration<LifeArea>
{
    public void Configure(EntityTypeBuilder<LifeArea> builder)
    {
        builder.HasIndex(t => t.Id);
        builder.Property(t => t.Name).HasColumnOrder(0);
        
        builder.HasDiscriminator<string>("LifeAreaType")
               .HasValue<LifeArea>("BasicLifeArea")
               .HasValue<LifeAreaAggregate>("FullLifeArea");
    }
}
