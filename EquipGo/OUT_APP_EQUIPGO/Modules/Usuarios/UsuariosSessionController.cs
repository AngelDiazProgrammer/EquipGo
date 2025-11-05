using Interface.Services.Usuarios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OUT_DOMAIN_EQUIPGO.Entities.Seguridad;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Usuarios;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class UsuariosSessionController : ControllerBase
{
    private readonly IUsuariosSessionService _usuariosSessionService;

    public UsuariosSessionController(IUsuariosSessionService usuariosSessionService)
    {
        _usuariosSessionService = usuariosSessionService;
    }

    // GET: api/usuariossession
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var usuarios = await _usuariosSessionService.GetAllAsync();
            var usuariosDto = usuarios.Select(u => new UsuarioSessionDto
            {
                Id = u.Id,
                IdTipodocumento = u.IdTipodocumento,
                TipoDocumento = u.TipoDocumento?.NombreDocumento,
                NumeroDocumento = u.NumeroDocumento,
                Nombre = u.Nombre,
                Apellido = u.Apellido,
                IdEstado = u.IdEstado,
                Estado = u.Estado?.NombreEstado,
                IdRol = u.IdRol,
                Rol = u.Rol?.NombreRol,
                IdSede = u.IdSede,
                Sede = u.Sede?.NombreSede,
                FechaCreacion = u.FechaCreacion,
                UltimaModificacion = u.UltimaModificacion
            }).ToList();

            return Ok(usuariosDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // GET: api/usuariossession/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var usuario = await _usuariosSessionService.GetByIdAsync(id);
            if (usuario == null)
                return NotFound(new { error = "Usuario no encontrado" });

            var usuarioDto = new UsuarioSessionDto
            {
                Id = usuario.Id,
                IdTipodocumento = usuario.IdTipodocumento,
                TipoDocumento = usuario.TipoDocumento?.NombreDocumento,
                NumeroDocumento = usuario.NumeroDocumento,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                IdEstado = usuario.IdEstado,
                Estado = usuario.Estado?.NombreEstado,
                IdRol = usuario.IdRol,
                Rol = usuario.Rol?.NombreRol,
                IdSede = usuario.IdSede,
                Sede = usuario.Sede?.NombreSede,
                FechaCreacion = usuario.FechaCreacion,
                UltimaModificacion = usuario.UltimaModificacion
            };

            return Ok(usuarioDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // POST: api/usuariossession
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UsuarioSessionCrearDto usuarioDto)
    {
        try
        {
            if (usuarioDto == null)
                return BadRequest(new { error = "Los datos del usuario son requeridos" });

            var usuario = new UsuariosSession
            {
                IdTipodocumento = usuarioDto.IdTipodocumento,
                NumeroDocumento = usuarioDto.NumeroDocumento,
                Nombre = usuarioDto.Nombre,
                Apellido = usuarioDto.Apellido,
                Contraseña = usuarioDto.Contraseña, // Considerar encriptación
                IdEstado = usuarioDto.IdEstado,
                IdRol = usuarioDto.IdRol,
                IdSede = usuarioDto.IdSede
            };

            var usuarioId = await _usuariosSessionService.CreateAsync(usuario);

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

    // PUT: api/usuariossession/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UsuarioSessionActualizarDto usuarioDto)
    {
        try
        {
            if (usuarioDto == null || id != usuarioDto.Id)
                return BadRequest(new { error = "Datos inválidos" });

            var usuarioExistente = await _usuariosSessionService.GetByIdAsync(id);
            if (usuarioExistente == null)
                return NotFound(new { error = "Usuario no encontrado" });

            // Actualizar propiedades
            usuarioExistente.IdTipodocumento = usuarioDto.IdTipodocumento;
            usuarioExistente.NumeroDocumento = usuarioDto.NumeroDocumento;
            usuarioExistente.Nombre = usuarioDto.Nombre;
            usuarioExistente.Apellido = usuarioDto.Apellido;
            usuarioExistente.IdEstado = usuarioDto.IdEstado;
            usuarioExistente.IdRol = usuarioDto.IdRol;
            usuarioExistente.IdSede = usuarioDto.IdSede;

            var actualizado = await _usuariosSessionService.UpdateAsync(usuarioExistente);
            if (!actualizado)
                return StatusCode(500, new { error = "Error al actualizar el usuario" });

            return Ok(new { message = "Usuario actualizado correctamente" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // DELETE: api/usuariossession/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var eliminado = await _usuariosSessionService.DeleteAsync(id);
            if (!eliminado)
                return NotFound(new { error = "Usuario no encontrado" });

            return Ok(new { message = "Usuario eliminado correctamente" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // POST: api/usuariossession/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.NumeroDocumento) || string.IsNullOrEmpty(loginDto.Contraseña))
                return BadRequest(new { error = "Número de documento y contraseña son requeridos" });

            var usuario = await _usuariosSessionService.AuthenticateAsync(loginDto.NumeroDocumento, loginDto.Contraseña);
            if (usuario == null)
                return Unauthorized(new { error = "Credenciales inválidas o usuario inactivo" });

            var usuarioDto = new UsuarioSessionDto
            {
                Id = usuario.Id,
                IdTipodocumento = usuario.IdTipodocumento,
                TipoDocumento = usuario.TipoDocumento?.NombreDocumento,
                NumeroDocumento = usuario.NumeroDocumento,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                IdEstado = usuario.IdEstado,
                Estado = usuario.Estado?.NombreEstado,
                IdRol = usuario.IdRol,
                Rol = usuario.Rol?.NombreRol,
                IdSede = usuario.IdSede,
                Sede = usuario.Sede?.NombreSede,
                FechaCreacion = usuario.FechaCreacion,
                UltimaModificacion = usuario.UltimaModificacion
            };

            return Ok(new
            {
                message = "Login exitoso",
                usuario = usuarioDto
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // PUT: api/usuariossession/{id}/cambiar-contraseña
    [HttpPut("{id}/cambiar-contraseña")]
    public async Task<IActionResult> ChangePassword(int id, [FromBody] CambioContraseñaDto cambioDto)
    {
        try
        {
            if (cambioDto == null || id != cambioDto.UsuarioId)
                return BadRequest(new { error = "Datos inválidos" });

            if (cambioDto.NuevaContraseña != cambioDto.ConfirmarContraseña)
                return BadRequest(new { error = "La nueva contraseña y la confirmación no coinciden" });

            // Verificar contraseña actual
            var usuario = await _usuariosSessionService.GetByIdAsync(id);
            if (usuario == null)
                return NotFound(new { error = "Usuario no encontrado" });

            if (usuario.Contraseña != cambioDto.ContraseñaActual) // Considerar encriptación
                return BadRequest(new { error = "La contraseña actual es incorrecta" });

            var cambiado = await _usuariosSessionService.ChangePasswordAsync(id, cambioDto.NuevaContraseña);
            if (!cambiado)
                return StatusCode(500, new { error = "Error al cambiar la contraseña" });

            return Ok(new { message = "Contraseña cambiada correctamente" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // GET: api/usuariossession/form-data
    [HttpGet("form-data")]
    public async Task<IActionResult> GetFormData()
    {
        try
        {
            var formData = await _usuariosSessionService.GetFormDataAsync();
            return Ok(formData);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}