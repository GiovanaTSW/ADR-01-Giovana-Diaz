using Dressly.Domain.Entities;
using Dressly.Application.Ports.Input;
using Dressly.Application.Ports.Output;

namespace Dressly.Application.UseCases;

public class SeedService : ISeedService
{
    private readonly IPrendaRepository _prendas;
    private readonly IOutfitRepository _outfits;
    private readonly IDonacionRepository _donaciones;

    public SeedService(IPrendaRepository prendas, IOutfitRepository outfits, IDonacionRepository donaciones)
    {
        _prendas = prendas;
        _outfits = outfits;
        _donaciones = donaciones;
    }

    public async Task SeedUserDataAsync(int usuarioId)
    {
        var existentes = await _prendas.GetByUsuarioIdAsync(usuarioId);
        if (existentes.Count > 0) return;

        var blazer = new Prenda
        {
            Id = await _prendas.GetNextIdAsync(),
            Nombre = "Blazer Negro",
            Categoria = "Superior",
            Color = "negro",
            Talla = "M",
            Estacion = "Todo el año",
            UsuarioId = usuarioId,
            VecesUsada = 12,
            FechaUltimoUso = DateTime.Now.AddDays(-5)
        };
        await _prendas.AddAsync(blazer);

        var camisa = new Prenda
        {
            Id = await _prendas.GetNextIdAsync(),
            Nombre = "Camisa Blanca",
            Categoria = "Superior",
            Color = "blanco",
            Talla = "M",
            Estacion = "Todo el año",
            UsuarioId = usuarioId,
            VecesUsada = 8,
            FechaUltimoUso = DateTime.Now.AddDays(-3)
        };
        await _prendas.AddAsync(camisa);

        var jeans = new Prenda
        {
            Id = await _prendas.GetNextIdAsync(),
            Nombre = "Jeans Rectos",
            Categoria = "Inferior",
            Color = "azul marino",
            Talla = "8",
            Estacion = "Todo el año",
            UsuarioId = usuarioId,
            VecesUsada = 15,
            FechaUltimoUso = DateTime.Now.AddDays(-2)
        };
        await _prendas.AddAsync(jeans);

        var zapatos = new Prenda
        {
            Id = await _prendas.GetNextIdAsync(),
            Nombre = "Zapatos Tacón",
            Categoria = "Calzado",
            Color = "negro",
            Talla = "7",
            Estacion = "Todo el año",
            UsuarioId = usuarioId,
            VecesUsada = 5,
            FechaUltimoUso = DateTime.Now.AddDays(-10)
        };
        await _prendas.AddAsync(zapatos);

        var vestido = new Prenda
        {
            Id = await _prendas.GetNextIdAsync(),
            Nombre = "Vestido Rojo",
            Categoria = "Vestido",
            Color = "rojo",
            Talla = "M",
            Estacion = "Verano",
            UsuarioId = usuarioId,
            VecesUsada = 3,
            FechaUltimoUso = DateTime.Now.AddDays(-20)
        };
        await _prendas.AddAsync(vestido);

        var collar = new Prenda
        {
            Id = await _prendas.GetNextIdAsync(),
            Nombre = "Collar Dorado",
            Categoria = "Accesorio",
            Color = "dorado",
            Talla = "Única",
            Estacion = "Todo el año",
            UsuarioId = usuarioId,
            VecesUsada = 7,
            FechaUltimoUso = DateTime.Now.AddDays(-8)
        };
        await _prendas.AddAsync(collar);

        var polo = new Prenda
        {
            Id = await _prendas.GetNextIdAsync(),
            Nombre = "Polo Coral",
            Categoria = "Superior",
            Color = "coral",
            Talla = "M",
            Estacion = "Primavera",
            UsuarioId = usuarioId,
            VecesUsada = 2,
            FechaUltimoUso = DateTime.Now.AddMonths(-4)
        };
        await _prendas.AddAsync(polo);

        var falda = new Prenda
        {
            Id = await _prendas.GetNextIdAsync(),
            Nombre = "Falda Lápiz",
            Categoria = "Inferior",
            Color = "negro",
            Talla = "8",
            Estacion = "Todo el año",
            UsuarioId = usuarioId,
            VecesUsada = 6,
            FechaUltimoUso = DateTime.Now.AddDays(-15)
        };
        await _prendas.AddAsync(falda);

        var sandalias = new Prenda
        {
            Id = await _prendas.GetNextIdAsync(),
            Nombre = "Sandalias Beige",
            Categoria = "Calzado",
            Color = "beige",
            Talla = "7",
            Estacion = "Verano",
            UsuarioId = usuarioId,
            VecesUsada = 1,
            FechaUltimoUso = DateTime.Now.AddMonths(-6)
        };
        await _prendas.AddAsync(sandalias);

        var reloj = new Prenda
        {
            Id = await _prendas.GetNextIdAsync(),
            Nombre = "Reloj Plateado",
            Categoria = "Accesorio",
            Color = "plateado",
            Talla = "Única",
            Estacion = "Todo el año",
            UsuarioId = usuarioId,
            VecesUsada = 10,
            FechaUltimoUso = DateTime.Now.AddDays(-1)
        };
        await _prendas.AddAsync(reloj);

        var points = await _donaciones.GetPuntosONGAsync();
        if (points.Count == 0)
        {
            // Puntos ONG de ejemplo vendran del JSON
        }
    }
}
