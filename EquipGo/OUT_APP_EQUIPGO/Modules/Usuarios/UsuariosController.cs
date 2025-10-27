using Interface.Services.Usuarios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OUT_DOMAIN_EQUIPGO.Entities.Procesos;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Usuarios;
using OUT_PERSISTENCE_EQUIPGO.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuariosInformacionService _usuariosService;
    private readonly EquipGoDbContext _context;

    public UsuariosController(IUsuariosInformacionService usuariosService, EquipGoDbContext context)
    {
        _usuariosService = usuariosService;
        _context = context;
    }

    // GET: api/usuarios
    [HttpGet]
    public async Task<IActionResult> ObtenerTodosLosUsuarios()
    {
        try
        {
            var usuarios = await _usuariosService.ObtenerTodosLosUsuariosInformacionAsync();
            return Ok(usuarios);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // GET: api/usuarios/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerUsuarioPorId(int id)
    {
        try
        {
            var usuario = await _usuariosService.ObtenerPorIdAsync(id);
            if (usuario == null)
                return NotFound(new { error = "Usuario no encontrado" });

            return Ok(usuario);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // POST: api/usuarios
    [HttpPost]
    public async Task<IActionResult> CrearUsuario([FromBody] UsuarioCrearDto usuarioDto)
    {
        try
        {
            if (usuarioDto == null)
                return BadRequest(new { error = "Los datos del usuario son requeridos" });

            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(usuarioDto.Nombres) || string.IsNullOrWhiteSpace(usuarioDto.Apellidos))
                return BadRequest(new { error = "Los nombres y apellidos son obligatorios" });

            if (string.IsNullOrWhiteSpace(usuarioDto.NumeroDocumento))
                return BadRequest(new { error = "El número de documento es obligatorio" });

            // 🔥 BUSCAR CAMPAÑA POR NOMBRE
            int? idCampaña = null;
            if (!string.IsNullOrWhiteSpace(usuarioDto.NombreCampaña))
            {
                var campaña = await _context.Campañas
                    .FirstOrDefaultAsync(c => c.NombreCampaña == usuarioDto.NombreCampaña.Trim());

                if (campaña == null)
                    return BadRequest(new { error = $"La campaña '{usuarioDto.NombreCampaña}' no existe" });

                idCampaña = campaña.Id;
            }

            var usuario = new UsuariosInformacion
            {
                IdTipodocumento = usuarioDto.IdTipoDocumento, // ✅ Puede ser null
                NumeroDocumento = usuarioDto.NumeroDocumento.Trim(),
                Nombres = usuarioDto.Nombres.Trim(),
                Apellidos = usuarioDto.Apellidos.Trim(),
                IdArea = usuarioDto.IdArea, // ✅ Puede ser null
                IdCampaña = idCampaña, // ✅ Usar el ID encontrado por nombre
                IdEstado = usuarioDto.IdEstado ?? 1,
                FechaCreacion = DateTime.UtcNow,
                UltimaModificacion = DateTime.UtcNow
            };

            var usuarioId = await _usuariosService.CrearUsuarioAsync(usuario);

            return Ok(new
            {
                message = "Usuario creado correctamente",
                id = usuarioId
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // PUT: api/usuarios/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> ActualizarUsuario(int id, [FromBody] UsuarioCrearDto usuarioDto)
    {
        try
        {
            if (usuarioDto == null)
                return BadRequest(new { error = "Los datos del usuario son requeridos" });

            // 🔥 BUSCAR CAMPAÑA POR NOMBRE PARA ACTUALIZACIÓN
            int? idCampaña = null;
            if (!string.IsNullOrWhiteSpace(usuarioDto.NombreCampaña))
            {
                var campaña = await _context.Campañas
                    .FirstOrDefaultAsync(c => c.NombreCampaña == usuarioDto.NombreCampaña.Trim());

                if (campaña == null)
                    return BadRequest(new { error = $"La campaña '{usuarioDto.NombreCampaña}' no existe" });

                idCampaña = campaña.Id;
            }

            // Crear un DTO temporal con el ID de campaña encontrado
            var usuarioDtoConId = new UsuarioCrearDto
            {
                IdTipoDocumento = usuarioDto.IdTipoDocumento,
                NumeroDocumento = usuarioDto.NumeroDocumento,
                Nombres = usuarioDto.Nombres,
                Apellidos = usuarioDto.Apellidos,
                IdArea = usuarioDto.IdArea,
                IdCampaña = idCampaña, // ✅ Asignar el ID encontrado
                IdEstado = usuarioDto.IdEstado
            };

            var actualizado = await _usuariosService.ActualizarUsuarioAdminAsync(id, usuarioDtoConId);
            if (!actualizado)
                return NotFound(new { error = "Usuario no encontrado para actualizar" });

            return Ok(new { message = "Usuario actualizado correctamente" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // DELETE: api/usuarios/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarUsuario(int id)
    {
        try
        {
            var eliminado = await _usuariosService.EliminarAsync(id);
            if (!eliminado)
                return NotFound(new { error = "Usuario no encontrado" });

            return Ok(new { message = "Usuario eliminado correctamente" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // GET: api/usuarios/form-data
    [HttpGet("form-data")]
    public async Task<IActionResult> ObtenerFormData()
    {
        try
        {
            var tiposDocumento = await _usuariosService.ObtenerTipoDocumentoAsync();
            var areas = await _usuariosService.ObtenerAreasAsync();
            var campanas = await _usuariosService.ObtenerCampañasAsync();
            var estados = await _usuariosService.ObtenerEstadosAsync();

            return Ok(new
            {
                tiposDocumento = tiposDocumento.Select(t => new { id = t.Id, nombreDocumento = t.NombreDocumento }),
                areas = areas.Select(a => new { id = a.Id, nombreArea = a.NombreArea }),
                campanas = campanas.Select(c => new { id = c.Id, nombreCampaña = c.NombreCampaña }),
                estados = estados.Select(e => new { id = e.Id, nombreEstado = e.NombreEstado })
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // POST: api/usuarios/carga-masiva
    [HttpPost("carga-masiva")]
    public async Task<ActionResult<ResultadoCargaMasivaDto>> CargaMasivaUsuarios([FromBody] List<UsuarioCrearDto> usuariosDto)
    {
        try
        {
            // 🔥 PRECARGAR CAMPAÑAS PARA OPTIMIZACIÓN
            var nombresCampañas = usuariosDto
                .Where(u => !string.IsNullOrEmpty(u.NombreCampaña))
                .Select(u => u.NombreCampaña.Trim())
                .Distinct()
                .ToList();

            var campañasExistentes = await _context.Campañas
                .Where(c => nombresCampañas.Contains(c.NombreCampaña))
                .Select(c => new { c.NombreCampaña, c.Id })
                .ToListAsync();

            var campañasDict = campañasExistentes
                .ToDictionary(c => c.NombreCampaña, c => c.Id);

            // Validar que todas las campañas existan
            var campañasNoEncontradas = nombresCampañas
                .Where(nombre => !campañasDict.ContainsKey(nombre))
                .ToList();

            if (campañasNoEncontradas.Any())
            {
                return BadRequest(new
                {
                    error = $"Las siguientes campañas no existen: {string.Join(", ", campañasNoEncontradas)}"
                });
            }

            // ✅ LLAMAR AL SERVICIO CON 2 PARÁMETROS
            var resultado = await _usuariosService.CargaMasivaUsuariosAsync(usuariosDto, campañasDict);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // GET: api/usuarios/descargar-plantilla
    [HttpGet("descargar-plantilla")]
    public async Task<IActionResult> DescargarPlantilla()
    {
        try
        {
            Console.WriteLine("📥 Solicitud de descarga de plantilla de usuarios recibida");

            var plantillaBytes = await _usuariosService.GenerarPlantillaCargaMasivaAsync();

            if (plantillaBytes == null || plantillaBytes.Length == 0)
            {
                return BadRequest(new { error = "No se pudo generar la plantilla" });
            }

            Console.WriteLine($"✅ Plantilla de usuarios generada: {plantillaBytes.Length} bytes");

            return File(plantillaBytes,
                       "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                       $"Plantilla_Carga_Masiva_Usuarios.xlsx");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error en endpoint DescargarPlantilla: {ex.Message}");
            return StatusCode(500, new { error = "Error interno al generar plantilla", detalle = ex.Message });
        }
    }

    // GET: api/usuarios/usuarios-combinados
    [HttpGet("usuarios-combinados")]
    public async Task<IActionResult> ObtenerUsuariosCombinados()
    {
        try
        {
            var usuariosCombinados = await _usuariosService.ObtenerUsuariosCombinadosAsync();
            return Ok(usuariosCombinados);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error en ObtenerUsuariosCombinados: {ex.Message}");
            return BadRequest(new { error = "Error al obtener usuarios combinados" });
        }
    }
}