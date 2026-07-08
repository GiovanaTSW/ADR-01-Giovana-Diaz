<div align = "center">
    <h1>ADR-06-Giovana-Diaz</h1>
    <h1>ADR-06: Extensión del Modelo de Negocio — Pilares 2, 3 y 4</h1>
</div>

---

| Campo  | Valor |
|--------|-------|
| Autor  | Giovana Ruby Díaz Anduze |
| Fecha  | 08/07/2026 |
| Estado | `Propuesto` |

---

> **Nota de versión:** este documento reemplaza una versión anterior de ADR-06 orientada a un sistema de identidad Kibbe, que se retomará en un ADR posterior. El contenido vigente de ADR-06 es el que sigue.

> 📎 Diagramas C4 (Contexto, Contenedores y Componentes) que ilustran esta decisión: [`diagramasC4.md`](./diagramasC4.md)

---

## Contexto

Dressly ya define un modelo de negocio de cuatro pilares posibles: (1) suscripción premium, (2) publicidad de negocios de ropa de paca, (3) patrocinio corporativo de empresas hacia puntos ONG con trazabilidad fiscal, y (4) trueque/intercambio de prendas entre usuarios. La decisión inicial fue construir en código solo los Pilares 1 y 3 por ser los de menor costo de implementación y mayor impacto argumental frente a la observación del profesor sobre depender de fondos gubernamentales.

Tras revisar de nuevo la prioridad, se decidió cambiar el alcance: se construirán en código los **Pilares 2, 3 y 4**, dejando fuera el Pilar 1 (suscripción premium) por ahora. Esta decisión implica renunciar al pilar más barato de construir (el gate de autorización sobre casos de uso ya existentes) a cambio de tres pilares que sí introducen dominio nuevo: un directorio de negocios locales, un modelo de patrocinio/trazabilidad y un sistema completo de intercambio entre usuarios.

Las restricciones siguen siendo las mismas que en ADRs anteriores: tiempo académico limitado, stack .NET ya establecido, y la condición de no romper la arquitectura hexagonal ni los patrones GOF ya integrados en ADR-05.

---

## Decisión

Se incorporan tres dominios nuevos al hexágono existente, cada uno resolviendo uno de los pilares:

**Pilar 2 — Publicidad de negocios de paca:** nueva entidad `NegocioPaca` (nombre, dirección, categoría de prenda, coordenadas, contacto) con su puerto de salida `INegocioPacaRepository`. `OutfitService` se extiende con una lógica de "prenda faltante": cuando detecta que a un outfit le falta una pieza para combinar, sugiere un `NegocioPaca` cercano en vez de una marca externa.

**Pilar 3 — Patrocinio corporativo vía ONGs:** nueva entidad `Empresa` (razón social, RFC, estatus de donataria autorizada) y una entidad `Patrocinio` que vincula una `Empresa` con uno o más `PuntoONG` ya existentes. Se agrega un caso de uso de **reporte de trazabilidad** (prendas donadas, lotes, usuarios impactados por punto ONG) que la empresa puede consultar como comprobante de su responsabilidad social.

**Pilar 4 — Trueque entre usuarios:** nueva entidad `Intercambio` con estado (`Publicado` → `Propuesto` → `Aceptado`/`Rechazado` → `Completado`), un nuevo caso de uso `IntercambioService`, y una comisión por transacción facilitada.

### ¿Por qué?

Modelar cada pilar como una entidad y un puerto propios — en vez de forzarlos dentro de entidades existentes — mantiene el principio ya establecido en ADR-03/ADR-05: el dominio crece por adición, no por modificación de lo que ya funciona. `NegocioPaca` no reutiliza `PuntoONG` porque, aunque ambos son "lugares externos", responden a relaciones de negocio distintas (publicidad pagada vs. donación altruista). Igual, `Intercambio` no se modela como una extensión de `LoteDonacion` porque el trueque tiene un ciclo de vida transaccional (propuesta → aceptación) que la donación no tiene — mezclarlos habría forzado un estado y una semántica que no le corresponden a `LoteDonacion`.

El reporte de trazabilidad se apoya directamente en las tablas de dominio que ya existen (`PuntoONG`, `LoteDonacion`), por lo que es la pieza más barata de las tres — solo agrega una capa de consulta/agregación, no un nuevo flujo transaccional.

### Alternativas consideradas

| Alternativa | Por qué la descarté |
|-------------|---------------------|
| **Mantener la decisión original (Pilares 1 y 3)** | Se revisó la prioridad de negocio y se determinó que 2, 3 y 4 representan mejor el eje de economía circular del proyecto, aunque cueste más construirlos. |
| **Modelar el trueque como una extensión de `LoteDonacion` (con un campo "tipo: donación/trueque")** | Mezclaría dos ciclos de vida distintos (uno sin retorno, otro con negociación de dos partes) en una sola entidad, complicando el estado en vez de simplificarlo. |
| **Reutilizar `PuntoONG` para representar también los negocios de paca del Pilar 2** | Ambos son "ubicaciones externas", pero uno es donatario sin fines de lucro y el otro es un negocio con relación comercial de publicidad; combinarlos obligaría a agregar campos condicionales según el tipo, perdiendo claridad. |
| **Delegar el matching de "prenda faltante" (Pilar 2) a un servicio externo de anuncios** | No encaja con la arquitectura hexagonal ya construida ni con el control local del negocio (cerrar tratos directos con negocios de Mérida), y añadiría una dependencia externa innecesaria para el alcance académico. |

---

## Consecuencias

**✅ Lo que gano:**

- **Técnica:** los tres pilares se integran como dominios aditivos con sus propios puertos, sin tocar `PrendaService`, `UsuarioService` ni la infraestructura de Factory/Decorator/Observer ya construida en ADR-05.
- **Proceso:** cada pilar puede desarrollarse y probarse de forma independiente (una rama o commit por dominio), lo que facilita mostrar avance incremental en vez de una sola entrega monolítica.

**⚠️ Lo que sacrifico o asumo:**

- **Limitación técnica:** al no implementar el Pilar 1, Dressly se queda sin el ingreso recurrente más predecible y sin el caso de uso más simple de mostrar (un gate de autorización); si se retoma después, deberá integrarse sin conflicto con los tres dominios nuevos.
- **Deuda o riesgo:** los tres pilares elegidos son, en conjunto, más costosos de construir que la combinación anterior — introducen tres entidades nuevas, un caso de uso de reporte, y un sistema completo de estados de intercambio — lo que aumenta el riesgo de quedar incompleto si el tiempo antes de la exposición se reduce. Además, el Pilar 3 sigue dependiendo de que las ONGs reales tramiten su registro de donataria autorizada ante el SAT, algo que no se puede resolver solo con código.

---

## Diagrama

Ver [`diagramasC4.md`](./diagramasC4.md) — incluye Nivel 1 (Contexto), Nivel 2 (Contenedores) y Nivel 3 (Componentes) con los tres pilares reflejados.

---

## Cláusula de IA
En este documento se ha utilizado Claude para la corrección de errores, estructuración del ADR según el formato oficial del curso y sugerencias para la redacción. Todas las ideas y decisiones de diseño son propias de la autora y no fueron generadas por la IA.
