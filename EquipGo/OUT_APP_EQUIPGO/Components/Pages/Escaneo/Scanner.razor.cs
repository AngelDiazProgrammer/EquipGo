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
        private int tipoTransaccionAutomatico = 1;
        #endregion

        #region Validación de Visitante
        private bool mostrarModalValidarVisitante = false;
        private string documentoVisitante = "";
        private RegistroVisitanteDto? visitanteDto;
        private bool visitanteEncontrado = false;
        private bool consultaEnProgreso = false;


        private async Task AbrirModalValidarVisitante()
        {
            documentoVisitante = "";
            visitanteEncontrado = false;
            visitanteDto = null;
            mostrarModalValidarVisitante = true;
        }

        private async Task ConsultarVisitante()
        {
            if (string.IsNullOrWhiteSpace(documentoVisitante))
                return;

            consultaEnProgreso = true;
            visitanteEncontrado = false;
            visitanteDto = null;

            try
            {
                visitanteDto = await EquipoService.ConsultarVisitantePorDocumentoAsync(documentoVisitante);
                visitanteEncontrado = visitanteDto != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al consultar visitante: {ex.Message}");
            }
            finally
            {
                consultaEnProgreso = false;
                await InvokeAsync(StateHasChanged);
            }
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

        private bool procesandoCodigo = false;
        private readonly object lockObject = new object();
        [JSInvokable]
        public async Task ProcesarCodigo(string codigoBarras)
        {
            // 🔒 PREVENIR MÚLTIPLES EJECUCIONES SIMULTÁNEAS
            lock (lockObject)
            {
                if (procesandoCodigo)
                {
                    Console.WriteLine($"🛑 Escaneo ignorado - Ya hay un código en proceso: {codigoBarras}");
                    return;
                }
                procesandoCodigo = true;
            }

            Console.WriteLine($"⚡️ ProcesarCodigo ejecutándose con código: {codigoBarras}");

            try
            {
                equipoEscaneado = await EquipoService.ConsultarPorCodigoBarrasAsync(codigoBarras);

                if (equipoEscaneado != null)
                {
                    Console.WriteLine("✅ Equipo encontrado.");

                    // ✅ NUEVO: Determinar tipo de transacción automáticamente
                    tipoTransaccionAutomatico = await DeterminarTipoTransaccionAutomatico(equipoEscaneado.CodigoBarras);
                    tipoTransaccionSeleccionado = tipoTransaccionAutomatico; // También actualizar la variable de enlace

                    await InvokeAsync(StateHasChanged);
                }
                else
                {
                    await JS.InvokeVoidAsync("alert", "⚠️ Equipo no encontrado.");
                    await ReiniciarEscaneo();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error procesando código: {ex.Message}");
                await JS.InvokeVoidAsync("alert", $"❌ Error: {ex.Message}");
                await ReiniciarEscaneo();
            }
            finally
            {
                // 🔓 LIBERAR EL LOCK
                lock (lockObject)
                {
                    procesandoCodigo = false;
                }
            }
        }

        private async Task<int> DeterminarTipoTransaccionAutomatico(string codigoBarras)
        {
            try
            {
                // ✅ CONSULTAR LA ÚLTIMA TRANSACCIÓN REAL DESDE LA BASE DE DATOS
                var ultimaTransaccion = await TransaccionService.ObtenerUltimaTransaccionPorCodigoBarrasAsync(codigoBarras);

                if (ultimaTransaccion == null)
                {
                    Console.WriteLine("📝 No hay transacciones previas - Tipo: ENTRADA (1)");
                    return 1; // Primera transacción = ENTRADA
                }

                Console.WriteLine($"📝 Última transacción encontrada - Tipo: {ultimaTransaccion.TipoTransaccion}");

                // ✅ LÓGICA: Si la última fue SALIDA (2), ahora es ENTRADA (1)
                // Si la última fue ENTRADA (1), ahora es SALIDA (2)
                int tipoSiguiente = ultimaTransaccion.TipoTransaccion == 2 ? 1 : 2;

                Console.WriteLine($"🔄 Transacción siguiente - Tipo: {tipoSiguiente}");
                return tipoSiguiente;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error determinando tipo automático: {ex.Message}");
                return 1; // Por defecto entrada en caso de error
            }
        }

        private async Task ReiniciarEscaneo()
        {
            await Task.Delay(1000); // Pequeña pausa antes de reiniciar
            await IniciarEscaneo();
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
                TipoTransaccion = tipoTransaccionAutomatico,
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
            tipoTransaccionAutomatico = 1;
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