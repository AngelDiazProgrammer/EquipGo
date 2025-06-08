using Interface;
using Interface.Services.Transacciones;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OUT_DOMAIN_EQUIPGO.Entities.Procesos;
using OUT_OS_APP.EQUIPGO.DTO.DTOs;
using OUT_PERSISTENCE_EQUIPGO.Hubs;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OUT_PERSISTENCE_EQUIPGO.Services.Transacciones
{
    public class TransaccionService : ITransaccionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<DashboardHub> _hubContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TransaccionService(IUnitOfWork unitOfWork, IHubContext<DashboardHub> hubContext, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<bool> RegistrarTransaccionAsync(TransaccionRequest request)
        {
            try
            {
                // 👇 Aquí suponemos que el usuario aprobador es el usuario actual de sesión
                var idUsuarioAprobador = ObtenerIdUsuarioSessionActual(); // Ajusta este método

                var transaccion = new OUT_DOMAIN_EQUIPGO.Entities.Procesos.Transacciones
                {
                    CodigoBarras = request.CodigoBarras,
                    IdTipoTransaccion = request.TipoTransaccion,
                    IdEquipoPersonal = request.IdEquipoPersonal,
                    IdUsuarioInfo = request.IdUsuarioInfo,
                    IdUsuarioSession = request.IdUsuarioSession, // El usuario que registra la transacción
                    IdUsuarioAprobador = request.IdUsuarioAprobador, // ✅ Nuevo campo
                    SedeOs = request.SedeOs,
                    FechaHora = DateTime.Now,
                };

                await _unitOfWork.Transacciones.AddAsync(transaccion);
                await _unitOfWork.CompleteAsync();

                // 🚨 Emitir la señal al frontend
                await _hubContext.Clients.All.SendAsync("NuevaTransaccion");
                return true;
            }
            catch (Exception)
            {
                // Podrías agregar logging aquí
                return false;
            }
        }
        private int ObtenerIdUsuarioSessionActual()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }
    }
}
