using Dressly.Domain.Entities;

namespace Dressly.Application.Ports.Output;

public interface IEmpresaRepository
{
    Task<Empresa?> GetByIdAsync(int id);
    Task<List<Empresa>> GetAllAsync();
    Task AddAsync(Empresa empresa);
    Task UpdateAsync(Empresa empresa);
}
