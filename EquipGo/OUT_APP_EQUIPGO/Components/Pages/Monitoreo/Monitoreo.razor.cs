using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Equipo;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OUT_APP_EQUIPGO.Components.Pages.Monitoreo
{
    public partial class Monitoreo : ComponentBase, IDisposable
    {
        [Inject] private IConfiguration Configuration { get; set; }
        [Inject] private Interface.Services.Equipos.IEquipoService EquipoService { get; set; }
        [Inject] private IJSRuntime JSRuntime { get; set; }

        // Filtros
        private string filtroAssetMonitoreo = "";
        private EquipoDto equipoSeleccionado;
        private bool cargando = false;
        private CancellationTokenSource _cancellationTokenSource;

        protected override async Task OnInitializedAsync()
        {
            // Inicialización si es necesaria
        }

        private async void OnFiltroCodigoDeBarrasChanged(ChangeEventArgs e)
        {
            var nuevoFiltro = e.Value?.ToString() ?? string.Empty;

            // Cancelar búsqueda anterior si existe
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();

            filtroAssetMonitoreo = nuevoFiltro;

            // Solo buscar si el filtro tiene al menos 3 caracteres
            if (nuevoFiltro.Length >= 3)
            {
                await BuscarEquipoEnTiempoReal(nuevoFiltro, _cancellationTokenSource.Token);
            }
            else if (string.IsNullOrEmpty(nuevoFiltro))
            {
                // Limpiar resultados si el filtro está vacío
                equipoSeleccionado = null;
                cargando = false;
                StateHasChanged();
            }
        }

        private async Task BuscarEquipoEnTiempoReal(string filtro, CancellationToken cancellationToken)
        {
            try
            {
                cargando = true;
                StateHasChanged();

                // Pequeño delay para evitar muchas llamadas mientras el usuario escribe
                await Task.Delay(300, cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                    return;

                // Buscar equipos que coincidan con el filtro
                var equiposResponse = await EquipoService.ObtenerTodosLosEquiposAsync();

                if (cancellationToken.IsCancellationRequested)
                    return;

                // Buscar coincidencias en código de barras, serial o modelo
                var equipoEncontrado = equiposResponse.FirstOrDefault(e =>
                    (!string.IsNullOrEmpty(e.CodigoBarras) && e.CodigoBarras.Contains(filtro, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(e.Serial) && e.Serial.Contains(filtro, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(e.Modelo) && e.Modelo.Contains(filtro, StringComparison.OrdinalIgnoreCase)));

                if (cancellationToken.IsCancellationRequested)
                    return;

                equipoSeleccionado = equipoEncontrado;

                // Si se encontró un equipo con coordenadas, cargar el mapa
                if (equipoSeleccionado?.Latitud.HasValue == true && equipoSeleccionado.Longitud.HasValue == true)
                {
                    await CargarMapa(equipoSeleccionado.Latitud.Value, equipoSeleccionado.Longitud.Value);
                }
            }
            catch (TaskCanceledException)
            {
                // Búsqueda cancelada, no hacer nada
                Console.WriteLine("Búsqueda cancelada");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"❌ Error en búsqueda en tiempo real: {ex.Message}");
                // Podrías mostrar un mensaje de error al usuario
            }
            finally
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    cargando = false;
                    StateHasChanged();
                }
            }
        }

        private async Task CargarMapa(double latitud, double longitud)
        {
            try
            {
                var apiKey = Configuration["GoogleMaps:ApiKey"];
                if (!string.IsNullOrEmpty(apiKey))
                {
                    await JSRuntime.InvokeVoidAsync("mostrarMapaGoogle", latitud, longitud, apiKey);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"❌ Error cargando mapa: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }
}