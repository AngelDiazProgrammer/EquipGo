using Infrastructure.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Active_Directory;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Active_Directory.OUT_OS_APP.EQUIPGO.DTO.DTOs.Active_Directory;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Equipo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OUT_APP_EQUIPGO.Components.Pages.Equipos
{
    public partial class Equipos : ComponentBase
    {
        [Inject] private IConfiguration Configuration { get; set; }
        [Inject] private Interface.Services.Equipos.IEquipoService EquipoService { get; set; }
        [Inject] private IJSRuntime JSRuntime { get; set; }
        [Inject] private Interface.Services.Active_Directory.IActiveDirectoryService ActiveDirectoryService { get; set; }

        private EquipoDto nuevoEquipo = new EquipoDto();
        private List<EquipoDto> equipos;
        private List<EquipoDto> equiposFiltrados = new();
        private List<EquipoDto> equiposPaginados = new();
        private EquipoDto RegistrarEquipo = new EquipoDto();
        private EquipoDto EditarEquipo = new EquipoDto();

        //Cargue masivo
        private bool mostrarResultados = false;
        private bool archivoCargado = false;
        private ResultadoCargaMasivaDto resultadoCarga = new();
        private List<CrearEquipoDto> equiposParaCargar = new();

        // Filtros
        private string filtroMarca = "";
        private string filtroModelo = "";
        private string filtroSerial = "";
        private string filtroAsset = "";
        private string filtroUsuario = "";

        // Paginación
        private int paginaActual = 1;
        private int registrosPorPagina = 5;
        private bool modoAuto = true;

        //Llamado Directorio Activo
        private List<UsuarioADDto> UsuariosAD = new();
        private string UsuarioSeleccionado = string.Empty;

        //Loader
        private bool isCargandoEquipos = true;

        // Propiedades para las tarjetas
        private int EquiposRegistradosFiltrados => equiposFiltrados?.Count ?? 0;
        private int EquiposActivosFiltrados => equiposFiltrados?.Count(e =>
            !string.IsNullOrEmpty(e.EstadoNombre) && e.EstadoNombre.Trim().ToLower() == "activo") ?? 0;
        private int EquiposInactivosFiltrados => equiposFiltrados?.Count(e =>
            !string.IsNullOrEmpty(e.EstadoNombre) && e.EstadoNombre.Trim().ToLower() == "inactivo") ?? 0;

        protected override async Task OnInitializedAsync()
        {
            apiKey = Configuration["GoogleMaps:ApiKey"];

            try
            {
                isCargandoEquipos = true;
                StateHasChanged();

                // Cargar equipos
                var equiposResponse = await EquipoService.ObtenerTodosLosEquiposAsync();
                equipos = equiposResponse.OrderByDescending(e => e.FechaCreacion).ToList();
                AplicarFiltros();

                // Cargar usuarios AD
                UsuariosAD = await ActiveDirectoryService.ObtenerUsuariosAsync();
                Console.WriteLine($"Usuarios cargados desde AD: {UsuariosAD.Count}");

                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"❌ Error al cargar equipos: {ex.Message}");
                equipos = new List<EquipoDto>();
                equiposFiltrados = new List<EquipoDto>();
                equiposPaginados = new List<EquipoDto>();
            }
            finally
            {
                isCargandoEquipos = false;
                StateHasChanged();
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("eval", @"
                if (typeof inicializarCargaMasiva === 'function') {
                    console.log('✅ Función de carga masiva disponible');
                }
            ");
            }
        }

        private void AplicarFiltros()
        {
            if (equipos == null || !equipos.Any())
            {
                equiposFiltrados = new List<EquipoDto>();
                equiposPaginados = new List<EquipoDto>();
                return;
            }

            var query = equipos.AsEnumerable();

            // Filtro por Marca
            if (!string.IsNullOrWhiteSpace(filtroMarca))
            {
                query = query.Where(e =>
                    !string.IsNullOrEmpty(e.Marca) &&
                    e.Marca.Contains(filtroMarca, StringComparison.OrdinalIgnoreCase));
            }

            // Filtro por Modelo
            if (!string.IsNullOrWhiteSpace(filtroModelo))
            {
                query = query.Where(e =>
                    !string.IsNullOrEmpty(e.Modelo) &&
                    e.Modelo.Contains(filtroModelo, StringComparison.OrdinalIgnoreCase));
            }

            // Filtro por Serial
            if (!string.IsNullOrWhiteSpace(filtroSerial))
            {
                query = query.Where(e =>
                    !string.IsNullOrEmpty(e.Serial) &&
                    e.Serial.Contains(filtroSerial, StringComparison.OrdinalIgnoreCase));
            }

            // Filtro por Estado
            if (!string.IsNullOrWhiteSpace(filtroAsset))
            {
                query = query.Where(e =>
                    !string.IsNullOrEmpty(e.CodigoBarras) &&
                    e.CodigoBarras.Contains(filtroAsset, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrWhiteSpace(filtroUsuario))
            {
                query = query.Where(e =>
                    !string.IsNullOrEmpty(e.UsuarioNombreCompleto) &&
                    e.UsuarioNombreCompleto.Contains(filtroUsuario, StringComparison.OrdinalIgnoreCase));
            }

            equiposFiltrados = query
                .OrderByDescending(e => e.FechaCreacion)
                .ToList();

            AplicarPaginacion();

            Console.WriteLine($"🔍 Filtros aplicados - Marca: '{filtroMarca}', Modelo: '{filtroModelo}', Serial: '{filtroSerial}', Estado: '{filtroAsset}'");
            Console.WriteLine($"📊 Resultados: {equiposFiltrados.Count} equipos");
        }

        private void AplicarPaginacion()
        {
            if (equiposFiltrados == null || !equiposFiltrados.Any())
            {
                equiposPaginados = new List<EquipoDto>();
                return;
            }

            var inicio = (paginaActual - 1) * registrosPorPagina;
            var fin = Math.Min(inicio + registrosPorPagina, equiposFiltrados.Count);

            equiposPaginados = equiposFiltrados
                .Skip(inicio)
                .Take(registrosPorPagina)
                .ToList();

            Console.WriteLine($"📄 Página {paginaActual} - Mostrando {equiposPaginados.Count} de {equiposFiltrados.Count} equipos");
        }

        // Métodos para manejar cambios en los filtros
        private void OnFiltroMarcaChanged(ChangeEventArgs e)
        {
            filtroMarca = e.Value?.ToString() ?? string.Empty;
            paginaActual = 1;
            AplicarFiltros();
            StateHasChanged();
        }

        private void OnFiltroModeloChanged(ChangeEventArgs e)
        {
            filtroModelo = e.Value?.ToString() ?? string.Empty;
            paginaActual = 1;
            AplicarFiltros();
            StateHasChanged();
        }

        private void OnFiltroSerialChanged(ChangeEventArgs e)
        {
            filtroSerial = e.Value?.ToString() ?? string.Empty;
            paginaActual = 1;
            AplicarFiltros();
            StateHasChanged();
        }

        private void OnFiltroCodigoDeBarrasChanged(ChangeEventArgs e)
        {
            filtroAsset = e.Value?.ToString() ?? string.Empty;
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
            filtroMarca = "";
            filtroModelo = "";
            filtroSerial = "";
            filtroAsset = "";
            paginaActual = 1;
            AplicarFiltros();
            StateHasChanged();
        }

        [JSInvokable]
        public async Task<string> RefrescarListaEquipos()
        {
            try
            {
                equipos = (await EquipoService.ObtenerTodosLosEquiposAsync())
                            .OrderByDescending(e => e.FechaCreacion)
                            .ToList();
                AplicarFiltros();
                StateHasChanged();
                return "ok";
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"❌ Error al refrescar equipos: {ex.Message}");
                return $"error: {ex.Message}";
            }
        }

        private EquipoDto equipoSeleccionado;
        private double latitud;
        private double longitud;

        [JSInvokable]
        public async Task MostrarMapalefleat()
        {
            await JSRuntime.InvokeVoidAsync("initializeLeafletMap", latitud, longitud);
        }

        private async Task MostrarDetallesEquipolefleat(int id)
        {
            equipoSeleccionado = await EquipoService.ObtenerPorIdAsync(id);
            if (equipoSeleccionado != null && equipoSeleccionado.Latitud.HasValue && equipoSeleccionado.Longitud.HasValue)
            {
                latitud = equipoSeleccionado.Latitud.Value;
                longitud = equipoSeleccionado.Longitud.Value;

                var dotNetRef = DotNetObjectReference.Create(this);
                await JSRuntime.InvokeVoidAsync("mostrarModalDetallesEquipo", dotNetRef);
            }
        }

        //Api Google
        private string apiKey;
        [JSInvokable]
        public async Task MostrarMapa()
        {
            await JSRuntime.InvokeVoidAsync("mostrarMapaGoogle", latitud, longitud, apiKey);
        }
        private async Task MostrarDetallesEquipo(int id)
        {
            equipoSeleccionado = await EquipoService.ObtenerPorIdAsync(id);
            if (equipoSeleccionado != null && equipoSeleccionado.Latitud.HasValue && equipoSeleccionado.Longitud.HasValue)
            {
                latitud = equipoSeleccionado.Latitud.Value;
                longitud = equipoSeleccionado.Longitud.Value;

                var dotNetRef = DotNetObjectReference.Create(this);
                await JSRuntime.InvokeVoidAsync("mostrarModalDetallesEquipo", dotNetRef);
            }
        }
    }
}