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

    classDef persona fill:#E1F5EE,stroke:#0F6E56,color:#04342C
    classDef sistema fill:#EEEDFE,stroke:#534AB7,color:#26215C
    classDef externo fill:#FAECE7,stroke:#993C1D,color:#4A1B0C

    class USR,EMP persona
    class DR sistema
    class NEG,ONG externo
```

