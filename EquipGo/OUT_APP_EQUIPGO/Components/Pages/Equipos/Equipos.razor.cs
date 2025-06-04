using Microsoft.AspNetCore.Components;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Equipo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OUT_APP_EQUIPGO.Components.Pages.Equipos
{
    public partial class Equipos : ComponentBase
    {
        [Inject]
        private Interface.Services.Equipos.IEquipoService EquipoService { get; set; }

        private List<EquipoDto> equipos;
        private List<EquipoDto> equiposFiltrados = new();

        // Filtros
        private string filtroMarca = "";
        private string filtroModelo = "";
        private string filtroSerial = "";
        private string filtroEstado = "";

        protected override async Task OnInitializedAsync()
        {
            try
            {
                equipos = await EquipoService.ObtenerTodosLosEquiposAsync();
                equiposFiltrados = equipos.ToList();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"❌ Error al cargar equipos: {ex.Message}");
                equipos = new List<EquipoDto>();
                equiposFiltrados = new List<EquipoDto>();
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
                    .ToList();
            }
        }

        private void LimpiarFiltros()
        {
            filtroMarca = "";
            filtroModelo = "";
            filtroSerial = "";
            filtroEstado = "";
            equiposFiltrados = equipos?.ToList() ?? new List<EquipoDto>();
        }
    }
}
