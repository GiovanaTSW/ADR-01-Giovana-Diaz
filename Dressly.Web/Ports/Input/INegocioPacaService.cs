using Dressly.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dressly.Application.Ports.Input
{
    public interface INegocioPacaService
    {
        Task<NegocioPaca> RegistrarNegocioAsyn(NegocioPaca negocio);
        Task<IEnumerable<NegocioPaca>> ListarNegociosAsync();
    }
}
