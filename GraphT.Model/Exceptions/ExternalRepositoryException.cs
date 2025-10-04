namespace GraphT.Model.Exceptions;

public class ExternalRepositoryException : Exception 
{
	public ExternalRepositoryException()
	{
	}
    
	public ExternalRepositoryException(string message) : base(message)
	{
	}
    
	public ExternalRepositoryException(string message, Exception inner) : base(message, inner)
	{
	}
}
