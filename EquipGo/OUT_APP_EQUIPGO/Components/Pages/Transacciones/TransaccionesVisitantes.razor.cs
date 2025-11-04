using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Transacciones;

namespace OUT_APP_EQUIPGO.Components.Pages.Transacciones
{
    public partial class TransaccionesVisitantes : ComponentBase
    {
        [Inject]
        public Interface.Services.Transacciones.ITransaccionService TransaccionesService { get; set; } = default!;

        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

        private List<TransaccionVisitanteDashboardDto> transacciones = new();
        private List<TransaccionVisitanteDashboardDto> transaccionesFiltradas = new();
        private List<TransaccionVisitanteDashboardDto> transaccionesPaginadas = new();

        // FILTROS
        private string filtroVisitante = string.Empty;
        private string filtroAprobador = string.Empty;
        private string filtroTipoTransaccion = string.Empty;
        private string filtroSede = string.Empty;
        private string filtroFecha = string.Empty;

        // Paginación
        private int paginaActual = 1;
        private int registrosPorPagina = 5;
        private bool modoAuto = true;
        private bool isCargandoTransacciones = true;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isCargandoTransacciones = true;
                transacciones = await TransaccionesService.ObtenerTodasLasTransaccionesVisitantesAsync();
                AplicarFiltros();

                Console.WriteLine($"✅ Transacciones de visitantes cargadas: {transacciones.Count}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"❌ Error al cargar transacciones de visitantes: {ex.Message}");
                transacciones = new List<TransaccionVisitanteDashboardDto>();
                transaccionesFiltradas = new List<TransaccionVisitanteDashboardDto>();
                transaccionesPaginadas = new List<TransaccionVisitanteDashboardDto>();
            }
            finally
            {
                isCargandoTransacciones = false;
            }
        }

        private void AplicarFiltros()
        {
            if (transacciones == null || !transacciones.Any())
            {
                transaccionesFiltradas = new List<TransaccionVisitanteDashboardDto>();
                transaccionesPaginadas = new List<TransaccionVisitanteDashboardDto>();
                return;
            }

            var query = transacciones.AsEnumerable();

            // Filtro por Visitante
            if (!string.IsNullOrWhiteSpace(filtroVisitante))
            {
                query = query.Where(t =>
                    !string.IsNullOrEmpty(t.NombresVisitante) &&
                    t.NombresVisitante.Contains(filtroVisitante, StringComparison.OrdinalIgnoreCase));
            }

            // Filtro por Aprobador
            if (!string.IsNullOrWhiteSpace(filtroAprobador))
            {
                query = query.Where(t =>
                    !string.IsNullOrEmpty(t.NombreAprobador) &&
                    t.NombreAprobador.Contains(filtroAprobador, StringComparison.OrdinalIgnoreCase));
            }

            // Filtro por Tipo Transacción
            if (!string.IsNullOrWhiteSpace(filtroTipoTransaccion))
            {
                query = query.Where(t =>
                    !string.IsNullOrEmpty(t.TipoTransaccion) &&
                    t.TipoTransaccion.Contains(filtroTipoTransaccion, StringComparison.OrdinalIgnoreCase));
            }

            // Filtro por Sede
            if (!string.IsNullOrWhiteSpace(filtroSede))
            {
                query = query.Where(t =>
                    !string.IsNullOrEmpty(t.NombreSede) &&
                    t.NombreSede.Contains(filtroSede, StringComparison.OrdinalIgnoreCase));
            }

            // Filtro por Fecha
            if (!string.IsNullOrWhiteSpace(filtroFecha))
            {
                if (DateTime.TryParse(filtroFecha, out DateTime fechaFiltro))
                {
                    query = query.Where(t => t.FechaTransaccion.Date == fechaFiltro.Date);
                }
            }

            transaccionesFiltradas = query.ToList();
            AplicarPaginacion();

            Console.WriteLine($"🔍 Filtros aplicados - Visitante: '{filtroVisitante}', Aprobador: '{filtroAprobador}', Tipo: '{filtroTipoTransaccion}', Sede: '{filtroSede}', Fecha: '{filtroFecha}'");
            Console.WriteLine($"📊 Resultados: {transaccionesFiltradas.Count} transacciones");
        }

        private void AplicarPaginacion()
        {
            if (transaccionesFiltradas == null || !transaccionesFiltradas.Any())
            {
                transaccionesPaginadas = new List<TransaccionVisitanteDashboardDto>();
                return;
            }

            var inicio = (paginaActual - 1) * registrosPorPagina;
            var fin = Math.Min(inicio + registrosPorPagina, transaccionesFiltradas.Count);

            transaccionesPaginadas = transaccionesFiltradas
                .Skip(inicio)
                .Take(registrosPorPagina)
                .ToList();

            Console.WriteLine($"📄 Página {paginaActual} - Mostrando {transaccionesPaginadas.Count} de {transaccionesFiltradas.Count} transacciones");
        }

        // MÉTODOS PARA LOS FILTROS
        private void OnFiltroVisitanteChanged(ChangeEventArgs e)
        {
            filtroVisitante = e.Value?.ToString() ?? string.Empty;
            paginaActual = 1;
            AplicarFiltros();
            StateHasChanged();
        }

        private void OnFiltroAprobadorChanged(ChangeEventArgs e)
        {
            filtroAprobador = e.Value?.ToString() ?? string.Empty;
            paginaActual = 1;
            AplicarFiltros();
            StateHasChanged();
        }

        private void OnFiltroTipoTransaccionChanged(ChangeEventArgs e)
        {
            filtroTipoTransaccion = e.Value?.ToString() ?? string.Empty;
            paginaActual = 1;
            AplicarFiltros();
            StateHasChanged();
        }

        private void OnFiltroSedeChanged(ChangeEventArgs e)
        {
            filtroSede = e.Value?.ToString() ?? string.Empty;
            paginaActual = 1;
            AplicarFiltros();
            StateHasChanged();
        }

        private void OnFiltroFechaChanged(ChangeEventArgs e)
        {
            filtroFecha = e.Value?.ToString() ?? string.Empty;
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

        private void OnModoAutoCambiado(bool nuevoModoAuto)
        {
            modoAuto = nuevoModoAuto;
            StateHasChanged();
        }

        public void LimpiarFiltros()
        {
            filtroVisitante = "";
            filtroAprobador = "";
            filtroTipoTransaccion = "";
            filtroSede = "";
            filtroFecha = "";
            paginaActual = 1;
            AplicarFiltros();
            StateHasChanged();
        }

        // ✅ MÉTODO: Obtener filtros actuales para JavaScript
        [JSInvokable]
        public object ObtenerFiltrosActuales()
        {
            return new
            {
                visitante = filtroVisitante,
                aprobador = filtroAprobador,
                tipoTransaccion = filtroTipoTransaccion,
                sede = filtroSede,
                fecha = filtroFecha
            };
        }

        private async Task DescargarExcelConFiltros()
        {
            try
            {
                // Pasar los filtros actuales a JavaScript
                var filtros = new
                {
                    visitante = filtroVisitante,
                    aprobador = filtroAprobador,
                    tipoTransaccion = filtroTipoTransaccion,
                    sede = filtroSede,
                    fecha = filtroFecha
                };

                await JSRuntime.InvokeVoidAsync("descargarExcelTransaccionesVisitantesConFiltros", filtros);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"❌ Error descargando Excel: {ex.Message}");
                await JSRuntime.InvokeVoidAsync("alert", $"Error al descargar Excel: {ex.Message}");
            }
        }

        // Propiedades para las cards
        private int TotalTransacciones => transacciones.Count;
        private int TransaccionesHoy => transacciones.Count(t =>
            t.FechaTransaccion.Date == DateTime.Today);
        private int TransaccionesSinAprobador => transacciones.Count(t =>
            t.NombreAprobador == "Sin aprobador");
    }
}