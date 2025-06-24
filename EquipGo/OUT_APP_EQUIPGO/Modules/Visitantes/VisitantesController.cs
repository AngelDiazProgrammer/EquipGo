using Interface.Services.Visitantes;
using Microsoft.AspNetCore.Mvc;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Visitantes;


    [ApiController]
    [Route("api/[controller]")]
    public class VisitantesController : ControllerBase
    {
        private readonly IVisitanteService _VisitanteService;

        public VisitantesController(IVisitanteService VisitanteService)
        {
            _VisitanteService = VisitanteService;
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarVisitante([FromBody] RegistroVisitanteDto dto)
        {
            try
            {
                await _VisitanteService.RegistrarVisitanteAsync(dto);
                return Ok(new { message = "✅ Visitante registrado correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"❌ Error: {ex.Message}" });
            }
        }
    }
