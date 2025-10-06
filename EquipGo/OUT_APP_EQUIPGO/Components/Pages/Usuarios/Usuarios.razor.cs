using Microsoft.AspNetCore.Components;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Equipo;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Usuarios;

namespace OUT_APP_EQUIPGO.Components.Pages.Usuarios
{
    public partial class Usuarios : ComponentBase
    {
        [Inject]
        Interface.Services.Usuarios.IUsuariosInformacionService UsuariosInformacionService { get; set; }

        private List<UsuarioInformacionDto> usuariosInformacion = new();
        private List<UsuarioInformacionDto> usuariosFiltrados = new();
       
        //filtros
        private string filtroArea = string.Empty;
        private string filtroCampaña = string.Empty;
        private string filtroEstado = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                usuariosInformacion = await UsuariosInformacionService.ObtenerTodosLosUsuariosInformacionAsync();
                usuariosFiltrados = usuariosInformacion.ToList();

                foreach (var usuario in usuariosInformacion)
                {
                    // Imprime el valor del estado tal cual viene de la base de datos
                    Console.WriteLine($"Estado: '{usuario.Estado}'");
                }

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"❌ Error al cargar usuarios: {ex.Message}");
                usuariosInformacion = new List<UsuarioInformacionDto>();
                usuariosFiltrados = new List<UsuarioInformacionDto>();
            }
        }

        private void Filtrar()
        {
            if (usuariosInformacion != null)
            {
                usuariosFiltrados = usuariosInformacion
                    .Where(u =>
                        (string.IsNullOrWhiteSpace(filtroArea) || (u.Area?.Contains(filtroArea, StringComparison.OrdinalIgnoreCase) ?? false)) &&
                        (string.IsNullOrWhiteSpace(filtroCampaña) || (u.Campana?.Contains(filtroCampaña, StringComparison.OrdinalIgnoreCase) ?? false)) &&
                        (string.IsNullOrWhiteSpace(filtroEstado) || (u.Estado?.Contains(filtroEstado, StringComparison.OrdinalIgnoreCase) ?? false))
                    )
                    .ToList();
            }
        }

        private void LimpiarFiltros()
        {
            filtroArea = "";
            filtroCampaña = "";
            filtroEstado = "";
            usuariosFiltrados = usuariosInformacion?.ToList() ?? new List<UsuarioInformacionDto>();
        }
        //LA LISTA  DE LAS CARD
        private int TotalUsuarios => usuariosInformacion.Count;
        private int UsuariosActivos => usuariosInformacion.Count(u => u.Estado?.Trim().ToLower() == "activo");
        private int UsuariosInactivos => usuariosInformacion.Count(u => u.Estado?.Trim().ToLower() == "inactivo");
    }

}

