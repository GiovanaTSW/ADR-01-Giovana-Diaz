using System.Text.Json;
using Dressly.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Dressly.Infrastructure.Data;

public class SqliteDbContext : DbContext
{
    public SqliteDbContext(DbContextOptions<SqliteDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Prenda> Prendas => Set<Prenda>();
    public DbSet<Outfit> Outfits => Set<Outfit>();
    public DbSet<LoteDonacion> LotesDonacion => Set<LoteDonacion>();
    public DbSet<PuntoONG> PuntosONG => Set<PuntoONG>();
    public DbSet<IdentidadKibbeInfo> IdentidadesKibbe => Set<IdentidadKibbeInfo>();
    public DbSet<NegocioPaca> NegociosPaca => Set<NegocioPaca>();
    public DbSet<Empresa> Empresas => Set<Empresa>();
    public DbSet<Patrocinio> Patrocinios => Set<Patrocinio>();
    public DbSet<Intercambio> Intercambios => Set<Intercambio>();

    private static readonly JsonSerializerOptions JsonOptions = new();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var listIntConverter = new ValueConverter<List<int>, string>(
            v => JsonSerializer.Serialize(v, JsonOptions),
            v => JsonSerializer.Deserialize<List<int>>(v, JsonOptions) ?? new List<int>());

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuarios");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(200);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.PasswordHash).IsRequired();

            entity.OwnsOne(u => u.Perfil, perfil =>
            {
                perfil.Ignore(p => p.Id);
                perfil.Ignore(p => p.UsuarioId);
                perfil.Property(p => p.TipoCuerpo).HasMaxLength(50);
                perfil.Property(p => p.TonoPiel).HasMaxLength(50);
                perfil.Property(p => p.SubtonoPiel).HasMaxLength(50);
                perfil.Property(p => p.IntensidadCabello).HasMaxLength(50);
                perfil.Property(p => p.ColorOjos).HasMaxLength(50);
                perfil.Property(p => p.Colorimetria).HasMaxLength(50);
                perfil.Property(p => p.Contraste).HasMaxLength(50);
                perfil.Property(p => p.KibbeInfoId);
                perfil.Property(p => p.Altura);
                perfil.Property(p => p.FotoUrl).HasMaxLength(500);
                perfil.Property(p => p.Saturacion);

                perfil.HasOne(p => p.KibbeInfo)
                      .WithMany()
                      .HasForeignKey(p => p.KibbeInfoId);
            });

            entity.HasMany(u => u.Prendas)
                  .WithOne()
                  .HasForeignKey(p => p.UsuarioId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Prenda>(entity =>
        {
            entity.ToTable("Prendas");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(p => p.Categoria).HasMaxLength(50);
            entity.Property(p => p.Color).HasMaxLength(50);
            entity.Property(p => p.Talla).HasMaxLength(20);
            entity.Property(p => p.Estacion).HasMaxLength(50);
            entity.Property(p => p.FotoUrl).HasMaxLength(500);
        });

        modelBuilder.Entity<Outfit>(entity =>
        {
            entity.ToTable("Outfits");
            entity.HasKey(o => o.Id);
            entity.Property(o => o.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(o => o.Ocasion).HasMaxLength(50);
            entity.Property(o => o.Descripcion).HasMaxLength(500);
            entity.Property(o => o.PrendaIds)
                  .HasConversion(listIntConverter)
                  .HasColumnName("PrendaIdsJson");
        });

        modelBuilder.Entity<LoteDonacion>(entity =>
        {
            entity.ToTable("LotesDonacion");
            entity.HasKey(l => l.Id);
            entity.Property(l => l.Estado).IsRequired().HasMaxLength(20);
            entity.Property(l => l.PrendaIds)
                  .HasConversion(listIntConverter)
                  .HasColumnName("PrendaIdsJson");

            entity.HasOne<PuntoONG>()
                  .WithMany()
                  .HasForeignKey(l => l.PuntoONGId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PuntoONG>(entity =>
        {
            entity.ToTable("PuntosONG");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Nombre).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Direccion).HasMaxLength(300);
            entity.Property(p => p.Telefono).HasMaxLength(50);
        });

        modelBuilder.Entity<NegocioPaca>(entity =>
        {
            entity.ToTable("NegociosPaca");
            entity.HasKey(n => n.Id);
            entity.Property(n => n.Nombre).IsRequired().HasMaxLength(200);
            entity.Property(n => n.Direccion).HasMaxLength(300);
            entity.Property(n => n.CategoriaPrenda).HasMaxLength(50);
            entity.Property(n => n.Coordenadas).HasMaxLength(100);
            entity.Property(n => n.Telefono).HasMaxLength(50);
        });

        modelBuilder.Entity<Empresa>(entity =>
        {
            entity.ToTable("Empresas");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RazonSocial).IsRequired().HasMaxLength(200);
            entity.Property(e => e.RFC).IsRequired().HasMaxLength(13);
            entity.Property(e => e.Telefono).HasMaxLength(50);
            entity.Property(e => e.Direccion).HasMaxLength(300);
        });

        modelBuilder.Entity<Patrocinio>(entity =>
        {
            entity.ToTable("Patrocinios");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Monto).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<Intercambio>(entity =>
        {
            entity.ToTable("Intercambios");
            entity.HasKey(i => i.Id);
            entity.Property(i => i.Estado).HasMaxLength(20).HasConversion<string>();
            entity.Property(i => i.Comision).HasColumnType("decimal(18,2)");
        });
    }
}
