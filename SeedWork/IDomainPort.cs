namespace SeedWork;

public interface IFullPort<TIn, TOut>
{
	ValueTask<TOut> HandleAsync(TIn input);
}

public interface IPortWithInput<TIn>
{
	ValueTask HandleAsync(TIn input);
}

public interface IPortWithOutput<TOut>
{
	ValueTask<TOut> HandleAsync();
}
