using VeterinariaAPI.Models.Pago;
using VeterinariaAPI.Repository.DAO;
using Microsoft.AspNetCore.Mvc;

namespace VeterinariaAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PagoController : ControllerBase
{
    [HttpGet("ObtenerTiposDePago")]
    public async Task<ActionResult<List<PayOpts>>> ObtenerTiposDePago()
    {
        var lista = await Task.Run(() => new PayOptsDAO().ListarTiposDePago());
        return Ok(lista);
    }

    [HttpGet("ListarPagosGeneral")]
    public async Task<ActionResult<List<Pago>>> ListarPagosGeneral()
    {
        var lista = await Task.Run(() => new PagoDAO().ListarPagos());
        return Ok(lista);
    }

    [HttpGet("ListarPagosPorCliente/{idUsuario}")]
    public async Task<ActionResult<List<Pago>>> ListarPagosPorCliente(long idUsuario)
    {
        var lista = await Task.Run(() => new PagoDAO().ListarPagosPorCliente(idUsuario));
        return Ok(lista);
    }

    [HttpPost("AgregarPago/{idUsuario}")]
    public async Task<ActionResult<long>> AgregarPago(PagoO pago, long idUsuario)
    {
        var idPago = await Task.Run(() => new PagoDAO().AgregarPago(pago, idUsuario));
        return Ok(idPago);
    }

    [HttpGet("ObtenerPagoPorId/{id}")]
    public async Task<ActionResult<PagoO>> ObtenerPagoPorId(long id)
    {
        var pago = await Task.Run(() => new PagoDAO().ObtenerPagoPorId(id));
        return Ok(pago);
    }

    [HttpGet("ObtenerPagoPorIdFront/{id}")]
    public async Task<ActionResult<Pago>> ObtenerPagoPorIdFront(long id)
    {
        var pago = await Task.Run(() => new PagoDAO().ObtenerPagoPorIdFront(id));
        return Ok(pago);
    }

    [HttpPut("ActualizarPago")]
    public async Task<ActionResult<string>> ActualizarPago(PagoO pago)
    {
        var mensaje = await Task.Run(() => new PagoDAO().ActualizarPago(pago));
        return Ok(mensaje);
    }

    [HttpDelete("EliminarPago/{id}")]
    public async Task<ActionResult<string>> EliminarPago(long id)
    {
        var mensaje = await Task.Run(() => new PagoDAO().EliminarPago(id));
        return Ok(mensaje);
    }
}