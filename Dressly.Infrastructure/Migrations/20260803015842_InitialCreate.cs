using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dressly.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Empresas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RazonSocial = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    RFC = table.Column<string>(type: "TEXT", maxLength: 13, nullable: false),
                    EstatusDonatariaAutorizada = table.Column<bool>(type: "INTEGER", nullable: false),
                    Telefono = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Direccion = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IdentidadesKibbe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    TipoEnergia = table.Column<string>(type: "TEXT", nullable: false),
                    DescripcionFisica = table.Column<string>(type: "TEXT", nullable: false),
                    LineasRecomendadas = table.Column<string>(type: "TEXT", nullable: false),
                    LineasNoRecomendadas = table.Column<string>(type: "TEXT", nullable: false),
                    TelasSugeridas = table.Column<string>(type: "TEXT", nullable: false),
                    Evitar = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentidadesKibbe", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Intercambios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UsuarioOfertanteId = table.Column<int>(type: "INTEGER", nullable: false),
                    UsuarioInteresadoId = table.Column<int>(type: "INTEGER", nullable: true),
                    PrendaOfertadaId = table.Column<int>(type: "INTEGER", nullable: false),
                    PrendaInteresadoId = table.Column<int>(type: "INTEGER", nullable: true),
                    Estado = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Comision = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Intercambios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NegociosPaca",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Direccion = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    CategoriaPrenda = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Coordenadas = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Telefono = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NegociosPaca", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Outfits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Ocasion = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    PrendaIdsJson = table.Column<string>(type: "TEXT", nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    UsuarioId = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Outfits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Patrocinios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmpresaId = table.Column<int>(type: "INTEGER", nullable: false),
                    PuntoONGId = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patrocinios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PuntosONG",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Direccion = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    Latitud = table.Column<double>(type: "REAL", nullable: false),
                    Longitud = table.Column<double>(type: "REAL", nullable: false),
                    Telefono = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PuntosONG", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Perfil_TipoCuerpo = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Perfil_TonoPiel = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Perfil_SubtonoPiel = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Perfil_IntensidadCabello = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Perfil_ColorOjos = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Perfil_Colorimetria = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Perfil_Contraste = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Perfil_Altura = table.Column<decimal>(type: "TEXT", nullable: true),
                    Perfil_KibbeInfoId = table.Column<int>(type: "INTEGER", nullable: true),
                    Perfil_FotoUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Perfil_Saturacion = table.Column<double>(type: "REAL", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuarios_IdentidadesKibbe_Perfil_KibbeInfoId",
                        column: x => x.Perfil_KibbeInfoId,
                        principalTable: "IdentidadesKibbe",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LotesDonacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PrendaIdsJson = table.Column<string>(type: "TEXT", nullable: false),
                    PuntoONGId = table.Column<int>(type: "INTEGER", nullable: false),
                    Estado = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UsuarioId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LotesDonacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LotesDonacion_PuntosONG_PuntoONGId",
                        column: x => x.PuntoONGId,
                        principalTable: "PuntosONG",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Prendas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Categoria = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Color = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Talla = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Estacion = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    FotoUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    VecesUsada = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaUltimoUso = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EnDesuso = table.Column<bool>(type: "INTEGER", nullable: false),
                    UsuarioId = table.Column<int>(type: "INTEGER", nullable: false),
                    EsDonada = table.Column<bool>(type: "INTEGER", nullable: false),
                    LoteId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prendas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prendas_LotesDonacion_LoteId",
                        column: x => x.LoteId,
                        principalTable: "LotesDonacion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Prendas_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LotesDonacion_PuntoONGId",
                table: "LotesDonacion",
                column: "PuntoONGId");

            migrationBuilder.CreateIndex(
                name: "IX_Prendas_LoteId",
                table: "Prendas",
                column: "LoteId");

            migrationBuilder.CreateIndex(
                name: "IX_Prendas_UsuarioId",
                table: "Prendas",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Perfil_KibbeInfoId",
                table: "Usuarios",
                column: "Perfil_KibbeInfoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Empresas");

            migrationBuilder.DropTable(
                name: "Intercambios");

            migrationBuilder.DropTable(
                name: "NegociosPaca");

            migrationBuilder.DropTable(
                name: "Outfits");

            migrationBuilder.DropTable(
                name: "Patrocinios");

            migrationBuilder.DropTable(
                name: "Prendas");

            migrationBuilder.DropTable(
                name: "LotesDonacion");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "PuntosONG");

            migrationBuilder.DropTable(
                name: "IdentidadesKibbe");
        }
    }
}
