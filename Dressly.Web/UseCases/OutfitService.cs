using Dressly.Domain.Entities;
using Dressly.Domain.Events;
using Dressly.Domain.DomainServices;
using Dressly.Application.Ports.Input;
using Dressly.Application.Ports.Output;

namespace Dressly.Application.UseCases;

public class OutfitService : IOutfitService
{
    private readonly IOutfitRepository _outfits;
    private readonly IPrendaRepository _prendas;
    private readonly IColorimetriaService _colorimetria;
    private readonly IUsuarioRepository _usuarios;
    private readonly IPerfilService _perfil;
    private readonly IPerfilConocimientoService _conocimiento;
    private readonly List<IEventObserver<OutfitGeneradoEvent>> _outfitObservers = new();

    public OutfitService(
        IOutfitRepository outfits,
        IPrendaRepository prendas,
        IColorimetriaService colorimetria,
        IUsuarioRepository usuarios,
        IPerfilService perfil,
        IPerfilConocimientoService conocimiento)
    {
        _outfits = outfits;
        _prendas = prendas;
        _colorimetria = colorimetria;
        _usuarios = usuarios;
        _perfil = perfil;
        _conocimiento = conocimiento;
    }

    public void SubscribeOutfitGenerado(IEventObserver<OutfitGeneradoEvent> observer)
        => _outfitObservers.Add(observer);

    public Task<List<Outfit>> GetOutfitsAsync(int usuarioId)
        => _outfits.GetByUsuarioIdAsync(usuarioId);

    public Task<Outfit?> GetByIdAsync(int id)
        => _outfits.GetByIdAsync(id);

    public async Task<List<Prenda>> GenerarSugerenciaAsync(int usuarioId, string ocasion)
    {
        var disponibles = await _prendas.GetDisponiblesAsync(usuarioId);
        var perfil = await _perfil.GetPerfilAsync(usuarioId);
        var colorInfo = _conocimiento.ObtenerInfoColorimetria(perfil?.Colorimetria);

        var coloresPaleta = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (colorInfo != null)
        {
            foreach (var c in colorInfo.ColoresPrincipales) coloresPaleta.Add(c.Key);
            foreach (var c in colorInfo.ColoresComplementarios) coloresPaleta.Add(c.Key);
            foreach (var c in colorInfo.ColoresNeutros) coloresPaleta.Add(c.Key);
        }

        var outfits = await _outfits.GetByUsuarioIdAsync(usuarioId);
        var idsEnOutfits = outfits.SelectMany(o => o.PrendaIds).ToHashSet();

        var prioridad = ocasion?.ToLower() switch
        {
            "formal" => new[] { "Superior", "Inferior", "Calzado", "Accesorio" },
            "deportivo" => new[] { "Calzado", "Inferior", "Superior", "Accesorio" },
            "playa" => new[] { "Superior", "Inferior", "Calzado", "Accesorio" },
            "fiesta" => new[] { "Superior", "Inferior", "Calzado", "Accesorio" },
            "trabajo" => new[] { "Superior", "Inferior", "Calzado", "Accesorio" },
            _ => new[] { "Superior", "Inferior", "Calzado", "Accesorio" }
        };

        var sugerencia = new List<Prenda>();
        var rng = new Random();
        foreach (var cat in prioridad)
        {
            var candidatas = disponibles
                .Where(p => p.Categoria == cat)
                .Select(p => new
                {
                    Prenda = p,
                    Score = Math.Max(CalcularPuntaje(p, sugerencia, coloresPaleta)
                        - (idsEnOutfits.Contains(p.Id) ? 20 : 0), 1)
                })
                .ToList();

            if (candidatas.Count == 0) continue;

            List<(Prenda Prenda, int Score)> elegibles;
            if (sugerencia.Count > 0)
            {
                var ultima = sugerencia.Last();
                elegibles = candidatas
                    .Where(x => _colorimetria.SonCompatibles(ultima.Color, x.Prenda.Color))
                    .Select(x => (x.Prenda, x.Score))
                    .ToList();
            }
            else
            {
                elegibles = candidatas
                    .Select(x => (x.Prenda, x.Score))
                    .ToList();
            }

            if (elegibles.Count == 0)
            {
                elegibles = candidatas
                    .Select(x => (x.Prenda, x.Score))
                    .ToList();
            }

            var topN = elegibles
                .OrderByDescending(x => x.Score)
                .Take(3)
                .ToList();

            sugerencia.Add(topN[rng.Next(topN.Count)].Prenda);
        }

        return sugerencia;
    }

    private int CalcularPuntaje(Prenda p, List<Prenda> seleccionadas, HashSet<string> coloresPaleta)
    {
        int score = 50;

        if (coloresPaleta.Contains(p.Color))
            score += 15;

        if (seleccionadas.Any())
        {
            var ultima = seleccionadas.Last();
            if (_colorimetria.SonCompatibles(ultima.Color, p.Color))
                score += 15;
        }

        var mes = DateTime.Now.Month;
        var estacionActual = mes switch
        {
            3 or 4 or 5 => "Primavera",
            6 or 7 or 8 => "Verano",
            9 or 10 or 11 => "Otoño",
            _ => "Invierno"
        };
        if (p.Estacion == "Todo el año" || p.Estacion == estacionActual)
            score += 10;

        if (p.VecesUsada <= 2)
            score += 5;

        return score;
    }

    public async Task<Outfit> GuardarOutfitAsync(int usuarioId, string nombre, string ocasion, List<int> prendaIds)
    {
        var todas = await _prendas.GetAllAsync();
        var descripcion = string.Join(", ", prendaIds
            .Select(id => todas.FirstOrDefault(p => p.Id == id)?.Nombre ?? $"#{id}"));

        var outfit = new Outfit
        {
            Id = await _outfits.GetNextIdAsync(),
            Nombre = nombre,
            Ocasion = ocasion,
            PrendaIds = prendaIds,
            Descripcion = descripcion,
            UsuarioId = usuarioId,
            FechaCreacion = DateTime.Now
        };

        foreach (var prenda in todas.Where(p => prendaIds.Contains(p.Id)))
        {
            prenda.VecesUsada++;
            prenda.FechaUltimoUso = DateTime.Now;
        }

        await _prendas.SaveAsync(todas);
        await _outfits.AddAsync(outfit);

        var evento = new OutfitGeneradoEvent(usuarioId, outfit.Id, outfit.Nombre, DateTime.Now);
        foreach (var obs in _outfitObservers)
            await obs.HandleAsync(evento);

        return outfit;
    }

    public async Task<List<Prenda>> GetPrendasByOutfitAsync(Outfit outfit)
    {
        var todas = await _prendas.GetAllAsync();
        return outfit.PrendaIds
            .Select(id => todas.FirstOrDefault(p => p.Id == id))
            .Where(p => p != null)
            .Cast<Prenda>()
            .ToList();
    }

    public async Task EliminarOutfitAsync(int id)
    {
        await _outfits.DeleteAsync(id);
    }
}
