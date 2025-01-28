using Microsoft.Extensions.DependencyInjection;

namespace GraphT.UseCases;

public static class DependencyContainer
{
	public static IServiceCollection AddGraphTUseCases(this IServiceCollection services)
	{
		services.AddScoped<AddNewTask.IInputPort, AddNewTask.UseCase>();
		services.AddScoped<UpdateTask.IInputPort, UpdateTask.UseCase>();
		services.AddScoped<FindTaskUpstreamsById.IInputPort, FindTaskUpstreamsById.UseCase>();
		services.AddScoped<FindTaskById.IInputPort, FindTaskById.UseCase>();
		services.AddScoped<FindReadyToStartTasks.IInputPort, FindReadyToStartTasks.UseCase>();
		services.AddScoped<FindTaskDownstreamsById.IInputPort, FindTaskDownstreamsById.UseCase>();
		services.AddScoped<FindTaskLifeAreasById.IInputPort, FindTaskLifeAreasById.UseCase>();
		services.AddScoped<FindFinishedTasks.IInputPort, FindFinishedTasks.UseCase>();
		
		return services;
	}
}
