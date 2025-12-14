using VeterinariaWebApp.Models.Mascota;
using VeterinariaWebApp.Models.Usuario;
using VeterinariaWebApp.Models.Usuario.Cliente;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        #region Métodos Auxiliares

        private async Task<List<UserDoc>> ObtenerTiposDocumentoAsync()
        {
            try
            {
                var client = GetClient();
                var response = await client.GetAsync("/api/Usuario/ListarDocumentos");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<UserDoc>>(data) ?? new List<UserDoc>();
                }
            }
            catch { }
            return new List<UserDoc>();
        }

        private async Task<ClienteO?> ObtenerClienteBackendPorUsuarioAsync(long ide_usr)
        {
            try
            {
                var client = GetClient();
                var response = await client.GetAsync("/api/Cliente/listaClientesBackend");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var clientes = JsonConvert.DeserializeObject<List<ClienteO>>(data) ?? new List<ClienteO>();
                    return clientes.FirstOrDefault(c => c.ide_usr == ide_usr);
                }
            }
            catch { }
            return null;
        }

        #endregion

        #region Perfil

        [HttpGet]
        public async Task<IActionResult> Perfil()
        {
            var idUsuario = HttpContext.Session.GetInt32("ClienteId");
            if (!idUsuario.HasValue)
                return RedirectToAction("Index", "Login");

            var clienteBackend = await ObtenerClienteBackendPorUsuarioAsync(idUsuario.Value);
            if (clienteBackend == null)
            {
                TempData["Error"] = "No se pudo cargar la información del perfil.";
                return RedirectToAction("Index");
            }

            var documentos = await ObtenerTiposDocumentoAsync();
            var tipoDoc = documentos.FirstOrDefault(d => d.ide_doc == clienteBackend.ide_doc);

            var perfil = new PerfilClienteViewModel
            {
                ide_cli = clienteBackend.ide_cli,
                ide_usr = clienteBackend.ide_usr,
                cor_usr = clienteBackend.cor_usr,
                pwd_usr = clienteBackend.pwd_usr,
                nom_usr = clienteBackend.nom_usr,
                ape_usr = clienteBackend.ape_usr,
                fna_usr = clienteBackend.fna_usr,
                num_doc = clienteBackend.num_doc,
                ide_doc = clienteBackend.ide_doc,
                nom_doc = tipoDoc?.nom_doc,
                ide_rol = clienteBackend.ide_rol
            };

            ViewBag.TiposDocumento = new SelectList(documentos, "ide_doc", "nom_doc", perfil.ide_doc);
            return View(perfil);
        }

        [HttpPost]
        public async Task<IActionResult> Perfil(PerfilClienteViewModel modelo)
        {
            var idUsuario = HttpContext.Session.GetInt32("ClienteId");
            if (!idUsuario.HasValue)
                return RedirectToAction("Index", "Login");

            var documentos = await ObtenerTiposDocumentoAsync();
            ViewBag.TiposDocumento = new SelectList(documentos, "ide_doc", "nom_doc", modelo.ide_doc);

            // Obtener datos actuales para mantener correo y contraseña
            var clienteActual = await ObtenerClienteBackendPorUsuarioAsync(idUsuario.Value);
            if (clienteActual == null)
            {
                TempData["Error"] = "Error al obtener datos del cliente.";
                return View(modelo);
            }

            // Mantener correo y contraseña originales
            modelo.cor_usr = clienteActual.cor_usr;
            modelo.pwd_usr = clienteActual.pwd_usr;
            modelo.ide_cli = clienteActual.ide_cli;
            modelo.ide_usr = clienteActual.ide_usr;

            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            // Crear objeto para actualizar
            var clienteActualizar = new ClienteO
            {
                ide_cli = modelo.ide_cli,
                ide_usr = modelo.ide_usr,
                cor_usr = modelo.cor_usr,
                pwd_usr = modelo.pwd_usr,
                nom_usr = modelo.nom_usr,
                ape_usr = modelo.ape_usr,
                fna_usr = modelo.fna_usr,
                num_doc = modelo.num_doc,
                ide_doc = modelo.ide_doc,
                ide_rol = 1
            };

            var client = GetClient();
            var json = JsonConvert.SerializeObject(clienteActualizar);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PutAsync("/api/Cliente/actualizarCliente", content);

            if (response.IsSuccessStatusCode)
            {
                // Actualizar el nombre en la sesión
                HttpContext.Session.SetString("NombreCliente", modelo.nom_usr ?? "Cliente");
                TempData["Exito"] = "Perfil actualizado correctamente.";
                return RedirectToAction("Perfil");
            }

            TempData["Error"] = "Error al actualizar el perfil. Intente nuevamente.";
            return View(modelo);
        }

        #endregion

        #region Citas y Clientes

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

        #endregion

        #region Mascotas

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

            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var client = GetClient();
            var json = JsonConvert.SerializeObject(modelo);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"/api/Cliente/agregarMascota/{idCliente.Value}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Exito"] = $"¡Mascota '{modelo.Nombre}' registrada correctamente!";
                return RedirectToAction("listaMascotasPorCliente", new { ide_usr = idCliente.Value });
            }

            TempData["Error"] = "Error al registrar la mascota. Intente nuevamente.";
            return View(modelo);
        }

        [HttpGet]
        public async Task<IActionResult> DetalleMascota(long id)
        {
            var idCliente = HttpContext.Session.GetInt32("ClienteId");
            if (!idCliente.HasValue)
                return RedirectToAction("Index", "Login");

            var client = GetClient();
            var response = await client.GetAsync($"/api/Cliente/listarMascotas/{idCliente.Value}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var mascotas = JsonConvert.DeserializeObject<List<Mascota>>(data) ?? new List<Mascota>();
                var mascota = mascotas.FirstOrDefault(m => m.IdMascota == id);

                if (mascota != null)
                {
                    return View(mascota);
                }
            }

            TempData["Error"] = "No se encontró la mascota solicitada.";
            return RedirectToAction("listaMascotasPorCliente", new { ide_usr = idCliente.Value });
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
            var clienteId = HttpContext.Session.GetInt32("ClienteId");
            if (!clienteId.HasValue)
                return RedirectToAction("Index", "Login");

            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var client = GetClient();
            var json = JsonConvert.SerializeObject(modelo);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"/api/Cliente/actualizarMascota", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Exito"] = $"¡Mascota '{modelo.Nombre}' actualizada correctamente!";
                return RedirectToAction("listaMascotasPorCliente", new { ide_usr = clienteId });
            }

            TempData["Error"] = "Error al actualizar la mascota.";
            return View(modelo);
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

        #endregion

        #region Dashboard

        public async Task<IActionResult> Index()
        {
            var idUsuario = HttpContext.Session.GetInt32("ClienteId");
            if (!idUsuario.HasValue)
                return RedirectToAction("Index", "Login");

            // Cargar nombre del cliente si no está en sesión
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("NombreCliente")))
            {
                var clienteBackend = await ObtenerClienteBackendPorUsuarioAsync(idUsuario.Value);
                if (clienteBackend != null)
                {
                    HttpContext.Session.SetString("NombreCliente", clienteBackend.nom_usr ?? "Cliente");
                }
            }

            return View();
        }

        #endregion

        #region Cerrar Sesión

        public IActionResult CerrarSesion()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }

        #endregion
    }
}
