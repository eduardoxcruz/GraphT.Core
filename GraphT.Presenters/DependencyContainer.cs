using GraphT.Presenters.GetFinishedOrUnfinishedTasks;
using GraphT.UseCases.GetFinishedAndUnfinishedTasks;

using Microsoft.Extensions.DependencyInjection;

namespace GraphT.Presenters
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddGraphTPresenters(this IServiceCollection services)
        {
	        services.AddScoped<IGetFinishedTasksOutputPort, GetFinishedTasksPresenter>();
	        services.AddScoped<IGetUnfinishedTasksOutputPort, GetUnfinishedTasksPresenter>();
            return services;
        }
    }
}
