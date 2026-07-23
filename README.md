<div align = "center">
    <h1>ADR-06-Giovana-Diaz</h1>
    <h1>ADR-06: Nuevas implementaciones para el modelo de negocio</h1>
</div>

---

| Campo  | Valor |
|--------|-------|
| Autor  | Giovana Ruby Díaz Anduze |
| Fecha  | 08/07/2026 |
| Estado | `Propuesto / Implementado` |

---

> 📎 Diagramas C4 (Contexto, Contenedores y Componentes) que ilustran esta decisión: [`diagramasC4.md`](./diagramasC4.md)

---

## Contexto

Dressly es un proyecto que busca implementar ideas para aportar a la economía circular y cuestiones sociales; sin embargo, a pesar que el proyecto es una buena idea hace falta un modelo de negocio viable y sostenible.

Mediante investigación, se pensaron en 4 ideas que actúan como posibles pilares para el proyecto: (1) suscripción premium, (2) publicidad de negocios de ropa de paca, (3) patrocinio corporativo de empresas hacia puntos ONG con trazabilidad fiscal, y (4) trueque/intercambio de prendas entre usuarios. En un principio, se optó por construir en código solo los Pilares 1 y 3 debido a que son los que tienen un menor costo de implementación; sin embargo, al ser un proyecto con una iniciativa nueva, es importante recalcar que es complicado conseguir patrocinios y la cuestión de que el público conozca acerca de esta iniciativa.

Es mediante a esto, y hacer una consulta con mi mentor Jorge Pedrozo, se decidió cambiar el alcance: se construirán en código los **Pilares 2, 3 y 4**, dejando fuera el Pilar 1 (suscripción premium) por ahora. Esta decisión implica renunciar al pilar más barato de construir (el gate de autorización sobre casos de uso ya existentes) a cambio de tres pilares que sí introducen dominio nuevo: un directorio de negocios locales, un modelo de patrocinio/trazabilidad y un sistema completo de intercambio entre usuarios.

Las restricciones siguen siendo las mismas que en ADRs anteriores: tiempo académico limitado, stack .NET ya establecido, y la condición de no romper la arquitectura hexagonal ni los patrones GOF ya integrados en ADR-05.

Por otro lado, se busca hacerle mejoras al proyecto dejando de lado una parte más genérica y enfocándome en la parte del perfil del usuario, pues se busca implementar nuevos patrones de implementación al momento de que el usuario decida qué tipo de prendas sea la cuál más le acomode todo esto basado en un sistema llamado kibbe

---

## Decisión

Se incorporan tres dominios nuevos al hexágono existente, cada uno resolviendo uno de los pilares:

**Pilar 2 — Publicidad de negocios de paca:** nueva entidad `NegocioPaca` (nombre, dirección, categoría de prenda, coordenadas, contacto) con su puerto de salida `INegocioPacaRepository`. `OutfitService` se extiende con una lógica de "prenda faltante": cuando detecta que a un outfit le falta una pieza para combinar, sugiere un `NegocioPaca` cercano en vez de una marca externa.

**Pilar 3 — Patrocinio corporativo vía ONGs:** nueva entidad `Empresa` (razón social, RFC, estatus de donataria autorizada) y una entidad `Patrocinio` que vincula una `Empresa` con uno o más `PuntoONG` ya existentes. Se agrega un caso de uso de **reporte de trazabilidad** (prendas donadas, lotes, usuarios impactados por punto ONG) que la empresa puede consultar como comprobante de su responsabilidad social.

**Pilar 4 — Trueque entre usuarios:** nueva entidad `Intercambio` con estado (`Publicado` → `Propuesto` → `Aceptado`/`Rechazado` → `Completado`), un nuevo caso de uso `IntercambioService`, y una comisión por transacción facilitada.

**Identidad Kibbe:** nueva entidad `IdentidadKibbeInfo` paralela a `TipoCuerpoInfo` (no reemplazo), con familia (Dramático, Natural, Clásico, Gamine, Romántico), líneas favorecedoras y prendas recomendadas/evitar. Campo nuevo en `PerfilFisico` para guardar la identidad Kibbe del usuario. `IPerfilConocimientoService` se extiende con `ObtenerInfoKibbe()`.

**Eje de Saturación:** campo nuevo en `PerfilFisico`, adicional a subtono y contraste, sin modificar `DetectarEstacion()` existente — deja base para un futuro sistema de 12 sub-estaciones sin rehacer las 4 paletas actuales.

**Strategy de combinación cromática:** nueva interfaz `IEstrategiaCombinacionColor` con implementaciones `EstrategiaMonocromatica`, `EstrategiaAnaloga`, `EstrategiaComplementaria` y `EstrategiaTriada`, que reemplazan la regla única de `SonCompatibles()`.

**Aseguramiento de Calidad y Pipeline CI:** Se implementa un conjunto de pruebas unitarias utilizando **xUnit** bajo el patrón **Arrange-Act-Assert (AAA)** cubriendo componentes clave del dominio y la infraestructura (`PrendaTests`, `UsuarioTests` y `SqlitePrendaRepositoryTests`). Asimismo, se automatiza la ejecución de dichas pruebas mediante un flujo de **Integración Continua (CI) con GitHub Actions**, asegurando el ciclo de validación de código ante cada cambio en la rama de desarrollo.


### ¿Por qué?

Modelar cada pilar como una entidad y un puerto propios — en vez de forzarlos dentro de entidades existentes — mantiene el principio ya establecido en ADR-03/ADR-05: el dominio crece por adición, no por modificación de lo que ya funciona. `NegocioPaca` no reutiliza `PuntoONG` porque, aunque ambos son "lugares externos", responden a relaciones de negocio distintas (publicidad pagada vs. donación altruista). Igual, `Intercambio` no se modela como una extensión de `LoteDonacion` porque el trueque tiene un ciclo de vida transaccional (propuesta → aceptación) que la donación no tiene — mezclarlos habría forzado un estado y una semántica que no le corresponden a `LoteDonacion`.

El reporte de trazabilidad se apoya directamente en las tablas de dominio que ya existen (`PuntoONG`, `LoteDonacion`), por lo que es la pieza más barata de las tres — solo agrega una capa de consulta/agregación, no un nuevo flujo transaccional.

La identidad Kibbe se modela aparte de `TipoCuerpo` porque el propio sistema Kibbe advierte que confundir forma con identidad es el error más común al aplicarlo — son dos preguntas distintas ("qué silueta tengo" vs. "qué líneas me favorecen"). El eje de Saturación es aditivo para no romper `DetectarEstacion()` ni forzar una migración de datos existentes. El Strategy de combinación cromática resuelve el mismo problema de acoplamiento que Factory Method, Observer y Decorator ya resolvieron en ADR-05: una regla única que no puede crecer sin volverse un bloque de condicionales.

Incorporar xUnit y GitHub Actions valida de forma automatizada que las reglas de negocio de los nuevos dominios mantengan la estabilidad del sistema mediante el ciclo de retroalimentación temprana (rojo/verde).

### Alternativas consideradas

| Alternativa | Por qué la descarté |
|-------------|---------------------|
| **Mantener la decisión original (Pilares 1 y 3)** | Se revisó la prioridad de negocio y se determinó que 2, 3 y 4 representan mejor el eje de economía circular del proyecto, aunque cueste más construirlos. |
| **Modelar el trueque como una extensión de `LoteDonacion` (con un campo "tipo: donación/trueque")** | Mezclaría dos ciclos de vida distintos (uno sin retorno, otro con negociación de dos partes) en una sola entidad, complicando el estado en vez de simplificarlo. |
| **Reutilizar `PuntoONG` para representar también los negocios de paca del Pilar 2** | Ambos son "ubicaciones externas", pero uno es donatario sin fines de lucro y el otro es un negocio con relación comercial de publicidad; combinarlos obligaría a agregar campos condicionales según el tipo, perdiendo claridad. |
| **Delegar el matching de "prenda faltante" (Pilar 2) a un servicio externo de anuncios** | No encaja con la arquitectura hexagonal ya construida ni con el control local del negocio (cerrar tratos directos con negocios de Mérida), y añadiría una dependencia externa innecesaria para el alcance académico. |
| **Reemplazar `TipoCuerpo` directamente por la identidad Kibbe** | Contradice la premisa del propio sistema Kibbe (forma ≠ identidad) y rompería lógica ya construida sobre la forma corporal. |
| **Implementar de una vez el sistema completo de 12 sub-estaciones (ej. Otoño profundo)** | Implica rehacer las 4 paletas ya construidas y migrar perfiles existentes; se descarta por alcance y tiempo académico. |
| **Seguir extendiendo `SonCompatibles()` con más condicionales** | Repetiría el mismo problema de acoplamiento que Factory Method, Observer y Decorator ya resolvieron en ADR-05 para otras capas del sistema. |

---

## Consecuencias

**✅ Lo que gano:**

- **Técnica:** los tres pilares y los tres componentes de perfil se integran como dominios/extensiones aditivas con sus propios puertos, sin tocar `PrendaService`, `UsuarioService`, `TipoCuerpoInfo`, `ColorimetriaInfo` ni la infraestructura de Factory/Decorator/Observer ya construida en ADR-05.
- **Proceso:** cada pilar y cada concepto de perfil puede desarrollarse y probarse de forma independiente (una rama o commit por dominio), lo que facilita mostrar avance incremental en vez de una sola entrega monolítica.

**⚠️ Lo que sacrifico o asumo:**

- **Limitación técnica:** al no implementar el Pilar 1, Dressly se queda sin el ingreso recurrente más predecible y sin el caso de uso más simple de mostrar (un gate de autorización); si se retoma después, deberá integrarse sin conflicto con los tres dominios nuevos. Además, los perfiles ya existentes tendrán `IdentidadKibbe` y `Saturacion` nulos hasta que el usuario actualice su perfil, por lo que la lógica que los consuma debe tolerar la ausencia de dato.
- **Deuda o riesgo:** los tres pilares elegidos son, en conjunto, más costosos de construir que la combinación anterior — introducen tres entidades nuevas, un caso de uso de reporte, y un sistema completo de estados de intercambio — lo que aumenta el riesgo de quedar incompleto si el tiempo antes de la exposición se reduce. Además, el Pilar 3 sigue dependiendo de que las ONGs reales tramiten su registro de donataria autorizada ante el SAT, algo que no se puede resolver solo con código. Del lado de perfil, quedan fuera de alcance deliberadamente los aspectos de cabello y maquillaje del documento original, por no pertenecer al dominio de guardarropa de Dressly.

---

## Cláusula de IA
En este documento se ha utilizado Claude para la corrección de errores, estructuración del ADR según el formato oficial del curso y sugerencias para la redacción. Todas las ideas y decisiones de diseño son propias de la autora y no fueron generadas por la IA.
