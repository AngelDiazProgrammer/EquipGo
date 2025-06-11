using Interface;
using Interface.Services.Transacciones;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OUT_DOMAIN_EQUIPGO.Entities.Procesos;
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

        public TransaccionService(
            IUnitOfWork unitOfWork,
            IHubContext<DashboardHub> hubContext,
            IHttpContextAccessor httpContextAccessor,
            EquipGoDbContext context)
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public async Task<bool> RegistrarTransaccionAsync(TransaccionRequest request)
        {
            try
            {
                var transaccion = new OUT_DOMAIN_EQUIPGO.Entities.Procesos.Transacciones
                {
                    CodigoBarras = request.CodigoBarras,
                    IdTipoTransaccion = request.TipoTransaccion,
                    IdEquipoPersonal = request.IdEquipoPersonal,
                    IdUsuarioInfo = request.IdUsuarioInfo,
                    IdUsuarioSession = request.IdUsuarioSession,      // el usuario que dispara
                    SedeOs = request.SedeOs,
                    FechaHora = DateTime.Now
                };

                await _unitOfWork.Transacciones.AddAsync(transaccion);
                await _unitOfWork.CompleteAsync();

                // Emitir señal al frontend
                await _hubContext.Clients.All.SendAsync("NuevaTransaccion");
                return true;
            }
            catch (Exception)
            {
                // Podrías agregar logging aquí
                return false;
            }
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



        private int ObtenerIdUsuarioSessionActual()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }
    }
}
