using Dressly.Application.Ports.Output;
using Dressly.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Dressly.Infrastructure.Repositories;

public static class RepositoryFactory
{
    public static IPrendaRepository CreatePrendaRepository(string environment, IServiceProvider sp)
    {
        if (environment == "Production")
        {
            var db = sp.GetRequiredService<SqliteDbContext>();
            return new SqlitePrendaRepository(db);
        }
        return new PrendaRepository();
    }

    public static IUsuarioRepository CreateUsuarioRepository(string environment, IServiceProvider sp)
    {
        if (environment == "Production")
        {
            var db = sp.GetRequiredService<SqliteDbContext>();
            return new SqliteUsuarioRepository(db);
        }
        return new UsuarioRepository();
    }

    public static IOutfitRepository CreateOutfitRepository(string environment, IServiceProvider sp)
    {
        if (environment == "Production")
        {
            var db = sp.GetRequiredService<SqliteDbContext>();
            return new SqliteOutfitRepository(db);
        }
        return new OutfitRepository();
    }

    public static IDonacionRepository CreateDonacionRepository(string environment, IServiceProvider sp)
    {
        if (environment == "Production")
        {
            var db = sp.GetRequiredService<SqliteDbContext>();
            return new SqliteDonacionRepository(db);
        }
        return new DonacionRepository();
    }

    public static INegocioPacaRepository CreateNegocioPacaRepository(string environment, IServiceProvider sp)
    {
        if(environment == "Production")
        {
            var db = sp.GetRequiredService<SqliteDbContext>();
            return new SqliteNegocioPacaRepository(db);
        }
        return new CsvNegocioPacaRepository();
    }
}
