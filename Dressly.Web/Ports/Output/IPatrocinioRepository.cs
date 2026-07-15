using Dressly.Domain.Entities;

namespace Dressly.Application.Ports.Output;

public interface IPatrocinioRepository
{
    Task<Patrocinio?> GetByIdAsync(int id);
    Task<List<Patrocinio>> GetByEmpresaIdAsync(int empresaId);
    Task<List<Patrocinio>> GetByPuntoONGIdAsync(int puntoONGId);
    Task AddAsync(Patrocinio patrocinio);
    Task UpdateAsync(Patrocinio patrocinio);
}
