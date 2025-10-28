using Microsoft.AspNetCore.Components;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Usuarios;

namespace OUT_APP_EQUIPGO.Components.Pages.Usuarios
{
    public partial class Usuarios : ComponentBase
    {
        [Inject]
        public Interface.Services.Usuarios.IUsuariosInformacionService UsuariosInformacionService { get; set; } = default!;

        private List<UsuarioInformacionDto> usuariosInformacion = new();
        private List<UsuarioInformacionDto> usuariosFiltrados = new();
        private List<UsuarioInformacionDto> usuariosPaginados = new();

        // FILTROS
        private string filtroNumeroDocumento = string.Empty;
        private string filtroCampaña = string.Empty;
        private string filtroNombreCompleto = string.Empty;

        // Paginación
        private int paginaActual = 1;
        private int registrosPorPagina = 5;
        private bool modoAuto = true;
        private bool isCargandoUsuarios = true;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isCargandoUsuarios = true;
                usuariosInformacion = await UsuariosInformacionService.ObtenerTodosLosUsuariosInformacionAsync();
                AplicarFiltros();

                Console.WriteLine($"✅ Usuarios cargados: {usuariosInformacion.Count}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"❌ Error al cargar usuarios: {ex.Message}");
                usuariosInformacion = new List<UsuarioInformacionDto>();
                usuariosFiltrados = new List<UsuarioInformacionDto>();
                usuariosPaginados = new List<UsuarioInformacionDto>();
            }
            finally
            {
                isCargandoUsuarios = false;
            }
        }

        private void AplicarFiltros()
        {
            if (usuariosInformacion == null || !usuariosInformacion.Any())
            {
                usuariosFiltrados = new List<UsuarioInformacionDto>();
                usuariosPaginados = new List<UsuarioInformacionDto>();
                return;
            }

            var query = usuariosInformacion.AsEnumerable();

            // Filtro por Número de Documento - CORREGIDO para long
            if (!string.IsNullOrWhiteSpace(filtroNumeroDocumento))
            {
                query = query.Where(u =>
                    u.NumeroDocumento.ToString().Contains(filtroNumeroDocumento, StringComparison.OrdinalIgnoreCase));
            }

            // Filtro por Campaña
            if (!string.IsNullOrWhiteSpace(filtroCampaña))
            {
                query = query.Where(u =>
                    !string.IsNullOrEmpty(u.Campana) &&
                    u.Campana.Contains(filtroCampaña, StringComparison.OrdinalIgnoreCase));
            }

            // Filtro por Nombre Completo
            if (!string.IsNullOrWhiteSpace(filtroNombreCompleto))
            {
                query = query.Where(u =>
                    (!string.IsNullOrEmpty(u.Nombres) && u.Nombres.Contains(filtroNombreCompleto, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(u.Apellidos) && u.Apellidos.Contains(filtroNombreCompleto, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(u.Nombres) && !string.IsNullOrEmpty(u.Apellidos) &&
                     (u.Nombres + " " + u.Apellidos).Contains(filtroNombreCompleto, StringComparison.OrdinalIgnoreCase)));
            }

            usuariosFiltrados = query.ToList();
            AplicarPaginacion();

            Console.WriteLine($"🔍 Filtros aplicados - Documento: '{filtroNumeroDocumento}', Campaña: '{filtroCampaña}', Nombre: '{filtroNombreCompleto}'");
            Console.WriteLine($"📊 Resultados: {usuariosFiltrados.Count} usuarios");
        }

        private void AplicarPaginacion()
        {
            if (usuariosFiltrados == null || !usuariosFiltrados.Any())
            {
                usuariosPaginados = new List<UsuarioInformacionDto>();
                return;
            }

            var inicio = (paginaActual - 1) * registrosPorPagina;
            var fin = Math.Min(inicio + registrosPorPagina, usuariosFiltrados.Count);

            usuariosPaginados = usuariosFiltrados
                .Skip(inicio)
                .Take(registrosPorPagina)
                .ToList();

            Console.WriteLine($"📄 Página {paginaActual} - Mostrando {usuariosPaginados.Count} de {usuariosFiltrados.Count} usuarios");
        }

        // MÉTODOS PARA LOS FILTROS
        private void OnFiltroNumeroDocumentoChanged(ChangeEventArgs e)
        {
            filtroNumeroDocumento = e.Value?.ToString() ?? string.Empty;
            paginaActual = 1;
            AplicarFiltros();
            StateHasChanged();
        }

        private void OnFiltroCampañaChanged(ChangeEventArgs e)
        {
            filtroCampaña = e.Value?.ToString() ?? string.Empty;
            paginaActual = 1;
            AplicarFiltros();
            StateHasChanged();
        }

        private void OnFiltroNombreCompletoChanged(ChangeEventArgs e)
        {
            filtroNombreCompleto = e.Value?.ToString() ?? string.Empty;
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
            filtroNumeroDocumento = "";
            filtroCampaña = "";
            filtroNombreCompleto = "";
            paginaActual = 1;
            AplicarFiltros();
            StateHasChanged();
        }

        // Propiedades para las cards
        private int TotalUsuarios => usuariosInformacion.Count;
        private int UsuariosActivos => usuariosInformacion.Count(u =>
            !string.IsNullOrEmpty(u.Estado) && u.Estado.Trim().ToLower() == "activo");
        private int UsuariosInactivos => usuariosInformacion.Count(u =>
            !string.IsNullOrEmpty(u.Estado) && u.Estado.Trim().ToLower() == "inactivo");
    }
}