using GraphT.Controllers;
using GraphT.Presenters;
using GraphT.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace GraphT.IoC
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddGraphTServices(
            this IServiceCollection services
        )
        {
            services
                .AddGraphTUseCases()
                .AddGraphTPresenters()
                .AddGraphTControllers();

            return services;
        }
    }
}
