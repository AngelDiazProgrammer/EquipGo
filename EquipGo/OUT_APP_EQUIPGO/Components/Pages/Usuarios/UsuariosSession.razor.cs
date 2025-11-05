using Interface.Services.Usuarios;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OUT_DOMAIN_EQUIPGO.Entities.Seguridad;
using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Usuarios;
using System.ComponentModel.DataAnnotations;

namespace OUT_APP_EQUIPGO.Components.Pages.Usuarios
{
    public partial class UsuariosSession
    {
        // Servicios
        [Inject] private IUsuariosSessionService UsuariosSessionService { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;

        // Listas de datos
        private List<UsuarioSessionDto> usuarios = new();
        private List<UsuarioSessionDto> usuariosFiltrados = new();
        private List<UsuarioSessionDto> usuariosPaginados = new();
        private List<TipoDocumento> tiposDocumento = new();
        private List<Estado> estados = new();
        private List<Rol> roles = new();
        private List<OUT_DOMAIN_EQUIPGO.Entities.Smart.Sedes> sedes = new();

        // Filtros
        private string filtroNumeroDocumento = string.Empty;
        private string filtroNombreCompleto = string.Empty;
        private string filtroRol = string.Empty;

        // Paginación
        private int paginaActual = 1;
        private int registrosPorPagina = 10;
        private bool modoAuto = true;
        private bool isCargandoUsuarios = true;
        private bool isGuardando = false;
        private bool isCambiandoContraseña = false;

        // Formularios
        private UsuarioSessionCrearDto usuarioForm = new();
        private UsuarioSessionDto? usuarioEditando = null;
        private CambioContraseñaDto cambioContraseñaForm = new();
        private string confirmarContraseña = string.Empty;
        private int? usuarioIdParaCambioContraseña = null;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isCargandoUsuarios = true;
                await CargarDatosIniciales();
                AplicarFiltros();

                Console.WriteLine($"✅ Usuarios Session cargados: {usuarios.Count}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"❌ Error al cargar usuarios session: {ex.Message}");
                await MostrarError("Error al cargar los usuarios");
            }
            finally
            {
                isCargandoUsuarios = false;
                StateHasChanged();
            }
        }

        private async Task CargarDatosIniciales()
        {
            try
            {
                // Cargar usuarios
                var usuariosResponse = await UsuariosSessionService.GetAllAsync();
                usuarios = usuariosResponse.Select(u => new UsuarioSessionDto
                {
                    Id = u.Id,
                    TipoDocumento = u.TipoDocumento?.NombreDocumento ?? "",
                    NumeroDocumento = u.NumeroDocumento,
                    Nombre = u.Nombre,
                    Apellido = u.Apellido,
                    Rol = u.Rol?.NombreRol ?? "",
                    Sede = u.Sede?.NombreSede ?? "",
                    Estado = u.Estado?.NombreEstado ?? "",
                    FechaCreacion = u.FechaCreacion,
                    UltimaModificacion = u.UltimaModificacion
                }).ToList();

                // Cargar datos para combos
                tiposDocumento = await UsuariosSessionService.GetTiposDocumentoAsync();
                estados = await UsuariosSessionService.GetEstadosAsync();
                roles = await UsuariosSessionService.GetRolesAsync();
                sedes = await UsuariosSessionService.GetSedesAsync();

                Console.WriteLine($"📊 Datos cargados: {usuarios.Count} usuarios, {tiposDocumento.Count} tipos doc, {roles.Count} roles, {sedes.Count} sedes");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"❌ Error cargando datos iniciales: {ex.Message}");
                throw;
            }
        }

        private void AplicarFiltros()
        {
            if (usuarios == null || !usuarios.Any())
            {
                usuariosFiltrados = new List<UsuarioSessionDto>();
                usuariosPaginados = new List<UsuarioSessionDto>();
                return;
            }

            var query = usuarios.AsEnumerable();

            // Filtro por Número de Documento
            if (!string.IsNullOrWhiteSpace(filtroNumeroDocumento))
            {
                query = query.Where(u =>
                    u.NumeroDocumento.Contains(filtroNumeroDocumento, StringComparison.OrdinalIgnoreCase));
            }

            // Filtro por Nombre Completo
            if (!string.IsNullOrWhiteSpace(filtroNombreCompleto))
            {
                query = query.Where(u =>
                    u.Nombre.Contains(filtroNombreCompleto, StringComparison.OrdinalIgnoreCase) ||
                    u.Apellido.Contains(filtroNombreCompleto, StringComparison.OrdinalIgnoreCase) ||
                    u.NombreCompleto.Contains(filtroNombreCompleto, StringComparison.OrdinalIgnoreCase));
            }

            // Filtro por Rol
            if (!string.IsNullOrWhiteSpace(filtroRol))
            {
                query = query.Where(u =>
                    u.Rol.Contains(filtroRol, StringComparison.OrdinalIgnoreCase));
            }

            usuariosFiltrados = query.ToList();
            AplicarPaginacion();

            Console.WriteLine($"🔍 Filtros aplicados - Documento: '{filtroNumeroDocumento}', Nombre: '{filtroNombreCompleto}', Rol: '{filtroRol}'");
            Console.WriteLine($"📊 Resultados: {usuariosFiltrados.Count} usuarios");
        }

        private void AplicarPaginacion()
        {
            if (usuariosFiltrados == null || !usuariosFiltrados.Any())
            {
                usuariosPaginados = new List<UsuarioSessionDto>();
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

        private void OnFiltroNombreCompletoChanged(ChangeEventArgs e)
        {
            filtroNombreCompleto = e.Value?.ToString() ?? string.Empty;
            paginaActual = 1;
            AplicarFiltros();
            StateHasChanged();
        }

        private void OnFiltroRolChanged(ChangeEventArgs e)
        {
            filtroRol = e.Value?.ToString() ?? string.Empty;
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

        // MÉTODOS CRUD
        private async void AbrirModalCrearUsuario()
        {
            try
            {
                usuarioEditando = null;
                usuarioForm = new UsuarioSessionCrearDto();
                confirmarContraseña = string.Empty;

                await JSRuntime.InvokeVoidAsync("eval", "new bootstrap.Modal(document.getElementById('modalCrearUsuarioSession')).show()");
                StateHasChanged();
            }
            catch (Exception ex)
            {
                await MostrarError($"Error al abrir modal: {ex.Message}");
            }
        }

        private async Task EditarUsuario(int id)
        {
            try
            {
                var usuario = await UsuariosSessionService.GetByIdAsync(id);
                if (usuario != null)
                {
                    usuarioEditando = new UsuarioSessionDto
                    {
                        Id = usuario.Id,
                        TipoDocumento = usuario.TipoDocumento?.NombreDocumento,
                        NumeroDocumento = usuario.NumeroDocumento,
                        Nombre = usuario.Nombre,
                        Apellido = usuario.Apellido,
                        Rol = usuario.Rol?.NombreRol,
                        Sede = usuario.Sede?.NombreSede,
                        Estado = usuario.Estado?.NombreEstado
                    };

                    usuarioForm = new UsuarioSessionCrearDto
                    {
                        IdTipodocumento = usuario.IdTipodocumento,
                        NumeroDocumento = usuario.NumeroDocumento,
                        Nombre = usuario.Nombre,
                        Apellido = usuario.Apellido,
                        IdEstado = usuario.IdEstado,
                        IdRol = usuario.IdRol,
                        IdSede = usuario.IdSede
                    };

                    await JSRuntime.InvokeVoidAsync("eval", "new bootstrap.Modal(document.getElementById('modalCrearUsuarioSession')).show()");
                    StateHasChanged();
                }
                else
                {
                    await MostrarError("Usuario no encontrado");
                }
            }
            catch (Exception ex)
            {
                await MostrarError($"Error al cargar usuario: {ex.Message}");
            }
        }

        private async Task GuardarUsuario()
        {
            try
            {
                isGuardando = true;
                StateHasChanged();

                // Validar contraseña si es creación
                if (usuarioEditando == null)
                {
                    if (string.IsNullOrEmpty(usuarioForm.Contraseña))
                    {
                        await MostrarError("La contraseña es obligatoria");
                        isGuardando = false;
                        StateHasChanged();
                        return;
                    }

                    if (usuarioForm.Contraseña != confirmarContraseña)
                    {
                        await MostrarError("Las contraseñas no coinciden");
                        isGuardando = false;
                        StateHasChanged();
                        return;
                    }
                }

                if (usuarioEditando == null)
                {
                    // Crear nuevo usuario
                    var usuario = new OUT_DOMAIN_EQUIPGO.Entities.Seguridad.UsuariosSession
                    {
                        IdTipodocumento = usuarioForm.IdTipodocumento,
                        NumeroDocumento = usuarioForm.NumeroDocumento?.Trim(),
                        Nombre = usuarioForm.Nombre?.Trim(),
                        Apellido = usuarioForm.Apellido?.Trim(),
                        Contraseña = usuarioForm.Contraseña, // En producción, encriptar
                        IdEstado = usuarioForm.IdEstado,
                        IdRol = usuarioForm.IdRol,
                        IdSede = usuarioForm.IdSede,
                        FechaCreacion = DateTime.UtcNow,
                        UltimaModificacion = DateTime.UtcNow
                    };

                    await UsuariosSessionService.CreateAsync(usuario);
                    await MostrarExito("Usuario creado correctamente");
                }
                else
                {
                    // Actualizar usuario existente
                    var usuario = await UsuariosSessionService.GetByIdAsync(usuarioEditando.Id);
                    if (usuario != null)
                    {
                        usuario.IdTipodocumento = usuarioForm.IdTipodocumento;
                        usuario.NumeroDocumento = usuarioForm.NumeroDocumento?.Trim();
                        usuario.Nombre = usuarioForm.Nombre?.Trim();
                        usuario.Apellido = usuarioForm.Apellido?.Trim();
                        usuario.IdEstado = usuarioForm.IdEstado;
                        usuario.IdRol = usuarioForm.IdRol;
                        usuario.IdSede = usuarioForm.IdSede;
                        usuario.UltimaModificacion = DateTime.UtcNow;

                        await UsuariosSessionService.UpdateAsync(usuario);
                        await MostrarExito("Usuario actualizado correctamente");
                    }
                }

                // Cerrar modal y recargar datos
                await JSRuntime.InvokeVoidAsync("cerrarModalUsuarioSession");
                await CargarDatosIniciales();
                AplicarFiltros();
            }
            catch (Exception ex)
            {
                await MostrarError($"Error al guardar usuario: {ex.Message}");
            }
            finally
            {
                isGuardando = false;
                StateHasChanged();
            }
        }

        private async Task EliminarUsuario(int id)
        {
            try
            {
                var usuario = usuarios.FirstOrDefault(u => u.Id == id);
                if (usuario != null)
                {
                    var confirmar = await JSRuntime.InvokeAsync<bool>("mostrarConfirmacion", $"¿Está seguro de eliminar al usuario {usuario.NombreCompleto}?");
                    if (confirmar)
                    {
                        await UsuariosSessionService.DeleteAsync(id);
                        await MostrarExito("Usuario eliminado correctamente");
                        await CargarDatosIniciales();
                        AplicarFiltros();
                    }
                }
            }
            catch (Exception ex)
            {
                await MostrarError($"Error al eliminar usuario: {ex.Message}");
            }
        }

        private async Task CambiarContraseña(int id)
        {
            try
            {
                usuarioIdParaCambioContraseña = id;
                cambioContraseñaForm = new CambioContraseñaDto { UsuarioId = id };
                await JSRuntime.InvokeVoidAsync("eval", "new bootstrap.Modal(document.getElementById('modalCambiarContraseña')).show()");
                StateHasChanged();
            }
            catch (Exception ex)
            {
                await MostrarError($"Error al abrir modal de cambio de contraseña: {ex.Message}");
            }
        }

        private async Task ProcesarCambioContraseña()
        {
            try
            {
                isCambiandoContraseña = true;
                StateHasChanged();

                if (string.IsNullOrEmpty(cambioContraseñaForm.NuevaContraseña))
                {
                    await MostrarError("La nueva contraseña es obligatoria");
                    isCambiandoContraseña = false;
                    StateHasChanged();
                    return;
                }

                if (cambioContraseñaForm.NuevaContraseña != cambioContraseñaForm.ConfirmarContraseña)
                {
                    await MostrarError("Las contraseñas no coinciden");
                    isCambiandoContraseña = false;
                    StateHasChanged();
                    return;
                }

                await UsuariosSessionService.ChangePasswordAsync(
                    cambioContraseñaForm.UsuarioId,
                    cambioContraseñaForm.NuevaContraseña
                );

                await MostrarExito("Contraseña cambiada correctamente");
                await JSRuntime.InvokeVoidAsync("cerrarModalCambioContraseña");
                cambioContraseñaForm = new CambioContraseñaDto();
            }
            catch (Exception ex)
            {
                await MostrarError($"Error al cambiar contraseña: {ex.Message}");
            }
            finally
            {
                isCambiandoContraseña = false;
                StateHasChanged();
            }
        }

        public void LimpiarFiltros()
        {
            filtroNumeroDocumento = "";
            filtroNombreCompleto = "";
            filtroRol = "";
            paginaActual = 1;
            AplicarFiltros();
            StateHasChanged();
        }

        // Propiedades para las cards
        private int TotalUsuarios => usuarios.Count;
        private int UsuariosActivos => usuarios.Count(u => u.Estado?.ToLower() == "activo");
        private int UsuariosInactivos => usuarios.Count(u => u.Estado?.ToLower() == "inactivo");

        // Métodos de utilidad para notificaciones
        private async Task MostrarExito(string mensaje)
        {
            await JSRuntime.InvokeVoidAsync("mostrarExito", mensaje);
        }

        private async Task MostrarError(string mensaje)
        {
            await JSRuntime.InvokeVoidAsync("mostrarError", mensaje);
        }
    }
}