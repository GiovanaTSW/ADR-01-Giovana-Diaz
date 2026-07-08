# Diagramas C4 — Dressly

Arquitectura documentada como código (Mermaid), versionada en el repositorio. Refleja el hexágono base (ADR-03), los patrones GOF ya integrados (ADR-05) y la extensión del modelo de negocio con los **Pilares 2, 3 y 4** (ADR-06).

---

## Nivel 1 — Contexto

**Para quién es:** cualquier persona no técnica: una empresa patrocinadora, un negocio local. No requiere saber nada de código.
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

**Para quién es:** los desarrolladores y arquitectos del equipo o para quienes revisan el codigo de forma técnica.
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
---

## Nivel 3 — Componentes

**Para quién es:** quien va a modificar o revisar el código de la capa de aplicación (`Dressly.Web`) e infraestructura.
**Pregunta que responde:** ¿Qué hay dentro del hexágono — qué servicios, puertos y patrones GOF ya implementados soportan los Pilares 2, 3 y 4?

```mermaid
flowchart TD
    subgraph PuertosIn["Dressly.Web / Ports/Input"]
        IOS["IOutfitService"]
        IDS["IDonacionService"]
        INS["INegocioPacaService\n(NUEVO - Pilar 2)"]
        IPS3["IPatrocinioService\n(NUEVO - Pilar 3)"]
        IIS["IIntercambioService\n(NUEVO - Pilar 4)"]
    end

    subgraph UseCases["Dressly.Web / UseCases"]
        OS["OutfitService\n(extendido: sugiere NegocioPaca\ncuando falta prenda - Pilar 2)"]
        DS["DonacionService\n(existente)"]
        NPS["NegocioPacaService\n(NUEVO - Pilar 2)"]
        PTS["PatrocinioService\n(NUEVO - genera reporte\nde trazabilidad - Pilar 3)"]
        ITS["IntercambioService\n(NUEVO - maquina de estados\nPublicado-Propuesto-Aceptado-Completado - Pilar 4)"]
    end

    subgraph PuertosOut["Dressly.Web / Ports/Output"]
        IOR["IOutfitRepository"]
        IDR["IDonacionRepository"]
        INR["INegocioPacaRepository\n(NUEVO)"]
        IPR3["IPatrocinioRepository\n(NUEVO)"]
        IIR["IIntercambioRepository\n(NUEVO)"]
        IEO["IEventObserver<T>"]
    end

    subgraph Infra["Dressly.Infrastructure"]
        RF["RepositoryFactory\n(Factory Method - ADR-05)"]
        LOG["LoggingXRepository x N\n(Decorator - ADR-05)"]
        CN["ConsoleNotifier<T>\n(Observer - ADR-05)"]
    end

    IOS --> OS
    IDS --> DS
    INS --> NPS
    IPS3 --> PTS
    IIS --> ITS

    OS --> IOR
    OS --> INR
    DS --> IDR
    PTS --> IDR
    PTS --> IPR3
    NPS --> INR
    ITS --> IIR

    OS --> IEO
    DS --> IEO
    ITS --> IEO

    IOR --> RF
    IDR --> RF
    INR --> RF
    IPR3 --> RF
    IIR --> RF
    RF --> LOG
    IEO --> CN
```

---

### Notas de lectura

- Los componentes marcados **NUEVO** corresponden a los Pilares 2, 3 y 4 (ADR-06). El resto (`RepositoryFactory`, `LoggingXRepository`, `ConsoleNotifier<T>`) ya existía desde ADR-05 y se reutiliza sin modificación para los dominios nuevos.
- El Pilar 1 (suscripción premium) se dejó fuera de estos diagramas a propósito, según lo decidido en ADR-06.
- Diagramas escritos como código Mermaid dentro de este `.md`, con alias simples y sin bloques `rect`/`Note`, para que rendericen correctamente en GitHub.
