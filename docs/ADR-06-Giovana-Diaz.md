<div align = "center">
    <h1>ADR-06-Giovana-Diaz</h1>
    <h1>ADR-06: Nuevas implementaciones para el modelo de negocio</h1>
</div>

---

| Campo  | Valor |
|--------|-------|
| Autor  | Giovana Ruby Díaz Anduze |
| Fecha  | 08/07/2026 |
| Estado | `Propuesto` |

---

> 📎 Diagramas C4 (Contexto, Contenedores y Componentes) que ilustran esta decisión: [`diagramasC4.md`](./diagramasC4.md)

---

## Contexto

Dressly es un proyecto que busca implementar ideas para aportar a la economía circular y cuestiones sociales; sin embargo, a pesar que el proyecto es una buena idea hace falta un modelo de negocio viable y sostenible.

Mediante investigación, se pensaron en 4 ideas que actúan como posibles pilares para el proyecto: (1) suscripción premium, (2) publicidad de negocios de ropa de paca, (3) patrocinio corporativo de empresas hacia puntos ONG con trazabilidad fiscal, y (4) trueque/intercambio de prendas entre usuarios. En un principio, se optó por construir en código solo los Pilares 1 y 3 debido a que son los que tienen un menor costo de implementación; sin embargo, al ser un proyecto con una iniciativa nueva, es importante recalcar que es complicado conseguir patrocinios y la cuestión de que el público conozca acerca de esta iniciativa.

Es mediante a esto, y hacer una consulta con mi mentor Jorge Pedrozo, se decidió cambiar el alcance: se construirán en código los **Pilares 2, 3 y 4**, dejando fuera el Pilar 1 (suscripción premium) por ahora. Esta decisión implica renunciar al pilar más barato de construir (el gate de autorización sobre casos de uso ya existentes) a cambio de tres pilares que sí introducen dominio nuevo: un directorio de negocios locales, un modelo de patrocinio/trazabilidad y un sistema completo de intercambio entre usuarios.

Las restricciones siguen siendo las mismas que en ADRs anteriores: tiempo académico limitado, stack .NET ya establecido, y la condición de no romper la arquitectura hexagonal ni los patrones GOF ya integrados en ADR-05.

Por otro lado, se busca hacerle mejoras al proyecto dejando de lado una parte más genérica y enfocándome en la parte del perfil del usuario, pues se busca implementar nuevos patrones de implementación al momento de que el usuario decida qué tipo de prendas sea la cuál más le acomode todo esto basado en un sistema llamado kibbe

