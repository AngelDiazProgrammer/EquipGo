using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.SubEstado;
using Interface.Services.SubEstado;

namespace OUT_APP_EQUIPGO.Components.Pages.SubEstados
{
    public partial class SubEstados : ComponentBase
    {
        [Inject]
        ISubEstadoService SubEstadoService { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        private List<SubEstadoDto> subEstados = new();
        private List<SubEstadoDto> subEstadosFiltrados = new();
        private List<SubEstadoDto> subEstadosPaginados = new();

        // FILTROS
        private string filtroNombre = string.Empty;

        // Paginación
        private int paginaActual = 1;
        private int registrosPorPagina = 5;
        private bool modoAuto = true;
        private bool isCargandoSubEstados = true;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isCargandoSubEstados = true;
                subEstados = await SubEstadoService.ObtenerTodosAsync();
                AplicarFiltros();

                Console.WriteLine($"✅ Subestados cargados: {subEstados.Count}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"❌ Error al cargar subestados: {ex.Message}");
                subEstados = new List<SubEstadoDto>();
                subEstadosFiltrados = new List<SubEstadoDto>();
                subEstadosPaginados = new List<SubEstadoDto>();
            }
            finally
            {
                isCargandoSubEstados = false;
            }
        }

        private void AplicarFiltros()
        {
            if (subEstados == null || !subEstados.Any())
            {
                subEstadosFiltrados = new List<SubEstadoDto>();
                subEstadosPaginados = new List<SubEstadoDto>();
                return;
            }

            var query = subEstados.AsEnumerable();

            // Filtro por Nombre
            if (!string.IsNullOrWhiteSpace(filtroNombre))
            {
                query = query.Where(s =>
                    !string.IsNullOrEmpty(s.NombreSubEstado) &&
                    s.NombreSubEstado.Contains(filtroNombre, StringComparison.OrdinalIgnoreCase));
            }

            subEstadosFiltrados = query.ToList();
            AplicarPaginacion();

            Console.WriteLine($"🔍 Filtros aplicados - Nombre: '{filtroNombre}'");
            Console.WriteLine($"📊 Resultados: {subEstadosFiltrados.Count} subestados");
        }

        private void AplicarPaginacion()
        {
            if (subEstadosFiltrados == null || !subEstadosFiltrados.Any())
            {
                subEstadosPaginados = new List<SubEstadoDto>();
                return;
            }

            var inicio = (paginaActual - 1) * registrosPorPagina;
            var fin = Math.Min(inicio + registrosPorPagina, subEstadosFiltrados.Count);

            subEstadosPaginados = subEstadosFiltrados
                .Skip(inicio)
                .Take(registrosPorPagina)
                .ToList();

            Console.WriteLine($"📄 Página {paginaActual} - Mostrando {subEstadosPaginados.Count} de {subEstadosFiltrados.Count} subestados");
        }

        // MÉTODOS PARA LOS FILTROS
        private void OnFiltroNombreChanged(ChangeEventArgs e)
        {
            filtroNombre = e.Value?.ToString() ?? string.Empty;
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
            filtroNombre = "";
            paginaActual = 1;
            AplicarFiltros();
            StateHasChanged();
        }

        // Propiedades para las cards
        private int TotalSubEstados => subEstados.Count;

        [JSInvokable]
        public async Task<string> RefrescarListaSubEstados()
        {
            try
            {
                subEstados = (await SubEstadoService.ObtenerTodosAsync())
                             .OrderByDescending(s => s.Id)
                             .ToList();
                AplicarFiltros();
                StateHasChanged();
                return "ok";
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"❌ Error al refrescar subestados: {ex.Message}");
                return $"error: {ex.Message}";
            }
        }
    }
}