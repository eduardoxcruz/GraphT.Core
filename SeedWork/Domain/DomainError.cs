namespace SeedWork.Domain;

public record DomainError(int Type, string Description, string? Value = null);
