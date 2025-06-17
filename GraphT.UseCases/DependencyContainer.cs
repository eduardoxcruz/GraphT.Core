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
		services.AddScoped<FindInProgressTasks.IInputPort, FindInProgressTasks.UseCase>();
		services.AddScoped<FindFinishedTasks.IInputPort, FindFinishedTasks.UseCase>();
		services.AddScoped<DeleteTask.IInputPort, DeleteTask.UseCase>();
		services.AddScoped<GetTaskEnumsItems.IInputPort, GetTaskEnumsItems.UseCase>();
		services.AddScoped<FindTasksWithoutUpstreams.IInputPort, FindTasksWithoutUpstreams.UseCase>();
		services.AddScoped<AddDownstream.IInputPort, AddDownstream.UseCase>();
		services.AddScoped<AddUpstream.IInputPort, AddUpstream.UseCase>();
		services.AddScoped<RemoveDownstream.IInputPort, RemoveDownstream.UseCase>();
		services.AddScoped<RemoveUpstream.IInputPort, RemoveUpstream.UseCase>();
		services.AddScoped<GetTasksOrderedByCreationDateDesc.IInputPort, GetTasksOrderedByCreationDateDesc.UseCase>();
		
		return services;
	}
}
