# ADR-07: Deuda técnica identificada en Dressly

**Estado:** Pendiente de resolución  
**Fecha:** 2026-07-15  
**Contexto:** Durante el análisis del código existente se identificaron dos focos de deuda técnica que no pertenecen a nuevas funcionalidades sino a código legacy mal escrito o mal configurado.

---

## Deuda #1: Credenciales de seed hardcodeadas

**Archivo:** `Dressly.Web/UseCases/AuthService.cs`  
**Línea:** Aproximadamente línea 80, dentro de `SeedDefaultUserAsync()`.

```csharp
var hash = BCrypt.Net.BCrypt.HashPassword("123456");
var usuario = new Usuario
{
    Nombre = "Giovana Díaz",
    Email = "giovana@dressly.com",
    PasswordHash = hash
};
```

El nombre, email y password del usuario semilla están escritos como literales en el código de producción. Esto impide cambiar las credenciales sin recompilar, y expone la contraseña por defecto en el repositorio.

**Solución acordada:**  
Mover las tres credenciales a `appsettings.json` bajo la sección `SeedUser` y leerlas por inyección de `IConfiguration` en `AuthService`. La contraseña por defecto (`123456`) sigue siendo un valor por defecto débil, pero al menos ahora es configurable sin recompilar.

---

## Deuda #2: Long method en OutfitService

**Archivo:** `Dressly.Web/UseCases/OutfitService.cs`  
**Método:** `GenerarSugerenciaAsync` (~70 líneas)

El método `GenerarSugerenciaAsync` mezcla tres responsabilidades distintas en un solo bloque:
1. Construcción de la paleta de colores a partir del perfil del usuario.
2. Determinación del orden de prioridad de categorías según la ocasión.
3. Selección probabilística de prendas dentro de cada categoría, con puntuación y filtro de compatibilidad cromática.

**Solución acordada:**  
Extraer tres métodos privados sin cambiar la lógica de negocio: `ConstruirPaletaColores`, `ObtenerPrioridadPorOcasion` y `SeleccionarPrendaParaCategoria`. El método original queda como un orchestrator que delega en estos tres.
