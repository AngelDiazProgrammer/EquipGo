#region Usings
using Application.Services.Visitantes;
using Interface.Services.Equipos;
using Interface.Services.Transacciones;
using Interface.Services.Visitantes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using OUT_DOMAIN_EQUIPGO.Entities.Procesos;
using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using OUT_OS_APP.EQUIPGO.DTO.DTOs;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Equipo;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Transacciones;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Visitantes;
using System.Security.Claims;
#endregion

namespace OUT_APP_EQUIPGO.Components.Pages.Escaneo
{
    public partial class Scanner : ComponentBase, IDisposable
    {
        #region Inyección de Dependencias
        [Inject] public IJSRuntime JS { get; set; }
        [Inject] public NavigationManager Navigation { get; set; }
        [Inject] public IEquipoService EquipoService { get; set; }
        [Inject] public ITransaccionService TransaccionService { get; set; }
        [Inject] public AuthenticationStateProvider AuthProvider { get; set; }
        private readonly IVisitanteService _visitanteService;

        #endregion

        #region Variables Generales
        private DotNetObjectReference<Scanner>? dotNetRef;
        private EquipoEscaneadoDto? equipoEscaneado;
        private int tipoTransaccionSeleccionado = 2; // Por defecto 'Salida'
        #endregion

        #region Validación de Visitante
        private bool mostrarModalValidarVisitante = false;
        private string documentoVisitante = "";
        private RegistroVisitanteDto? visitanteDto;
        private bool visitanteEncontrado = false;

        private async Task AbrirModalValidarVisitante()
        {
            documentoVisitante = "";
            visitanteEncontrado = false;
            visitanteDto = null;
            mostrarModalValidarVisitante = true;
        }

        private async Task ConsultarVisitante()
        {
            if (string.IsNullOrWhiteSpace(documentoVisitante)) return;

            try
            {
                visitanteDto = await EquipoService.ConsultarVisitantePorDocumentoAsync(documentoVisitante);
                visitanteEncontrado = visitanteDto != null;
            }
            catch (Exception ex)
            {
                visitanteEncontrado = false;
                Console.WriteLine($"❌ Error al consultar visitante: {ex.Message}");
            }
            await InvokeAsync(StateHasChanged);
        }

        private async Task RegistrarTransaccionVisitante()
        {
            if (visitanteDto == null) return;

            var authState = await AuthProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            var idUsuarioSessionClaim = user.Claims.FirstOrDefault(c => c.Type == "id_usuarioSession");

            if (idUsuarioSessionClaim == null)
            {
                await JS.InvokeVoidAsync("alert", "⚠️ Usuario de sesión no identificado.");
                return;
            }

            var transaccion = new TransaccionVisitanteRequest
            {
                NumeroDocumento = visitanteDto.NumeroDocumento,
                TipoTransaccion = tipoTransaccionSeleccionado,
                IdUsuarioSession = int.Parse(idUsuarioSessionClaim.Value),
                FechaTransaccion = DateTime.Now
            };

            var exito = await TransaccionService.RegistrarTransaccionVisitanteAsync(transaccion);

            if (exito)
            {
                await JS.InvokeVoidAsync("alert", "✅ Transacción de visitante registrada.");
            }
            else
            {
                await JS.InvokeVoidAsync("alert", "❌ No se pudo registrar la transacción.");
            }

            await CerrarModalValidarVisitante();
            await JS.InvokeVoidAsync("startScanner", dotNetRef);
        }

        private async Task CerrarModalValidarVisitante()
        {
            mostrarModalValidarVisitante = false;
            visitanteDto = null;
            documentoVisitante = "";
            visitanteEncontrado = false;
            await InvokeAsync(StateHasChanged);
        }
        #endregion

        #region OnAfterRenderAsync
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                dotNetRef = DotNetObjectReference.Create(this);
                await Task.Delay(100);
                await IniciarEscaneo();
            }
        }
        #endregion

        #region Escáner
        private async Task IniciarEscaneo()
        {
            if (dotNetRef != null)
            {
                await JS.InvokeVoidAsync("startScanner", dotNetRef);
            }
        }

        [JSInvokable]
        public async Task ProcesarCodigo(string codigoBarras)
        {
            Console.WriteLine($"⚡️ ProcesarCodigo ejecutándose con código: {codigoBarras}");
            equipoEscaneado = await EquipoService.ConsultarPorCodigoBarrasAsync(codigoBarras);

            if (equipoEscaneado != null)
            {
                Console.WriteLine("✅ Equipo encontrado.");
            }
            else
            {
                await JS.InvokeVoidAsync("alert", "⚠️ Equipo no encontrado.");
                await IniciarEscaneo();
                return;
            }

            await InvokeAsync(StateHasChanged);
        }

        private async Task AprobarTransaccion()
        {
            if (equipoEscaneado == null) return;

            var authState = await AuthProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            var idUsuarioSessionClaim = user.Claims.FirstOrDefault(c => c.Type == "id_usuarioSession");
            if (idUsuarioSessionClaim == null)
            {
                await JS.InvokeVoidAsync("alert", "⚠️ Usuario de sesión no identificado.");
                return;
            }

            var transaccion = new TransaccionRequest
            {
                CodigoBarras = equipoEscaneado.CodigoBarras,
                TipoTransaccion = tipoTransaccionSeleccionado,
                IdEquipoPersonal = equipoEscaneado.IdEquipoPersonal,
                IdUsuarioInfo = equipoEscaneado.IdUsuarioInfo ?? 0,
                IdUsuarioSession = int.Parse(idUsuarioSessionClaim.Value),
                SedeOs = equipoEscaneado.IdSedeOs
            };

            var result = await TransaccionService.RegistrarTransaccionAsync(transaccion);
            await JS.InvokeVoidAsync("alert", result
                ? "✅ Transacción registrada."
                : "❌ Error al registrar la transacción.");

            equipoEscaneado = null;
            await InvokeAsync(StateHasChanged);
            await IniciarEscaneo();
        }

        private async Task CerrarModal()
        {
            equipoEscaneado = null;
            await InvokeAsync(StateHasChanged);
            await IniciarEscaneo();
        }
        #endregion

        #region Cerrar sesión y Dispose
        private async Task CerrarSesion()
        {
            try
            {
                await JS.InvokeVoidAsync("authInterop.logout");
                Navigation.NavigateTo("/login");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cerrar sesión: {ex.Message}");
            }
        }

        public void Dispose()
        {
            dotNetRef?.Dispose();
        }
        #endregion
    }
}