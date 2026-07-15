using Dressly.Domain.Entities;

namespace Dressly.Application.Ports.Input;

public interface IPatrocinioService
{
    Task<Patrocinio> RegistrarPatrocinioAsync(int empresaId, int puntoONGId, decimal monto);
    Task<List<Patrocinio>> ListarPatrociniosPorEmpresaAsync(int empresaId);
    Task FinalizarPatrocinioAsync(int patrocinioId);
}
