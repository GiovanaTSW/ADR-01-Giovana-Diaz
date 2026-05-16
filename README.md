# ADR-01-Giovana-Diaz

# ADR-01: Selección del Estilo Arquitectónico en Capas para el Sistema: Dressly

| Campo  | Valor |
|--------|-------|
| Autor  | Giovana Ruby Díaz Anduze |
| Fecha  | 15/05/2026 |
| Estado | `Propuesto` |

---

## Contexto

Actualmente, muchas personas se responden una pregunta que se realizan todos los días: ¿Qué outfit me pondré hoy?, esto puede llegar a ser cansado para algunos, pues a pesar de contar con ropa suficiente dentro de su armario, ocurre ese sentimiento de no saber qué ponerse debido a que no recuerdan las prendas que tienen y no saben cómo combinarlas, sumándole el hecho de que muchos usuarios realizan compras innecesarias desconociendo datos acerca de su propio tipo de cuerpo y colorimetría, como al no saber qué tipos de cortes y tonos les favorecen, adquiriendo prendas que terminan sin un solo uso; esto formando parte del consumo desmedido e innecesario de ropa, y acumulación de desperdicio textil que pierde la oportunidad de ser aprovechado por otros. 

El proyecto busca resolver la pérdida de tiempo y el agobio que sentimos al elegir un outfit, dándonos sugerencias rápidas que ya no sea un proceso cansado; por otro lado, también ayuda en poner orden en el descontrol de prendas del armario, evitando que compremos ropa casi igual a la que ya tenemos solo porque no recordamos que está ahí, de igual manera, resuelve el problema de comprar por impulsividad prendas que no nos favorecen, ayudando a elegir prendas que realmente favorezcan nuestras características físicas. Finalmente, el proyecto ayuda a resolver un problema que persiste actualmente que es el hiperconsumismo de textiles, contribuyendo a la economía circular y facilitando a donarla a quiénes la necesiten y dándole un propósito mejor a lo que ya nos ponemos.

El proyecto va dirigido a personas que buscan una nueva forma de gestionar su guardarropa y buscan tener recomendaciones basadas en sus características físicas, así como personas comprometidas con la economía circular que requieren una vía eficiente para canalizar sus prendas en desuso.


---

## Restricciones 

Las restricciones de este proyecto académico, principalmente se cuenta con un tiempo limitado para la entrega y avances para el desarrollo del sistema; de igual manera, el enfoque del proyecto, Dressly, debe centrarse principalmente para la verificar de forma eficiente del flujo de los datos como:

- El estilo del usuario
- Gestión del inventario
- Inteligencia para la vestimenta
- El apartado para el módulo de economía circular

Implementando estos elementos sin añadir mayores complejidades de red mucho más avanzadas para el principio del proyecto.

## Decisión

Después de investigar y analizar las diferentes opciones de estilos arquitectónicos que existen, se ha optado en adoptar un enfoque de arquitectura en capas para el desarrollo del proyecto.Con ello, podemos destacar que el estilo se caracteriza por organizar el sistema en capas jerárquicas, en el que cada una tiene una responsabilidad específica y se comunica con las otras capas por medio de interfaces.

### ¿Por qué he optado por esta decisión?

Se eligió esta arquitectura porque permite estructurar de forma clara y ordenada toda la lógica y los datos específicos que maneja el proyecto:

* **Organización de los Datos:** Permite separar limpiamente los datos del Core de usuario y biometría (Usuario, Perfil físico, Regla de estilo), la Gestión de inventario (Prenda, Categoría, Temporada), la Inteligencia de vestimenta (Outfit, Ocasión) y el Módulo de economía circular (Lote de Donación, Punto de Donación) en una base de datos centralizada, facilitando que se relacionen entre sí sin problemas de sincronización.

* **Procesamiento de Reglas:** La capa de negocio centralizará la lógica del sistema, permitiendo cruzar de forma eficiente la matriz lógica de "Regla de estilo" con el "Perfil físico" del usuario para generar las sugerencias rápidas de outfits.

* **Límites de tiempo:** Desarrollar en capas dentro de un mismo proyecto reduce los tiempos de configuración inicial, permitiéndonos cumplir con los plazos escolares establecidos.

