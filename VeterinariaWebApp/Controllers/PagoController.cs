using VeterinariaWebApp.Models.Pago;
using VeterinariaWebApp.Models.Usuario;
using VeterinariaWebApp.Models.Usuario.Cliente;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Rotativa.AspNetCore;
using System.Text;

namespace VeterinariaWebApp.Controllers;

public class PagoController : Controller
{
    private readonly HttpClient _httpClient;

    public PagoController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("ClinicaAPI");
    }

    public List<Pago> listadoPagosGeneral()
    {
        List<Pago> aPagos = new List<Pago>();
        try
        {
            HttpResponseMessage response = _httpClient.GetAsync("Pago/ListarPagosGeneral").Result;
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                aPagos = JsonConvert.DeserializeObject<List<Pago>>(data);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener pagos generales: {ex.Message}");
        }
        return aPagos;
    }

    public Pago ObtenerPagoPorId(long id)
    {
        Pago pago = new Pago();
        try
        {
            HttpResponseMessage response = _httpClient.GetAsync($"Pago/ObtenerPagoPorIdFront/{id}").Result;
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                pago = JsonConvert.DeserializeObject<Pago>(data);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener pago por ID: {ex.Message}");
        }
        return pago;
    }

    public List<Pago> listadoPagosPorCliente(long token)
    {
        List<Pago> aPagos = new List<Pago>();
        if (token == 0)
            return aPagos;

        try
        {
            HttpResponseMessage response = _httpClient.GetAsync($"Pago/ListarPagosPorCliente/{token}").Result;
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                aPagos = JsonConvert.DeserializeObject<List<Pago>>(data) ?? new List<Pago>();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                Console.WriteLine("Token inválido o usuario no autenticado.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener pagos por cliente: {ex.Message}");
        }
        return aPagos;
    }

    public async Task<IActionResult> PagosPendientes()
    {
        var pagos = await ListarPagosPendientes();
        return View(pagos);
    }

    public async Task<List<Pago>> ListarPagosPendientes()
    {
        List<Pago> pagos = new List<Pago>();
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync("Pago/ListarPagosPendientes");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                pagos = JsonConvert.DeserializeObject<List<Pago>>(data) ?? new List<Pago>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener pagos pendientes: {ex.Message}");
        }
        return pagos;
    }

    public async Task<List<Pago>> ListarPagosRealizados()
    {
        List<Pago> pagos = new List<Pago>();
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync("Pago/ListarPagosRealizados");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                pagos = JsonConvert.DeserializeObject<List<Pago>>(data) ?? new List<Pago>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener pagos realizados: {ex.Message}");
        }
        return pagos;
    }

    public async Task<IActionResult> PagosRealizados()
    {
        var pagos = await ListarPagosRealizados();
        return View(pagos);
    }

    public List<UserDoc> listadoTipoDocumentos()
    {
        List<UserDoc> aTDocumentos = new List<UserDoc>();
        try
        {
            HttpResponseMessage response = _httpClient.GetAsync("Usuario/ListarDocumentos").Result;
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                aTDocumentos = JsonConvert.DeserializeObject<List<UserDoc>>(data);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener tipos de documento: {ex.Message}");
        }
        return aTDocumentos;
    }

    public List<Cliente> listadoCliente()
    {
        List<Cliente> aClientes = new List<Cliente>();
        try
        {
            HttpResponseMessage response = _httpClient.GetAsync("Cliente/listaClientes").Result;
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                aClientes = JsonConvert.DeserializeObject<List<Cliente>>(data);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener clientes: {ex.Message}");
        }
        return aClientes;
    }

    public List<PayOpts> ListadoPayOpts()
    {
        List<PayOpts> aPayOpts = new List<PayOpts>();
        try
        {
            HttpResponseMessage response = _httpClient.GetAsync("Pago/ObtenerTiposDePago").Result;
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                aPayOpts = JsonConvert.DeserializeObject<List<PayOpts>>(data);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener tipos de pago: {ex.Message}");
        }
        return aPayOpts;
    }

    public IActionResult PagosRecepcionista()
    {
        var pagos = listadoPagosGeneral();
        return View(pagos);
    }

    public IActionResult PagosCliente()
    {
        long token = long.Parse(HttpContext.Session.GetString("token"));
        var pagos = listadoPagosPorCliente(token);
        return View(pagos);
    }

    public IActionResult DetallePago(long id)
    {
        if (id == 0)
        {
            return Content("Error al intentar obtener el pago");
        }
        return View(ObtenerPagoPorId(id));
    }

    public IActionResult DetallePagoPDF(long id)
    {
        String hoy = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        return new ViewAsPdf("DetallePagoPDF", ObtenerPagoPorId(id))
        {
            FileName = $"DetallePago-{hoy}.pdf",
            PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
            PageSize = Rotativa.AspNetCore.Options.Size.A5
        };
    }

    public IActionResult Crear()
    {
        ViewBag.tipoPagos = new SelectList(ListadoPayOpts(), "ide_pay", "nom_pay");
        ViewBag.clientes = new SelectList(listadoCliente(), "IdCliente", "NombreUsuario");
        return View(new PagoO());
    }

    [HttpPost]
    public async Task<IActionResult> Crear(PagoO obj)
    {
        obj.HoraPago = DateTime.Now;
        obj.IdCliente = int.Parse(HttpContext.Session.GetString("token"));
        obj.MontoPago = 30.00m;

        if (!ModelState.IsValid)
        {
            ViewBag.tipoPagos = new SelectList(ListadoPayOpts(), "ide_pay", "nom_pay");
            ViewBag.clientes = new SelectList(listadoCliente(), "IdCliente", "NombreUsuario");
            return View(obj);
        }

        var json = JsonConvert.SerializeObject(obj);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var idUsuario = obj.IdCliente;
        var responseC = await _httpClient.PostAsync($"Pago/AgregarPago/{idUsuario}", content);

        if (responseC.IsSuccessStatusCode)
        {
            ViewBag.mensaje = "Pago registrado correctamente..!!!";

            var idPagoStr = await responseC.Content.ReadAsStringAsync();
            long IdPago = long.Parse(idPagoStr);

            return RedirectToAction("nuevaCita", "Cita", new { PagoId = IdPago });
        }
        else
        {
            ViewBag.mensaje = $"Error al registrar el pago. Código: {responseC.StatusCode}";
            ViewBag.tipoPagos = new SelectList(ListadoPayOpts(), "ide_pay", "nom_pay");
            ViewBag.clientes = new SelectList(listadoCliente(), "IdCliente", "NombreUsuario");
            return View(obj);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EliminarPago(long id)
    {
        try
        {
            long token = long.Parse(HttpContext.Session.GetString("token"));

            var response = _httpClient.DeleteAsync($"Pago/EliminarPago/{id}").Result;

            if (response.IsSuccessStatusCode)
            {
                TempData["Mensaje"] = "El pago ha sido eliminado correctamente.";
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                TempData["Error"] = "No se puede eliminar un pago que ya está asociado a una cita.";
            }
            else
            {
                TempData["Error"] = "Error al eliminar el pago.";
            }
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error interno: {ex.Message}";
        }

        return RedirectToAction(nameof(PagosCliente));
    }

    public IActionResult Index()
    {
        return View();
    }
}