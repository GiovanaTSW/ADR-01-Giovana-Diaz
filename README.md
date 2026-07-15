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

## Deuda #2: Long method en OutfitService

*Archivo:* Dressly.Web/UseCases/OutfitService.cs  
*Método:* GenerarSugerenciaAsync (~70 líneas)

El método GenerarSugerenciaAsync mezcla tres responsabilidades distintas en un solo bloque:
1. Construcción de la paleta de colores a partir del perfil del usuario.
2. Determinación del orden de prioridad de categorías según la ocasión.
3. Selección probabilística de prendas dentro de cada categoría, con puntuación y filtro de compatibilidad cromática.

*Solución acordada:*  
Extraer tres métodos privados sin cambiar la lógica de negocio: ConstruirPaletaColores, ObtenerPrioridadPorOcasion y SeleccionarPrendaParaCategoria. El método original queda como un orchestrator que delega en estos tres.

---

### ¿Por qué?

Se ha decidido priorizar la seguridad en el ciclo de vida del desarrollo (DevSecOps). La práctica de incluir credenciales en el código fuente viola el principio de separación de configuración y código. Esta decisión permite que el equipo de desarrollo mantenga la flexibilidad de cambiar entornos sin comprometer la integridad del repositorio, evitando la exposición accidental de secretos en sistemas de control de versiones.

El objetivo es reducir la complejidad cognitiva y facilitar la testabilidad. Al aplicar el principio de responsabilidad única (SRP), garantizamos que cada nuevo requerimiento de estilo en Dressly pueda ser implementado como un método independiente. Esta decisión reduce drásticamente el costo de mantenimiento, ya que permite realizar pruebas unitarias aisladas para cada componente de la lógica, minimizando el riesgo de regresiones cuando el algoritmo de recomendación evolucione.

### Alternativas consideradas

| Alternativa | Por qué la descarté |
|-------------|---------------------|
| **Leer credenciales desde variables de entorno del SO** | Aunque es segura, complica la configuración local para nuevos desarrolladores y es menos intuitiva que el sistema de proveedores de configuración de .NET (`appsettings.json`). |
| **Uso de servicios externos (Azure/AWS Secrets Manager)** | Es una sobreingeniería excesiva para el alcance académico actual; se reserva como mejora para una etapa de despliegue en producción. |
| **Implementar el patrón Strategy para `GenerarSugerenciaAsync`** | Introduciría una complejidad estructural innecesaria en este momento; el uso de `Extract Method` resuelve el problema de legibilidad sin alterar la arquitectura. |
| **Mantener el método `GenerarSugerenciaAsync` como está** | El costo de mantenimiento y el riesgo de errores en la lógica de estilo superan el esfuerzo de realizar la refactorización ahora. |

---

## Consecuencias

**✅ Lo que gano:**

- **Técnica:** Al externalizar la configuración, el código queda limpio de secretos y preparado para diferentes entornos. Con la refactorización del *Long Method*, se logra cumplir con el principio de responsabilidad única (SRP), mejorando drásticamente la mantenibilidad y permitiendo pruebas unitarias aisladas para cada sub-proceso de sugerencia.
- **Proceso:** La separación de responsabilidades permite desarrollar y probar la lógica de estilo de forma independiente, facilitando un flujo de trabajo más ordenado donde cada método tiene un propósito claro.

**⚠️ Lo que sacrifico o asumo:**

- **Limitación técnica:** Al mover las credenciales a `appsettings.json`, el desarrollador debe asegurar que dicho archivo se agregue correctamente al `.gitignore` para no subir secretos al repositorio, lo cual es una responsabilidad adicional de configuración del entorno local.
- **Deuda o riesgo:** La refactorización del `OutfitService` implica una manipulación directa de la lógica de negocio central; existe un riesgo menor de introducir regresiones si no se cuenta con una cobertura de pruebas unitarias robusta antes de mover el código. Además, al tratarse de un sistema de recomendación, cualquier cambio pequeño en los métodos extraídos podría alterar sutilmente el resultado de las sugerencias, lo cual debe validarse.

---

## Cláusula de IA
En este documento se ha utilizado IA para la estructuración del ADR según el formato oficial del curso, la redacción técnica y la claridad en las propuestas de solución. Todas las identificaciones de deuda y decisiones de diseño son propias de la autora.
