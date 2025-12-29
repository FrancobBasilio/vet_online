using VeterinariaWebApp.Models.Cita;
using VeterinariaWebApp.Models.Pago;
using VeterinariaWebApp.Models.Usuario.Veterinario;
using VeterinariaWebApp.Models.Usuario.Cliente;
using VeterinariaWebApp.Models.Mascota;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Rotativa.AspNetCore;
using System.Text;

namespace VeterinariaWebApp.Controllers;

public class CitaController : Controller
{
    private readonly HttpClient _httpClient;

    public CitaController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("ClinicaAPI");
    }

    // Método auxiliar para obtener citas
    public List<Cita> ArregloCitas()
    {
        List<Cita> aCitas = new List<Cita>();
        try
        {
            HttpResponseMessage response = _httpClient.GetAsync("Cita/listaCita").Result;
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                aCitas = JsonConvert.DeserializeObject<List<Cita>>(data) ?? new List<Cita>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener citas: {ex.Message}");
        }
        return aCitas;
    }

    public List<Veterinario> listadoVeterinario()
    {
        List<Veterinario> aVeterinarios = new List<Veterinario>();
        try
        {
            HttpResponseMessage response = _httpClient.GetAsync("Veterinario/listaVeterinarios").Result;
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                aVeterinarios = JsonConvert.DeserializeObject<List<Veterinario>>(data) ?? new List<Veterinario>();
            }
            else
            {
                Console.WriteLine($"Error al obtener veterinarios: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener veterinarios: {ex.Message}");
        }
        return aVeterinarios;
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
                aClientes = JsonConvert.DeserializeObject<List<Cliente>>(data) ?? new List<Cliente>();
            }
            else
            {
                Console.WriteLine($"Error al obtener clientes: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener clientes: {ex.Message}");
        }
        return aClientes;
    }

    //LISTADO DE CITAS PENDIENTES 
    public async Task<IActionResult> CitasPendientes()
    {
        var response = await _httpClient.GetAsync("Cita/listaCitasPendientes");
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var lista = JsonConvert.DeserializeObject<List<Cita>>(json);
            return View(lista);
        }
        return View(new List<Cita>());
    }

    //LISTADO DE CITAS VENCIDAS
    public async Task<IActionResult> CitasVencidas()
    {
        var response = await _httpClient.GetAsync("Cita/listaCitasVencidas");
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var lista = JsonConvert.DeserializeObject<List<Cita>>(json);
            return View(lista);
        }
        return View(new List<Cita>());
    }

    //Marcar como no asistido a la cita
    [HttpPost]
    public async Task<IActionResult> MarcarComoNoAsistio(long idCita)
    {
        var response = await _httpClient.PutAsync($"Cita/cancelarPorInasistencia/{idCita}", null);
        if (response.IsSuccessStatusCode)
        {
            TempData["Exito"] = "La cita ha sido marcada como 'No Asistió'.";
        }
        else
        {
            TempData["Error"] = "Error al actualizar la cita.";
        }
        return RedirectToAction("CitasVencidas");
    }

    //Listado de Citas Atendidas
    public async Task<IActionResult> CitasAtendidas()
    {
        var response = await _httpClient.GetAsync("Cita/listaCitasAtendidas");
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var lista = JsonConvert.DeserializeObject<List<Cita>>(json);
            return View(lista);
        }
        return View(new List<Cita>());
    }

    //Listado de Citas Canceladas
    public async Task<IActionResult> CitasCanceladas()
    {
        var response = await _httpClient.GetAsync("Cita/listaCitasCanceladas");
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var lista = JsonConvert.DeserializeObject<List<Cita>>(json);
            return View(lista);
        }
        return View(new List<Cita>());
    }

    private async Task<bool> ExisteCita(CitaO obj)
    {
        List<Cita> citas = ArregloCitas();
        return citas.Any(c => c.NombreVeterinario != null &&
                             c.CalendarioCita.Date == obj.CalendarioCita.Date &&
                             c.CalendarioCita.Hour == obj.CalendarioCita.Hour &&
                             c.CalendarioCita.Minute == obj.CalendarioCita.Minute);
    }

    //  Cita/IniciarCreacionCita
    [HttpGet]
    public async Task<IActionResult> IniciarCreacionCita()
    {
        var pagoPendiente = await ObtenerPagoPendiente();

        if (pagoPendiente != null)
        {
            return RedirectToAction("nuevaCita", new { PagoId = pagoPendiente.IdPago });
        }
        else
        {
            return RedirectToAction("Crear", "Pago");
        }
    }

    //  Cita/nuevaCita
    [HttpGet]
    public IActionResult nuevaCita(int PagoId)
    {
        int? idCliente = HttpContext.Session.GetInt32("ClienteId");
        if (idCliente == null || idCliente == 0)
            return RedirectToAction("Index", "Login");

        if (PagoId > 0)
        {
            ViewBag.MensajePago = "Pago registrado: Su pago ya fue procesado. Al confirmar la cita, quedará agendada automáticamente.";
        }

        CargarViewBagsParaCita(idCliente.Value);

        CitaO citaPagada = new CitaO() { IdPago = PagoId };
        return View(citaPagada);
    }

    // POST: Cita/nuevaCita
    [HttpPost]
    public async Task<IActionResult> nuevaCita(CitaO obj)
    {
        int? idCliente = HttpContext.Session.GetInt32("ClienteId");
        if (idCliente == null || idCliente == 0)
            return RedirectToAction("Index", "Login");

        if (!ModelState.IsValid)
        {
            CargarViewBagsParaCita(idCliente.Value);
            return View(obj);
        }

        var json = JsonConvert.SerializeObject(obj);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var responseC = await _httpClient.PostAsync("Cita/agregaCita", content);

        if (responseC.IsSuccessStatusCode)
        {
            TempData["Exito"] = "¡Cita agendada correctamente!";
            return RedirectToAction("listaCitaPorCliente", "Cliente", new { ide_usr = idCliente.Value });
        }
        else
        {
            var errorMessage = await responseC.Content.ReadAsStringAsync();

            if (errorMessage.Contains("horario ya está ocupado"))
            {
                ModelState.AddModelError("CalendarioCita", "Ya existe una cita programada para este veterinario en esta fecha y hora.");
            }
            else
            {
                TempData["Error"] = "Error al registrar la cita. Intente nuevamente.";
            }

            CargarViewBagsParaCita(idCliente.Value);
            return View(obj);
        }
    }

    // GET: Cita/EditarCitaCliente
    [HttpGet]
    public async Task<IActionResult> EditarCitaCliente(int id)
    {
        int? idCliente = HttpContext.Session.GetInt32("ClienteId");
        if (idCliente == null || idCliente == 0)
            return RedirectToAction("Index", "Login");

        var response = await _httpClient.GetAsync($"Cita/buscarCita/{id}");
        if (!response.IsSuccessStatusCode)
        {
            TempData["Error"] = "No se encontró la cita solicitada.";
            return RedirectToAction("listaCitaPorCliente", "Cliente", new { ide_usr = idCliente.Value });
        }

        var content = await response.Content.ReadAsStringAsync();
        var cita = JsonConvert.DeserializeObject<CitaO>(content);

        if (cita.CalendarioCita <= DateTime.Now)
        {
            TempData["Error"] = "Solo puede editar citas futuras.";
            return RedirectToAction("listaCitaPorCliente", "Cliente", new { ide_usr = idCliente.Value });
        }

        CargarViewBagsParaCita(idCliente.Value);
        return View(cita);
    }

    // POST: Cita/EditarCitaClientePost
    [HttpPost]
    public async Task<IActionResult> EditarCitaClientePost(CitaO obj)
    {
        int? idCliente = HttpContext.Session.GetInt32("ClienteId");
        if (idCliente == null || idCliente == 0)
            return RedirectToAction("Index", "Login");

        if (!ModelState.IsValid)
        {
            CargarViewBagsParaCita(idCliente.Value);
            return View("EditarCitaCliente", obj);
        }

        var json = JsonConvert.SerializeObject(obj);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"Cita/actualizaCita?id={obj.IdCita}", content);

        if (response.IsSuccessStatusCode)
        {
            TempData["Exito"] = "¡Cita actualizada correctamente!";
            return RedirectToAction("listaCitaPorCliente", "Cliente", new { ide_usr = idCliente.Value });
        }
        else
        {
            var errorMessage = await response.Content.ReadAsStringAsync();

            if (errorMessage.Contains("horario ya está ocupado"))
            {
                ModelState.AddModelError("CalendarioCita", "Ya existe una cita programada para este veterinario en esta fecha y hora.");
            }
            else
            {
                TempData["Error"] = "Error al actualizar la cita.";
            }

            CargarViewBagsParaCita(idCliente.Value);
            return View("EditarCitaCliente", obj);
        }
    }

    // Método auxiliar para cargar ViewBags de citas
    private void CargarViewBagsParaCita(int idCliente)
    {
        var veterinarios = listadoVeterinario()
            .Select(v => new SelectListItem
            {
                Value = v.IdVeterinario.ToString(),
                Text = $"Dr. {v.NombreUsuario} - {v.especialidad}"
            })
            .ToList();

        var clientMascotas = new List<Mascota>();
        try
        {
            var clientResponse = _httpClient.GetAsync($"Cliente/listarMascotas/{idCliente}").Result;
            if (clientResponse.IsSuccessStatusCode)
            {
                var data = clientResponse.Content.ReadAsStringAsync().Result;
                clientMascotas = JsonConvert.DeserializeObject<List<Mascota>>(data) ?? new List<Mascota>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener mascotas: {ex.Message}");
        }

        ViewBag.veterinarios = veterinarios;
        ViewBag.mascotas = new SelectList(clientMascotas, "IdMascota", "Nombre");
        ViewBag.clientes = new SelectList(listadoCliente(), "IdCliente", "NombreUsuario");
    }

    [HttpGet]
    public async Task<IActionResult> actualizarCita(int id)
    {
        var response = await _httpClient.GetAsync($"Cita/buscarCita/{id}");
        if (!response.IsSuccessStatusCode)
        {
            ViewBag.mensaje = "No Hay Cita";
            return View();
        }

        var content = await response.Content.ReadAsStringAsync();
        var objC = JsonConvert.DeserializeObject<CitaO>(content);

        var mascotaResponse = await _httpClient.GetAsync($"Cliente/listarMascotasPorId/{objC.IdMascota}");
        if (!mascotaResponse.IsSuccessStatusCode)
        {
            ViewBag.mensaje = "Error al obtener los datos de la mascota.";
            return View();
        }

        var mascotaData = await mascotaResponse.Content.ReadAsStringAsync();
        var mascota = JsonConvert.DeserializeObject<MascotaConCliente>(mascotaData);

        var todasLasMascotasResponse = await _httpClient.GetAsync($"Cliente/listarMascotas/{mascota.IdUsuario}");
        List<Mascota> mascotasDelDueño = new List<Mascota>();
        if (todasLasMascotasResponse.IsSuccessStatusCode)
        {
            var todasLasMascotasData = await todasLasMascotasResponse.Content.ReadAsStringAsync();
            mascotasDelDueño = JsonConvert.DeserializeObject<List<Mascota>>(todasLasMascotasData) ?? new List<Mascota>();
        }

        var veterinarios = listadoVeterinario()
            .Select(v => new SelectListItem
            {
                Value = v.IdVeterinario.ToString(),
                Text = $"Dr. {v.NombreUsuario} - {v.especialidad}"
            })
            .ToList();

        ViewBag.veterinarios = veterinarios;
        ViewBag.mascotas = new SelectList(mascotasDelDueño, "IdMascota", "Nombre");
        ViewBag.clientes = new SelectList(listadoCliente(), "IdCliente", "NombreUsuario");

        return View(objC);
    }

    [HttpPost]
    public async Task<IActionResult> actualizarCitaPost(int id, CitaO obj)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.veterinarios = listadoVeterinario()
                .Select(v => new SelectListItem
                {
                    Value = v.IdVeterinario.ToString(),
                    Text = $"Dr. {v.NombreUsuario} - {v.especialidad}"
                })
                .ToList();
            ViewBag.mascotas = new SelectList(new List<Mascota>(), "IdMascota", "Nombre");
            ViewBag.clientes = new SelectList(listadoCliente(), "IdCliente", "NombreUsuario");
            return View(obj);
        }

        var json = JsonConvert.SerializeObject(obj);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"Cita/actualizaCita?id={id}", content);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("CitasPendientes");
        }

        ViewBag.veterinarios = listadoVeterinario()
            .Select(v => new SelectListItem
            {
                Value = v.IdVeterinario.ToString(),
                Text = $"Dr. {v.NombreUsuario} - {v.especialidad}"
            })
            .ToList();
        ViewBag.mascotas = new SelectList(new List<Mascota>(), "IdMascota", "Nombre");
        ViewBag.clientes = new SelectList(listadoCliente(), "IdCliente", "NombreUsuario");
        ViewBag.mensaje = "Error al actualizar la cita";
        return View(obj);
    }

    // POST: Cita/CancelarCitaCliente (AJAX)
    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> CancelarCitaCliente(int id)
    {
        try
        {
            var response = await _httpClient.PutAsync($"Cita/actualizarEstado/{id}?estado=C", null);
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "La cita ha sido cancelada correctamente." });
            }
            else
            {
                return Json(new { success = false, message = "No se pudo cancelar la cita." });
            }
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error: " + ex.Message });
        }
    }

    // POST: Cita/eliminarCita (AJAX)
    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> eliminarCita(int id)
    {
        var response = await _httpClient.DeleteAsync($"Cita/eliminarCita/{id}");
        if (response.IsSuccessStatusCode)
        {
            return Json(new { success = true, message = "La cita ha sido cancelada correctamente." });
        }
        else
        {
            return Json(new { success = false, message = "No se pudo cancelar la cita. Es posible que tenga pagos asociados." });
        }
    }

    public Cita ObtenerCitaPorId(long id)
    {
        Cita cita = null;
        try
        {
            HttpResponseMessage response = _httpClient.GetAsync($"Cita/buscarCitaFront/{id}").Result;
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                cita = JsonConvert.DeserializeObject<Cita>(data);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener cita por ID: {ex.Message}");
        }
        return cita;
    }

    public IActionResult DetalleCita(long id)
    {
        Cita cita = ObtenerCitaPorId(id);
        if (cita == null)
        {
            return NotFound();
        }
        return View(cita);
    }

    public IActionResult GenerarDetalleCitaPDF(long id)
    {
        String hoy = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        return new ViewAsPdf("GenerarDetalleCitaPDF", ObtenerCitaPorId(id))
        {
            FileName = $"DetalleCita-{hoy}.pdf",
            PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
            PageSize = Rotativa.AspNetCore.Options.Size.A5
        };
    }

    public async Task<IActionResult> GenerarHistorialCitaPDF(long id)
    {
        var clienteId = HttpContext.Session.GetInt32("ClienteId");
        if (clienteId == null || clienteId == 0)
        {
            return RedirectToAction("Index", "Login");
        }

        CitaCliente? cita = null;
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"Cliente/listaCitasPorCliente/{clienteId}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var citas = JsonConvert.DeserializeObject<List<CitaCliente>>(data);
                cita = citas?.FirstOrDefault(c => c.ide_cit == id);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener cita: {ex.Message}");
        }

        if (cita == null)
        {
            TempData["Error"] = "No se encontró la cita especificada.";
            return RedirectToAction("ListaCitaPorCliente", "Cliente");
        }

        String hoy = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        return new ViewAsPdf("GenerarHistorialCitaPDF", cita)
        {
            FileName = $"HistorialMedico-{cita.mascota}-{hoy}.pdf",
            PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
            PageSize = Rotativa.AspNetCore.Options.Size.A4
        };
    }

    private async Task<Pago?> ObtenerPagoPendiente()
    {
        try
        {
            var idCliente = HttpContext.Session.GetInt32("ClienteId");
            if (idCliente == null || idCliente == 0)
                return null;

            var response = await _httpClient.GetAsync($"Pago/ListarPagosPorCliente/{idCliente}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var pagos = JsonConvert.DeserializeObject<List<Pago>>(data) ?? new List<Pago>();
                return pagos.FirstOrDefault(p => p.EstadoPago == "Pendiente");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener pago pendiente: {ex.Message}");
        }
        return null;
    }

    public IActionResult Index()
    {
        return View();
    }
}