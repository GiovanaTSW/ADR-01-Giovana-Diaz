using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dressly.Domain.Entities;

namespace Dressly.Infrastructure.Repositories
{
    public class CsvNegocioPacaRepository : INegocioPacaRepository
    {
        // Línea 10 Corregida: Ahora devuelve un null compatible con NegocioPaca?
        public Task<NegocioPaca?> GetByIdAsc(int id) => Task.FromResult<NegocioPaca?>(null);

        // Agregado: El método que faltaba para cumplir con el contrato de la interfaz
        public Task<IEnumerable<NegocioPaca>> GetAllAsync() => Task.FromResult<IEnumerable<NegocioPaca>>([]);

        // Corregida: Eliminado el ")" extra que rompía la sintaxis al final
        public Task<IEnumerable<NegocioPaca>> GetCercanosACategoriaAsync(string categoria, string coordenadasUsuario) => Task.FromResult<IEnumerable<NegocioPaca>>([]);
    }
}