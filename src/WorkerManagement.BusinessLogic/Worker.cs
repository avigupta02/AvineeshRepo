namespace WorkerManagement.BusinessLogic;

public sealed record Worker(Guid WorkerId, string Name, string Email, string Department, string Designation);
public sealed record WorkerInput(string Name, string Email, string Department, string Designation);
public sealed class ValidationException(IReadOnlyDictionary<string, string[]> errors) : Exception("Worker validation failed") { public IReadOnlyDictionary<string, string[]> Errors { get; } = errors; }
public sealed class NotFoundException(string message) : Exception(message);
