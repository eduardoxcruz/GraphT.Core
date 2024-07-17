using Microsoft.Extensions.DependencyInjection;

namespace GraphT.UseCases
{
    public static class DependencyContainer
    {
        public static IServiceCollection AddGraphTUseCases(
            this IServiceCollection services)
        {
            return services;
        }
    }
}
