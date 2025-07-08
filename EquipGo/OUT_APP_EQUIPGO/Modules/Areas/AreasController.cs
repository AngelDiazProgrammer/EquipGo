using Interface.Services.Areas;
using Microsoft.AspNetCore.Mvc;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Equipo;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Areas;




[ApiController]
[Route("api/[controller]")]

public class AreasController : ControllerBase
{
    private readonly IAreasService _areasService;

    public AreasController(IAreasService areasService)
    {
        _areasService = areasService;
    }

    [HttpPost("Admin")]
    public async Task<IActionResult> CrearAreaAdmin([FromBody] AreaDto areaDto)
    {
        try
        {
            await _areasService.CrearAreaAdminAsync(areaDto);
            return Ok(new { message = "Estado creado correctamente." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var area = await _areasService.ObtenerPorIdAsync(id);
        if (area == null)
            return NotFound();

        return Ok(area);
    }

    [HttpPut("Admin/{id}")]
    public async Task<IActionResult> ActualizarAreaAdmin(int id, [FromBody] AreaDto areaDto)
    {
        try
        {
            var actualizado = await _areasService.ActualizarAreaAdminAsync(id, areaDto);
            if (!actualizado)
                return NotFound(new { error = "Area no encontrada para actualizar." });

            return Ok(new { message = "Area actualizada correctamente." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("admin/{id}")]
    public async Task<IActionResult> EliminarArea(int id)
    {
        var resultado = await _areasService.EliminarAsync(id);
        return resultado ? Ok() : NotFound();
    }

}