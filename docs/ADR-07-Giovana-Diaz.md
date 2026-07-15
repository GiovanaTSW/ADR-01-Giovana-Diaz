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

# Decisión
Se incorporan tres dominios nuevos al hexágono existente, cada uno resolviendo uno de los pilares:

**Pilar 2 — Publicidad de negocios de paca:** nueva entidad `NegocioPaca` (nombre, dirección, categoría de prenda, coordenadas, contacto) con su puerto de salida `INegocioPacaRepository`. `OutfitService` se extiende con una lógica de "prenda faltante": cuando detecta que a un outfit le falta una pieza para combinar, sugiere un `NegocioPaca` cercano en vez de una marca externa.

**Pilar 3 — Patrocinio corporativo vía ONGs:** nueva entidad `Empresa` (razón social, RFC, estatus de donataria autorizada) y una entidad `Patrocinio` que vincula una `Empresa` con uno o más `PuntoONG` ya existentes. Se agrega un caso de uso de **reporte de trazabilidad** (prendas donadas, lotes, usuarios impactados por punto ONG) que la empresa puede consultar como comprobante de su responsabilidad social.

**Pilar 4 — Trueque entre usuarios:** nueva entidad `Intercambio` con estado (`Publicado` → `Propuesto` → `Aceptado`/`Rechazado` → `Completado`), un nuevo caso de uso `IntercambioService`, y una comisión por transacción facilitada.

**Identidad Kibbe:** nueva entidad `IdentidadKibbeInfo` paralela a `TipoCuerpoInfo` (no reemplazo), con familia (Dramático, Natural, Clásico, Gamine, Romántico), líneas favorecedoras y prendas recomendadas/evitar. Campo nuevo en `PerfilFisico` para guardar la identidad Kibbe del usuario. `IPerfilConocimientoService` se extiende con `ObtenerInfoKibbe()`.

**Eje de Saturación:** campo nuevo en `PerfilFisico`, adicional a subtono y contraste, sin modificar `DetectarEstacion()` existente — deja base para un futuro sistema de 12 sub-estaciones sin rehacer las 4 paletas actuales.

**Strategy de combinación cromática:** nueva interfaz `IEstrategiaCombinacionColor` con implementaciones `EstrategiaMonocromatica`, `EstrategiaAnaloga`, `EstrategiaComplementaria` y `EstrategiaTriada`, que reemplazan la regla única de `SonCompatibles()`.

