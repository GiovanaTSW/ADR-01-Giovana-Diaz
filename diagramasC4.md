# Diagramas C4 — Dressly

Arquitectura documentada como código (Mermaid), versionada en el repositorio. Refleja el hexágono base (ADR-03), los patrones GOF ya integrados (ADR-05) y la extensión del modelo de negocio con los **Pilares 2, 3 y 4** (ADR-06).

---

## Nivel 1 — Contexto

**Para quién es:** cualquier persona no técnica — el profesor, una empresa patrocinadora, un negocio local. No requiere saber nada de código.
**Pregunta que responde:** ¿Qué es Dressly y quién interactúa con él?

```mermaid
flowchart TD
    USR(["Usuario\nGestiona guardarropa, genera outfits,\ndona o intercambia prendas"])
    EMP(["Empresa Patrocinadora\nBusca deducir impuestos donando\ny necesita trazabilidad - Pilar 3"])
    NEG(["Negocio de Paca\nQuiere aparecer sugerido\ncuando falta una prenda - Pilar 2"])
    ONG(["Punto ONG\nDonataria autorizada por el SAT"])

    subgraph SIS["Dressly (Sistema)"]
        DR["Dressly\nGuardarropa, outfits, perfil,\ndonaciones, patrocinios e intercambios"]
    end

    USR -->|"usa via web/app"| DR
    USR -->|"propone/acepta intercambios - Pilar 4"| DR
    EMP -->|"consulta reporte de trazabilidad,\npatrocina puntos ONG"| DR
    NEG -->|"aparece como sugerencia\nde prenda faltante"| DR
    DR -->|"registra lotes de donacion"| ONG
```

---

## Nivel 2 — Contenedores

**Para quién es:** desarrolladores y arquitectos del equipo (o quien revise el repo técnicamente).
**Pregunta que responde:** ¿Cuáles son las piezas técnicas grandes de Dressly y cómo se comunican entre sí?

```mermaid
flowchart TD
    USR(["Usuario"])
    EMP(["Empresa Patrocinadora"])
    NEG(["Negocio de Paca"])

    subgraph SIS["Dressly"]
        WEB["Dressly\nASP.NET Core MVC\nInterfaz web tradicional"]
        API["Dressly.Api\nASP.NET Core Web API\nInterfaz REST"]
        APP["Dressly.Web\nPuertos + Casos de Uso\nCapa de aplicacion hexagonal"]
        DOM["Dressly.Domain\nEntidades, eventos,\nservicios de dominio"]
        INF["Dressly.Infrastructure\nAdaptadores: repos, decorators,\nfactory, notifications"]
        JSON[("Datos JSON/CSV\n(Development)")]
        SQLITE[("SQLite\n(Production)")]
    end

    USR -->|"HTTP/HTML"| WEB
    USR -->|"HTTP/JSON"| API
    EMP -->|"consulta reporte via HTTP"| API
    NEG -->|"registra/actualiza perfil via HTTP"| API
    WEB -->|"llama"| APP
    API -->|"llama"| APP
    APP -->|"usa reglas de"| DOM
    APP -->|"puertos de salida"| INF
    INF -->|"lee/escribe"| JSON
    INF -->|"lee/escribe"| SQLITE
```
