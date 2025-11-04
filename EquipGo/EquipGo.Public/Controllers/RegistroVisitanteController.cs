using EquipGo.Public.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace EquipGo.Public.Controllers
{
    public class RegistroVisitanteController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public RegistroVisitanteController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new RegistroVisitanteViewModel
            {
                Proveedores = await ObtenerProveedores(),
                TiposDocumento = await ObtenerTiposDocumento()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(RegistroVisitanteViewModel model, string accion)
        {
            // Siempre aseguramos tener proveedores y tipos de documento disponibles
            model.Proveedores = await ObtenerProveedores();
            model.TiposDocumento = await ObtenerTiposDocumento();

            switch (accion)
            {
                case "comenzar":
                    model.InicioFormulario = true;
                    model.PasoActual = 1;
                    break;

                case "siguiente":
                    if (string.IsNullOrWhiteSpace(model.Visitante.TipoDocumento) ||
                        string.IsNullOrWhiteSpace(model.Visitante.NumeroDocumento) ||
                        string.IsNullOrWhiteSpace(model.Visitante.Nombres) ||
                        string.IsNullOrWhiteSpace(model.Visitante.Apellidos) ||
                        string.IsNullOrWhiteSpace(model.Visitante.TipoUsuario))
                    {
                        model.Mensaje = "⚠️ Completa todos los campos del visitante.";
                        break;
                    }

                    if (model.Visitante.TipoUsuario == "proveedor" && model.Visitante.IdProveedor == null)
                    {
                        model.Mensaje = "⚠️ Debes seleccionar un proveedor.";
                        break;
                    }

                    model.PasoActual = 2;
                    model.Mensaje = "";
                    break;

                case "atras":
                    model.PasoActual = 1;
                    break;

                case "registrar":
                    if (string.IsNullOrWhiteSpace(model.Visitante.Marca))
                    {
                        model.Mensaje = "⚠️ La marca del equipo es obligatoria.";
                        break;
                    }

                    var client = _clientFactory.CreateClient();
                    var response = await client.PostAsJsonAsync("https://test.outsourcing.col:6997/api/visitantes", model.Visitante);

                    if (response.IsSuccessStatusCode)
                    {
                        model = new RegistroVisitanteViewModel
                        {
                            Proveedores = await ObtenerProveedores(),
                            TiposDocumento = await ObtenerTiposDocumento(),
                            Mensaje = "✅ Registro exitoso. Dirígete al punto de control."
                        };
                    }
                    else
                    {
                        model.Mensaje = "❌ Error al registrar visitante.";
                    }
                    break;
            }

            return View(model);
        }

        private async Task<List<ProveedorDto>> ObtenerProveedores()
        {
            var client = _clientFactory.CreateClient();

            try
            {
                var response = await client.GetAsync("https://test.outsourcing.col:6997/api/proveedores");

                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<List<ProveedorDto>>() ?? new();
            }
            catch
            {
                // Puedes registrar el error si deseas
            }

            return new();
        }

        private async Task<List<TipoDocumentoDto>> ObtenerTiposDocumento()
        {
            var client = _clientFactory.CreateClient();

            try
            {
                var response = await client.GetAsync("https://test.outsourcing.col:6997/api/tiposdocumentos");
                Console.WriteLine($"Status Code: {response.StatusCode}"); // Debug

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Response Content: {content}"); // Debug

                    var tipos = await response.Content.ReadFromJsonAsync<List<TipoDocumentoDto>>();
                    Console.WriteLine($"Tipos deserializados: {tipos?.Count ?? 0}"); // Debug

                    return tipos ?? new List<TipoDocumentoDto>();
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }

            return new List<TipoDocumentoDto>();
        }
    }
}