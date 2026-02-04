namespace SeedWork;

public class DomainResult
{
	public bool IsSuccess { get; }
	public string Error { get; }

	public static DomainResult Success() => new(true, string.Empty);
	public static DomainResult Failure(string error) => new(false, error);
    
	protected DomainResult(bool isSuccess, string error)
	{
		switch (isSuccess)
		{
			case true when !string.IsNullOrWhiteSpace(error):
				throw new InvalidOperationException("Success result cannot have an error.");
			case false when string.IsNullOrWhiteSpace(error):
				throw new InvalidOperationException("Failure result must have an error.");
			default:
				IsSuccess = isSuccess;
				Error = error;
				break;
		}
	}
}
