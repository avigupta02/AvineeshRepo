using System.Text.Json;
using WorkerManagement.BusinessLogic;
namespace WorkerManagement.DataAccess;

public sealed class JsonWorkerRepository(IConfiguration configuration) : IWorkerRepository
{
    private readonly string path = configuration["WorkerStorage:Path"] ?? "workers.json";
    private readonly SemaphoreSlim gate = new(1, 1);
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web) { WriteIndented = true };
    public async Task<IReadOnlyCollection<Worker>> GetAllAsync(CancellationToken ct) { await gate.WaitAsync(ct); try { return await ReadAsync(ct); } finally { gate.Release(); } }
    public async Task<Worker?> GetAsync(Guid id, CancellationToken ct) => (await GetAllAsync(ct)).FirstOrDefault(x => x.WorkerId == id);
    public async Task AddAsync(Worker worker, CancellationToken ct) { await gate.WaitAsync(ct); try { var items = (await ReadAsync(ct)).ToList(); items.Add(worker); await WriteAsync(items, ct); } finally { gate.Release(); } }
    public async Task UpdateAsync(Worker worker, CancellationToken ct) { await gate.WaitAsync(ct); try { var items = (await ReadAsync(ct)).ToList(); var i = items.FindIndex(x => x.WorkerId == worker.WorkerId); if (i >= 0) { items[i] = worker; await WriteAsync(items, ct); } } finally { gate.Release(); } }
    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct) { await gate.WaitAsync(ct); try { var items = (await ReadAsync(ct)).ToList(); var removed = items.RemoveAll(x => x.WorkerId == id) > 0; if (removed) await WriteAsync(items, ct); return removed; } finally { gate.Release(); } }
    private async Task<IReadOnlyCollection<Worker>> ReadAsync(CancellationToken ct) { if (!File.Exists(path)) return []; await using var stream = File.OpenRead(path); return await JsonSerializer.DeserializeAsync<List<Worker>>(stream, Options, ct) ?? []; }
    private async Task WriteAsync(IReadOnlyCollection<Worker> workers, CancellationToken ct) { var temp = path + ".tmp"; Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!); await using (var stream = File.Create(temp)) await JsonSerializer.SerializeAsync(stream, workers, Options, ct); File.Move(temp, path, true); }
}
