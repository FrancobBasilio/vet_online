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
using System.Net.Http;

namespace VeterinariaWebApp.Controllers;

public class CitaController : Controller
{
    private readonly Uri _baseUri = new("https://localhost:7054/api");
    private readonly HttpClient _httpClient;

    public CitaController()
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = _baseUri;
    }

    // Método auxiliar para obtener citas
    public List<Cita> ArregloCitas()
    {
        List<Cita> aCitas = new List<Cita>();
        try
        {
            string url = $"{_baseUri}/Cita/listaCita";
            HttpResponseMessage response = _httpClient.GetAsync(url).Result;
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

    // Método corregido: listadoVeterinario
    public List<Veterinario> listadoVeterinario()
    {
        List<Veterinario> aVeterinarios = new List<Veterinario>();
        try
        {
            string url = $"{_baseUri}/Veterinario/listaVeterinarios";
            HttpResponseMessage response = _httpClient.GetAsync(url).Result;
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

    // Método corregido: listadoCliente
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

    // Método para listar citas por fecha
    public List<Cita> listarCitasPorFecha(int dia, int mes, int año)
    {
        List<Cita> citas = new List<Cita>();
        try
        {
            string url = $"{_baseUri}/Cita/listaCitaPorFecha?dia={dia}&mes={mes}&año={año}";
            HttpResponseMessage response = _httpClient.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                var data = response.Content.ReadAsStringAsync().Result;
                citas = JsonConvert.DeserializeObject<List<Cita>>(data) ?? new List<Cita>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener citas por fecha: {ex.Message}");
        }
        return citas;
    }

  
    public IActionResult ListadoCitas(int? dia, int? mes, int? año)
    {
        List<Cita> citas = ArregloCitas();
        if (dia.HasValue && mes.HasValue && año.HasValue)
        {
            citas = listarCitasPorFecha(dia.Value, mes.Value, año.Value);
        }

        ViewBag.Dia = dia;
        ViewBag.Mes = mes;
        ViewBag.Año = año;
        return View(citas);
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
    public IActionResult IniciarCreacionCita()
    {
      
        return RedirectToAction("Crear", "Pago");
    }




    //  Cita/nuevaCita
    // GET: Cita/nuevaCita
    [HttpGet]
    public IActionResult nuevaCita(int PagoId)
    {
        int IdCliente = int.Parse(HttpContext.Session.GetString("token"));

        // Obtener listado de veterinarios con NombreUsuario + Especialidad
        var veterinarios = listadoVeterinario()
            .Select(v => new SelectListItem
            {
                Value = v.IdVeterinario.ToString(),
                Text = $"Dr. {v.NombreUsuario} - {v.especialidad}"
            })
            .ToList();

        // Obtener listado de mascotas del cliente actual
        var clientMascotas = new List<Mascota>(); // Inicializamos como lista vacía
        try
        {
            var clientSessionId = HttpContext.Session.GetInt32("ClienteId");
            if (clientSessionId.HasValue)
            {
                string url = $"{_baseUri}/Cliente/listarMascotas/{clientSessionId.Value}";
                var clientResponse = _httpClient.GetAsync(url).Result;
                if (clientResponse.IsSuccessStatusCode)
                {
                    var data = clientResponse.Content.ReadAsStringAsync().Result;
                    clientMascotas = JsonConvert.DeserializeObject<List<Mascota>>(data) ?? new List<Mascota>();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener mascotas del cliente: {ex.Message}");
        }

        ViewBag.veterinarios = veterinarios;
        ViewBag.mascotas = new SelectList(clientMascotas, "IdMascota", "Nombre"); // Aseguramos que mascotas siempre tenga un valor
        ViewBag.clientes = new SelectList(listadoCliente(), "IdCliente", "NombreUsuario");

        CitaO citaPagada = new CitaO() { IdPago = PagoId };
        return View(citaPagada);
    }



    // POST: Cita/nuevaCita
    [HttpPost]
    public async Task<IActionResult> nuevaCita(CitaO obj)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.veterinarios = new SelectList(listadoVeterinario(), "IdVeterinario", "NombreUsuario");
            ViewBag.mascotas = new SelectList(new List<Mascota>(), "IdMascota", "Nombre"); // Lista vacía si hay error
            ViewBag.clientes = new SelectList(listadoCliente(), "IdCliente", "NombreUsuario");
            return View(obj);
        }

        // Validar que no haya una cita ya programada a la misma hora para el mismo veterinario
        bool citaExiste = await ExisteCita(obj);
        if (citaExiste)
        {
            ModelState.AddModelError("CalendarioCita", "Ya existe una cita programada para este veterinario en esta fecha y hora.");
            ViewBag.veterinarios = new SelectList(listadoVeterinario(), "IdVeterinario", "NombreUsuario");
            ViewBag.mascotas = new SelectList(new List<Mascota>(), "IdMascota", "Nombre"); // Lista vacía si hay error
            ViewBag.clientes = new SelectList(listadoCliente(), "IdCliente", "NombreUsuario");
            return View(obj);
        }

        var json = JsonConvert.SerializeObject(obj);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var responseC = await _httpClient.PostAsync($"{_baseUri}/Cita/agregaCita", content);

        if (responseC.IsSuccessStatusCode)
        {
            int? idCliente = HttpContext.Session.GetInt32("ClienteId");
            if (idCliente == null || idCliente == 0)
            {
                return RedirectToAction("Index", "Login");
            }
            return RedirectToAction("listaCitaPorCliente", "Cliente", new { ide_usr = Convert.ToInt64(idCliente) });
        }

        ViewBag.mensaje = "Error al registrar la cita.";
        ViewBag.veterinarios = new SelectList(listadoVeterinario(), "IdVeterinario", "NombreUsuario");
        ViewBag.mascotas = new SelectList(new List<Mascota>(), "IdMascota", "Nombre"); // Lista vacía si hay error
        ViewBag.clientes = new SelectList(listadoCliente(), "IdCliente", "NombreUsuario");
        return View(obj);
    }




    [HttpGet]
    public async Task<IActionResult> actualizarCita(int id)
    {
        // 1. Obtener la cita completa
        var response = await _httpClient.GetAsync($"{_baseUri}/Cita/buscarCita/{id}");
        if (!response.IsSuccessStatusCode)
        {
            ViewBag.mensaje = "No Hay Cita";
            return View();
        }

        var content = await response.Content.ReadAsStringAsync();
        var objC = JsonConvert.DeserializeObject<CitaO>(content);

        // 2. Obtener la mascota para saber quién es el dueño (cliente)
        var mascotaResponse = await _httpClient.GetAsync($"{_baseUri}/Cliente/listarMascotasPorId/{objC.IdMascota}");
        if (!mascotaResponse.IsSuccessStatusCode)
        {
            ViewBag.mensaje = "Error al obtener los datos de la mascota.";
            return View();
        }

        var mascotaData = await mascotaResponse.Content.ReadAsStringAsync();
        var mascota = JsonConvert.DeserializeObject<MascotaConCliente>(mascotaData);

        // 3. Ahora, con el IdUsuario del cliente, obtener todas sus mascotas
        var todasLasMascotasResponse = await _httpClient.GetAsync($"{_baseUri}/Cliente/listarMascotas/{mascota.IdUsuario}");
        List<Mascota> mascotasDelDueño = new List<Mascota>();
        if (todasLasMascotasResponse.IsSuccessStatusCode)
        {
            var todasLasMascotasData = await todasLasMascotasResponse.Content.ReadAsStringAsync();
            mascotasDelDueño = JsonConvert.DeserializeObject<List<Mascota>>(todasLasMascotasData) ?? new List<Mascota>();
        }

        // 4. Preparar los ViewBag
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
            // Mantener el formato de veterinarios con especialidad cuando hay error de validación
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
        var response = await _httpClient.PutAsync($"{_baseUri}/Cita/actualizaCita?id={id}", content);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("ListadoCitas");
        }

        // Si falla la actualización, mantener el formato de veterinarios con especialidad
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




    // POST: Cita/eliminarCita (AJAX)
    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> eliminarCita(int id)
    {
        var response = await _httpClient.DeleteAsync($"{_baseUri}/Cita/eliminarCita/{id}");
        if (response.IsSuccessStatusCode)
        {
            return Json(new { success = true });
        }
        else
        {
            return Json(new { success = false });
        }
    }



    public Cita ObtenerCitaPorId(long id)
    {
        Cita cita = null;
        try
        {
            string url = $"{_baseUri}/Cita/buscarCitaFront/{id}";
            HttpResponseMessage response = _httpClient.GetAsync(url).Result;
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

   
    public IActionResult Index()
    {
        return View();
    }
}