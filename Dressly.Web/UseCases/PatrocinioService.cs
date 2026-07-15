using Dressly.Domain.Entities;
using Dressly.Application.Ports.Input;
using Dressly.Application.Ports.Output;

namespace Dressly.Application.UseCases;

public class PatrocinioService : IPatrocinioService
{
    private readonly IPatrocinioRepository _patrocinios;
    private readonly IEmpresaRepository _empresas;
    private readonly IDonacionRepository _donaciones;

    public PatrocinioService(
        IPatrocinioRepository patrocinios,
        IEmpresaRepository empresas,
        IDonacionRepository donaciones)
    {
        _patrocinios = patrocinios;
        _empresas = empresas;
        _donaciones = donaciones;
    }

    public async Task<Patrocinio> RegistrarPatrocinioAsync(int empresaId, int puntoONGId, decimal monto)
    {
        var patrocinio = new Patrocinio
        {
            EmpresaId = empresaId,
            PuntoONGId = puntoONGId,
            FechaInicio = DateTime.Now,
            Monto = monto,
            Activo = true
        };
        await _patrocinios.AddAsync(patrocinio);
        return patrocinio;
    }

    public Task<List<Patrocinio>> ListarPatrociniosPorEmpresaAsync(int empresaId)
        => _patrocinios.GetByEmpresaIdAsync(empresaId);

    public async Task FinalizarPatrocinioAsync(int patrocinioId)
    {
        var patrocinio = await _patrocinios.GetByIdAsync(patrocinioId);
        if (patrocinio != null)
        {
            patrocinio.Activo = false;
            patrocinio.FechaFin = DateTime.Now;
            await _patrocinios.UpdateAsync(patrocinio);
        }
    }
}
