
- .NET 8 SDK
- SQL Server Express (o LocalDB)

##  Base de datos

El proyecto incluye los siguientes archivos en la raíz del repositorio:
- `EFSR_MivetOnline_BD.txt`: Script para crear las tablas.
- `EFSR_MivetOnline_BD_SP.txt`: Script con los procedimientos almacenados (SP).
- `EFSR_MivetOnline_BD_Inserts.txt`: Datos de prueba (clientes, mascotas, etc.).

###  Configuración de la conexión
La API se conecta a la base de datos mediante el archivo:  VeterinariaAPI/appsettings.json

**Debes actualizar la cadena de conexión (`"cn"`) según tu entorno local**, por ejemplo:

```json
"cn": "server=localhost\\SQLEXPRESS;database=MivetOnline_DB;uid=sa;pwd=tu_contraseña;"

 Si usas autenticación de Windows, cambia a: 
"cn": "server=localhost\\SQLEXPRESS;database=MivetOnline_DB;Trusted_Connection=true;"

