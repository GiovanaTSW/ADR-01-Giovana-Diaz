using Dressly.Application.Ports.Output;
using Dressly.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Dressly.Domain.Entities;

namespace Dressly.Infrastructure.Repositories
{
    public class SqliteIdentidadKibbeRepository : IIdentidadKibbeRepository
    {
        private readonly SqliteDbContext _context;
        public SqliteIdentidadKibbeRepository(SqliteDbContext context)
        {
            _context = context;
        }

        public async Task<IdentidadKibbeInfo?> GetByIdAsync(int id)
        {
            return await _context.IdentidadesKibbe.FirstOrDefaultAsync(k => k.Id == id);
        }
    }
}
