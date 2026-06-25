# ADR-02-Giovana-Diaz

# ADR-02: Adopción de Arquitectura Monolítica en Capas para el Sistema Dressly

| Campo  | Valor |
|--------|-------|
| Autor  | Giovana Ruby Díaz Anduze |
| Fecha  | 05/06/2026 |
| Estado | `Propuesto` |

---

## Contexto

Dressly es una aplicación web desarrollada en .NET Core 10 que digitaliza el guardarropa personal mediante fotografías reales de las prendas. A partir de ese inventario, genera sugerencias automáticas de outfits basadas en colorimetría y fisonomía del usuario, y facilita la donación de prendas en desuso promoviendo la economía circular.

El proyecto es de desarrollo unipersonal en etapa académica, con un horizonte de entrega de corto plazo. La persistencia se resuelve mediante archivos JSON locales y el despliegue final se realizará sobre infraestructura AWS (instancia EC2 con almacenamiento en S3).

El sistema ha avanzado en su diseño lógico y técnico; sin embargo, un sólo diagrama de arquitectura o una descripción plana del código no es suficiente para responder a ciertas inquietudes que surjan del proyectos:

1. El usuario final y el cliente necesitan comprender la funcionalidad y qué módulos resuelven sus necesidades.
2. Los desarrolladores necesitan saber cómo estructurar físicamente sus clases y proyectos en .NET.
3. El arquitecto requiere visualizar cómo se comporta el sistema bajo concurrencia y cómo interactúan las capas dinámicamente.
4. El equipo de DevOps/Sysadmin necesita entender la infrastructura física en la nube para el despliegue.

Para resolver esta brecha de comunicación, se requiere adoptar una metodología formal que represente el sistema desde múltiples prespectivas complementarias, evitando dejar a cualquier audiencia sin respuesta.

---

## Restricciones 

- El proyecto de desarrollo académico unipersonal con tiempos de entrega acotados.
- La persistencia resuelta con archivos JSON locales (sin motor de base de datos relacional).
- Despligue planificado en AWS EC2 + S3; no se requiere escalado horizontal en esta fase.
- La plataforma de desarrollo es .NET Core 10 con el patrón MVC como punto de entrada.
- Coherencia absoluta: los módulos identificados en las responsabilidades del negocio deben mapearse directamente al código y la infrastructura real descrita.
  
---

## Decisión

Para este proyecto se adopta formalmente el Modelo de Vistas, las cuales son la lógica, desarrollo, proceso y despliegue, como el mecanismo que permite la gestión para definir, diseñar y documentar la arquitectura de Dressly.

### 1. Vista Lógica (¿Qué hace el sistema?)
En esta vista muestra la descomposición funcional de Dressly en sus cuatro áreas de dominio principales como componentes conceptuales de negocio independientes:

- *Módulo de Catálogo de Prendas:* iventario y categorización.
- *Módulo de Inteligencia de Outfits:* Algoritmos de combinación estilística.
- *Módulo de Perfil y Biometría:* Características físicas del usuario.
- *Módulo de Economía Circular:* Directorio de donaciones y ONGs.

### 2. Vista de Desarrollo (¿Cómo está organizador el código?)
Muestra la organización del código en la solución .NET de tres capas. Mapea la estructura física de proyectos para garantizar que las dependencias apunten ahcie el núcleo de negocio:

- *Dressly.Web* (Presentación)
- *Dressly.Domain* (Lógica/Negocio pura y contratos de repositorios)
- *Dressly.Infrastructure* (Persistencia local en JSON / AWS S3)

### 3. Vista de Procesos (¿Cómo se comporta en tiempo de ejecución?)
Describe la interacción dinámica y la secuencia de llamadas síncronas/asíncronas en un caso de uso prioritario: **Generar una sugerencia de Outfit compatible**

- El usuario interactúa con la presentación -> Se invoca al servicio de Dominio -> se consultan las prendas mediante el repositorio -> se computa la colorimetría -> se retorna el resultado.

### 4. Vista de Despliegue (¿Dónde corre físicamente?)
Describe el mapa de infrastructura física planificado en Amazon Web Services (AWS) para soportar el monolito en capas:

- Una instancia EC2 para la aplicación MVC.
- Un Bucket S3 para el almacenamiento de imágenes de prendas.
- Aislamiento de red mediante una VPC con accesos controlados por Security Groups.

## ¿Por qué se optó por esa decisión?
La separación de esta documentación en cuatro perspectivas diferenciadas se justifica por los siguientes motivos técnicos y de comunicación:

Justificación de la Vista Lógica (Interesados: Clientes / Product Owner):
Permite validar con el cliente que todos los requerimientos funcionales y reglas del negocio (colorimetría, tallas, donaciones) están mapeados a un componente específico, abstrayéndolos de la complejidad técnica del código en C#.

- Vista de Desarrollo (Interesados: Desarrolladores / Programadores):
Establece directrices de codificación estrictas para el equipo de desarrollo. Al estructurar la vista física de las capas en .NET, se garantiza visualmente que la capa de Presentación y de Infraestructura dependan de la capa de Dominio, impidiendo la creación accidental de dependencias circulares u "organizaciones espagueti".

- Vista de Procesos (Interesados: Arquitecto de Software / QA):
Es indispensable para modelar la concurrencia, los tiempos de respuesta y la comunicación entre capas. Al mapear el flujo de generación de outfits, se puede identificar dónde ocurren cuellos de botella y cómo viajan los datos desde la interfaz de usuario hasta los adaptadores de datos.

- Vista de Despliegue (Interesados: DevOps / Administradores de Sistemas):
La aplicación requiere almacenar imágenes de prendas y archivos JSON. Justificamos modelar el despliegue físico para dimensionar los costos de los servidores EC2 de AWS, planificar las políticas de seguridad de red y configurar el almacenamiento externo persistente en buckets S3 antes de realizar cualquier instalación.

---

### Alternativas consideradas y la razón del por qué las descarté para el proyecto

1. **Documentación con un Digrama de Contexto Único (Caja Negra).**
   - **Razón de descarte:** Es demasiado genérico, aunque es excelente para el cliente, pues este no proporciona información acerca de cómo se debe de estructurar los proyectos en .NET ni orienta sobre el aprovisionamiento de recursos en AWS.
     
2. **Uso del Modelo C4 en sus Niveles completos**
   - **Razón de descarte:** Si bien el modelo C4 es una excelente herramienta para diagramar el código (Nivel 3 y 4), no provee de forma nativa vistas específicas para la infraestructura física de red ni para analizar los flujos dinámicos de procesos temporales con la flexibilidad que ofrece el modelo que estoy escogiendo para los requerimientos de la asignatura.
    
---

## Consecuencias

### Lo que gano (Beneficios)

- **Comunicación eficaz:** Cada interesado consulta la perspectiva técnica que le compete directamente.
- **Mantenibilidad:** El código .NET se mantiene limpio y ordenado siguiendo el sieño trazado.
- **Alineación con la rúbrica:** Cumplimiento estricto de las4 vistas requeridas para la materia con tecnología concreta definida.

### Lo que sacrifico o asumo (Limitaciones)

- **Esfuerzo de sincronización:** Cualquier modificación posterior en las funcionalidades o en el código fuente de .NET requerirá actualizar manualmente los diagramas en draw.io para evitar que la documentación se vuelva obsoleta.


---

## Diagramas de las vistas

### Vista lógica
<img width="621" height="541" alt="vista_logica_original drawio" src="https://github.com/user-attachments/assets/dee6516b-6279-466f-9cce-f4ddbcb5d35b" />


### Vista de Desarrollo
<img width="891" height="1081" alt="Desarrollo drawio" src="https://github.com/user-attachments/assets/22a095b1-abf8-4d94-81a7-88ab7dba6dd3" />

### Vista de Procesos
<img width="541" height="691" alt="Procesos drawio" src="https://github.com/user-attachments/assets/701415b9-d589-49f7-9bf1-3e745db8c33b" />


### Vista de Despliegue
<img width="1571" height="1600" alt="WhatsApp Image 2026-06-05 at 9 13 57 PM" src="https://github.com/user-attachments/assets/c0238b51-1f69-4f3e-8f8b-9ca6f324a6b8" />

---

## Declaración de Uso de IA
Se declara de manera formal y honesta el uso de asistencia de Inteligencia Artificial (IA) en este proyecto:

- *Autoría Intelectual:* La arquitectura monolítica en capas, los Bounded Contexts (Catálogo, Inteligencia, Perfil y Economía) y la estructura de directorios en .NET 10 fueron diseñados por la autora.

- *Soporte de IA Generativa:* Se utilizó exclusivamente para traducir la estructura física y lógica de la aplicación al formato de diagramación Mermaid (permitiendo su renderizado nativo en GitHub) y para optimizar el formato Markdown del ADR-02.

Todos los diagramas y textos generados con asistencia de IA fueron auditados, validados y ajustados por la autora para asegurar su veracidad técnica con el código del proyecto.