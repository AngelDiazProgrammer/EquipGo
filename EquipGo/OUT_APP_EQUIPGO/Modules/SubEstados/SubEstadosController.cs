using Interface.Services.SubEstado;
using Microsoft.AspNetCore.Mvc;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.SubEstado;

[ApiController]
[Route("api/[controller]")]
public class SubEstadosController : ControllerBase
{
    private readonly ISubEstadoService _subEstadoService;

    public SubEstadosController(ISubEstadoService subEstadoService)
    {
        _subEstadoService = subEstadoService;
    }

    [HttpPost("Admin")]
    public async Task<IActionResult> CrearSubEstadoAdmin([FromBody] SubEstadoDto subEstadoDto)
    {
        try
        {
            await _subEstadoService.CrearSubEstadoAdminAsync(subEstadoDto);
            return Ok(new { message = "Subestado creado correctamente." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var subEstado = await _subEstadoService.ObtenerPorIdAsync(id);
        if (subEstado == null)
            return NotFound();

        return Ok(subEstado);
    }

    [HttpPut("Admin/{id}")]
    public async Task<IActionResult> ActualizarSubEstadoAdmin(int id, [FromBody] SubEstadoDto subEstadoDto)
    {
        try
        {
            var actualizado = await _subEstadoService.ActualizarSubEstadoAdminAsync(id, subEstadoDto);
            if (!actualizado)
                return NotFound(new { error = "Subestado no encontrado para actualizar." });

            return Ok(new { message = "Subestado actualizado correctamente." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("admin/{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var resultado = await _subEstadoService.EliminarAsync(id);
        return resultado ? Ok() : NotFound();
    }
}
