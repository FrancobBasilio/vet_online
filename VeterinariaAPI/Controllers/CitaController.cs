using VeterinariaAPI.Models.Cita;
using VeterinariaAPI.Repository.DAO;
using Microsoft.AspNetCore.Mvc;

namespace VeterinariaAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CitaController : ControllerBase
{


    //listaCitasPendientes

    [HttpGet("listaCitasPendientes")]
    public async Task<ActionResult<List<Cita>>> ListaCitasPendientes()
    {
        var lista = await Task.Run(() => new CitaDAO().ListarCitasPendientes());
        return Ok(lista);
    }

    //listaCitasVencidas

    [HttpGet("listaCitasVencidas")]
    public async Task<ActionResult<List<Cita>>> ListaCitasVencidas()
    {
        var lista = await Task.Run(() => new CitaDAO().ListarCitasVencidas());
        return Ok(lista);
    }

    [HttpPut("cancelarPorInasistencia/{id}")]
    public async Task<ActionResult<string>> CancelarPorInasistencia(long id)
    {
        var mensaje = await Task.Run(() => new CitaDAO().CancelarCitaPorInasistencia(id));
        return Ok(mensaje);
    }
    //listaCitasAtendidas

    [HttpGet("listaCitasAtendidas")]
    public async Task<ActionResult<List<Cita>>> ListaCitasAtendidas()
    {
        var lista = await Task.Run(() => new CitaDAO().ListarCitasAtendidas());
        return Ok(lista);
    }
    //listaCitasCanceladas

    [HttpGet("listaCitasCanceladas")]
    public async Task<ActionResult<List<Cita>>> ListaCitasCanceladas()
    {
        var lista = await Task.Run(() => new CitaDAO().ListarCitasCanceladas());
        return Ok(lista);
    }


    //Listar Citas por estado 

    [HttpGet("listaCitasPorEstado")]
    public async Task<ActionResult<List<Cita>>> ListaCitasPorEstado(string estado)
    {
        var lista = await Task.Run(() => new CitaDAO().ListarCitasPorEstado(estado));
        return Ok(lista);
    }






    [HttpPost("agregaCita")]
    public async Task<ActionResult<string>> AgregaCita(CitaO obj)
    {
        var mensaje = await Task.Run(() => new CitaDAO().AgregarCita(obj));

      
        if (mensaje.Contains("horario ya está ocupado"))
        {
            return Conflict(mensaje); 
        }

        return Ok(mensaje); 
    }


    [HttpPut("actualizaCita")]
    public async Task<ActionResult<string>> ActualizaCita(CitaO obj)
    {
        var mensaje = await Task.Run(() => new CitaDAO().ModificarCita(obj));

        
        if (mensaje.Contains("horario ya está ocupado"))
        {
            return Conflict(mensaje); 
        }

        return Ok(mensaje); 
    }





    [HttpDelete("eliminarCita/{id}")]
    public async Task<ActionResult> EliminarCita(long id)
    {
        await Task.Run(() => new CitaDAO().EliminarCita(id));
        return Ok();
    }

    [HttpGet("buscarCita/{id}")]
    public async Task<ActionResult<CitaO>> BuscarCita(long id)
    {
        var cita = await Task.Run(() => new CitaDAO().BuscarCita(id));
        return Ok(cita);
    }

    [HttpGet("buscarCitaFront/{id}")]
    public async Task<ActionResult<Cita>> BuscarCitaFront(long id)
    {
        var cita = await Task.Run(() => new CitaDAO().BuscarCitaFront(id));
        return Ok(cita);
    }


    // Endpoint para actualizar el estado de una cita
    [HttpPut("actualizarEstado/{id}")]
    public async Task<ActionResult<string>> ActualizarEstadoCita(long id, [FromQuery] string estado)
    {
        // Validar que el estado sea válido
        var estadosValidos = new[] { "P", "E", "A", "C" };
        if (!estadosValidos.Contains(estado.ToUpper()))
        {
            return BadRequest("Estado no válido. Use: P (Pendiente), E (En Atención), A (Atendida), C (Cancelada)");
        }

        var mensaje = await Task.Run(() => new CitaDAO().ActualizarEstadoCita(id, estado.ToUpper()));
        return Ok(new { success = true, message = mensaje });
    }

    // ==================== HISTORIAL MÉDICO ====================

    // Agregar historial médico (llamado al finalizar atención)
    [HttpPost("agregarHistorial")]
    public async Task<ActionResult<string>> AgregarHistorialMedico([FromBody] HistorialMedicoDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Diagnostico) || string.IsNullOrWhiteSpace(dto.Tratamiento))
        {
            return BadRequest("Diagnóstico y tratamiento son obligatorios");
        }

        var mensaje = await Task.Run(() => new CitaDAO().AgregarHistorialMedico(dto));
        return Ok(new { success = true, message = mensaje });
    }

    // Obtener historial médico de una cita
    [HttpGet("historial/{idCita}")]
    public async Task<ActionResult<HistorialMedico>> ObtenerHistorialPorCita(long idCita)
    {
        var historial = await Task.Run(() => new CitaDAO().ObtenerHistorialPorCita(idCita));
        if (historial == null)
        {
            return Ok(new { existe = false, message = "No hay historial médico para esta cita" });
        }
        return Ok(historial);
    }
}