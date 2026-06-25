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
    }
}
