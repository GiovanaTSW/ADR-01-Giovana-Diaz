<div align = "center">
  <h1>ADR-07-Giovana-Diaz</h1>
  <h1>ADR-07: Deudas técnicas identificadas en Dressly</h1>
</div>

---

| Campo  | Valor |
|--------|-------|
| Autor  | Giovana Ruby Díaz Anduze |
| Fecha  | 15/07/2026 |
| Estado | `Propuesto` |

---

# Contexto
Al hacer una instrospección profunda acerca del proyecto, se ha identificado dos puntos críticos de deuda técnica. Estos elementos no representan nuevas funcionalidades, sino decisiones de diseño/implementación que impactan la mantenibilidad, seguridad y escalabilidad del sistema. Es por eso que se ha optado por crear este nuevo ADR en el que se describe la forma en la que ocurre las deudas técnicas.

---

## Deuda #1: Credenciales de seed hardcodeadas

*Archivo:* Dressly.Web/UseCases/AuthService.cs  
*Línea:* Aproximadamente línea 80, dentro de SeedDefaultUserAsync().

csharp
var hash = BCrypt.Net.BCrypt.HashPassword("123456");
var usuario = new Usuario
{
    Nombre = "Giovana Díaz",
    Email = "giovana@dressly.com",
    PasswordHash = hash
};


El nombre, email y password del usuario semilla están escritos como literales en el código de producción. Esto impide cambiar las credenciales sin recompilar, y expone la contraseña por defecto en el repositorio.

*Solución acordada:*  
Mover las tres credenciales a appsettings.json bajo la sección SeedUser y leerlas por inyección de IConfiguration en AuthService. La contraseña por defecto (123456) sigue siendo un valor por defecto débil, pero al menos ahora es configurable sin recompilar.


---

### ¿Por qué?

Modelar cada pilar como una entidad y un puerto propios — en vez de forzarlos dentro de entidades existentes — mantiene el principio ya establecido en ADR-03/ADR-05: el dominio crece por adición, no por modificación de lo que ya funciona. `NegocioPaca` no reutiliza `PuntoONG` porque, aunque ambos son "lugares externos", responden a relaciones de negocio distintas (publicidad pagada vs. donación altruista). Igual, `Intercambio` no se modela como una extensión de `LoteDonacion` porque el trueque tiene un ciclo de vida transaccional (propuesta → aceptación) que la donación no tiene — mezclarlos habría forzado un estado y una semántica que no le corresponden a `LoteDonacion`.

El reporte de trazabilidad se apoya directamente en las tablas de dominio que ya existen (`PuntoONG`, `LoteDonacion`), por lo que es la pieza más barata de las tres — solo agrega una capa de consulta/agregación, no un nuevo flujo transaccional.

La identidad Kibbe se modela aparte de `TipoCuerpo` porque el propio sistema Kibbe advierte que confundir forma con identidad es el error más común al aplicarlo — son dos preguntas distintas ("qué silueta tengo" vs. "qué líneas me favorecen"). El eje de Saturación es aditivo para no romper `DetectarEstacion()` ni forzar una migración de datos existentes. El Strategy de combinación cromática resuelve el mismo problema de acoplamiento que Factory Method, Observer y Decorator ya resolvieron en ADR-05: una regla única que no puede crecer sin volverse un bloque de condicionales.

### Alternativas consideradas
