// Controllers/EquiposController.cs
using Interface.Services.Equipos;
using Interface.Services.Estados;
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

    public EquiposController(IEquipoService equipoService, IUsuariosInformacionService usuariosInformacionService, IEstadoService estadoService, ISedesService sedesService, ITipoDispositivosService tipoDispositivosService)
    {
        _equipoService = equipoService;
        _usuariosInformacionService = usuariosInformacionService;
        _estadoService = estadoService;
        _sedesService = sedesService;
        _tipoDispositivosService = tipoDispositivosService;
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

    [HttpGet("admin/form-data")]
    public async Task<IActionResult> ObtenerFormData()
    {
        var usuarios = await _usuariosInformacionService.ObtenerTodosLosUsuariosInformacionAsync();
        var estados = await _estadoService.ObtenerTodasAsync();
        var equiposPersonales = await _equipoService.ObtenerEquiposPersonalesAsync();
        var sedes = await _sedesService.ObtenerTodasAsync();
        var tiposDispositivo = await _tipoDispositivosService.ObtenerTodasAsync();

        return Ok(new
        {
            usuarios,
            estados,
            equiposPersonales,
            sedes,
            tiposDispositivo
        });
    }



}
