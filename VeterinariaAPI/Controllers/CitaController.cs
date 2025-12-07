using VeterinariaAPI.Models.Cita;
using VeterinariaAPI.Repository.DAO;
using Microsoft.AspNetCore.Mvc;

namespace VeterinariaAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CitaController : ControllerBase
{
    [HttpGet("listaCita")]
    public async Task<ActionResult<List<Cita>>> ListaCita()
    {
        var lista = await Task.Run(() => new CitaDAO().ListarCitas());
        return Ok(lista);
    }

    [HttpGet("listaCitaO")]
    public async Task<ActionResult<List<CitaO>>> ListaCitaO()
    {
        var lista = await Task.Run(() => new CitaDAO().ListarCitasO());
        return Ok(lista);
    }


    [HttpPost("agregaCita")]
    public async Task<ActionResult<string>> AgregaCita(CitaO obj)
    {
        var mensaje = await Task.Run(() => new CitaDAO().AgregarCita(obj));
        return Ok(mensaje);
    }

    [HttpPut("actualizaCita")]
    public async Task<ActionResult<string>> ActualizaCita(CitaO obj)
    {
        var mensaje = await Task.Run(() => new CitaDAO().ModificarCita(obj));
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

    [HttpGet("listaCitaPorFecha")]
    public async Task<ActionResult<List<Cita>>> ListaCitaPorFecha(int dia, int mes, int año)
    {
        var lista = await Task.Run(() => new CitaDAO().ListarCitasPorFecha(dia, mes, año));
        return Ok(lista);
    }


}