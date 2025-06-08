using Interface.Services.Transacciones;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OUT_OS_APP.EQUIPGO.DTO.DTOs;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Transacciones;
using OUT_PERSISTENCE_EQUIPGO.Context;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OUT_APP_EQUIPGO.Modules
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransaccionesController : ControllerBase
    {
        private readonly EquipGoDbContext _context;
        private readonly ITransaccionService _transaccionService;

        public TransaccionesController(EquipGoDbContext context, ITransaccionService transaccionService)
        {
            _context = context;
            _transaccionService = transaccionService;
        }

        [HttpGet("GetTransaccionesHoy")]
        public async Task<IActionResult> GetTransaccionesHoy()
        {
            var hoy = DateTime.Today;

            var transacciones = await _context.Transacciones
                .Where(t => t.FechaHora.Date == hoy)
                .Include(t => t.IdTipoTransaccionNavigation)
                .Include(t => t.IdEquipoPersonalNavigation)
                .Include(t => t.IdUsuarioInfoNavigation)
                .Include(t => t.IdUsuarioSessionNavigation)
                .Include(t => t.UsuarioAprobadorNavigation) // 👈 Nuevo include
                .Include(t => t.SedeOsNavigation)
                .OrderByDescending(t => t.FechaHora)
                .Take(12)
                .Select(t => new TransaccionDashboardDTO
                {
                    CodigoBarras = t.CodigoBarras,
                    NombreUsuarioInfo = t.IdUsuarioInfoNavigation.Nombres + " " + t.IdUsuarioInfoNavigation.Apellidos,
                    NombreTipoTransaccion = t.IdTipoTransaccionNavigation.NombreTransaccion,
                    NombreEquipoPersonal = t.IdEquipoPersonalNavigation.NombrePersonal,
                    NombreUsuarioSession = t.UsuarioAprobadorNavigation != null
                        ? t.UsuarioAprobadorNavigation.Nombre + " " + t.UsuarioAprobadorNavigation.Apellido
                        : "Sin aprobar", // Evitar nulos
                    NombreSedeOs = t.SedeOsNavigation.NombreSede
                })
                .ToListAsync();

            return Ok(transacciones);
        }

        [HttpPost("RegistrarTransaccion")]
        public async Task<IActionResult> RegistrarTransaccion([FromBody] TransaccionRequest request)
        {
            // Tomar el usuario aprobador desde la sesión activa
            request.IdUsuarioAprobador = ObtenerIdUsuarioSessionActual();

            var resultado = await _transaccionService.RegistrarTransaccionAsync(request);
            if (resultado)
            {
                return Ok(new { mensaje = "✅ Transacción registrada exitosamente" });
            }
            else
            {
                return StatusCode(500, new { mensaje = "❌ Error al registrar la transacción" });
            }
        }

        private int ObtenerIdUsuarioSessionActual()
        {
            var userIdClaim = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }
    }
}
