using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
using System.Text;
using VeterinariaWebApp.Models.Cita;
using VeterinariaWebApp.Models.Usuario;
using VeterinariaWebApp.Models.Usuario.Cliente;
using VeterinariaWebApp.Models.Usuario.Veterinario;

namespace VeterinariaWebApp.Controllers
{
    public class RecepcionistaController : Controller
    {
        private readonly Uri _baseUri = new("https://localhost:7054/api");
        private readonly HttpClient _httpClient;

        public RecepcionistaController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = _baseUri;
        }



        public async Task<IActionResult> Index()
        {
            var conteoPendientes = await ObtenerConteoCitas("P");
            var conteoVencidas = await ObtenerConteoCitas("P", soloVencidas: true);
            var conteoAtendidas = await ObtenerConteoCitas("A");
            var conteoCanceladas = await ObtenerConteoCitas("C");

            ViewBag.CitasPendientes = conteoPendientes;
            ViewBag.CitasVencidas = conteoVencidas;
            ViewBag.CitasAtendidas = conteoAtendidas;
            ViewBag.CitasCanceladas = conteoCanceladas;

            return View();
        }


        private async Task<int> ObtenerConteoCitas(string estado, bool soloVencidas = false)
        {
            var url = soloVencidas
                ? $"{_baseUri}/Cita/listaCitasVencidas"
                : $"{_baseUri}/Cita/listaCitasPorEstado?estado={estado}";

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var lista = JsonConvert.DeserializeObject<List<Cita>>(json);
                return lista.Count;
            }
            return 0;
        }





        public List<Veterinario> ArregloVeterinarios()
        {
            List<Veterinario> aVeterinarios = new List<Veterinario>();
            try
            {
                string url = $"{_baseUri}/Veterinario/listaVeterinarios";
                HttpResponseMessage response = _httpClient.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    aVeterinarios = JsonConvert.DeserializeObject<List<Veterinario>>(data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener veterinarios: {ex.Message}");
            }
            return aVeterinarios;
        }

      
        public List<Cliente> ArregloClientes()
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

        public List<Especialidad> ArregloEspecialidad()
        {
            List<Especialidad> aEspecialidad = new List<Especialidad>();
            try
            {
                string url = $"{_baseUri}/Veterinario/listarEspecialidad";
                HttpResponseMessage response = _httpClient.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    aEspecialidad = JsonConvert.DeserializeObject<List<Especialidad>>(data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener especialidades: {ex.Message}");
            }
            return aEspecialidad;
        }

        public List<UserDoc> ArregloTipoDocumentos()
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

        public String AgregarVeterinario(VeterinarioO veterinario)
        {
            StringContent content = new StringContent(JsonConvert.SerializeObject(veterinario), Encoding.UTF8, "application/json");
            try
            {
                string url = $"{_baseUri}/Veterinario/nuevoVeterinario";
                HttpResponseMessage response = _httpClient.PostAsync(url, content).Result;
                return response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al agregar veterinario: {ex.Message}");
                return "Error al guardar veterinario";
            }
        }

        public String AgregarCliente(ClienteO cliente)
        {
            StringContent content = new StringContent(JsonConvert.SerializeObject(cliente), Encoding.UTF8, "application/json");
            try
            {
                string url = $"{_baseUri}/Cliente/nuevoCliente";
                HttpResponseMessage response = _httpClient.PostAsync(url, content).Result;
                return response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al agregar cliente: {ex.Message}");
                return "Error al guardar cliente";
            }
        }

        [HttpGet]
        public IActionResult NuevoVeterinario()
        {
            ViewBag.especialidad = new SelectList(ArregloEspecialidad(), "ide_esp", "nom_esp");
            ViewBag.documentos = new SelectList(ArregloTipoDocumentos(), "ide_doc", "nom_doc");
            return View();
        }

        [HttpPost]
        public IActionResult NuevoVeterinario(VeterinarioO veterinario)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.especialidad = new SelectList(ArregloEspecialidad(), "ide_esp", "nom_esp");
                ViewBag.documentos = new SelectList(ArregloTipoDocumentos(), "ide_doc", "nom_doc");
                return View(veterinario);
            }

            ViewBag.respuesta = AgregarVeterinario(veterinario);
            return RedirectToAction("listarVeterinarios");
        }

        public IActionResult listarVeterinarios()
        {
            var veterinarios = ArregloVeterinarios(); 
            return View(veterinarios); 
        }

        public IActionResult listarVeterinariosPDF()
        {
            String hoy = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            return new ViewAsPdf("listarVeterinariosPDF", ArregloVeterinarios())
            {
                FileName = $"ListadoVeterinarios-{hoy}.pdf",
                PageOrientation = Orientation.Portrait,
                PageSize = Size.A4
            };
        }

        [HttpGet]
        public IActionResult NuevoCliente()
        {
            ViewBag.documentos = new SelectList(ArregloTipoDocumentos(), "ide_doc", "nom_doc");
            return View();
        }

        [HttpPost]
        public IActionResult NuevoCliente(ClienteO cliente)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.documentos = new SelectList(ArregloTipoDocumentos(), "ide_doc", "nom_doc");
                return View(cliente);
            }

            ViewBag.respuesta = AgregarCliente(cliente);
            return RedirectToAction("listarClientes");
        }

        public IActionResult listarClientes()
        {
            var clientes = ArregloClientes(); 
            return View(clientes);
        }



        public IActionResult listarClientesPDF()
        {
            String hoy = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            return new ViewAsPdf("listarClientesPDF", ArregloClientes())
            {
                FileName = $"ListadoClientes-{hoy}.pdf",
                PageOrientation = Orientation.Portrait,
                PageSize = Size.A4
            };
        }

   
    }
}