using Interface.Services.Transacciones;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OUT_OS_APP.EQUIPGO.DTO.DTOs;
using OUT_PERSISTENCE_EQUIPGO.Context;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OUT_APP_EQUIPGO.Modules.Transacciones
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransaccionesController : ControllerBase
    {
        private readonly ITransaccionService _transaccionService;
        private readonly EquipGoDbContext _context;

        public TransaccionesController(ITransaccionService transaccionService, EquipGoDbContext context)
        {
            _transaccionService = transaccionService;
            _context = context;
        }

        [HttpGet("GetTransaccionesHoy")]
        public async Task<IActionResult> GetTransaccionesHoy()
        {
            var transacciones = await _transaccionService.ObtenerTransaccionesHoyAsync();
            return Ok(transacciones);
        }
        [HttpGet("GetTransaccionesVisitantesHoy")]
        public async Task<IActionResult> GetTransaccionesVisitantesHoy()
        {
            var transacciones = await _transaccionService.ObtenerTransaccionesVisitantesHoyAsync();
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

        [HttpGet("GetConteosDashboard")]
        public async Task<IActionResult> GetConteosDashboard()
        {
            var hoy = DateTime.Today;

            var totalNormales = await _context.Transacciones
                .Where(t => t.FechaHora.Date == hoy)
                .CountAsync();

            var totalVisitantes = await _context.TransaccionesVisitantes
                .Where(t => t.FechaTransaccion.Date == hoy)
                .CountAsync();

            var totalHoy = totalNormales + totalVisitantes;

            return Ok(new
            {
                totalHoy,
                totalNormales,
                totalVisitantes
            });
        }


        private int ObtenerIdUsuarioSessionActual()
        {
            var userIdClaim = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }
    }
}
