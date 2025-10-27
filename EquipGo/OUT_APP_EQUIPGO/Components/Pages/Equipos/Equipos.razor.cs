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
using OfficeOpenXml;
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
        private string filtroEstado = "";


        //Llamado Directorio Activo
        private List<UsuarioADDto> UsuariosAD = new();
        private string UsuarioSeleccionado = string.Empty;

        //Loader
        private bool isCargandoEquipos = true;

        // Propiedades para las tarjetas (igual que en usuarios)

        private int EquiposRegistradosFiltrados => equiposFiltrados?.Count ?? 0;
        private int EquiposActivosFiltrados => equiposFiltrados?.Count(e => e.EstadoNombre?.Trim().ToLower() == "activo") ?? 0;
        private int EquiposInactivosFiltrados => equiposFiltrados?.Count(e => e.EstadoNombre?.Trim().ToLower() == "inactivo") ?? 0;

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
                equiposFiltrados = equipos.ToList();

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

        //Cargue masivo


        private void Filtrar()
        {
            if (equipos != null)
            {
                equiposFiltrados = equipos
                    .Where(e =>
                        (string.IsNullOrWhiteSpace(filtroMarca) || (e.Marca?.Contains(filtroMarca, StringComparison.OrdinalIgnoreCase) ?? false)) &&
                        (string.IsNullOrWhiteSpace(filtroModelo) || (e.Modelo?.Contains(filtroModelo, StringComparison.OrdinalIgnoreCase) ?? false)) &&
                        (string.IsNullOrWhiteSpace(filtroSerial) || (e.Serial?.Contains(filtroSerial, StringComparison.OrdinalIgnoreCase) ?? false)) &&
                        (string.IsNullOrWhiteSpace(filtroEstado) || (e.EstadoNombre?.Contains(filtroEstado, StringComparison.OrdinalIgnoreCase) ?? false))
                    )
                    .OrderByDescending(e => e.FechaCreacion)
                    .ToList();
                StateHasChanged();
            }
        }

        private void LimpiarFiltros()
        {
            filtroMarca = "";
            filtroModelo = "";
            filtroSerial = "";
            filtroEstado = "";
            equiposFiltrados = equipos
                .OrderByDescending(e => e.FechaCreacion)
                .ToList();
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
                equiposFiltrados = equipos.ToList();
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