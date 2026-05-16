# ADR-01-Giovana-Diaz
README.md del ADR para mi proyecto de Arquitectura de Software

Dressly

# ADR-01: Selección del Estilo Arquitectónico en Capas para el Sistema Dressly

| Campo  | Valor |
|--------|-------|
| Autor  | Giovana Ruby Díaz Anduze |
| Fecha  | 15/05/2026 |
| Estado | `Propuesto` |

---

## Contexto

El proyecto busca resolver la pérdida de tiempo y el agobio diario al elegir un outfit, dándonos sugerencias rápidas para evitar compras innecesarias de ropa que ya tenemos o que no nos favorece. De igual manera, ayuda a elegir prendas que realmente resalten nuestras características físicas basándose en datos reales de nuestro cuerpo. Finalmente, contribuye a frenar el hiperconsumismo textil mediante la economía circular, facilitando la donación de prendas en desuso para darles un mejor propósito. 

Para construir este sistema, contamos con restricciones de tiempo al ser un proyecto escolar para la materia de Arquitectura de Software. Además, el equipo debe enfocarse en validar la lógica del negocio (el armario, las reglas de estilo y la conexión con ONGs) antes de implementar configuraciones de red o servidores muy avanzados.

---

## Decisión

He decidido utilizar un **Estilo Arquitectónico Monolítico basado en Capas (Capa de Presentación, Capa de Negocio y Capa de Datos)** para estructurar el backend y frontend de la aplicación. 

### ¿Por qué?

Elegí esta arquitectura porque nos permite organizar el código de manera muy clara y separada sin añadir una complejidad innecesaria en esta primera etapa del proyecto. 
* **Separación de responsabilidades:** La lógica para analizar el cuerpo del usuario, organizar los outfits y gestionar los lotes de donación estará completamente separada de las pantallas de la app y de la base de datos.
* **Facilidad de desarrollo:** Al estar todo en un solo proyecto, es mucho más sencillo y rápido de programar, probar y corregir errores, adaptándose perfectamente al tiempo que tenemos disponible para la entrega escolar.
* **Consistencia de los datos:** Al centralizar la información en un solo sitio, es más fácil asegurar que cuando una prenda cambie su estado a "donación", se refleje inmediatamente en el inventario del usuario sin problemas de sincronización.

### Alternativas consideradas

| Alternativa | Por qué la descarté |
|-------------|---------------------|
| **Arquitectura de Microservicios** | Aunque permitiría separar el módulo de IA y el de donaciones en servidores independientes, la descarté porque añade mucha complejidad en la comunicación por red y configuración de servidores, lo cual supera el alcance actual y el tiempo del proyecto. |
| **Arquitectura Basada en Eventos** | Se pensó para reaccionar de forma automática cuando una prenda pasa meses sin usarse, pero se descartó porque requiere herramientas adicionales de mensajería que complicarían el código en esta fase inicial de boceto. |
| **Arquitectura Cliente-Servidor Simple (Sin capas intermedias)** | Consiste en conectar las pantallas de la app directo a la base de datos. Se descartó porque revolvería la lógica de las recomendaciones de ropa con el diseño de la app, haciendo que el código sea caótico y muy difícil de mantener si el proyecto crece. |

---

## Consecuencias

**✅ Lo que gano:**

- **Consecuencia técnica:** El sistema se vuelve mucho más fácil de construir y mantener. Si en el futuro necesitamos cambiar las reglas de estilo o agregar nuevos tipos de prendas, solo modificamos la capa de negocio sin alterar cómo se guardan los datos o cómo se ve la aplicación.
- **Consecuencia sobre el proceso:** El ritmo de trabajo es más rápido y fluido, ya que nos podemos concentrar en programar las funciones principales de la aplicación (como el ropero virtual o el sistema de donación) en lugar de perder tiempo configurando conexiones de red complejas.

**⚠️ Lo que sacrifico o asumo:**

- **Limitación técnica:** Al ser un monolito en capas, si la aplicación llega a fallar críticamente en el módulo de análisis de imágenes, todo el sistema (incluyendo el catálogo de ropa del usuario) podría dejar de funcionar temporalmente hasta que se reinicie el servidor.
- **Deuda o riesgo:** Si el proyecto crece demasiado en el futuro y decidimos que el procesamiento de imágenes con Inteligencia Artificial necesita su propio servidor exclusivo en otro lenguaje (como Python), tendremos que separar ese código de las capas actuales, lo que requerirá una reestructuración del backend.

## Diagrama

Un boceto de cómo se estructura tu sistema.

![Diagrama del sistema]( ./ruta/diagrama-nivel-1.png )