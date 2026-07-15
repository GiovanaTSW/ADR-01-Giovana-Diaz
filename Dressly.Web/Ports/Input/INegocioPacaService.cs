using Dressly.Domain.Entities;

namespace Dressly.Application.Ports.Input;

public interface INegocioPacaService
{
    Task<NegocioPaca> RegistrarNegocioAsync(NegocioPaca negocio);
    Task<IEnumerable<NegocioPaca>> ListarNegociosAsync();
}
