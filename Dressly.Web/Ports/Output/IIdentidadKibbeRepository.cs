using System;
using System.Collections.Generic;
using System.Text;
using Dressly.Domain.Entities;

namespace Dressly.Application.Ports.Output
{
    public interface IIdentidadKibbeRepository
    {
        Task<IdentidadKibbeInfo?> GetByIdAsync(int id);
        Task<List<IdentidadKibbeInfo>> GetAllAsync();
    }
}
