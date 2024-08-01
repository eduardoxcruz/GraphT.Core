using GraphT.UseCases.GetFinishedAndUnfinishedTasks;

using Microsoft.Extensions.DependencyInjection;

namespace GraphT.UseCases
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddGraphTUseCases(this IServiceCollection services)
        {
	        services.AddScoped<IGetFinishedTasksInputPort, GetFinishedTasks>();
	        services.AddScoped<IGetUnfinishedTasksInputPort, GetUnfinishedTasks>();
            return services;
        }
    }
}
