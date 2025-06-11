#region Usings
using Interface.Services.Equipos;
using Interface.Services.Transacciones;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using OUT_DOMAIN_EQUIPGO.Entities.Procesos;
using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using OUT_OS_APP.EQUIPGO.DTO.DTOs;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Equipo;
using System.Security.Claims;
#endregion

namespace OUT_APP_EQUIPGO.Components.Pages.Escaneo
{
    public partial class Scanner : ComponentBase, IDisposable
    {
        #region Inyección de Dependencias
        [Inject] public IJSRuntime JS { get; set; }
        [Inject] public NavigationManager Navigation { get; set; }
        [Inject] public IEquipoService EquipoService { get; set; }
        [Inject] public ITransaccionService TransaccionService { get; set; }
        [Inject] public AuthenticationStateProvider AuthProvider { get; set; }
        #endregion

        #region Variables Generales
        private DotNetObjectReference<Scanner>? dotNetRef;
        private EquipoEscaneadoDto? equipoEscaneado;
        private int tipoTransaccionSeleccionado = 2; // Por defecto 'Salida'
        #endregion

        #region OnAfterRenderAsync
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                dotNetRef = DotNetObjectReference.Create(this);
                await Task.Delay(100);
                await IniciarEscaneo();
            }
        }
        #endregion

        #region Escáner
        private async Task IniciarEscaneo()
        {
            if (dotNetRef != null)
            {
                await JS.InvokeVoidAsync("startScanner", dotNetRef);
            }
        }

        [JSInvokable]
        public async Task ProcesarCodigo(string codigoBarras)
        {
            Console.WriteLine($"⚡️ ProcesarCodigo ejecutándose con código: {codigoBarras}");
            equipoEscaneado = await EquipoService.ConsultarPorCodigoBarrasAsync(codigoBarras);

            if (equipoEscaneado != null)
            {
                Console.WriteLine("✅ Equipo encontrado.");
            }
            else
            {
                await JS.InvokeVoidAsync("alert", "⚠️ Equipo no encontrado.");
                await IniciarEscaneo();
                return;
            }

            await InvokeAsync(StateHasChanged);
        }

        private async Task AprobarTransaccion()
        {
            if (equipoEscaneado == null) return;

            var authState = await AuthProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            var idUsuarioSessionClaim = user.Claims.FirstOrDefault(c => c.Type == "id_usuarioSession");
            if (idUsuarioSessionClaim == null)
            {
                await JS.InvokeVoidAsync("alert", "⚠️ Usuario de sesión no identificado.");
                return;
            }

            var transaccion = new TransaccionRequest
            {
                CodigoBarras = equipoEscaneado.CodigoBarras,
                TipoTransaccion = tipoTransaccionSeleccionado,
                IdEquipoPersonal = equipoEscaneado.IdEquipoPersonal,
                IdUsuarioInfo = equipoEscaneado.IdUsuarioInfo,
                IdUsuarioSession = int.Parse(idUsuarioSessionClaim.Value),
                SedeOs = equipoEscaneado.IdSedeOs
            };

            var result = await TransaccionService.RegistrarTransaccionAsync(transaccion);
            await JS.InvokeVoidAsync("alert", result
                ? "✅ Transacción registrada."
                : "❌ Error al registrar la transacción.");

            equipoEscaneado = null;
            await InvokeAsync(StateHasChanged);
            await IniciarEscaneo();
        }

        private async Task CerrarModal()
        {
            equipoEscaneado = null;
            await InvokeAsync(StateHasChanged);
            await IniciarEscaneo();
        }
        #endregion

        #region Modal Registro de Equipo

        // Control modal
        private bool mostrarModalRegistroEquipo = false;

        // Datos usuario
        private string documentoUsuario = "";
        private string usuarioNombres = "";
        private string usuarioApellidos = "";
        private int idTipodocumentoSeleccionado;
        private int idAreaSeleccionada;
        private int idCampañaSeleccionada;

        // Datos equipo
        private string equipoMarca = "";
        private string equipoModelo = "";
        private string equipoSerial = "";
        private string equipoUbicacion = "";

        // Listas dinámicas
        private List<TipoDocumento> listaTiposDocumento = new();
        private List<Area> listaAreas = new();
        private List<Campaña> listaCampañas = new();
        private List<EquiposPersonal> listaEquiposPersonales = new();

        // Equipo personal
        private int idEquipoPersonalSeleccionado;

        private async Task AbrirModalRegistroEquipo()
        {
            mostrarModalRegistroEquipo = true;
            await CargarListasSelect();
        }

        private async Task CargarListasSelect()
        {
            try
            {
                // Cargar listas con await por orden de prioridad (puedes hacerlas paralelas si quieres)
                listaTiposDocumento = await EquipoService.ObtenerTipoDocumentoAsync() ?? new List<TipoDocumento>();
                listaAreas = await EquipoService.ObtenerAreasAsync() ?? new List<Area>();
                listaCampañas = await EquipoService.ObtenerCampañasAsync() ?? new List<Campaña>();
                listaEquiposPersonales = await EquipoService.ObtenerEquiposPersonalesAsync() ?? new List<EquiposPersonal>();
            }
            catch (Exception ex)
            {
                // Si ocurre un error al cargar alguna lista
                await JS.InvokeVoidAsync("alert", $"❌ Error al cargar las listas de selección: {ex.Message}");
                Console.WriteLine($"Error en CargarListasSelect(): {ex}");
            }
        }

        private async Task BuscarUsuarioPorDocumento()
        {
            if (!string.IsNullOrWhiteSpace(documentoUsuario))
            {
                var usuario = await EquipoService.ConsultarUsuarioPorDocumentoAsync(documentoUsuario);
                if (usuario != null)
                {
                    usuarioNombres = usuario.Nombres;
                    usuarioApellidos = usuario.Apellidos;
                    idTipodocumentoSeleccionado = usuario.IdTipodocumento;
                    idAreaSeleccionada = usuario.IdArea;
                    idCampañaSeleccionada = usuario.IdCampaña;
                }
                else
                {
                    usuarioNombres = "";
                    usuarioApellidos = "";
                    idTipodocumentoSeleccionado = 0;
                    idAreaSeleccionada = 0;
                    idCampañaSeleccionada = 0;
                }
                await InvokeAsync(StateHasChanged);
            }
        }

        private async Task AprobarRegistroEquipo()
        {
            // 🛑 Validar selección de tipo de documento
            if (idTipodocumentoSeleccionado == 0)
            {
                await JS.InvokeVoidAsync("alert", "⚠️ Debes seleccionar un tipo de documento válido.");
                return;
            }
            // Verificar o crear usuario
            var usuario = await EquipoService.ConsultarUsuarioPorDocumentoAsync(documentoUsuario);
            int idUsuarioInfo;

            if (usuario == null)
            {
                var nuevoUsuario = new UsuariosInformacion
                {
                    NumeroDocumento = documentoUsuario,
                    Nombres = usuarioNombres,
                    Apellidos = usuarioApellidos,
                    IdTipodocumento = idTipodocumentoSeleccionado,
                    IdArea = idAreaSeleccionada,
                    IdCampaña = idCampañaSeleccionada,
                    IdEstado = 1,
                    FechaCreacion = DateTime.Now,
                    UltimaModificacion = DateTime.Now
                };
                idUsuarioInfo = await EquipoService.CrearUsuarioAsync(nuevoUsuario);
            }
            else
            {
                idUsuarioInfo = usuario.Id;
            }

            // Validar equipo personal
            var equipoPersonal = await EquipoService.ObtenerEquipoPersonalPorIdAsync(idEquipoPersonalSeleccionado);
            if (equipoPersonal == null)
            {
                await JS.InvokeVoidAsync("alert", "⚠️ Debes seleccionar un equipo personal válido.");
                return;
            }

            // Generar código de barras
            string codigoBarras = $"{equipoPersonal.NombrePersonal}{documentoUsuario}{DateTime.Now:yyyyMMddHHmmss}";

            // Registrar equipo
            var equipo = new OUT_DOMAIN_EQUIPGO.Entities.Configuracion.Equipos()
            {
                Marca = equipoMarca,
                Modelo = equipoModelo,
                Serial = equipoSerial,
                CodigoBarras = codigoBarras,
                Ubicacion = equipoUbicacion,
                IdUsuarioInfo = idUsuarioInfo,
                IdEstado = 1,
                IdEquipoPersonal = equipoPersonal.Id,
                IdSede = 1,
                IdTipoDispositivo = 1,
                FechaCreacion = DateTime.Now,
                UltimaModificacion = DateTime.Now
            };

            bool equipoRegistrado = await EquipoService.CrearEquipoAsync(equipo);
            if (!equipoRegistrado)
            {
                await JS.InvokeVoidAsync("alert", "❌ Error al registrar el equipo.");
                return;
            }

            // Registrar transacción
            var authState = await AuthProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            var idUsuarioSessionClaim = user.Claims.FirstOrDefault(c => c.Type == "id_usuarioSession");
            int idUsuarioSession = int.Parse(idUsuarioSessionClaim?.Value ?? "0");

            var transaccion = new TransaccionRequest
            {
                CodigoBarras = codigoBarras,
                TipoTransaccion = tipoTransaccionSeleccionado,
                IdEquipoPersonal = equipoPersonal.Id,
                IdUsuarioInfo = idUsuarioInfo,
                IdUsuarioSession = idUsuarioSession,
                SedeOs = 1
            };

            var resultado = await TransaccionService.RegistrarTransaccionAsync(transaccion);
            await JS.InvokeVoidAsync("alert", resultado
                ? "✅ Equipo y transacción registrados."
                : "❌ Error al registrar la transacción.");

            LimpiarModalRegistroEquipo();
            mostrarModalRegistroEquipo = false;
            await IniciarEscaneo();
        }

        private void LimpiarModalRegistroEquipo()
        {
            documentoUsuario = "";
            usuarioNombres = "";
            usuarioApellidos = "";
            idTipodocumentoSeleccionado = 0;
            idAreaSeleccionada = 0;
            idCampañaSeleccionada = 0;
            equipoMarca = "";
            equipoModelo = "";
            equipoSerial = "";
            equipoUbicacion = "";
        }

        private async Task CerrarModalRegistroEquipo()
        {
            LimpiarModalRegistroEquipo();
            mostrarModalRegistroEquipo = false;
            await InvokeAsync(StateHasChanged);
        }
        #endregion

        #region Cerrar sesión y Dispose
        private async Task CerrarSesion()
        {
            try
            {
                await JS.InvokeVoidAsync("authInterop.logout");
                Navigation.NavigateTo("/login");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cerrar sesión: {ex.Message}");
            }
        }

        public void Dispose()
        {
            dotNetRef?.Dispose();
        }
        #endregion
    }
}
