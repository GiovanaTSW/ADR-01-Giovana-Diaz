<div align = "center">
    <h1>ADR-05-Giovana-Diaz</h1>
    <h1>ADR-05: Integración de Patrones de Diseño GOF en Dressly</h1>
</div>

---

| Campo  | Valor |
|--------|-------|
| Autor  | Giovana Ruby Díaz Anduze |
| Fecha  | 26/06/2026 |
| Estado | `Propuesto` |

---

## Contexto

El siguiente paso para este proyecto es enriquecer la infraestructura del sistema con patrones de diseño GOF (Gang of Four) que resuelvan problemas concretos de Dressly sin romper la separación entre dominio, aplicación e infraestructura.

Los tres problemas identificados fueron:

- **Creación de repositorios:** en `Program.cs` se necesita decidir en tiempo de ejecución qué implementación concreta (JSON o SQLite) instanciar según el entorno, sin que Application ni Domain lo sepan.
- **Notificaciones al crear prendas y outfits:** cuando el usuario crea una prenda o guarda un outfit, otros componentes del sistema (notificadores, loggers, futuros servicios) deben enterarse sin que `PrendaService` ni `OutfitService` dependan directamente de ellos.
- **Logging transversal en repositorios:** cada operación de repositorio debe registrarse (entrada y salida) sin modificar las implementaciones concretas de JSON, CSV ni SQLite.

---

## Decisión

Se integran tres patrones GOF, cada uno resolviendo uno de los problemas identificados:

### 1. Factory Method — `RepositoryFactory`

**Problema:** `Program.cs` necesita instanciar el repositorio correcto según el entorno (`Development` → JSON, `Production` → SQLite) sin que Application conozca las implementaciones concretas.

**Solución:** Una clase estática `RepositoryFactory` en `Dressly.Infrastructure` expone métodos de creación por entidad. Recibe el nombre del entorno y un `IServiceProvider`, y devuelve la implementación adecuada a través del puerto de salida.
