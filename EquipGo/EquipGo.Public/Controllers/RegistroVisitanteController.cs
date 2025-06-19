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
                Proveedores = await ObtenerProveedores()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(RegistroVisitanteViewModel model, string accion)
        {
            model.Proveedores = await ObtenerProveedores(); // Siempre aseguramos tener proveedores disponibles

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
                    if (string.IsNullOrWhiteSpace(model.Visitante.Marca) ||
                        string.IsNullOrWhiteSpace(model.Visitante.Modelo) ||
                        string.IsNullOrWhiteSpace(model.Visitante.Serial))
                    {
                        model.Mensaje = "⚠️ Completa todos los campos del equipo.";
                        break;
                    }

                    var client = _clientFactory.CreateClient();
                    var response = await client.PostAsJsonAsync("https://localhost:7096/api/visitantes", model.Visitante); // Ajusta URL según tu entorno

                    if (response.IsSuccessStatusCode)
                    {
                        model = new RegistroVisitanteViewModel
                        {
                            Proveedores = await ObtenerProveedores(),
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
                var response = await client.GetAsync("https://localhost:7096/api/proveedores"); // Ajusta la URL si usas IP, dominio o ruta distinta

                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<List<ProveedorDto>>() ?? new();
            }
            catch
            {
                // Puedes registrar el error si deseas
            }

            return new();
        }
    }
}
