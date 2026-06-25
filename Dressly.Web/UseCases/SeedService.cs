using Dressly.Domain.Entities;
using Dressly.Application.Ports.Input;
using Dressly.Application.Ports.Output;

namespace Dressly.Application.UseCases;

public class SeedService : ISeedService
{
    private readonly IDonacionRepository _donaciones;

    public SeedService(IDonacionRepository donaciones)
    {
        _donaciones = donaciones;
    }

    public async Task SeedUserDataAsync(int usuarioId)
    {
        var points = await _donaciones.GetPuntosONGAsync();
        if (points.Count == 0)
        {
            var puntosEjemplo = new List<PuntoONG>
            {
                new() { Id = 1, Nombre = "Cáritas México", Direccion = "Av. Reforma 222, CDMX", Telefono = "55-1234-5678", Latitud = 19.4326, Longitud = -99.1332 },
                new() { Id = 2, Nombre = "Banco de Ropa", Direccion = "Calle 5 de Mayo 100, Monterrey", Telefono = "81-8765-4321", Latitud = 25.6866, Longitud = -100.3161 },
                new() { Id = 3, Nombre = "Fundación Toca", Direccion = "Blvd. Kukulcán Km 4, Cancún", Telefono = "998-555-1234", Latitud = 21.1619, Longitud = -86.8515 },
            };
            foreach (var punto in puntosEjemplo)
                await _donaciones.AddPuntoONGAsync(punto);
        }
    }
}
