using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dressly.Domain.Entities;

namespace Dressly.Infrastructure.Repositories
{
    public interface INegocioPacaRepository {
        public Task<NegocioPaca?> GetByIdAsc(int id) => Task.FromResult<NegocioPaca?>(null);

        public Task<IEnumerable<NegocioPaca>> GetAllAsync() => Task.FromResult<IEnumerable<NegocioPaca>>([]);

        public Task<IEnumerable<NegocioPaca>> GetCercanosACategoriaAsync(string categoria, string coordenadasUsuario) => Task.FromResult<IEnumerable<NegocioPaca>>([]);
    }
}