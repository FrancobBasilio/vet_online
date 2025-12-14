# MiVet Online – Sistema de Gestión Veterinaria

## 📋 Descripción General

**MiVet Online** es un sistema web desarrollado para la gestión integral de clínicas veterinarias, orientado a optimizar los procesos administrativos y médicos mediante la digitalización de citas, historiales clínicos, pagos y gestión de usuarios. El proyecto está implementado bajo la arquitectura **ASP.NET MVC**, utilizando **SQL Server** como sistema de gestión de base de datos.

La solución está diseñada para mejorar la eficiencia operativa de la clínica veterinaria, reducir errores en el manejo de información y elevar la experiencia del cliente a través de una plataforma centralizada, segura y escalable.

---

## 🎯 Objetivo del Proyecto

### Objetivo General

Implementar un sistema web integral que permita reducir los tiempos de los procesos administrativos y minimizar errores de registro, contribuyendo a una gestión veterinaria más eficiente y confiable.

### Objetivos Específicos

* Digitalizar el proceso de programación de citas veterinarias.
* Gestionar clientes, mascotas y su historial clínico.
* Administrar roles de usuarios (clientes, veterinarios y recepcionistas).
* Controlar pagos y facturación de servicios veterinarios.
* Facilitar la toma de decisiones mediante información centralizada y estructurada.

---

## 🧠 Justificación Técnica

El sector veterinario presenta una creciente demanda de servicios especializados, lo que exige soluciones tecnológicas que permitan atender mayores volúmenes de información con precisión y rapidez. MiVet Online responde a esta necesidad mediante una plataforma web que centraliza los procesos críticos del negocio, reduce la dependencia de registros manuales y mejora el control operativo y administrativo.

---

## 🧩 Alcance Funcional

El sistema está compuesto por los siguientes módulos:

### 🔐 Gestión de Usuarios

* Registro y autenticación de usuarios.
* Manejo de roles: Cliente, Veterinario y Recepcionista.
* Administración de credenciales y datos personales.

### 🐾 Gestión de Clientes y Mascotas

* Registro y mantenimiento de información de clientes.
* Registro de mascotas (especie, raza, edad, historial).
* Consulta de citas asociadas a cada mascota.

### 👨‍⚕️ Gestión de Veterinarios

* Registro de especialistas y especialidades.
* Gestión de horarios y disponibilidad.
* Actualización de información profesional.

### 🧾 Gestión de Recepcionistas

* Registro y administración del personal administrativo.
* Actualización de datos del personal.

### 📅 Gestión de Citas

* Programación y administración de citas veterinarias.
* Asignación de consultorios.
* Búsqueda de citas por fecha y estado.

### 💳 Gestión de Pagos

* Registro de métodos de pago.
* Facturación de servicios veterinarios.
* Seguimiento de pagos por cliente.

---

## 🏗️ Arquitectura del Sistema

* **Patrón:** MVC (Model – View – Controller)
* **Backend:** ASP.NET MVC
* **Base de Datos:** SQL Server
* **Frontend:** Razor Views, HTML, CSS, JavaScript
* **Control de Acceso:** Roles y autorización por perfil

---

## 🛠️ Tecnologías Utilizadas

* C# / .NET Framework
* ASP.NET MVC
* SQL Server
* Entity Framework / ADO.NET
* HTML5, CSS3, JavaScript
* Visual Studio

---

## ⚙️ Instalación y Configuración

1. Clonar o descargar el repositorio del proyecto.
2. Restaurar la base de datos SQL Server incluida en el proyecto.
3. Configurar la cadena de conexión en el archivo `Web.config`.
4. Abrir la solución en **Visual Studio**.
5. Compilar y ejecutar el proyecto.

---

## 📦 Entregables del Proyecto

* Aplicación web ASP.NET MVC funcional.
* Modelo de base de datos relacional.
* Diagramas de casos de uso.
* Documentación técnica del sistema.

---

## 📈 Beneficios del Sistema

* Reducción de tiempos de atención y espera.
* Disminución de errores administrativos.
* Mejor control de información clínica y financiera.
* Incremento en la satisfacción del cliente.

---


- .NET 8 SDK
- SQL Server Express (o LocalDB)

##  Base de datos

El proyecto incluye los siguientes archivos en la raíz del repositorio:
- `DSWEB_MivetOnline_BD.txt`: Script para crear las tablas.
- `DSWEB_MivetOnline_BD_SP.txt`: Script con los procedimientos almacenados (SP).
- `DSWEB_MivetOnline_BD_Inserts.txt`: Datos de prueba (clientes, mascotas, etc.).

###  Configuración de la conexión
La API se conecta a la base de datos mediante el archivo:  VeterinariaAPI/appsettings.json

**Debes actualizar la cadena de conexión (`"cn"`) según tu entorno local**, por ejemplo:

```json
"cn": "server=localhost\\SQLEXPRESS;database=MivetOnline_DB;uid=sa;pwd=tu_contraseña;"

 Si usas autenticación de Windows, cambia a: 
"cn": "server=localhost\\SQLEXPRESS;database=MivetOnline_DB;Trusted_Connection=true;"



## 📌 Nota Final

MiVet Online ha sido desarrollado como un proyecto académico con enfoque profesional, aplicando buenas prácticas de desarrollo de software, arquitectura web y gestión de bases de datos, alineado a necesidades reales del sector veterinario.




