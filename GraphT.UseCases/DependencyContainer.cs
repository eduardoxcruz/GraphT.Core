using Microsoft.Extensions.DependencyInjection;

namespace GraphT.UseCases
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddGraphTUseCases(this IServiceCollection services)
        {
	        services.AddScoped<GetFinishedTasks.IInputPort, GetFinishedTasks.UseCase>();
	        services.AddScoped<GetUnfinishedTasks.IInputPort, GetUnfinishedTasks.UseCase>();
            return services;
        }
    }
}
