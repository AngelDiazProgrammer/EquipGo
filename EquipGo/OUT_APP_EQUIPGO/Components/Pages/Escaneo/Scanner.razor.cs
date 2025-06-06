using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.JSInterop;
using OUT_APP_EQUIPGO.Modules;
using OUT_OS_APP.EQUIPGO.DTO.DTOs;
using OUT_PERSISTENCE_EQUIPGO.Services.Equipos;
using OUT_PERSISTENCE_EQUIPGO.Services.Transacciones;

namespace OUT_APP_EQUIPGO.Components.Pages.Escaneo
{
    public partial class Scanner : ComponentBase, IDisposable
    {
        #region Injections
        [Inject]
        private CookieService _CookieService { get; set; }
        [Inject]
        private NavigationManager _navigator { get; set; }

        #endregion


        private EquipoEscaneadoDto? equipoEscaneado;
        private DotNetObjectReference<Scanner>? dotNetRef;
        private int tipoTransaccionSeleccionado = 2; // Por defecto 'Salida'

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                return;             
            }

            if (await EsSesionValida())
            {
                await IniciarEscaneo();
            }
            else
            {
                _navigator.NavigateTo("/", true);
            }            
        }

        #region ValidarSesion
        private async Task<bool> CheckCookie()
        {
            await _CookieService.ValidarSesion();
            return _CookieService.CookieValida;
        }
        private async Task<bool> EsSesionValida()
        {
            return await CheckCookie();
        }

        #endregion


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
                Console.WriteLine("✅ Equipo encontrado y cargado en equipoEscaneado.");
            }
            else
            {
                Console.WriteLine("❌ No se encontró el equipo con ese código.");
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
            var idUsuarioSession = idUsuarioSessionClaim != null
                ? int.Parse(idUsuarioSessionClaim.Value)
                : 1;

            var transaccion = new TransaccionRequest
            {
                CodigoBarras = equipoEscaneado.CodigoBarras,
                TipoTransaccion = tipoTransaccionSeleccionado,
                IdEquipoPersonal = equipoEscaneado.IdEquipoPersonal,
                IdUsuarioInfo = equipoEscaneado.IdUsuarioInfo,
                IdUsuarioSession = idUsuarioSession,
                SedeOs = equipoEscaneado.IdSedeOs
            };

            var result = await TransaccionService.RegistrarTransaccionAsync(transaccion);

            if (result)
            {
                await JS.InvokeVoidAsync("alert", "Transacción registrada correctamente.");
            }
            else
            {
                await JS.InvokeVoidAsync("alert", "Error al registrar la transacción.");
            }

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

        public void Dispose()
        {
            dotNetRef?.Dispose();
        }
    }
}
