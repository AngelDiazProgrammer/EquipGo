using Interface.Services.Estados;
using Microsoft.AspNetCore.Mvc;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Equipo;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Estado;




[ApiController]
[Route("api/[controller]")]

public class EstadosController : ControllerBase
{
    private readonly IEstadoService _estadoService;

    public EstadosController(IEstadoService estadoService)
    {
        _estadoService = estadoService; 
    }

    [HttpPost("Admin")]
    public async Task<IActionResult> CrearEstadoAdmin([FromBody] EstadoDto estadoDto)
    {
        try
        {
            await _estadoService.CrearEstadoAdminAsync(estadoDto);
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
        var estado = await _estadoService.ObtenerPorIdAsync(id);
        if (estado == null)
            return NotFound();

        return Ok(estado);
    }

    [HttpPut("Admin/{id}")]
    public async Task<IActionResult> ActualizarEquipoAdmin(int id, [FromBody] EstadoDto estadoDto)
    {
        try
        {
            var actualizado = await _estadoService.ActualizarEstadoAdminAsync(id, estadoDto);
            if (!actualizado)
                return NotFound(new { error = "Estado no encontrado para actualizar." });

            return Ok(new { message = "Estado actualizado correctamente." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("admin/{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var resultado = await _estadoService.EliminarAsync(id);
        return resultado ? Ok() : NotFound();
    }

}