using GraphT.Model.Aggregates;
using GraphT.Model.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphT.EfCore.Repositories.EntityTypeConfigurations;

public class TaskAggregateEntityTypeConfiguration : IEntityTypeConfiguration<TaskAggregate>
{
    public void Configure(EntityTypeBuilder<TaskAggregate> builder)
    {
	    // Configure the self-referencing many-to-many relationship for task dependencies
	    builder.HasMany(t => t.Downstreams)
		    .WithMany() // No inverse navigation since TodoTask doesn't have collections
		    .UsingEntity<Dictionary<string, object>>(
			    "TaskStreams",
			    j => j
				    .HasOne<TodoTask>()
				    .WithMany()
				    .HasForeignKey("DownstreamTaskId")
				    .OnDelete(DeleteBehavior.Restrict),
			    j => j
				    .HasOne<TaskAggregate>()
				    .WithMany()
				    .HasForeignKey("UpstreamTaskId")
				    .OnDelete(DeleteBehavior.Restrict),
			    j =>
			    {
				    j.HasKey("UpstreamTaskId", "DownstreamTaskId");
				    j.ToTable("TaskStreams");
			    }
		    );
	    
	    builder.HasMany(t => t.Upstreams)
		    .WithMany() // No inverse navigation since TodoTask doesn't have collections
		    .UsingEntity<Dictionary<string, object>>(
			    "TaskStreams",
			    j => j
				    .HasOne<TodoTask>()
				    .WithMany()
				    .HasForeignKey("UpstreamTaskId")
				    .OnDelete(DeleteBehavior.Restrict),
			    j => j
				    .HasOne<TaskAggregate>()
				    .WithMany()
				    .HasForeignKey("DownstreamTaskId")
				    .OnDelete(DeleteBehavior.Restrict),
			    j =>
			    {
				    j.HasKey("DownstreamTaskId", "UpstreamTaskId");
				    j.ToTable("TaskStreams");
			    }
		    );


        builder.HasMany(t => t.LifeAreas)
            .WithMany()
            .UsingEntity(
                "TaskLifeAreas",
                l => l.HasOne(typeof(LifeArea)).WithMany().HasForeignKey("LifeAreaId").OnDelete(DeleteBehavior.Restrict),
                r => r.HasOne(typeof(TaskAggregate)).WithMany().HasForeignKey("TaskAggregateId").OnDelete(DeleteBehavior.Cascade)
            );

        builder.Navigation(t => t.Upstreams).HasField("_upstreams").UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(t => t.Downstreams).HasField("_downstreams").UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(t => t.LifeAreas).HasField("_lifeAreas").UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
