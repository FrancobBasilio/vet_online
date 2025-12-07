using VeterinariaWebApp.Models.Usuario.Veterinario;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Rotativa.AspNetCore;
using System.Net.Http;

namespace VeterinariaWebApp.Controllers;

public class VeterinarioController : Controller
{
    private readonly Uri _baseUri = new("https://localhost:7054/api");
    private readonly HttpClient _httpClient;

    public VeterinarioController()
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = _baseUri;
    }

    // Método para obtener citas asignadas al veterinario logueado
    public async Task<List<CitaVeterinario>> ArregloCitaVeterinario(long ide_usr)
    {
        List<CitaVeterinario> aCitaVeterinario = new();
        try
        {
            string url = $"{_baseUri}/Veterinario/listaCitasPorVeterinario/{ide_usr}";
            HttpResponseMessage response = await _httpClient.GetAsync(url);
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

    // Acción para mostrar la lista de citas asignadas
    [HttpGet]
    public async Task<IActionResult> listaCitaPorVeterinarios(long ide_usr)
    {
        if (ide_usr == 0)
        {
            return Content("ID del veterinario no recibido o inválido");
        }
        var citas = await ArregloCitaVeterinario(ide_usr);
        return View(citas);
    }

    // Método para obtener mascotas atendidas por el veterinario logueado
    public async Task<List<MascotaPorVeterinario>> ArregloMascotaPorVeterinario(long ide_usr)
    {
        List<MascotaPorVeterinario> aMascotaVeterinario = new();
        try
        {
            string url = $"{_baseUri}/Veterinario/listaMascotasPorVeterinario/{ide_usr}";
            HttpResponseMessage response = await _httpClient.GetAsync(url);
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

    // Acción para mostrar la lista de mascotas atendidas
    [HttpGet]
    public async Task<IActionResult> listaMascotaPorVeterinarios(long ide_usr)
    {
        if (ide_usr == 0)
        {
            return Content("ID del veterinario no recibido o inválido");
        }
        var mascotas = await ArregloMascotaPorVeterinario(ide_usr);
        return View(mascotas);
    }

    // Acción para generar el PDF de la lista de mascotas atendidas
    [HttpGet]
    public async Task<IActionResult> GenerarPDFMascotasAtendidas(long ide_usr)
    {
        if (ide_usr == 0)
        {
            return Content("ID del veterinario no recibido o inválido");
        }

        var mascotas = await ArregloMascotaPorVeterinario(ide_usr);
        String hoy = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        return new ViewAsPdf("GenerarPDFMascotasAtendidas", mascotas)
        {
            FileName = $"MascotasAtendidas-{hoy}.pdf",
            PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
            PageSize = Rotativa.AspNetCore.Options.Size.A4
        };
    }

    // Acción para el dashboard del veterinario
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
            string url = $"{_baseUri}/Veterinario/estadisticasVeterinario/{veterinarioId}";
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                stats = JsonConvert.DeserializeObject<VeterinarioStats>(data);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener estadísticas del veterinario: {ex.Message}");
        }

        // Obtener los datos del veterinario
        var veterinario = new Veterinario();
        try
        {
            string url = $"{_baseUri}/Veterinario/buscarVeterinario/{veterinarioId}";
            HttpResponseMessage response = await _httpClient.GetAsync(url);
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

        ViewBag.Stats = stats;
        ViewBag.Veterinario = veterinario;

        return View();
    }
}