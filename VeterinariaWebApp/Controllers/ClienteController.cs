using VeterinariaWebApp.Models.Mascota;
using VeterinariaWebApp.Models.Usuario.Cliente;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Rotativa.AspNetCore;
using System.Text;

namespace VeterinariaWebApp.Controllers
{
    public class ClienteController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ClienteController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient GetClient() => _httpClientFactory.CreateClient("ClinicaAPI");

        public async Task<List<CitaCliente>> aCitaCliente(long ide_usr)
        {
            var client = GetClient();
            var response = await client.GetAsync($"/api/Cliente/listaCitasPorCliente/{ide_usr}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<CitaCliente>>(data) ?? new List<CitaCliente>();
            }
            return new List<CitaCliente>();
        }

        public async Task<Cliente> ObtenerClientePorId(long id)
        {
            var client = GetClient();
            var response = await client.GetAsync($"/api/Cliente/buscarCliente/{id}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Cliente>(data) ?? new Cliente();
            }
            return new Cliente();
        }

        public async Task<IActionResult> listaCitaPorCliente(long ide_usr)
        {
            if (ide_usr == 0)
                return Content("ID del cliente no recibido o inválido");

            var citas = await aCitaCliente(ide_usr);
            return View(citas);
        }

        public async Task<IActionResult> DetalleCliente(long id)
        {
            if (id == 0)
                return Content("ID del cliente no recibido");
            var cliente = await ObtenerClientePorId(id);
            return View(cliente);
        }

        public async Task<IActionResult> DetalleClientePDF(long id)
        {
            var cliente = await ObtenerClientePorId(id);
            string hoy = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            return new ViewAsPdf("DetalleClientePDF", cliente)
            {
                FileName = $"DetalleCliente-{hoy}.pdf",
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageSize = Rotativa.AspNetCore.Options.Size.A5
            };
        }

        public async Task<IActionResult> listaMascotasPorCliente(long ide_usr)
        {
            if (ide_usr == 0)
                return Content("ID del cliente no recibido o inválido");

            var client = GetClient();
            var response = await client.GetAsync($"/api/Cliente/listarMascotas/{ide_usr}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var mascotas = JsonConvert.DeserializeObject<List<Mascota>>(data);
                return View(mascotas ?? new List<Mascota>());
            }

            return View(new List<Mascota>());
        }

        public IActionResult AgregarMascota()
        {
            var idCliente = HttpContext.Session.GetInt32("ClienteId");
            if (!idCliente.HasValue)
                return RedirectToAction("Index", "Login");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AgregarMascota(Mascota modelo)
        {
            var idCliente = HttpContext.Session.GetInt32("ClienteId");
            if (!idCliente.HasValue)
                return RedirectToAction("Index", "Login");

            var client = GetClient();
            var json = JsonConvert.SerializeObject(modelo);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"/api/Cliente/agregarMascota/{idCliente.Value}", content);

            if (response.IsSuccessStatusCode)
            {
                ViewBag.Mensaje = "Mascota registrada correctamente.";
            }
            else
            {
                ViewBag.Mensaje = "Error al registrar la mascota.";
            }

            return View(modelo);
        }


        [HttpGet]
        public async Task<IActionResult> ActualizarMascota(long id)
        {
            var client = GetClient();
            var response = await client.GetAsync($"/api/Cliente/listarMascotas/{HttpContext.Session.GetInt32("ClienteId")}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var todasLasMascotas = JsonConvert.DeserializeObject<List<Mascota>>(data) ?? new List<Mascota>();

              
                var mascotaAEditar = todasLasMascotas.FirstOrDefault(m => m.IdMascota == id);

                if (mascotaAEditar == null)
                {
                    ViewBag.Mensaje = "No se encontró la mascota.";
                    return View(new Mascota());
                }

                return View(mascotaAEditar);
            }

            ViewBag.Mensaje = "Error al cargar los datos de la mascota.";
            return View(new Mascota());
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarMascota(Mascota modelo)
        {
            var client = GetClient();
            var json = JsonConvert.SerializeObject(modelo);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"/api/Cliente/actualizarMascota", content);

            if (response.IsSuccessStatusCode)
            {
              
                var clienteId = HttpContext.Session.GetInt32("ClienteId");
                return RedirectToAction("listaMascotasPorCliente", new { ide_usr = clienteId });
            }
            else
            {
        
                ViewBag.Mensaje = "Error al actualizar la mascota.";
                return View(modelo);
            }
        }



        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> EliminarMascota(long id)
        {
            var client = GetClient();
            var response = await client.DeleteAsync($"/api/Cliente/eliminarMascota/{id}");

            if (response.IsSuccessStatusCode)
            {
                var mensaje = await response.Content.ReadAsStringAsync();
               
                if (mensaje.Contains("correctamente"))
                {
                    return Json(new { success = true, message = "Mascota eliminada correctamente." });
                }
                else
                {
                    return Json(new { success = false, message = mensaje.Replace("\"", "") });
                }
            }

            return Json(new { success = false, message = "Error al intentar eliminar la mascota." });
        }






        public IActionResult Index()
        {
            return View();
        }
    }
}