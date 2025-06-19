using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Visitantes;
using System.Net.Http.Json;

namespace OUT_APP_EQUIPGO.Pages.Visitantes
{
    public partial class RegistroVisitante : ComponentBase
    {
        [Inject] private IHttpClientFactory ClientFactory { get; set; } = default!;
        private HttpClient Http => ClientFactory.CreateClient("API");

        [Inject] private IJSRuntime JS { get; set; } = default!;

        protected int pasoActual = 1;
        protected bool inicioFormulario = false;
        protected RegistroVisitanteDto visitante = new();
        protected string mensaje = "";

        private string tipoUsuario = "";
        protected string TipoUsuario
        {
            get => tipoUsuario;
            set
            {
                tipoUsuario = value;
                visitante.TipoUsuario = value;
                if (value != "proveedor")
                {
                    visitante.IdProveedor = null;
                }
            }
        }

        protected void IrPaso2()
        {
            if (string.IsNullOrWhiteSpace(visitante.TipoDocumento) ||
                string.IsNullOrWhiteSpace(visitante.NumeroDocumento) ||
                string.IsNullOrWhiteSpace(visitante.Nombres) ||
                string.IsNullOrWhiteSpace(visitante.Apellidos) ||
                string.IsNullOrWhiteSpace(visitante.TipoUsuario))
            {
                mensaje = "⚠️ Completa todos los campos del visitante.";
                return;
            }

            if (visitante.TipoUsuario == "proveedor" && visitante.IdProveedor == null)
            {
                mensaje = "⚠️ Debes seleccionar un proveedor.";
                return;
            }

            mensaje = "";
            pasoActual = 2;
        }

        protected void VolverPaso1()
        {
            pasoActual = 1;
        }

        protected async Task Registrar()
        {
            if (string.IsNullOrWhiteSpace(visitante.Marca) ||
                string.IsNullOrWhiteSpace(visitante.Modelo) ||
                string.IsNullOrWhiteSpace(visitante.Serial))
            {
                mensaje = "⚠️ Completa todos los campos del equipo.";
                return;
            }

            var response = await Http.PostAsJsonAsync("/api/visitantes", visitante);

            if (response.IsSuccessStatusCode)
            {
                mensaje = "✅ Registro exitoso. Dirígete al punto de control.";
                visitante = new();
                pasoActual = 1;
                tipoUsuario = "";
                inicioFormulario = false;
            }
            else
            {
                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                mensaje = error.ContainsKey("error") ? error["error"] : "❌ Error al registrar.";
            }
        }
    }
}
