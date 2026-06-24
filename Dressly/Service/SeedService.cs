using Dressly_MVC.Models;
using Dressly_MVC.Repositories;

namespace Dressly_MVC.Services;

public class SeedService
{
    private readonly IPrendaRepository _prendas;
    private readonly IUsuarioRepository _usuarios;
    private readonly IOutfitRepository _outfits;
    private readonly IDonacionRepository _donaciones;

    public SeedService(
        IPrendaRepository prendas,
        IUsuarioRepository usuarios,
        IOutfitRepository outfits,
        IDonacionRepository donaciones)
    {
        _prendas = prendas;
        _usuarios = usuarios;
        _outfits = outfits;
        _donaciones = donaciones;
    }

    public async Task SeedUserDataAsync(int usuarioId)
    {
        var existentes = await _prendas.GetByUsuarioIdAsync(usuarioId);
        if (existentes.Any()) return;

        await SeedPrendasAsync(usuarioId);
        await SeedPerfilAsync(usuarioId);
        await SeedOutfitsAsync(usuarioId);
        await SeedPuntosONGAsync();
    }

    private async Task SeedPrendasAsync(int usuarioId)
    {
        var prendas = new List<Prenda>
        {
            new() { Id = 1, Nombre = "Blusa blanca de algodón", Categoria = "Superior", Color = "blanco", Talla = "M", Estacion = "Todo el año", UsuarioId = usuarioId, VecesUsada = 5, FechaUltimoUso = DateTime.Now.AddDays(-3) },
            new() { Id = 2, Nombre = "Jean azul clásico", Categoria = "Inferior", Color = "azul marino", Talla = "28", Estacion = "Todo el año", UsuarioId = usuarioId, VecesUsada = 12, FechaUltimoUso = DateTime.Now.AddDays(-1) },
            new() { Id = 3, Nombre = "Chamarra de cuero negra", Categoria = "Superior", Color = "negro", Talla = "M", Estacion = "Otoño", UsuarioId = usuarioId, VecesUsada = 8, FechaUltimoUso = DateTime.Now.AddDays(-15) },
            new() { Id = 4, Nombre = "Vestido floral primavera", Categoria = "Vestido", Color = "rosa palo", Talla = "M", Estacion = "Primavera", UsuarioId = usuarioId, VecesUsada = 3, FechaUltimoUso = DateTime.Now.AddDays(-20) },
            new() { Id = 5, Nombre = "Tenis blancos deportivos", Categoria = "Calzado", Color = "blanco", Talla = "24", Estacion = "Todo el año", UsuarioId = usuarioId, VecesUsada = 20, FechaUltimoUso = DateTime.Now.AddDays(-2) },
            new() { Id = 6, Nombre = "Sandalias color camel", Categoria = "Calzado", Color = "beige", Talla = "24", Estacion = "Verano", UsuarioId = usuarioId, VecesUsada = 2, FechaUltimoUso = DateTime.Now.AddDays(-60) },
            new() { Id = 7, Nombre = "Falda lápiz gris", Categoria = "Inferior", Color = "gris", Talla = "M", Estacion = "Todo el año", UsuarioId = usuarioId, VecesUsada = 6, FechaUltimoUso = DateTime.Now.AddDays(-10) },
            new() { Id = 8, Nombre = "Suéter beige de lana", Categoria = "Superior", Color = "beige", Talla = "M", Estacion = "Invierno", UsuarioId = usuarioId, VecesUsada = 4, FechaUltimoUso = DateTime.Now.AddDays(-45) },
            new() { Id = 9, Nombre = "Short vaquero", Categoria = "Inferior", Color = "azul marino", Talla = "28", Estacion = "Verano", UsuarioId = usuarioId, VecesUsada = 1, FechaUltimoUso = DateTime.Now.AddDays(-90) },
            new() { Id = 10, Nombre = "Blazer azul marino", Categoria = "Superior", Color = "azul marino", Talla = "M", Estacion = "Todo el año", UsuarioId = usuarioId, VecesUsada = 7, FechaUltimoUso = DateTime.Now.AddDays(-5) },
            new() { Id = 11, Nombre = "Vestido largo playero", Categoria = "Vestido", Color = "lavanda", Talla = "M", Estacion = "Verano", UsuarioId = usuarioId, VecesUsada = 0, FechaUltimoUso = DateTime.Now, EnDesuso = true },
            new() { Id = 12, Nombre = "Corbata de seda", Categoria = "Accesorio", Color = "rojo", Talla = "Única", Estacion = "Todo el año", UsuarioId = usuarioId, VecesUsada = 2, FechaUltimoUso = DateTime.Now.AddDays(-100) },
            new() { Id = 13, Nombre = "Pantalón de vestir negro", Categoria = "Inferior", Color = "negro", Talla = "30", Estacion = "Todo el año", UsuarioId = usuarioId, VecesUsada = 9, FechaUltimoUso = DateTime.Now.AddDays(-8) },
            new() { Id = 14, Nombre = "Chamarra deportiva", Categoria = "Superior", Color = "gris", Talla = "M", Estacion = "Todo el año", UsuarioId = usuarioId, VecesUsada = 3, FechaUltimoUso = DateTime.Now.AddDays(-30) },
            new() { Id = 15, Nombre = "Bufanda mostaza", Categoria = "Accesorio", Color = "mostaza", Talla = "Única", Estacion = "Invierno", UsuarioId = usuarioId, VecesUsada = 1, FechaUltimoUso = DateTime.Now.AddDays(-120), EnDesuso = true },
        };

        foreach (var p in prendas)
            await _prendas.AddAsync(p);
    }

    private async Task SeedPerfilAsync(int usuarioId)
    {
        var usuario = await _usuarios.GetByIdAsync(usuarioId);
        if (usuario == null) return;

        usuario.Perfil = new PerfilFisico
        {
            Id = usuarioId,
            TipoCuerpo = "Reloj de arena",
            TonoPiel = "medio",
            SubtonoPiel = "Cálido",
            IntensidadCabello = "Oscuro",
            ColorOjos = "Marrón",
            Colorimetria = "Otoño",
            Contraste = "Medio",
            Altura = 1.65m,
            UsuarioId = usuarioId
        };
        await _usuarios.UpdateAsync(usuario);
    }

    private async Task SeedOutfitsAsync(int usuarioId)
    {
        var outfits = new List<Outfit>
        {
            new() { Id = 1, Nombre = "Oficina elegante", Ocasion = "Trabajo", PrendaIds = new() { 10, 13, 5, 12 }, Descripcion = "Blazer azul marino + pantalón negro + tenis blancos + corbata roja", UsuarioId = usuarioId, FechaCreacion = DateTime.Now.AddDays(-20) },
            new() { Id = 2, Nombre = "Casual de fin de semana", Ocasion = "Casual", PrendaIds = new() { 1, 2, 5 }, Descripcion = "Blusa blanca + jean azul + tenis blancos", UsuarioId = usuarioId, FechaCreacion = DateTime.Now.AddDays(-15) },
            new() { Id = 3, Nombre = "Noche de fiesta", Ocasion = "Fiesta", PrendaIds = new() { 4, 6 }, Descripcion = "Vestido floral + sandalias camel", UsuarioId = usuarioId, FechaCreacion = DateTime.Now.AddDays(-5) },
        };

        foreach (var o in outfits)
            await _outfits.AddAsync(o);
    }

    private async Task SeedPuntosONGAsync()
    {
        var data = await _donaciones.GetDataAsync();
        if (data.PuntosONG.Any()) return;

        data.PuntosONG = new List<PuntoONG>
        {
            new() { Id = 1, Nombre = "Cáritas México", Direccion = "Av. Reforma 222, CDMX", Telefono = "55-1234-5678", Latitud = 19.4326, Longitud = -99.1332 },
            new() { Id = 2, Nombre = "Banco de Ropa", Direccion = "Calle 5 de Mayo 100, Monterrey", Telefono = "81-8765-4321", Latitud = 25.6866, Longitud = -100.3161 },
            new() { Id = 3, Nombre = "Fundación Toca", Direccion = "Blvd. Kukulcán Km 4, Cancún", Telefono = "998-555-1234", Latitud = 21.1619, Longitud = -86.8515 },
        };
        await _donaciones.SaveDataAsync(data);
    }
}
