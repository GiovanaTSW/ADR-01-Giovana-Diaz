using System;
using Dressly.Application.Ports.Output;
using Dressly.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Dressly.Infrastructure.Repositories;

public static class RepositoryFactory
{
    public static IPrendaRepository CreatePrendaRepository(string environment, IServiceProvider sp)
    {
        // Eliminamos el 'if' del entorno, ahora siempre resolvemos con SQLite
        var db = sp.GetRequiredService<SqliteDbContext>();
        return new SqlitePrendaRepository(db);
    }

    public static IUsuarioRepository CreateUsuarioRepository(string environment, IServiceProvider sp)
    {
        var db = sp.GetRequiredService<SqliteDbContext>();
        return new SqliteUsuarioRepository(db);
    }

    public static IOutfitRepository CreateOutfitRepository(string environment, IServiceProvider sp)
    {
        var db = sp.GetRequiredService<SqliteDbContext>();
        return new SqliteOutfitRepository(db);
    }

    public static IDonacionRepository CreateDonacionRepository(string environment, IServiceProvider sp)
    {
        var db = sp.GetRequiredService<SqliteDbContext>();
        return new SqliteDonacionRepository(db);
    }

    public static INegocioPacaRepository CreateNegocioPacaRepository(string environment, IServiceProvider sp)
    {
        var db = sp.GetRequiredService<SqliteDbContext>();
        return new SqliteNegocioPacaRepository(db);
    }
}