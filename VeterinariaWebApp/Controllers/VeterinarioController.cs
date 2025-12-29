using VeterinariaWebApp.Models.Usuario.Veterinario;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Rotativa.AspNetCore;

namespace VeterinariaWebApp.Controllers;

public class VeterinarioController : Controller
{
    private readonly HttpClient _httpClient;

    public VeterinarioController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("ClinicaAPI");
    }

    // ==================== DASHBOARD ====================

    public async Task<IActionResult> Index()
    {
        var veterinarioId = HttpContext.Session.GetInt32("VeterinarioId");
        if (veterinarioId == null || veterinarioId == 0)
        {
            return RedirectToAction("Index", "Login");
        }

        // Obtener las estadísticas
        VeterinarioStats stats = new VeterinarioStats();
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"Veterinario/estadisticasVeterinario/{veterinarioId}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                stats = JsonConvert.DeserializeObject<VeterinarioStats>(data);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener estadísticas: {ex.Message}");
        }

        // Obtener los datos del veterinario
        var veterinario = new Veterinario();
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"Veterinario/buscarVeterinario/{veterinarioId}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                veterinario = JsonConvert.DeserializeObject<Veterinario>(data);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener datos del veterinario: {ex.Message}");
        }

        // Obtener citas del día (pendientes)
        var citasHoy = new List<CitaVeterinario>();
        try
        {
            var todasLasCitas = await ArregloCitaVeterinario(veterinarioId.Value);
            citasHoy = todasLasCitas
                .Where(c => c.cal_cit.Date == DateTime.Today && c.est_cit == "P")
                .OrderBy(c => c.cal_cit)
                .Take(5)
                .ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener citas de hoy: {ex.Message}");
        }

        ViewBag.Stats = stats;
        ViewBag.Veterinario = veterinario;
        ViewBag.CitasHoy = citasHoy;

        return View();
    }

    // ==================== CITAS ====================

    public async Task<List<CitaVeterinario>> ArregloCitaVeterinario(long ide_usr)
    {
        List<CitaVeterinario> aCitaVeterinario = new();
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"Veterinario/listaCitasPorVeterinario/{ide_usr}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                aCitaVeterinario = JsonConvert.DeserializeObject<List<CitaVeterinario>>(data);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener citas del veterinario: {ex.Message}");
        }
        return aCitaVeterinario;
    }

    [HttpGet]
    public async Task<IActionResult> listaCitaPorVeterinarios()
    {
        var veterinarioId = HttpContext.Session.GetInt32("VeterinarioId");
        if (veterinarioId == null || veterinarioId == 0)
        {
            return RedirectToAction("Index", "Login");
        }

        var citas = await ArregloCitaVeterinario(veterinarioId.Value);
        return View(citas);
    }

    [HttpGet]
    public async Task<IActionResult> IniciarAtencion(long id)
    {
        var veterinarioId = HttpContext.Session.GetInt32("VeterinarioId");
        if (veterinarioId == null || veterinarioId == 0)
        {
            return RedirectToAction("Index", "Login");
        }

        try
        {
            var response = await _httpClient.PutAsync($"Cita/actualizarEstado/{id}?estado=E", null);

            if (response.IsSuccessStatusCode)
            {
                TempData["Exito"] = "Atención iniciada. Complete el formulario de atención médica.";
                return RedirectToAction("AtenderCita", new { id = id });
            }
            else
            {
                TempData["Error"] = "No se pudo iniciar la atención de la cita.";
            }
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Error: " + ex.Message;
        }

        return RedirectToAction("listaCitaPorVeterinarios");
    }

    [HttpGet]
    public async Task<IActionResult> AtenderCita(long id)
    {
        var veterinarioId = HttpContext.Session.GetInt32("VeterinarioId");
        if (veterinarioId == null || veterinarioId == 0)
        {
            return RedirectToAction("Index", "Login");
        }

        var citas = await ArregloCitaVeterinario(veterinarioId.Value);
        var cita = citas.FirstOrDefault(c => c.ide_cit == id);

        if (cita == null)
        {
            TempData["Error"] = "No se encontró la cita especificada.";
            return RedirectToAction("listaCitaPorVeterinarios");
        }

        if (cita.est_cit != "E")
        {
            TempData["Error"] = "Esta cita no está en proceso de atención.";
            return RedirectToAction("listaCitaPorVeterinarios");
        }

        var viewModel = new AtencionCitaViewModel
        {
            IdCita = cita.ide_cit,
            FechaCita = cita.cal_cit,
            Consultorio = cita.con_cit,
            NombreMascota = cita.mascota,
            Especie = cita.especie,
            NombreDueno = cita.nombre_dueno,
            DocumentoDueno = cita.doc_dueno,
            MontoPago = cita.mon_pag,
            MetodoPago = cita.nom_pay,
            EstadoCita = cita.est_cit
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> FinalizarAtencion(AtencionCitaViewModel model)
    {
        var veterinarioId = HttpContext.Session.GetInt32("VeterinarioId");
        if (veterinarioId == null || veterinarioId == 0)
        {
            return RedirectToAction("Index", "Login");
        }

        if (string.IsNullOrWhiteSpace(model.Diagnostico) || string.IsNullOrWhiteSpace(model.Tratamiento))
        {
            TempData["Error"] = "Debe completar el diagnóstico y tratamiento.";

            var citas = await ArregloCitaVeterinario(veterinarioId.Value);
            var cita = citas.FirstOrDefault(c => c.ide_cit == model.IdCita);
            if (cita != null)
            {
                model.NombreMascota = cita.mascota;
                model.Especie = cita.especie;
                model.NombreDueno = cita.nombre_dueno;
                model.DocumentoDueno = cita.doc_dueno;
                model.MontoPago = cita.mon_pag;
                model.MetodoPago = cita.nom_pay;
                model.FechaCita = cita.cal_cit;
                model.Consultorio = cita.con_cit;
                model.EstadoCita = cita.est_cit;
            }
            return View("AtenderCita", model);
        }

        try
        {
            var historialDto = new
            {
                IdCita = model.IdCita,
                Sintomas = model.Sintomas,
                Diagnostico = model.Diagnostico,
                Tratamiento = model.Tratamiento,
                Medicamentos = model.Medicamentos,
                Observaciones = model.Observaciones
            };

            var jsonContent = new StringContent(
                JsonConvert.SerializeObject(historialDto),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            var historialResponse = await _httpClient.PostAsync("Cita/agregarHistorial", jsonContent);

            if (!historialResponse.IsSuccessStatusCode)
            {
                TempData["Error"] = "Error al guardar el historial médico.";
                return RedirectToAction("AtenderCita", new { id = model.IdCita });
            }

            var estadoResponse = await _httpClient.PutAsync($"Cita/actualizarEstado/{model.IdCita}?estado=A", null);

            if (estadoResponse.IsSuccessStatusCode)
            {
                TempData["Exito"] = $"¡Atención finalizada! La cita de {model.NombreMascota} ha sido completada y el historial médico guardado.";
                return RedirectToAction("listaCitaPorVeterinarios");
            }
            else
            {
                TempData["Error"] = "No se pudo finalizar la atención. Intente nuevamente.";
                return RedirectToAction("AtenderCita", new { id = model.IdCita });
            }
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Error: " + ex.Message;
            return RedirectToAction("AtenderCita", new { id = model.IdCita });
        }
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> CancelarAtencion(long id)
    {
        try
        {
            var response = await _httpClient.PutAsync($"Cita/actualizarEstado/{id}?estado=P", null);

            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "La atención ha sido cancelada. La cita volvió a estado Pendiente." });
            }
            else
            {
                return Json(new { success = false, message = "No se pudo cancelar la atención." });
            }
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error: " + ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> DetalleCitaAtendida(long id)
    {
        var veterinarioId = HttpContext.Session.GetInt32("VeterinarioId");
        if (veterinarioId == null || veterinarioId == 0)
        {
            return RedirectToAction("Index", "Login");
        }

        var citas = await ArregloCitaVeterinario(veterinarioId.Value);
        var cita = citas.FirstOrDefault(c => c.ide_cit == id);

        if (cita == null)
        {
            TempData["Error"] = "No se encontró la cita especificada.";
            return RedirectToAction("listaCitaPorVeterinarios");
        }

        return View(cita);
    }

    // ==================== MASCOTAS ====================

    public async Task<List<MascotaPorVeterinario>> ArregloMascotaPorVeterinario(long ide_usr)
    {
        List<MascotaPorVeterinario> aMascotaVeterinario = new();
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"Veterinario/listaMascotasPorVeterinario/{ide_usr}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                aMascotaVeterinario = JsonConvert.DeserializeObject<List<MascotaPorVeterinario>>(data);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener mascotas del veterinario: {ex.Message}");
        }
        return aMascotaVeterinario;
    }

    public async Task<List<MascotaAtendida>> ArregloMascotasAtendidasConHistorial(long ide_usr)
    {
        List<MascotaAtendida> lista = new();
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"Veterinario/listaMascotasAtendidasConHistorial/{ide_usr}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                lista = JsonConvert.DeserializeObject<List<MascotaAtendida>>(data);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener mascotas atendidas: {ex.Message}");
        }
        return lista;
    }

    [HttpGet]
    public async Task<IActionResult> listaMascotaPorVeterinarios()
    {
        var veterinarioId = HttpContext.Session.GetInt32("VeterinarioId");
        if (veterinarioId == null || veterinarioId == 0)
        {
            return RedirectToAction("Index", "Login");
        }

        var mascotas = await ArregloMascotasAtendidasConHistorial(veterinarioId.Value);
        return View("listaMascotaAtendidasConHistorial", mascotas);
    }

    [HttpGet]
    public async Task<IActionResult> GenerarPDFMascotaAtendida(long idCita)
    {
        var veterinarioId = HttpContext.Session.GetInt32("VeterinarioId");
        if (veterinarioId == null || veterinarioId == 0)
        {
            return RedirectToAction("Index", "Login");
        }

        var mascotas = await ArregloMascotasAtendidasConHistorial(veterinarioId.Value);
        var mascota = mascotas.FirstOrDefault(m => m.ide_cit == idCita);

        if (mascota == null)
        {
            TempData["Error"] = "No se encontró el registro de atención.";
            return RedirectToAction("listaMascotaPorVeterinarios");
        }

        string hoy = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

        return new ViewAsPdf("GenerarPDFMascotaAtendida", mascota)
        {
            FileName = $"HistorialMedico-{mascota.mascota}-{hoy}.pdf",
            PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
            PageSize = Rotativa.AspNetCore.Options.Size.A4
        };
    }

    [HttpGet]
    public async Task<IActionResult> GenerarPDFMascotasAtendidas()
    {
        var veterinarioId = HttpContext.Session.GetInt32("VeterinarioId");
        if (veterinarioId == null || veterinarioId == 0)
        {
            return RedirectToAction("Index", "Login");
        }

        var mascotas = await ArregloMascotaPorVeterinario(veterinarioId.Value);
        string hoy = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

        return new ViewAsPdf("GenerarPDFMascotasAtendidas", mascotas)
        {
            FileName = $"MascotasAtendidas-{hoy}.pdf",
            PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
            PageSize = Rotativa.AspNetCore.Options.Size.A4
        };
    }
}