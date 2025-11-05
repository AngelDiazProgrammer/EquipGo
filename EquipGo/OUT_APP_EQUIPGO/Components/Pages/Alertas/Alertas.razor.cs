// Pages/Alertas/Alertas.razor.cs
using Infrastructure.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Alertas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OUT_APP_EQUIPGO.Components.Pages.Alertas
{
    public partial class Alertas : ComponentBase
    {
        [Inject] private Interface.Services.Alertas.IAlertasService AlertasService { get; set; }
        [Inject] private IJSRuntime JSRuntime { get; set; }

        private List<AlertaDto> alertas = new();
        private List<AlertaDto> alertasFiltradas = new();
        private List<AlertaDto> alertasPaginadas = new();
        private List<TipoAlertaDto> tiposAlerta = new();
        private List<AlertaDto> equiposBloqueados = new();

        // Filtros
        private string filtroSerial = "";
        private string filtroMarca = "";
        private int? filtroTipoAlerta = null;
        private bool soloBloqueados = false;
        private DateTime? filtroFechaDesde = null;
        private DateTime? filtroFechaHasta = null;

        // Paginación
        private int paginaActual = 1;
        private int registrosPorPagina = 10;
        private bool modoAuto = true;

        // Loader
        private bool isCargandoAlertas = true;

        // Propiedades para las tarjetas
        private int TotalAlertas => alertasFiltradas?.Count ?? 0;
        private int AlertasHoy { get; set; }
        private int EquiposBloqueadosCount => equiposBloqueados?.Count ?? 0;
        private int AlertasCriticas => alertasFiltradas?.Count(a => a.IdTipoAlerta == 4) ?? 0; // Tipo 4 = Bloqueo

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isCargandoAlertas = true;
                StateHasChanged();

                // Cargar datos
                await CargarDatosAlertas();

                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"❌ Error al cargar alertas: {ex.Message}");
                alertas = new List<AlertaDto>();
                alertasFiltradas = new List<AlertaDto>();
                tiposAlerta = new List<TipoAlertaDto>();
            }
            finally
            {
                isCargandoAlertas = false;
                StateHasChanged();
            }
        }

        private async Task CargarDatosAlertas()
        {
            alertas = await AlertasService.ObtenerTodasAlertasAsync();
            tiposAlerta = await AlertasService.ObtenerTiposAlertaAsync();
            equiposBloqueados = await AlertasService.ObtenerEquiposBloqueadosAsync();
            AlertasHoy = await AlertasService.ObtenerTotalAlertasHoyAsync();

            AplicarFiltros();
        }

        private void AplicarFiltros()
        {
            if (alertas == null || !alertas.Any())
            {
                alertasFiltradas = new List<AlertaDto>();
                alertasPaginadas = new List<AlertaDto>();
                return;
            }

            var query = alertas.AsEnumerable();

            // Filtro por Serial
            if (!string.IsNullOrWhiteSpace(filtroSerial))
            {
                query = query.Where(a =>
                    !string.IsNullOrEmpty(a.SerialEquipo) &&
                    a.SerialEquipo.Contains(filtroSerial, StringComparison.OrdinalIgnoreCase));
            }

            // Filtro por Marca
            if (!string.IsNullOrWhiteSpace(filtroMarca))
            {
                query = query.Where(a =>
                    !string.IsNullOrEmpty(a.MarcaEquipo) &&
                    a.MarcaEquipo.Contains(filtroMarca, StringComparison.OrdinalIgnoreCase));
            }

            // Filtro por Tipo de Alerta
            if (filtroTipoAlerta.HasValue)
            {
                query = query.Where(a => a.IdTipoAlerta == filtroTipoAlerta.Value);
            }

            // Filtro por Fecha Desde
            if (filtroFechaDesde.HasValue)
            {
                query = query.Where(a => a.Fecha >= filtroFechaDesde.Value);
            }

            // Filtro por Fecha Hasta
            if (filtroFechaHasta.HasValue)
            {
                query = query.Where(a => a.Fecha <= filtroFechaHasta.Value.AddDays(1).AddSeconds(-1));
            }

            // Filtro por Solo Bloqueados
            if (soloBloqueados)
            {
                query = query.Where(a => a.EstaBloqueado);
            }

            alertasFiltradas = query
                .OrderByDescending(a => a.Fecha)
                .ToList();

            AplicarPaginacion();

            Console.WriteLine($"🔍 Filtros aplicados - Serial: '{filtroSerial}', Marca: '{filtroMarca}', Tipo: '{filtroTipoAlerta}'");
            Console.WriteLine($"📊 Resultados: {alertasFiltradas.Count} alertas");
        }

        private void AplicarPaginacion()
        {
            if (alertasFiltradas == null || !alertasFiltradas.Any())
            {
                alertasPaginadas = new List<AlertaDto>();
                return;
            }

            var inicio = (paginaActual - 1) * registrosPorPagina;
            var fin = Math.Min(inicio + registrosPorPagina, alertasFiltradas.Count);

            alertasPaginadas = alertasFiltradas
                .Skip(inicio)
                .Take(registrosPorPagina)
                .ToList();

            Console.WriteLine($"📄 Página {paginaActual} - Mostrando {alertasPaginadas.Count} de {alertasFiltradas.Count} alertas");
        }

        // Métodos para manejar cambios en los filtros
        private void OnFiltroSerialChanged(ChangeEventArgs e)
        {
            filtroSerial = e.Value?.ToString() ?? string.Empty;
            paginaActual = 1;
            AplicarFiltros();
            StateHasChanged();
        }

        private void OnFiltroMarcaChanged(ChangeEventArgs e)
        {
            filtroMarca = e.Value?.ToString() ?? string.Empty;
            paginaActual = 1;
            AplicarFiltros();
            StateHasChanged();
        }

        private void OnFiltroTipoAlertaChanged(ChangeEventArgs e)
        {
            var valor = e.Value?.ToString();
            filtroTipoAlerta = string.IsNullOrEmpty(valor) ? null : int.Parse(valor);
            paginaActual = 1;
            AplicarFiltros();
            StateHasChanged();
        }

        private void OnSoloBloqueadosChanged(ChangeEventArgs e)
        {
            soloBloqueados = (bool)(e.Value ?? false);
            paginaActual = 1;
            AplicarFiltros();
            StateHasChanged();
        }

        private void OnFiltroFechaDesdeChanged(ChangeEventArgs e)
        {
            if (DateTime.TryParse(e.Value?.ToString(), out DateTime fecha))
                filtroFechaDesde = fecha;
            else
                filtroFechaDesde = null;

            paginaActual = 1;
            AplicarFiltros();
            StateHasChanged();
        }

        private void OnFiltroFechaHastaChanged(ChangeEventArgs e)
        {
            if (DateTime.TryParse(e.Value?.ToString(), out DateTime fecha))
                filtroFechaHasta = fecha;
            else
                filtroFechaHasta = null;

            paginaActual = 1;
            AplicarFiltros();
            StateHasChanged();
        }

        // Métodos para la paginación
        private void OnPaginaCambiada(int nuevaPagina)
        {
            paginaActual = nuevaPagina;
            AplicarPaginacion();
            StateHasChanged();
        }

        private void OnRegistrosPorPaginaCambiados(int nuevosRegistros)
        {
            registrosPorPagina = nuevosRegistros;
            paginaActual = 1;
            AplicarPaginacion();
            StateHasChanged();
        }

        public void LimpiarFiltros()
        {
            filtroSerial = "";
            filtroMarca = "";
            filtroTipoAlerta = null;
            soloBloqueados = false;
            filtroFechaDesde = null;
            filtroFechaHasta = null;
            paginaActual = 1;
            AplicarFiltros();
            StateHasChanged();
        }

        //  DesbloquearEquipo
        private async Task DesbloquearEquipo(string serial, int idAlerta)
        {
            try
            {
                var resultado = await AlertasService.ReiniciarContadorAsync(serial);
                if (resultado)
                {
                    // Recargar datos para reflejar los cambios
                    await CargarDatosAlertas();
                    StateHasChanged();

                    Console.WriteLine($"✅ Equipo {serial} desbloqueado exitosamente - Alerta actualizada a tipo 5");

                    // Mostrar notificación
                    await JSRuntime.InvokeVoidAsync("mostrarToast", "success", "Equipo desbloqueado correctamente");
                }
                else
                {
                    await JSRuntime.InvokeVoidAsync("mostrarToast", "error", "Error al desbloquear equipo");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"❌ Error al desbloquear equipo: {ex.Message}");
                await JSRuntime.InvokeVoidAsync("mostrarToast", "error", "Error al desbloquear equipo");
            }
        }

        // Método para refrescar datos
        [JSInvokable]
        public async Task<string> RefrescarAlertas()
        {
            try
            {
                await CargarDatosAlertas();
                StateHasChanged();
                return "ok";
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"❌ Error al refrescar alertas: {ex.Message}");
                return $"error: {ex.Message}";
            }
        }
    }
}