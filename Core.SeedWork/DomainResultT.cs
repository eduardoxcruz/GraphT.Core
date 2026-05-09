namespace SeedWork;

public class DomainResult
{
	public bool IsSuccess { get; }
	public DomainError? Error { get; }

	protected DomainResult(bool isSuccess, DomainError? error)
	{
		IsSuccess = isSuccess;
		Error = error;
	}

	public static DomainResult Success() => new(true, null);
	public static DomainResult Failure(DomainError error) => new(false, error ?? throw new ArgumentNullException(nameof(error)));
}

public class DomainResult<T> : DomainResult
{
	public T? Value { get; }

	private DomainResult(T value) : base(true, null) => Value = value;
	private DomainResult(DomainError error) : base(false, error) { }

	public static DomainResult<T> Success(T value) => new(value);
	public new static DomainResult<T> Failure(DomainError error) => new(error ?? throw new ArgumentNullException(nameof(error)));
}
