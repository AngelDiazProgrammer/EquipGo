// Controllers/EquiposController.cs
using Interface.Services.Active_Directory;
using Interface.Services.Equipos;
using Interface.Services.Estados;
using Interface.Services.Proveedores;
using Interface.Services.Sedes;
using Interface.Services.TipoDispositivos;
using Interface.Services.Usuarios;
using Microsoft.AspNetCore.Mvc;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Equipo;

[ApiController]
[Route("api/[controller]")]
public class EquiposController : ControllerBase
{
    private readonly IEquipoService _equipoService;
    private readonly IUsuariosInformacionService _usuariosInformacionService;
    private readonly IEstadoService _estadoService;
    private readonly ISedesService  _sedesService;
    private readonly ITipoDispositivosService _tipoDispositivosService;
    private readonly IProveedoresService _proveedoresService;
    private readonly IActiveDirectoryService _activeDirectoryService;

    public EquiposController(IEquipoService equipoService, IUsuariosInformacionService usuariosInformacionService, IEstadoService estadoService, ISedesService sedesService, ITipoDispositivosService tipoDispositivosService, IProveedoresService proveedoresService, IActiveDirectoryService activeDirectoryService)
    {
        _equipoService = equipoService;
        _usuariosInformacionService = usuariosInformacionService;
        _estadoService = estadoService;
        _sedesService = sedesService;
        _tipoDispositivosService = tipoDispositivosService;
        _proveedoresService = proveedoresService;
        _activeDirectoryService = activeDirectoryService;
    }

    [HttpPost("Admin")]
    public async Task<IActionResult> CrearEquipoAdmin([FromBody] CrearEquipoDto equipoDto)
    {
        try
        {
            await _equipoService.CrearEquipoAdminAsync(equipoDto);
            return Ok(new { message = "Equipo creado correctamente." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var equipo = await _equipoService.ObtenerPorIdAsync(id);
        if (equipo == null)
            return NotFound();

        return Ok(equipo);
    }

    [HttpPut("Admin/{id}")]
    public async Task<IActionResult> ActualizarEquipoAdmin(int id, [FromBody] CrearEquipoDto equipoDto)
    {
        try
        {
            var actualizado = await _equipoService.ActualizarEquipoAdminAsync(id, equipoDto);
            if (!actualizado)
                return NotFound(new { error = "Equipo no encontrado para actualizar." });

            return Ok(new { message = "Equipo actualizado correctamente." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("admin/{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var resultado = await _equipoService.EliminarAsync(id);
        return resultado ? Ok() : NotFound();
    }

    [HttpGet("admin/form-data")]
    public async Task<IActionResult> ObtenerFormData()
    {
        var usuariosAD = await _activeDirectoryService.ObtenerUsuariosAsync();
        var estados = await _estadoService.ObtenerTodasAsync();
        var equiposPersonales = await _equipoService.ObtenerEquiposPersonalesAsync();
        var sedes = await _sedesService.ObtenerTodasAsync();
        var tiposDispositivo = await _tipoDispositivosService.ObtenerTodasAsync();
        var proveedores = await _proveedoresService.ObtenerTodasAsync();

        return Ok(new
        {
            usuarios = usuariosAD,
            estados,
            equiposPersonales,
            sedes,
            tiposDispositivo,
            proveedores
        });
    }

    [HttpPost("sync")]
    public async Task<IActionResult> SincronizarEquipo([FromBody] EquipoSyncRequestDto dto)
    {
        if (dto == null)
            return BadRequest("El cuerpo de la solicitud no puede estar vacío.");

        var resultado = await _equipoService.SincronizarEquipoAsync(dto);

        if (resultado.Contains("actualizado"))
            return Ok(resultado); // 200 OK
        if (resultado.Contains("registrado"))
            return StatusCode(201, resultado); // 201 Created

        return BadRequest(resultado); // 400 BadRequest por validaciones fallidas
    }

}
