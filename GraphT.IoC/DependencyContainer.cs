using GraphT.Controllers;
using GraphT.EfCore.Repositories;
using GraphT.Presenters;
using GraphT.UseCases;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GraphT.IoC;

public static class DependencyContainer
{
	public static IServiceCollection AddGraphTServices(this IServiceCollection services, IConfiguration configuration)
	{
		services
			.AddGraphTUseCases()
			.AddGraphTPresenters()
			.AddGraphTControllers()
			.AddGraphTEfCoreRepositories(configuration);
		
		return services;
	}
}
