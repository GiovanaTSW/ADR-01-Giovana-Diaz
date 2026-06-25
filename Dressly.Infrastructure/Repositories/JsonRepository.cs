using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using Dressly.Application.Ports.Output;

namespace Dressly_MVC.Repositories;

public class JsonRepository<T> : IRepository<T> where T : class
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _options;
    private readonly SemaphoreSlim _writeLock = new(1, 1);

    public JsonRepository(string fileName)
    {
        _filePath = Path.Combine(Directory.GetCurrentDirectory(), "data", fileName);
        _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };
    }

    public async Task<List<T>> GetAllAsync()
    {
        if (!File.Exists(_filePath)) return new();
        var json = await File.ReadAllTextAsync(_filePath);
        return JsonSerializer.Deserialize<List<T>>(json, _options) ?? new();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        var items = await GetAllAsync();
        return items.FirstOrDefault(e =>
        {
            var prop = typeof(T).GetProperty("Id");
            return prop != null && prop.GetValue(e)?.Equals(id) == true;
        });
    }

    public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        var items = await GetAllAsync();
        return items.AsQueryable().Where(predicate).ToList();
    }

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        var items = await GetAllAsync();
        return items.AsQueryable().FirstOrDefault(predicate);
    }

    public async Task<int> GetNextIdAsync()
    {
        var items = await GetAllAsync();
        if (!items.Any()) return 1;
        var prop = typeof(T).GetProperty("Id");
        if (prop == null) return 1;
        return items.Max(e => (int)prop.GetValue(e)!) + 1;
    }

    public async Task AddAsync(T entity)
    {
        await _writeLock.WaitAsync();
        try
        {
            var items = await GetAllAsync();
            items.Add(entity);
            await SaveAsync(items);
        }
        finally { _writeLock.Release(); }
    }

    public async Task UpdateAsync(T entity)
    {
        await _writeLock.WaitAsync();
        try
        {
            var items = await GetAllAsync();
            var id = (int)typeof(T).GetProperty("Id")!.GetValue(entity)!;
            var index = items.FindIndex(e =>
            {
                var prop = typeof(T).GetProperty("Id");
                return prop != null && prop.GetValue(e)?.Equals(id) == true;
            });
            if (index >= 0) items[index] = entity;
            await SaveAsync(items);
        }
        finally { _writeLock.Release(); }
    }

    public async Task DeleteAsync(int id)
    {
        await _writeLock.WaitAsync();
        try
        {
            var items = await GetAllAsync();
            items.RemoveAll(e =>
            {
                var prop = typeof(T).GetProperty("Id");
                return prop != null && prop.GetValue(e)?.Equals(id) == true;
            });
            await SaveAsync(items);
        }
        finally { _writeLock.Release(); }
    }

    private async Task SaveAsync(List<T> items)
    {
        var dir = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
        var json = JsonSerializer.Serialize(items, _options);
        var tempPath = _filePath + ".tmp";
        await File.WriteAllTextAsync(tempPath, json);
        File.Move(tempPath, _filePath, overwrite: true);
    }
}
