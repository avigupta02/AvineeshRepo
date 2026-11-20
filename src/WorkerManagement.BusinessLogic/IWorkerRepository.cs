namespace WorkerManagement.BusinessLogic;

public interface IWorkerRepository
{
    Task<IReadOnlyCollection<Worker>> GetAllAsync(CancellationToken cancellationToken);
    Task<Worker?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Worker worker, CancellationToken cancellationToken);
    Task UpdateAsync(Worker worker, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
