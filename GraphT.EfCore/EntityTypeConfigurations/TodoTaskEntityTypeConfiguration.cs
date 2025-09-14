using GraphT.Model.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphT.EfCore.EntityTypeConfigurations;

public class TodoTaskEntityTypeConfiguration : IEntityTypeConfiguration<TodoTask>
{
    public void Configure(EntityTypeBuilder<TodoTask> builder)
    {
        builder.HasIndex(todoTask => todoTask.Id);
        builder.Property(todoTask => todoTask.Id).HasColumnOrder(0);
        builder.Property(todoTask => todoTask.Name).HasColumnOrder(1);
        builder.Property(todoTask => todoTask.IsFun).HasColumnOrder(2);
        builder.Property(todoTask => todoTask.IsProductive).HasColumnOrder(3);
        builder.Property(todoTask => todoTask.OldStatus).HasColumnOrder(4);
        builder.Property(todoTask => todoTask.Complexity).HasColumnOrder(5);
        builder.Property(todoTask => todoTask.Priority).HasColumnOrder(6);
        builder.Property(todoTask => todoTask.OldRelevance).HasColumnOrder(7);
        builder.Property(todoTask => todoTask.Progress).HasColumnOrder(8);
        builder.ComplexProperty(todoTask => todoTask.DateTimeInfo);

        builder.Ignore(todoTask => todoTask.StatusLabel);
        builder.Ignore(todoTask => todoTask.ComplexityLabel);
        builder.Ignore(todoTask => todoTask.PriorityLabel);
        builder.Ignore(todoTask => todoTask.RelevanceLabel);
        builder.Ignore(todoTask => todoTask.UpstreamsCount);
        builder.Ignore(todoTask => todoTask.DownstreamsCount);
        builder.Ignore(todoTask => todoTask.LifeAreasCount);
    }
}
