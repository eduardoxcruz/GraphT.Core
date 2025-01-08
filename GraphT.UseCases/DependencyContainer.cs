using GraphT.UseCases.UpdateTask;

using Microsoft.Extensions.DependencyInjection;

namespace GraphT.UseCases;

public static class DependencyContainer
{
	public static IServiceCollection AddGraphTUseCases(this IServiceCollection services)
	{
		services.AddScoped<IInputPort, UseCase>();

		return services;
	}
}
