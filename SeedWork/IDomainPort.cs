namespace SeedWork;

public interface IDomainPort<TOutput, TInput>
{
	ValueTask<TOutput> Handle(TInput input);
}
