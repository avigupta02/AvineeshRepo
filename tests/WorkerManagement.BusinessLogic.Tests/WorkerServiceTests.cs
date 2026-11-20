using WorkerManagement.BusinessLogic;
using Xunit;
namespace WorkerManagement.BusinessLogic.Tests;
public sealed class WorkerServiceTests
{
    [Fact] public async Task Create_validates_and_assigns_unique_id() { var repo = new FakeRepository(); var result = await new WorkerService(repo).CreateAsync(new("Ada", "ada@example.com", "Engineering", "Developer"), default); Assert.NotEqual(Guid.Empty, result.WorkerId); }
    [Fact] public async Task Create_rejects_invalid_email() { await Assert.ThrowsAsync<ValidationException>(() => new WorkerService(new FakeRepository()).CreateAsync(new("Ada", "bad", "Engineering", "Developer"), default)); }
    private sealed class FakeRepository : IWorkerRepository { readonly List<Worker> items=[]; public Task<IReadOnlyCollection<Worker>> GetAllAsync(CancellationToken _) => Task.FromResult<IReadOnlyCollection<Worker>>(items); public Task<Worker?> GetAsync(Guid id,CancellationToken _) => Task.FromResult(items.FirstOrDefault(x=>x.WorkerId==id)); public Task AddAsync(Worker w,CancellationToken _) {items.Add(w);return Task.CompletedTask;} public Task UpdateAsync(Worker w,CancellationToken _)=>Task.CompletedTask; public Task<bool> DeleteAsync(Guid id,CancellationToken _) => Task.FromResult(items.RemoveAll(x=>x.WorkerId==id)>0); }
}
