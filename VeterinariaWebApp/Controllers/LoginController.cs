using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace VeterinariaWebApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LoginController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private async Task<string> IniciarSesionAsync(string uid, string pwd)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ClinicaAPI");
                var response = await client.GetAsync($"/api/Usuario/VerificarLogin?uid={Uri.EscapeDataString(uid)}&pwd={Uri.EscapeDataString(pwd)}");
                if (response.IsSuccessStatusCode)
                {
                    var contenido = await response.Content.ReadAsStringAsync();
                    return contenido.Trim('"', '\r', '\n', ' ');
                }
                return "denied";
            }
            catch
            {
                return "denied";
            }
        }

        private async Task<string> ObtenerTokenAsync(string uid)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ClinicaAPI");
                var response = await client.GetAsync($"/api/Usuario/ObtenerIdUsuario?correo={Uri.EscapeDataString(uid)}");
                if (response.IsSuccessStatusCode)
                {
                    var contenido = await response.Content.ReadAsStringAsync();
                    return contenido.Trim('"', '\r', '\n', ' ');
                }
                return "0";
            }
            catch
            {
                return "0";
            }
        }

        [HttpGet]
        public IActionResult Index()
        {
            HttpContext.Session.Clear();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string uid, string pwd)
        {
            if (string.IsNullOrWhiteSpace(uid) || string.IsNullOrWhiteSpace(pwd))
            {
                ViewBag.Mensaje = "Correo y contraseña son obligatorios.";
                return View();
            }

            string rol = await IniciarSesionAsync(uid, pwd);

            if (rol == "denied" || string.IsNullOrWhiteSpace(rol))
            {
                ViewBag.correo = uid;
                ViewBag.Mensaje = "Usuario o contraseña incorrectos.";
                return View();
            }

            string token = await ObtenerTokenAsync(uid);

            if (!long.TryParse(token, out long idUsuario) || idUsuario <= 0)
            {
                ViewBag.correo = uid;
                ViewBag.Mensaje = "Error al autenticar. Inténtelo de nuevo.";
                return View();
            }

            HttpContext.Session.SetString("token", token);

            if (rol == "Cliente")
            {
                HttpContext.Session.SetInt32("ClienteId", (int)idUsuario);
            }
            else if (rol == "Veterinario")
            {
                HttpContext.Session.SetInt32("VeterinarioId", (int)idUsuario);
            }
            else if (rol == "Recepcionista")
            {
                HttpContext.Session.SetInt32("RecepcionistaId", (int)idUsuario);
            }
            else
            {
                ViewBag.correo = uid;
                ViewBag.Mensaje = "Rol no reconocido.";
                return View();
            }

            return RedirectToAction("Index", rol);
        }
    }
}