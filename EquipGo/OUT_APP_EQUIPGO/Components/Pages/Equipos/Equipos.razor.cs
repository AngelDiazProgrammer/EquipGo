using Infrastructure.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Active_Directory;
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


        private List<EquipoDto> equipos;
        private List<EquipoDto> equiposFiltrados = new();

          // Filtros
        private string filtroMarca = "";
        private string filtroModelo = "";
        private string filtroSerial = "";
        private string filtroEstado = "";

        // Paginación
        private int paginaActual = 1;
        private int tamanoPagina = 5;

        //Llamado Directorio Activo
        private List<UsuarioADDto> UsuariosAD = new();
        private string UsuarioSeleccionado = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            UsuariosAD = await ActiveDirectoryService.ObtenerUsuariosAsync();
            Console.WriteLine($"Usuarios cargados desde AD: {UsuariosAD.Count}");
            foreach (var u in UsuariosAD.Take(5))
                Console.WriteLine($"🧍 {u.NombreCompleto} - {u.Usuario}");

            apiKey = Configuration["GoogleMaps:ApiKey"];
            try
            {
                equipos = (await EquipoService.ObtenerTodosLosEquiposAsync())
                            .OrderByDescending(e => e.FechaCreacion)
                            .ToList();
                equiposFiltrados = equipos.ToList();

                UsuariosAD = await ActiveDirectoryService.ObtenerUsuariosAsync();

                // 🔹 Forzar actualización de la UI después de obtener los usuarios
                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"❌ Error al cargar equipos: {ex.Message}");
                equipos = new List<EquipoDto>();
                equiposFiltrados = new List<EquipoDto>();
            }
        }



        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                
            }
        }

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

                paginaActual = 1;
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
            paginaActual = 1;
        }


        private void SiguientePagina()
        {
            if (paginaActual < TotalPaginas)
            {
                paginaActual++;
            }
        }

        private void AnteriorPagina()
        {
            if (paginaActual > 1)
            {
                paginaActual--;
            }
        }

        private IEnumerable<EquipoDto> EquiposPaginados =>
            equiposFiltrados
                .Skip((paginaActual - 1) * tamanoPagina)
                .Take(tamanoPagina);

        private int TotalPaginas =>
            (int)Math.Ceiling((double)(equiposFiltrados.Count) / tamanoPagina);

        [JSInvokable]
        public async Task<string> RefrescarListaEquipos()
        {
            try
            {
                equipos = (await EquipoService.ObtenerTodosLosEquiposAsync())
                            .OrderByDescending(e => e.FechaCreacion)
                            .ToList();
                equiposFiltrados = equipos.ToList();
                paginaActual = 1;
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
