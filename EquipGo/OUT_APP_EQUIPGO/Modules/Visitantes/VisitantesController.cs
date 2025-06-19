using Interface.Services.Visitantes;
using Microsoft.AspNetCore.Mvc;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Visitantes;


    [ApiController]
    [Route("api/[controller]")]
    public class VisitantesController : ControllerBase
    {
        private readonly IRegistroVisitanteService _registroVisitanteService;

        public VisitantesController(IRegistroVisitanteService registroVisitanteService)
        {
            _registroVisitanteService = registroVisitanteService;
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarVisitante([FromBody] RegistroVisitanteDto dto)
        {
            try
            {
                await _registroVisitanteService.RegistrarVisitanteAsync(dto);
                return Ok(new { message = "✅ Visitante registrado correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"❌ Error: {ex.Message}" });
            }
        }
    }
