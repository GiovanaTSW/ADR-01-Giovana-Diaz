using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dressly.Application.Ports.Output;
using Dressly.Domain.Entities;
using Dressly.Infrastructure.Data;

namespace Dressly.Infrastructure.Repositories
{
    public class SqliteNegocioPacaRepository : INegocioPacaRepository
    {
        private readonly SqliteDbContext _db;

        public SqliteNegocioPacaRepository(SqliteDbContext db)
        {
            _db = db;
        }

        // Usamos el nombre que tu interfaz tiene actualmente para que no marque error
        public Task<NegocioPaca?> GetByIdAsc(int id) => Task.FromResult<NegocioPaca?>(null);

        public Task<IEnumerable<NegocioPaca>> GetAllAsync() => Task.FromResult<IEnumerable<NegocioPaca>>([]);

        public Task<IEnumerable<NegocioPaca>> GetCercanosACategoriaAsync(string categoria, string coordenadasUsuario) => Task.FromResult<IEnumerable<NegocioPaca>>([]);
    }
}
