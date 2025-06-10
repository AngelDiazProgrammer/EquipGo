using Interface.Services.Transacciones;
using Microsoft.AspNetCore.Mvc;
using OUT_OS_APP.EQUIPGO.DTO.DTOs;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OUT_APP_EQUIPGO.Modules
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransaccionesController : ControllerBase
    {
        private readonly ITransaccionService _transaccionService;

        public TransaccionesController(ITransaccionService transaccionService)
        {
            _transaccionService = transaccionService;
        }

        [HttpGet("GetTransaccionesHoy")]
        public async Task<IActionResult> GetTransaccionesHoy()
        {
            var transacciones = await _transaccionService.ObtenerTransaccionesHoyAsync();
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
            var conteos = await _transaccionService.ObtenerConteosDashboardAsync();
            return Ok(conteos);
        }


        private int ObtenerIdUsuarioSessionActual()
        {
            var userIdClaim = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }
    }
}
