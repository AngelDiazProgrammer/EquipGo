using Interface;
using Interface.Services.Transacciones;
using Interface.Services.Usuarios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OUT_DOMAIN_EQUIPGO.Entities.Procesos;
using OUT_DOMAIN_EQUIPGO.Entities.Seguridad;
using OUT_OS_APP.EQUIPGO.DTO.DTOs;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Transacciones;
using OUT_PERSISTENCE_EQUIPGO.Context;
using OUT_PERSISTENCE_EQUIPGO.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OUT_PERSISTENCE_EQUIPGO.Services.Transacciones
{
    public class TransaccionService : ITransaccionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<DashboardHub> _hubContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EquipGoDbContext _context;
        private readonly IUsuariosSessionService _usuariosSessionService;

        public TransaccionService(
            IUnitOfWork unitOfWork,
            IHubContext<DashboardHub> hubContext,
            IHttpContextAccessor httpContextAccessor,
            EquipGoDbContext context,
            IUsuariosSessionService usuariosSessionService)
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _usuariosSessionService = usuariosSessionService;
        }

        public async Task<TransaccionRequest> ObtenerUltimaTransaccionPorCodigoBarrasAsync(string codigoBarras)
        {
            return await _context.Transacciones
                .Where(t => t.CodigoBarras == codigoBarras)
                .OrderByDescending(t => t.FechaHora)
                .Select(t => new TransaccionRequest
                {
                    CodigoBarras = t.CodigoBarras,
                    TipoTransaccion = t.IdTipoTransaccion,
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> RegistrarTransaccionAsync(TransaccionRequest request)
        {
            try
            {
                var usuarioSession = await _usuariosSessionService.GetWithSedeAsync(request.IdUsuarioSession);

                if (usuarioSession?.Sede == null)
                {
                    throw new ArgumentException("Usuario o sede no encontrada");
                }

                // ✅ Obtener última transacción directamente
                var ultimaTransaccion = await _context.Transacciones
                    .Where(t => t.CodigoBarras == request.CodigoBarras)
                    .OrderByDescending(t => t.FechaHora)
                    .FirstOrDefaultAsync();

                // ✅ Lógica automática de tipo de transacción
                int tipoTransaccion;
                if (ultimaTransaccion == null)
                {
                    tipoTransaccion = 1; // Primera transacción = ENTRADA
                }
                else
                {
                    tipoTransaccion = ultimaTransaccion.IdTipoTransaccion == 1 ? 2 : 1; // Alternar
                }

                var transaccion = new OUT_DOMAIN_EQUIPGO.Entities.Procesos.Transacciones
                {
                    CodigoBarras = request.CodigoBarras,
                    IdTipoTransaccion = tipoTransaccion,
                    IdUsuarioInfo = request.IdUsuarioInfo,
                    IdUsuarioSession = request.IdUsuarioSession,
                    SedeOs = usuarioSession.IdSede,
                    FechaHora = DateTime.Now
                };

                await _unitOfWork.Transacciones.AddAsync(transaccion);
                await _unitOfWork.CompleteAsync();

                await _hubContext.Clients.All.SendAsync("NuevaTransaccion");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en RegistrarTransaccionAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RegistrarTransaccionVisitanteAsync(TransaccionVisitanteRequest request)
        {
            try
            {
                // ✅ Obtener la sede del usuario que realiza la transacción
                var usuarioSession = await _usuariosSessionService.GetWithSedeAsync(request.IdUsuarioSession);

                if (usuarioSession?.Sede == null)
                {
                    throw new Exception("Usuario o sede no encontrada");
                }

                // Buscar visitante por tipo y número de documento
                var visitante = await _context.UsuariosVisitantes
                    .FirstOrDefaultAsync(v =>
                        v.NumeroDocumento == request.NumeroDocumento);

                if (visitante == null)
                    throw new Exception("El visitante no está registrado.");

                // Actualizar el ID real del visitante en la solicitud
                request.IdUsuarioVisitante = visitante.Id;

                // Obtener el equipo asociado al visitante
                var equipo = await _context.EquiposVisitantes
                    .FirstOrDefaultAsync(e => e.IdUsuarioVisitante == visitante.Id);

                if (equipo == null)
                    throw new Exception("El equipo del visitante no está registrado.");

                request.IdEquipoVisitante = equipo.Id;

                // Obtener la última transacción del visitante
                var ultimaTransaccion = await _context.TransaccionesVisitantes
                    .Where(t => t.IdUsuarioVisitante == visitante.Id)
                    .OrderByDescending(t => t.FechaTransaccion)
                    .FirstOrDefaultAsync();

                // Determinar tipo de transacción automáticamente
                int tipoTransaccion = (ultimaTransaccion == null || ultimaTransaccion.IdTipoTransaccion == 2) ? 1 : 2;

                // Crear nueva transacción
                var nueva = new TransaccionesVisitantes
                {
                    FechaTransaccion = DateTime.Now,
                    IdEquipoVisitante = equipo.Id,
                    IdUsuarioVisitante = visitante.Id,
                    IdUsuarioSession = request.IdUsuarioSession,
                    SedeOs = usuarioSession.IdSede,  // ✅ Sede automática del usuario
                    IdTipoTransaccion = tipoTransaccion
                };

                _context.TransaccionesVisitantes.Add(nueva);
                await _context.SaveChangesAsync();
                await _hubContext.Clients.All.SendAsync("NuevaTransaccionVisitante");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en RegistrarTransaccionVisitanteAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<List<TransaccionDashboardDto>> ObtenerTodasLasTransaccionesAsync()
        {
            var transacciones = await _context.Transacciones
                .Include(t => t.IdTipoTransaccionNavigation)
                .Include(t => t.IdEquipoPersonalNavigation)
                .Include(t => t.IdUsuarioInfoNavigation)
                .Include(t => t.IdUsuarioSessionNavigation)
                .Include(t => t.SedeOsNavigation)
                .OrderByDescending(t => t.FechaHora)
                .Select(t => new TransaccionDashboardDto
                {
                    CodigoBarras = t.CodigoBarras,
                    NombreUsuarioInfo = t.IdUsuarioInfoNavigation.Nombres + " " + t.IdUsuarioInfoNavigation.Apellidos,
                    NombreTipoTransaccion = t.IdTipoTransaccionNavigation.NombreTransaccion,
                    NombreEquipoPersonal = t.IdEquipoPersonalNavigation.NombrePersonal,
                    NombreUsuarioSession = t.IdUsuarioSessionNavigation.Nombre + " " + t.IdUsuarioSessionNavigation.Apellido,
                    NombreSedeOs = t.SedeOsNavigation.NombreSede,
                    FechaHora = t.FechaHora // Agregué este campo para tener referencia temporal
                })
                .ToListAsync();

            return transacciones;
        }

        public async Task<List<TransaccionVisitanteDashboardDto>> ObtenerTodasLasTransaccionesVisitantesAsync()
        {
            var transacciones = await _context.TransaccionesVisitantes
                .Include(t => t.IdUsuarioSessionNavigation)
                .Include(t => t.IdUsuarioVisitanteNavigation)
                .Include(t => t.IdEquipoVisitanteNavigation)
                .Include(t => t.IdTipoTransaccionNavigation)
                .Include(t => t.SedeOsNavigation)
                .OrderByDescending(t => t.FechaTransaccion)
                .Select(t => new TransaccionVisitanteDashboardDto
                {
                    NombresVisitante = t.IdUsuarioVisitanteNavigation.Nombres + " " + t.IdUsuarioVisitanteNavigation.Apellidos,
                    MarcaEquipo = t.IdEquipoVisitanteNavigation != null ? t.IdEquipoVisitanteNavigation.Marca : "",
                    NombreAprobador = t.IdUsuarioSessionNavigation != null
                        ? t.IdUsuarioSessionNavigation.Nombre + " " + t.IdUsuarioSessionNavigation.Apellido
                        : "Sin aprobador",
                    NombreSede = t.SedeOsNavigation != null ? t.SedeOsNavigation.NombreSede : "Sin sede",
                    TipoTransaccion = t.IdTipoTransaccionNavigation.NombreTransaccion,
                    FechaTransaccion = t.FechaTransaccion // Agregué este campo para tener referencia temporal
                })
                .ToListAsync();

            return transacciones;
        }

        public async Task<List<TransaccionDashboardDto>> ObtenerTransaccionesHoyAsync()
        {
            var hoy = DateTime.Today;

            var transacciones = await _context.Transacciones
                .Include(t => t.IdTipoTransaccionNavigation)
                .Include(t => t.IdEquipoPersonalNavigation)
                .Include(t => t.IdUsuarioInfoNavigation)
                .Include(t => t.IdUsuarioSessionNavigation)
                .Include(t => t.SedeOsNavigation)
                .Where(t => t.FechaHora.Date == hoy)
                .OrderByDescending(t => t.FechaHora)
                .Take(10)
                .Select(t => new TransaccionDashboardDto
                {
                    CodigoBarras = t.CodigoBarras,
                    NombreUsuarioInfo = t.IdUsuarioInfoNavigation.Nombres + " " + t.IdUsuarioInfoNavigation.Apellidos,
                    NombreTipoTransaccion = t.IdTipoTransaccionNavigation.NombreTransaccion,
                    NombreEquipoPersonal = t.IdEquipoPersonalNavigation.NombrePersonal,
                    NombreUsuarioSession = t.IdUsuarioSessionNavigation.Nombre + " " + t.IdUsuarioSessionNavigation.Apellido,
                    NombreSedeOs = t.SedeOsNavigation.NombreSede
                })
                .ToListAsync();

            return transacciones;
        }

        public async Task<List<TransaccionVisitanteDashboardDto>> ObtenerTransaccionesVisitantesHoyAsync()
        {
            var hoy = DateTime.Today;

            var transacciones = await _context.TransaccionesVisitantes
                .Include(t => t.IdUsuarioSessionNavigation)
                .Include(t => t.IdUsuarioVisitanteNavigation)
                .Include(t => t.IdEquipoVisitanteNavigation)
                .Include(t => t.IdTipoTransaccionNavigation)
                .Include(t => t.SedeOsNavigation)
                .Where(t => t.FechaTransaccion.Date == hoy)
                .OrderByDescending(t => t.FechaTransaccion)
                .Select(t => new TransaccionVisitanteDashboardDto
                {
                    NombresVisitante = t.IdUsuarioVisitanteNavigation.Nombres + " " + t.IdUsuarioVisitanteNavigation.Apellidos,
                    MarcaEquipo = t.IdEquipoVisitanteNavigation != null ? t.IdEquipoVisitanteNavigation.Marca : "",
                    NombreAprobador = t.IdUsuarioSessionNavigation != null
                ? t.IdUsuarioSessionNavigation.Nombre + " " + t.IdUsuarioSessionNavigation.Apellido
                : "Sin aprobador",
                    NombreSede = t.SedeOsNavigation != null ? t.SedeOsNavigation.NombreSede : "Sin sede",
                    TipoTransaccion = t.IdTipoTransaccionNavigation.NombreTransaccion
                })
                .ToListAsync();

            return transacciones;
        }

        public async Task<ConteoTransaccionesDto> ObtenerConteosDashboardAsync()
        {
            var hoy = DateTime.Today;

            // Transacciones de hoy (todas)
            var totalHoy = await _context.Transacciones
                .CountAsync(t => t.FechaHora.Date == hoy);

            // Transacciones de equipos corporativos (Id = 1)
            var totalPersonales = await _context.Transacciones
                .Include(t => t.IdEquipoPersonalNavigation)
                .CountAsync(t => t.FechaHora.Date == hoy && t.IdEquipoPersonalNavigation.NombrePersonal == "Corporativo");

            // Transacciones de equipos proveedor (Id = 2)
            var totalCorporativos = await _context.Transacciones
                .Include(t => t.IdEquipoPersonalNavigation)
                .CountAsync(t => t.FechaHora.Date == hoy && t.IdEquipoPersonalNavigation.NombrePersonal == "Proveedor");

            // Transacciones de equipos Personal (Id =3)
            var totalProveedores = await _context.Transacciones
                .Include(t => t.IdEquipoPersonalNavigation)
                .CountAsync(t => t.FechaHora.Date == hoy && t.IdEquipoPersonalNavigation.NombrePersonal == "Personal");

            return new ConteoTransaccionesDto
            {
                TotalHoy = totalHoy,
                TotalPersonales = totalPersonales,
                TotalCorporativos = totalCorporativos,
                TotalProveedores = totalProveedores
            };
        }





        //Exportar Excel
        public async Task<List<TransaccionDashboardDto>> ObtenerTransaccionesFiltradasAsync(FiltrosTransaccionDto filtros)
        {
            var query = _context.Transacciones
                .Include(t => t.IdTipoTransaccionNavigation)
                .Include(t => t.IdEquipoPersonalNavigation)
                .Include(t => t.IdUsuarioInfoNavigation)
                .Include(t => t.IdUsuarioSessionNavigation)
                .Include(t => t.SedeOsNavigation)
                .AsQueryable();

            // Aplicar filtros
            if (!string.IsNullOrWhiteSpace(filtros.CodigoBarras))
            {
                query = query.Where(t => t.CodigoBarras.Contains(filtros.CodigoBarras));
            }

            if (!string.IsNullOrWhiteSpace(filtros.Usuario))
            {
                query = query.Where(t =>
                    (t.IdUsuarioInfoNavigation.Nombres + " " + t.IdUsuarioInfoNavigation.Apellidos)
                    .Contains(filtros.Usuario));
            }

            if (!string.IsNullOrWhiteSpace(filtros.TipoTransaccion))
            {
                query = query.Where(t => t.IdTipoTransaccionNavigation.NombreTransaccion.Contains(filtros.TipoTransaccion));
            }

            if (!string.IsNullOrWhiteSpace(filtros.Sede))
            {
                query = query.Where(t => t.SedeOsNavigation.NombreSede.Contains(filtros.Sede));
            }

            // CORREGIDO: Filtro por fecha simple
            if (!string.IsNullOrWhiteSpace(filtros.Fecha) && DateTime.TryParse(filtros.Fecha, out var fechaFiltro))
            {
                query = query.Where(t => t.FechaHora.Date == fechaFiltro.Date);
            }

            var transacciones = await query
                .OrderByDescending(t => t.FechaHora)
                .Select(t => new TransaccionDashboardDto
                {
                    CodigoBarras = t.CodigoBarras,
                    NombreUsuarioInfo = t.IdUsuarioInfoNavigation.Nombres + " " + t.IdUsuarioInfoNavigation.Apellidos,
                    NombreTipoTransaccion = t.IdTipoTransaccionNavigation.NombreTransaccion,
                    NombreEquipoPersonal = t.IdEquipoPersonalNavigation.NombrePersonal,
                    NombreUsuarioSession = t.IdUsuarioSessionNavigation.Nombre + " " + t.IdUsuarioSessionNavigation.Apellido,
                    NombreSedeOs = t.SedeOsNavigation.NombreSede,
                    FechaHora = t.FechaHora
                })
                .ToListAsync();

            return transacciones;
        }

        public async Task<List<TransaccionVisitanteDashboardDto>> ObtenerTransaccionesVisitantesFiltradasAsync(FiltrosTransaccionVisitanteDto filtros)
        {
            var query = _context.TransaccionesVisitantes
                .Include(t => t.IdUsuarioSessionNavigation)
                .Include(t => t.IdUsuarioVisitanteNavigation)
                .Include(t => t.IdEquipoVisitanteNavigation)
                .Include(t => t.IdTipoTransaccionNavigation)
                .Include(t => t.SedeOsNavigation)
                .AsQueryable();

            // Aplicar filtros
            if (!string.IsNullOrWhiteSpace(filtros.Visitante))
            {
                query = query.Where(t =>
                    (t.IdUsuarioVisitanteNavigation.Nombres + " " + t.IdUsuarioVisitanteNavigation.Apellidos)
                    .Contains(filtros.Visitante));
            }

            if (!string.IsNullOrWhiteSpace(filtros.Aprobador))
            {
                query = query.Where(t =>
                    t.IdUsuarioSessionNavigation != null &&
                    (t.IdUsuarioSessionNavigation.Nombre + " " + t.IdUsuarioSessionNavigation.Apellido)
                    .Contains(filtros.Aprobador));
            }

            if (!string.IsNullOrWhiteSpace(filtros.TipoTransaccion))
            {
                query = query.Where(t => t.IdTipoTransaccionNavigation.NombreTransaccion.Contains(filtros.TipoTransaccion));
            }

            if (!string.IsNullOrWhiteSpace(filtros.Sede))
            {
                query = query.Where(t => t.SedeOsNavigation.NombreSede.Contains(filtros.Sede));
            }

            // CORREGIDO: Filtro por fecha simple para visitantes
            if (!string.IsNullOrWhiteSpace(filtros.Fecha) && DateTime.TryParse(filtros.Fecha, out var fechaFiltro))
            {
                query = query.Where(t => t.FechaTransaccion.Date == fechaFiltro.Date);
            }

            var transacciones = await query
                .OrderByDescending(t => t.FechaTransaccion)
                .Select(t => new TransaccionVisitanteDashboardDto
                {
                    NombresVisitante = t.IdUsuarioVisitanteNavigation.Nombres + " " + t.IdUsuarioVisitanteNavigation.Apellidos,
                    MarcaEquipo = t.IdEquipoVisitanteNavigation != null ? t.IdEquipoVisitanteNavigation.Marca : "",
                    NombreAprobador = t.IdUsuarioSessionNavigation != null
                        ? t.IdUsuarioSessionNavigation.Nombre + " " + t.IdUsuarioSessionNavigation.Apellido
                        : "Sin aprobador",
                    NombreSede = t.SedeOsNavigation != null ? t.SedeOsNavigation.NombreSede : "Sin sede",
                    TipoTransaccion = t.IdTipoTransaccionNavigation.NombreTransaccion,
                    FechaTransaccion = t.FechaTransaccion
                })
                .ToListAsync();

            return transacciones;
        }




        private int ObtenerIdUsuarioSessionActual()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }
    }
}
