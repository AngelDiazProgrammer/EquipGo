using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Transacciones;

namespace OUT_APP_EQUIPGO.Components.Pages.Transacciones
{
    public partial class TransaccionesCorporativos : ComponentBase
    {
        [Inject]
        public Interface.Services.Transacciones.ITransaccionService TransaccionesService { get; set; } = default!;

        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

        private List<TransaccionDashboardDto> transacciones = new();
        private List<TransaccionDashboardDto> transaccionesFiltradas = new();
        private List<TransaccionDashboardDto> transaccionesPaginadas = new();

        // FILTROS
        private string filtroCodigoBarras = string.Empty;
        private string filtroUsuario = string.Empty;
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
                transacciones = await TransaccionesService.ObtenerTodasLasTransaccionesAsync();
                AplicarFiltros();

                Console.WriteLine($"✅ Transacciones cargadas: {transacciones.Count}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"❌ Error al cargar transacciones: {ex.Message}");
                transacciones = new List<TransaccionDashboardDto>();
                transaccionesFiltradas = new List<TransaccionDashboardDto>();
                transaccionesPaginadas = new List<TransaccionDashboardDto>();
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
                transaccionesFiltradas = new List<TransaccionDashboardDto>();
                transaccionesPaginadas = new List<TransaccionDashboardDto>();
                return;
            }

            var query = transacciones.AsEnumerable();

            // Filtro por Código de Barras
            if (!string.IsNullOrWhiteSpace(filtroCodigoBarras))
            {
                query = query.Where(t =>
                    !string.IsNullOrEmpty(t.CodigoBarras) &&
                    t.CodigoBarras.Contains(filtroCodigoBarras, StringComparison.OrdinalIgnoreCase));
            }

            // Filtro por Usuario
            if (!string.IsNullOrWhiteSpace(filtroUsuario))
            {
                query = query.Where(t =>
                    !string.IsNullOrEmpty(t.NombreUsuarioInfo) &&
                    t.NombreUsuarioInfo.Contains(filtroUsuario, StringComparison.OrdinalIgnoreCase));
            }

            // Filtro por Tipo Transacción
            if (!string.IsNullOrWhiteSpace(filtroTipoTransaccion))
            {
                query = query.Where(t =>
                    !string.IsNullOrEmpty(t.NombreTipoTransaccion) &&
                    t.NombreTipoTransaccion.Contains(filtroTipoTransaccion, StringComparison.OrdinalIgnoreCase));
            }

            // Filtro por Sede
            if (!string.IsNullOrWhiteSpace(filtroSede))
            {
                query = query.Where(t =>
                    !string.IsNullOrEmpty(t.NombreSedeOs) &&
                    t.NombreSedeOs.Contains(filtroSede, StringComparison.OrdinalIgnoreCase));
            }

            // Filtro por Fecha
            if (!string.IsNullOrWhiteSpace(filtroFecha))
            {
                if (DateTime.TryParse(filtroFecha, out DateTime fechaFiltro))
                {
                    query = query.Where(t => t.FechaHora.Date == fechaFiltro.Date);
                }
            }

            transaccionesFiltradas = query.ToList();
            AplicarPaginacion();

            Console.WriteLine($"🔍 Filtros aplicados - Código: '{filtroCodigoBarras}', Usuario: '{filtroUsuario}', Tipo: '{filtroTipoTransaccion}', Sede: '{filtroSede}', Fecha: '{filtroFecha}'");
            Console.WriteLine($"📊 Resultados: {transaccionesFiltradas.Count} transacciones");
        }

        private void AplicarPaginacion()
        {
            if (transaccionesFiltradas == null || !transaccionesFiltradas.Any())
            {
                transaccionesPaginadas = new List<TransaccionDashboardDto>();
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
        private void OnFiltroCodigoBarrasChanged(ChangeEventArgs e)
        {
            filtroCodigoBarras = e.Value?.ToString() ?? string.Empty;
            paginaActual = 1;
            AplicarFiltros();
            StateHasChanged();
        }

        private void OnFiltroUsuarioChanged(ChangeEventArgs e)
        {
            filtroUsuario = e.Value?.ToString() ?? string.Empty;
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
            filtroCodigoBarras = "";
            filtroUsuario = "";
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
                codigoBarras = filtroCodigoBarras,
                usuario = filtroUsuario,
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
                    codigoBarras = filtroCodigoBarras,
                    usuario = filtroUsuario,
                    tipoTransaccion = filtroTipoTransaccion,
                    sede = filtroSede,
                    fecha = filtroFecha
                };

                await JSRuntime.InvokeVoidAsync("descargarExcelTransaccionesConFiltros", filtros);
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
            t.FechaHora.Date == DateTime.Today);
        private int TransaccionesUltimaSemana => transacciones.Count(t =>
            t.FechaHora.Date >= DateTime.Today.AddDays(-7));
    }
}