namespace SeedWork;

public record DomainError(int Type, string Description, string? Value = null);
