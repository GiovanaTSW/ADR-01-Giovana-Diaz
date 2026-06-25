using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Dressly.Infrastructure.Repositories;

public class CsvRepository<T> where T : class, new()
{
    private readonly string _filePath;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public CsvRepository(string fileName)
    {
        var dataDir = Path.Combine(Directory.GetCurrentDirectory(), "data");
        Directory.CreateDirectory(dataDir);
        _filePath = Path.Combine(dataDir, fileName);
    }

    public async Task<List<T>> GetAllAsync()
    {
        if (!File.Exists(_filePath)) return [];

        var lines = await File.ReadAllLinesAsync(_filePath);
        if (lines.Length < 2) return [];

        var headers = ParseCsvLine(lines[0]);
        var items = new List<T>();

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            var values = ParseCsvLine(lines[i]);
            if (values.Length != headers.Length) continue;

            var item = new T();
            for (int j = 0; j < headers.Length; j++)
                SetProperty(item, headers[j], values[j]);

            items.Add(item);
        }

        return items;
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

    public async Task<List<T>> FindAsync(Func<T, bool> predicate)
    {
        var items = await GetAllAsync();
        return items.Where(predicate).ToList();
    }

    public async Task<T?> FirstOrDefaultAsync(Func<T, bool> predicate)
    {
        var items = await GetAllAsync();
        return items.FirstOrDefault(predicate);
    }

    public async Task<int> GetNextIdAsync()
    {
        var items = await GetAllAsync();
        if (items.Count == 0) return 1;
        var prop = typeof(T).GetProperty("Id");
        if (prop == null) return 1;
        return items.Max(e => (int)prop.GetValue(e)!) + 1;
    }

    public async Task AddAsync(T entity)
    {
        var items = await GetAllAsync();
        items.Add(entity);
        await SaveAllAsync(items);
    }

    public async Task UpdateAsync(T entity)
    {
        var items = await GetAllAsync();
        var id = (int)typeof(T).GetProperty("Id")!.GetValue(entity)!;
        var index = items.FindIndex(e =>
        {
            var prop = typeof(T).GetProperty("Id");
            return prop != null && prop.GetValue(e)?.Equals(id) == true;
        });
        if (index >= 0) items[index] = entity;
        await SaveAllAsync(items);
    }

    public async Task DeleteAsync(int id)
    {
        var items = await GetAllAsync();
        items.RemoveAll(e =>
        {
            var prop = typeof(T).GetProperty("Id");
            return prop != null && prop.GetValue(e)?.Equals(id) == true;
        });
        await SaveAllAsync(items);
    }

    private static bool IsNavigationCollection(PropertyInfo p)
    {
        var type = p.PropertyType;
        if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(List<>))
            return false;

        var elementType = type.GetGenericArguments()[0];
        return elementType.IsClass && elementType != typeof(string) && elementType != typeof(object);
    }

    public async Task SaveAllAsync(List<T> items)
    {
        var allProps = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var props = allProps.Where(p => !IsNavigationCollection(p)).ToArray();
        var lines = new List<string>();

        // Header
        lines.Add(string.Join(",", props.Select(p => EscapeCsv(p.Name))));

        // Data rows
        foreach (var item in items)
        {
            var values = props.Select(p => FormatValue(p.GetValue(item)));
            lines.Add(string.Join(",", values.Select(EscapeCsv)));
        }

        await File.WriteAllLinesAsync(_filePath, lines);
    }

    private static string FormatValue(object? value)
    {
        if (value == null) return "";

        var type = value.GetType();

        if (type.IsPrimitive || type == typeof(string) || type == typeof(decimal) ||
            type == typeof(DateTime) || type == typeof(float) || type == typeof(double))
            return value.ToString() ?? "";

        return JsonSerializer.Serialize(value, JsonOptions);
    }

    private static string EscapeCsv(string value)
    {
        if (string.IsNullOrEmpty(value)) return "";
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }

    private static string[] ParseCsvLine(string line)
    {
        var result = new List<string>();
        var current = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            var c = line[i];
            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }
        result.Add(current.ToString());
        return [.. result];
    }

    private static void SetProperty(T obj, string name, string value)
    {
        var prop = typeof(T).GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (prop == null || !prop.CanWrite) return;

        object? parsed;
        var targetType = prop.PropertyType;

        if (targetType == typeof(string))
            parsed = value;
        else if (string.IsNullOrEmpty(value))
            parsed = null;
        else if (targetType == typeof(int))
            parsed = int.Parse(value);
        else if (targetType == typeof(int?))
            parsed = int.Parse(value);
        else if (targetType == typeof(bool))
            parsed = bool.Parse(value);
        else if (targetType == typeof(DateTime))
            parsed = DateTime.Parse(value);
        else if (targetType == typeof(decimal))
            parsed = decimal.Parse(value);
        else if (targetType == typeof(decimal?))
            parsed = decimal.Parse(value);
        else if (targetType == typeof(float))
            parsed = float.Parse(value);
        else if (targetType == typeof(float?))
            parsed = float.Parse(value);
        else if (targetType == typeof(double))
            parsed = double.Parse(value);
        else
        {
            try { parsed = JsonSerializer.Deserialize(value, targetType, JsonOptions); }
            catch { parsed = null; }
        }

        if (parsed != null || Nullable.GetUnderlyingType(targetType) != null || !targetType.IsValueType)
            prop.SetValue(obj, parsed);
    }
}
