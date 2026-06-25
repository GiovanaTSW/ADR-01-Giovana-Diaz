using System.Text.Json;
using Dressly.Domain.Entities;
using Dressly.Application.Ports.Output;

namespace Dressly_MVC.Repositories;

public class DonacionRepository : IDonacionRepository
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _options;

    public DonacionRepository()
    {
        _filePath = Path.Combine(Directory.GetCurrentDirectory(), "data", "donaciones.json");
        _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };
    }

    public async Task<List<LoteDonacion>> GetLotesByUsuarioIdAsync(int usuarioId)
    {
        var data = await GetDataAsync();
        return data.Lotes.Where(l => l.UsuarioId == usuarioId).ToList();
    }

    public async Task<LoteDonacion?> GetLoteByIdAsync(int id)
    {
        var data = await GetDataAsync();
        return data.Lotes.FirstOrDefault(l => l.Id == id);
    }

    public async Task<List<PuntoONG>> GetPuntosONGAsync()
    {
        var data = await GetDataAsync();
        return data.PuntosONG;
    }

    public async Task AddPuntoONGAsync(PuntoONG punto)
    {
        var data = await GetDataAsync();
        punto.Id = data.PuntosONG.Any() ? data.PuntosONG.Max(p => p.Id) + 1 : 1;
        data.PuntosONG.Add(punto);
        await SaveDataAsync(data);
    }

    public async Task AddLoteAsync(LoteDonacion lote)
    {
        var data = await GetDataAsync();
        lote.Id = data.Lotes.Any() ? data.Lotes.Max(l => l.Id) + 1 : 1;
        data.Lotes.Add(lote);
        await SaveDataAsync(data);
    }

    public async Task UpdateLoteAsync(LoteDonacion lote)
    {
        var data = await GetDataAsync();
        var index = data.Lotes.FindIndex(l => l.Id == lote.Id);
        if (index >= 0)
        {
            data.Lotes[index] = lote;
            await SaveDataAsync(data);
        }
    }

    public async Task DeleteLoteAsync(int loteId)
    {
        var data = await GetDataAsync();
        data.Lotes.RemoveAll(l => l.Id == loteId);
        await SaveDataAsync(data);
    }

    public async Task RemovePrendaFromLoteAsync(int loteId, int prendaId)
    {
        var data = await GetDataAsync();
        var lote = data.Lotes.FirstOrDefault(l => l.Id == loteId);
        if (lote == null) return;

        lote.PrendaIds.Remove(prendaId);
        await SaveDataAsync(data);
    }

    private async Task<DonacionData> GetDataAsync()
    {
        if (!File.Exists(_filePath)) return new();
        var json = await File.ReadAllTextAsync(_filePath);
        return JsonSerializer.Deserialize<DonacionData>(json, _options) ?? new();
    }

    private async Task SaveDataAsync(DonacionData data)
    {
        var dir = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
        var json = JsonSerializer.Serialize(data, _options);
        await File.WriteAllTextAsync(_filePath, json);
    }

    private class DonacionData
    {
        public List<LoteDonacion> Lotes { get; set; } = new();
        public List<PuntoONG> PuntosONG { get; set; } = new();
    }
}
