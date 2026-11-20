namespace WorkerManagement.BusinessLogic;

public sealed class WorkerService(IWorkerRepository repository)
{
    public Task<IReadOnlyCollection<Worker>> GetAllAsync(CancellationToken ct) => repository.GetAllAsync(ct);
    public async Task<Worker> CreateAsync(WorkerInput input, CancellationToken ct)
    {
        Validate(input);
        var worker = new Worker(Guid.NewGuid(), input.Name.Trim(), input.Email.Trim(), input.Department.Trim(), input.Designation.Trim());
        await repository.AddAsync(worker, ct); return worker;
    }
    public async Task<Worker> UpdateAsync(Guid id, WorkerInput input, CancellationToken ct)
    {
        Validate(input); if (await repository.GetAsync(id, ct) is null) throw new NotFoundException($"Worker '{id}' was not found.");
        var worker = new Worker(id, input.Name.Trim(), input.Email.Trim(), input.Department.Trim(), input.Designation.Trim()); await repository.UpdateAsync(worker, ct); return worker;
    }
    public async Task DeleteAsync(Guid id, CancellationToken ct) { if (!await repository.DeleteAsync(id, ct)) throw new NotFoundException($"Worker '{id}' was not found."); }
    private static void Validate(WorkerInput input)
    {
        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(input.Name)) errors[nameof(input.Name)] = ["Name is required."];
        if (string.IsNullOrWhiteSpace(input.Email) || !input.Email.Contains('@', StringComparison.Ordinal)) errors[nameof(input.Email)] = ["A valid email is required."];
        if (string.IsNullOrWhiteSpace(input.Department)) errors[nameof(input.Department)] = ["Department is required."];
        if (string.IsNullOrWhiteSpace(input.Designation)) errors[nameof(input.Designation)] = ["Designation is required."];
        if (errors.Count > 0) throw new ValidationException(errors);
    }
}
