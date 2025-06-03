using System.Diagnostics;

using GraphT.EfCore.Repositories.Repositories;
using GraphT.Model.Aggregates;
using GraphT.Model.Entities;
using GraphT.Model.Services.Repositories;
using GraphT.Model.ValueObjects;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using SeedWork;

namespace GraphT.EfCore.Repositories;

public static class DependencyContainer
{
    public static IServiceCollection AddGraphTEfCoreRepositories(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionString)
    {
        services.AddDbContext<EfDbContext>(options => options
            .UseSqlServer(configuration[$"ConnectionStrings:{connectionString}"])
            .LogTo(message => Debug.WriteLine(message), LogLevel.Warning));

        services.AddScoped<ILifeAreasRepository, LifeAreasRepository>();
        services.AddScoped<ITaskDownstreamsRepository, TaskDownstreamsRepository>();
        services.AddScoped<ITaskLogRepository, TaskLogRepository>();
        services.AddScoped<ITaskUpstreamsRepository, TaskUpstreamsRepository>();
        services.AddScoped<ITodoTaskRepository, TodoTaskRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
