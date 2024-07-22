using FluentValidation;

using GraphT.Model.Entities;

namespace GraphT.Model.Services.Validators;

public class TaskUpstreamsValidator : AbstractValidator<HashSet<TodoTask>>
{
	public TaskUpstreamsValidator()
	{
		RuleFor(todoTasks => todoTasks)
			.NotNull()
			.WithMessage("New Upstreams cannot be null.")
			.NotEmpty()
			.WithMessage("New Upstreams cannot be empty");
	}
}

public class TaskUpstreamValidator : AbstractValidator<TodoTask>
{
	public TaskUpstreamValidator()
	{
		RuleFor(todoTask => todoTask.Id)
			.NotEqual(Guid.Empty)
			.WithMessage("Upstream Id cannot be empty.");
	}
}
