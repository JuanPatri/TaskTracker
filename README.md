# TaskTrackPro

## ¿Qué aprendí y qué valor me aportó este proyecto?

**TaskTrackPro** fue mucho más que un ejercicio académico.
Me permitió poner en práctica y consolidar habilidades clave del desarrollo de software moderno, que hoy forman parte de mi perfil profesional y me diferencian como desarrollador.

### Principales habilidades y logros alcanzados:

#### ✔️ Arquitectura en capas y separación de responsabilidades

Diseñé e implementé una arquitectura **multicapa**, separando claramente las responsabilidades entre:

* **Dominio (Domain):** Modelado de entidades de negocio con reglas propias.
* **Servicios (Service):** Lógica de negocio desacoplada.
* **Repositorios (Repository):** Acceso a datos mediante una interfaz genérica `IRepository<T>`.
* **DTOs y Enums:** Desacople entre el dominio y la interfaz.

Este enfoque me permite desarrollar sistemas **mantenibles, escalables y fáciles de testear**, siguiendo las mejores prácticas de la industria.

---

#### ✔️ Aplicación de principios SOLID y GRASP en un caso real

Implementé y respeté los **principios SOLID y GRASP**, logrando un código limpio, extensible y robusto.
Entre ellos:

* **Single Responsibility Principle:** Cada clase tiene una única responsabilidad.
* **Open/Closed Principle:** Pude extender funcionalidades (como la exportación CSV/JSON) sin modificar código existente.
* **Dependency Inversion:** Las capas superiores dependen de abstracciones, no de implementaciones concretas.
* **Polimorfismo aplicado:** Usé el patrón **Strategy** en la exportación de proyectos, facilitando la extensión a nuevos formatos.

---

#### ✔️ Experiencia real en Test-Driven Development (TDD)

Desarrollé con **TDD desde el inicio**, logrando una cobertura de pruebas del **97%** en el backend.
Esto incluyó:

* Pruebas de dominio
* Pruebas de servicios
* Cobertura de casos borde y negativos

Me acostumbré a **desarrollar con foco en la calidad y en la detección temprana de errores**.

---

#### ✔️ Trabajo con Entity Framework Core y Fluent API

Aprendí a mapear entidades de dominio a una base relacional usando **EF Core con Fluent API**, sin acoplarme al modelo de datos.
Esto me permitió:

* Mantener el dominio limpio, sin atributos de persistencia.
* Manejar relaciones muchos a muchos de forma explícita (por ejemplo: `TaskResource`, `ProjectRole`).
* Aplicar el principio **Open/Closed** para extender el modelo sin romper el código existente.

---

#### ✔️ Uso de Docker y Azure SQL Edge

Aprendí a levantar la base de datos en un **contenedor Docker con Azure SQL Edge**, lo que me aportó:

* Experiencia en entornos de desarrollo modernos y portables.
* Preparación para trabajar con **microservicios o sistemas distribuidos**.

---

#### ✔️ Desarrollo frontend con Blazor Server

Diseñé una interfaz completa usando **Blazor Server**, trabajando con:

* Componentes reutilizables y desacoplados.
* Inyección de dependencias en la UI para consumir los servicios del backend.
* Mejora de experiencia de usuario con **Bootstrap y CSS**, diferenciando visualmente tareas críticas y no críticas.

---

#### ✔️ Resolución automática de conflictos de recursos

Desarrollé un algoritmo para **detectar y resolver automáticamente conflictos en la asignación de recursos**.
Esto implica:

* Análisis de dependencias entre tareas.
* Reasignación dinámica de dependencias para evitar solapamientos.
* Mejora de la experiencia del usuario al automatizar un proceso complejo.

---

#### ✔️ Exportación flexible de datos

Implementé un sistema de exportación con enfoque extensible, permitiendo:

* Exportar proyectos en **CSV y JSON**.
* Agregar nuevos formatos sin modificar el código existente.
* Aplicar el patrón **Strategy** de forma práctica.

---

#### ✔️ Uso responsable de Inteligencia Artificial

Aproveché herramientas como **ChatGPT** para:

* Generar y validar casos de prueba complejos.
* Consultar sobre buenas prácticas de exportación y estructura de datos.
* Mejorar la interfaz gráfica con estilos CSS adaptados.

Siempre revisando y adaptando el contenido generado, priorizando el **entendimiento profundo del código**.

---

## ¿Por qué este proyecto suma a mi perfil?

Gracias a **TaskTrackPro**, hoy cuento con experiencia práctica en:

* Diseño de sistemas robustos, escalables y orientados a objetos.
* Pruebas automatizadas y desarrollo guiado por tests.
* Despliegue y persistencia con Docker y bases de datos relacionales.
* Trabajo con patrones de diseño aplicados a casos reales.
* Desacoplamiento, modularidad y buenas prácticas de ingeniería de software.
* Uso de herramientas modernas de frontend y backend.

---

## Resumen en pocas palabras:

Este proyecto me ayudó a **pasar de conocer los conceptos a aplicarlos de forma real**, trabajando como lo hacen los equipos profesionales de desarrollo.

