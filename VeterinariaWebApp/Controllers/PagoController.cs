using VeterinariaWebApp.Models.Pago;
using VeterinariaWebApp.Models.Usuario; // Para UserDoc
using VeterinariaWebApp.Models.Usuario.Cliente; // Para el modelo Cliente
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Rotativa.AspNetCore;
using System.Net.Http;
using System.Text;

namespace VeterinariaWebApp.Controllers;

public class PagoController : Controller
{
    private readonly Uri _baseUri = new("https://localhost:7054/api");
    private readonly HttpClient _httpClient;

    public PagoController()
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = _baseUri;
    }

    // Método corregido: Cambiado de "listarPagos" a "ListarPagosGeneral"
    public List<Pago> listadoPagosGeneral()
    {
        List<Pago> aPagos = new List<Pago>();
        try
        {
            string url = $"{_baseUri}/Pago/ListarPagosGeneral";
            HttpResponseMessage response = _httpClient.GetAsync(url).Result;
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

    // Método corregido: Cambiado de "obtenerPagoPorIdFront" a "ObtenerPagoPorIdFront"
    public Pago ObtenerPagoPorId(long id)
    {
        Pago pago = new Pago();
        try
        {
            string url = $"{_baseUri}/Pago/ObtenerPagoPorIdFront/{id}";
            HttpResponseMessage response = _httpClient.GetAsync(url).Result;
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

    // Método corregido: Cambiado de "listarPagosPorCliente" a "ListarPagosPorCliente"
    public List<Pago> listadoPagosPorCliente(long token)
    {
        List<Pago> aPagos = new List<Pago>();
        try
        {
            string url = $"{_baseUri}/Pago/ListarPagosPorCliente/{token}";
            HttpResponseMessage response = _httpClient.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                aPagos = JsonConvert.DeserializeObject<List<Pago>>(data);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener pagos por cliente: {ex.Message}");
        }
        return aPagos;
    }

    // Método corregido: Cambiado de "ListarDocumentos" a "ListarDocumentos" (esto ya estaba correcto)
    public List<UserDoc> listadoTipoDocumentos()
    {
        List<UserDoc> aTDocumentos = new List<UserDoc>();
        try
        {
            string url = $"{_baseUri}/Usuario/ListarDocumentos";
            HttpResponseMessage response = _httpClient.GetAsync(url).Result;
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

    // Método corregido: Cambiado de "listarClientesFront" a "listaClientes"
    public List<Cliente> listadoCliente()
    {
        List<Cliente> aClientes = new List<Cliente>();
        try
        {
            string url = $"{_baseUri}/Cliente/listaClientes";
            HttpResponseMessage response = _httpClient.GetAsync(url).Result;
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

    // Método corregido: Cambiado de "ListarPayOpts" a "ObtenerTiposDePago"
    public List<PayOpts> ListadoPayOpts()
    {
        List<PayOpts> aPayOpts = new List<PayOpts>();
        try
        {
            string url = $"{_baseUri}/Pago/ObtenerTiposDePago";
            HttpResponseMessage response = _httpClient.GetAsync(url).Result;
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

    // Acción para mostrar los pagos realizados por recepcionista
    public IActionResult PagosRecepcionista()
    {
        var pagos = listadoPagosGeneral(); // Esta línea ya no debería ser null.
        return View(pagos); // Pasamos la lista directamente.
    }

    // Acción para mostrar los pagos del cliente logueado
    public IActionResult PagosCliente()
    {
        long token = long.Parse(HttpContext.Session.GetString("token"));
        var pagos = listadoPagosPorCliente(token); // Esta línea ya no debería ser null.
        return View(pagos); // Pasamos la lista directamente.
    }

    // Acción para ver el detalle de un pago
    public IActionResult DetallePago(long id)
    {
        if (id == 0)
        {
            return Content("Error al intentar obtener el pago");
        }
        return View(ObtenerPagoPorId(id));
    }

    // Acción para generar el PDF del detalle de un pago
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

    // Acción para crear un nuevo pago (esta parte parece estar bien)
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
        obj.MontoPago = 30.00m; // Monto fijo de 30.00

        if (!ModelState.IsValid)
        {
            ViewBag.tipoPagos = new SelectList(ListadoPayOpts(), "ide_pay", "nom_pay");
            ViewBag.clientes = new SelectList(listadoCliente(), "IdCliente", "NombreUsuario");
            return View(obj);
        }

        var json = JsonConvert.SerializeObject(obj);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // --- CORRECCIÓN AQUÍ ---
        // El endpoint debe incluir el idUsuario en la URL, como lo define la API.
        // La URL correcta es: /api/Pago/AgregarPago/{idUsuario}
        var idUsuario = obj.IdCliente; // Usamos el IdCliente que ya asignaste
        var responseC = await _httpClient.PostAsync($"{_baseUri}/Pago/AgregarPago/{idUsuario}", content);

        if (responseC.IsSuccessStatusCode)
        {
            ViewBag.mensaje = "Pago registrado correctamente..!!!";

            // --- CORRECCIÓN AQUÍ ---
            // Ahora sí puedes leer el ID del pago generado.
            var idPagoStr = await responseC.Content.ReadAsStringAsync();
            long IdPago = long.Parse(idPagoStr); // Esto debería funcionar ahora.

            return RedirectToAction("nuevaCita", "Cita", new { PagoId = IdPago });
        }
        else
        {
            // Manejo de errores si la API responde con un error.
            ViewBag.mensaje = $"Error al registrar el pago. Código: {responseC.StatusCode}";
            ViewBag.tipoPagos = new SelectList(ListadoPayOpts(), "ide_pay", "nom_pay");
            ViewBag.clientes = new SelectList(listadoCliente(), "IdCliente", "NombreUsuario");
            return View(obj);
        }
    }

    public IActionResult Index()
    {
        return View();
    }
}