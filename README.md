# ADR-02-Giovana-Diaz

# ADR-02: Adopción de Arquitectura Hexagonal para Dressly

| Campo  | Valor |
|--------|-------|
| Autor  | Giovana Ruby Díaz Anduze |
| Fecha  | Viernes 11 de junio de 2026 |
| Estado | `Propuesto` |

---

## Contexto

Dressly es una aplicación web desarrollada en .NET Core 10 que busca resolver el agobio diario de elegir un outfit, el desorden del guardarropa y el hiperconsumismo textil. El sistema gestiona el inventario de prendas mediante fotos reales, genera sugerencias de outfits basadas en colorimetría y fisonomía del usuario, y facilita la donación de ropa en desuso a redes de ONGs promoviendo la economía circular.

En el primer ADR se optó por una arquitectura en capas (Presentación -> Negocio -> Datos) como decisión inicial, justificada por la velocidad de configuración y la calidad estructural para el arranque del proyecto académico; sin embargo, al avanzar con el desarrollo surgieron nuevos requerimientos que esa arquitectura no puede satisfacer sin modificar el núcleo del sistema:

- Principalmente se requiere desplegar Dressly en un servidor AWS (Amazon EC2 / ECS), lo que implica integrar proveedores de infrastructura cloud como Amazon RD, Amazon S3 y APIs externas de ONGs.
- El sistema debe soportar múltiples tipos de persistencia de forma intercambiable: archivos JSON en memoria para el desarrollo local, Amazon RDS o DynamoDB para producción, y Amazon S3 para almacenamiento de imágenes.
- La lógica del negocio (reglas de colorimetría, generación de outfits, gestión de donaciones debe poder porbarse de forma aislada sin depender de infrastructura real.

En la arquitectura en capas, cambiar el proveedor de datos o agregar soporte cloud implica modificar directamente la capa de negocio, violando el principio de que el dominio no debe depender de detalles de infrastructura. Esto hace necesario adoptar un estilo arquitectónico que desacople el dominio desde el diseño, permitiendo que los detalles de infrastructura sean intercambiables sin afectar la lógica dentral del sistema.

---

## Restricciones 

Las restricciones de este proyecto académico, principalmente se cuenta con un tiempo limitado para la entrega y avances para el desarrollo del sistema; de igual manera, el enfoque del proyecto, Dressly, debe centrarse principalmente para la verificar de forma eficiente del flujo de los datos como:

- El estilo del usuario
- Gestión del inventario
- Inteligencia para la vestimenta
- El apartado para el módulo de economía circular

---

## Decisión
Después de investigar y analizar las diferentes opciones de estilos arquitectónicos que existen, se ha optado en adoptar un enfoque de arquitectura hexagonal (Ports and Adapaters) para el desarrollo del proyecto, reemplazando la arquitectura en capas establecida anteriormente.

La arquitectura hexagonal organiza el sistema en tres zonas:
- *Dominio:* contiene toda la lógica de negocio pura, pues están las reglas de estilo, colorimtería, gestión de prendas y donaciones. No depende de ningún framework, base de datos ni protocolo de red.
- *Puertos:* Interfaces C# que definen los contratos de comunicación de dominio, los puertos definen cómo el exterior invoca al dominio. Los puertos de salida definen qué servicios externos necesita el dominio.
- *Adaptadores:* implementaciones concretas de los puertos, intercambiables sin tocar el dominio.


### ¿Por qué he optado por esta decisión?

Se eligió esta arquitectura porque permite resuelve directamente los tres problemas identificador en el contexto:

- **Desacoplamiento de infrastructura:** el dominio de Dressly no conoce si los datos se guardan en JSO, RDS o DynamoDB. La lógica de colorimetría y generación de outfits puede desarrollarse, probarse y modificarse completamente independiente del proveedor de persistencia.
- **Soporte para múltiples bases de datos:** se pueden registrar distintos adaptadores según el entorno mediante inyección de dependencias, sin cambiar una sola línea del dominio.
- **Despliegue en AWS sin fricción:** la capa de infrastructura (adaptadores AWS) se configura de forma independiente. La aplicación arranca con adaptadores locales en desarrollo y con adaptadores cloud en producción usando variables de entorno, sin que el dominio sepa en cuál entorno se encuentra.

---
## Alternativas consideradas y la razón del por qué las descarté para el proyecto

- *Arquitectura en capas:* fue la decisión principal para mi proyecto pero he decidido descartarla porque genera un acomplamiento directo entre la lógica de negocio y la infrastructura; es decir, cuando necesite cambiar el proveedor de base de datos o agregar un soporte para AWS implicaría modificar la capa de negocio. No cumple con el requerimiento de múltiples adaptadores de persistencia intercambiables.
- *Microservicios:* permite aisla cada módulo (inventario, donación, sugerencias) en servicios independientes. Se descarta porque introduce una complejidad operativa alta, pues la comunicación entre servicios, gestión de red distribuida, múltiples despliegues, que está fuera del alcance del proyecto académico en esta fase.
- *Serverless (AWS Lambda):* permite ejecutar funciones sin administrar servidores, ideal para el módulo de sugerencias de outfits. Se descarta porque el modelo sin estado de Lambda no se adapta bien a la gestión de sesiones y al flujo continuo del guardarropa personal. Podría incorporarse como adaptador específico en una fase posterior.
- *Event-Driven:* adecuada para disparar notificaciones basadas en el contador de usos de una prenda o la fecha de última vez usada. Se descarta porque requiere implementar un broker de eventos (SNS/SQS en AWS) que añade complejidad de infrastructura innecesaria en la fase actual.
  
---

## Consecuencias

### Lo que gano

- El dominio de Dressly es completamente independiente de la infrastructura: se puede desarrollar, probar y modificar sin depender de AWS ni de ningún proveedor de base de datos.
- Cambiar entre JSON (desarrollo) y RDS/DynamoDB (producción) es una configuración de inyección de dependencias, no un cambio de código en el dominio.
- El despliegue en AWS EC2/ECS se vuelve una decisión de infrastructura, no arquitectónica, el dominiono no sabe en qué entorno está corriendo.
- La lógica de colorimetría, generación de outfits y donación queda protegida de cambios tecnológicos externos: si mañana se cambia S3 por otro proveedor de almacenamiento, solo se escribe un nuevo adaptador.
- El ritmo de desarrollo es más fluido proque se puede trabajar localmente con JSON y mocks sin necesitar conexión a AWS en ningún momento del desarrollo.


### Lo que sacrifico o asumo

- Al seguir siendo un monolito, si el módulo de análisis de imágenes presenta un fallo crítico, todo el sistema puede verse afectado hasta que se reinicie el servidor.
- La estructura inicial del proyecto requiere más planificación que una arquitectura en capas: es necesario definir los puertos antes de implementar los adaptadores, lo que implica un mayor esfuerzo de diseño al inicio.
- Para el equipo, la curva de aprendizajes es mayor que con capas tradicionales, pues el patrón de puertos y adaptadores requiere comprender la separación entre contrato e implementación.
- Si el proyecto no crece más allá del alcance académico, parte de la flexibilidad ganada (múltiples adaptadores, despliegue en AWS) no llegará a usarse en la práctica.
  
---

## Diagrama

<img width="1324" height="1124" alt="Diagrama_Hexagonal" src="https://github.com/user-attachments/assets/521a936b-1f23-476a-89e8-adbd0ba5b7a5" />


He decidido implementar mi diagrama de forma circular por la facilidad de realización y al investigar, un diagrama hexagonal es más conveniente que se utilice de forma circular por la implementación de los puertos y los adaptadores, pues no es necesario que tenga una implementación de seis adaptadores.

---

## Declaración de uso de inteligencia artificial

Para el desarrollo de este ADR se utilizó Claude *(Antropic)* como herramienta de asistencia en la redacción y estructuración del documento. Todas las decisiones de diseño, el análisis de alternativas y la justificación técnica aplicada al contexto de Dressly con propias de la autora. La IA fue utilizada como apoyo para expresar y documentar de forma clara las decisiones previamente razonadas.
