namespace SeedWork.Domain;

public record DomainError(DomainErrorType Type, string Description, string? Value = null);
