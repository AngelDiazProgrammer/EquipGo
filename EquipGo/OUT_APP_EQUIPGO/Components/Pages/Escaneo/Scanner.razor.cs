﻿using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.JSInterop;
using OUT_OS_APP.EQUIPGO.DTO.DTOs;
using OUT_PERSISTENCE_EQUIPGO.Services.Equipos;
using OUT_PERSISTENCE_EQUIPGO.Services.Transacciones;

namespace OUT_APP_EQUIPGO.Components.Pages.Escaneo
{
    public partial class Scanner : ComponentBase, IDisposable
    {

        private EquipoEscaneadoDto? equipoEscaneado;
        private DotNetObjectReference<Scanner>? dotNetRef;
        private int tipoTransaccionSeleccionado = 2; // Por defecto 'Salida'

        [Inject] public IJSRuntime JS { get; set; }
        [Inject] public NavigationManager Navigation { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                dotNetRef = DotNetObjectReference.Create(this);
                await Task.Delay(100); // Da tiempo al DOM de renderizar
                await IniciarEscaneo();
            }
        }

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
    }
}
