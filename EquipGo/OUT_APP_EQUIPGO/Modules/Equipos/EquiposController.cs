// Controllers/EquiposController.cs
using Interface.Services.Active_Directory;
using Interface.Services.Equipos;
using Interface.Services.Estados;
using Interface.Services.Proveedores;
using Interface.Services.Sedes;
using Interface.Services.SubEstado;
using Interface.Services.TipoDispositivos;
using Interface.Services.Usuarios;
using Microsoft.AspNetCore.Mvc;
using OUT_DOMAIN_EQUIPGO.Entities.Procesos;
using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Equipo;

[ApiController]
[Route("api/[controller]")]
public class EquiposController : ControllerBase
{
    private readonly IEquipoService _equipoService;
    private readonly IUsuariosInformacionService _usuariosInformacionService;
    private readonly IEstadoService _estadoService;
    private readonly ISubEstadoService _subestadoService;
    private readonly ISedesService _sedesService;
    private readonly ITipoDispositivosService _tipoDispositivosService;
    private readonly IProveedoresService _proveedoresService;
    private readonly IActiveDirectoryService _activeDirectoryService;

    public EquiposController(
        IEquipoService equipoService,
        IUsuariosInformacionService usuariosInformacionService,
        IEstadoService estadoService,
        ISubEstadoService subEstadoService,
        ISedesService sedesService,
        ITipoDispositivosService tipoDispositivosService,
        IProveedoresService proveedoresService,
        IActiveDirectoryService activeDirectoryService)
    {
        _equipoService = equipoService;
        _usuariosInformacionService = usuariosInformacionService;
        _estadoService = estadoService;
        _subestadoService = subEstadoService;
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

    // UpSert Asignacion de equipos
    [HttpPost("Admin/ConUsuario")]
    public async Task<IActionResult> GuardarEquipoConUsuario([FromBody] EquipoUsuarioDto dto)
    {
        try
        {
            // --- LÓGICA PARA EL USUARIO (UPSERT) ---
            int usuarioId;
            // ✅ CAMBIO: Usamos el servicio de usuarios para buscar por documento
            var usuarioExistente = await _usuariosInformacionService.ConsultarUsuarioPorDocumentoAsync(dto.NumeroDocumento);

            if (usuarioExistente != null)
            {
                // Si el usuario existe, lo ACTUALIZAMOS con los nuevos datos
                Console.WriteLine($"✅ Usuario existente encontrado con ID: {usuarioExistente.Id}. Actualizando...");

                usuarioExistente.IdTipodocumento = dto.IdTipoDocumento;
                usuarioExistente.Nombres = dto.Nombres;
                usuarioExistente.Apellidos = dto.Apellidos;
                usuarioExistente.IdArea = dto.IdArea;
                usuarioExistente.IdCampaña = dto.IdCampaña;
                usuarioExistente.UltimaModificacion = DateTime.UtcNow;

                // ✅ CAMBIO: Usamos el servicio de usuarios para actualizar
                await _usuariosInformacionService.ActualizarUsuarioAsync(usuarioExistente);
                usuarioId = usuarioExistente.Id;
            }
            else
            {
                // Si el usuario no existe, lo creamos
                Console.WriteLine($"🆕 Creando nuevo usuario con Documento: {dto.NumeroDocumento}...");
                var nuevoUsuario = new UsuariosInformacion
                {
                    IdTipodocumento = dto.IdTipoDocumento,
                    NumeroDocumento = dto.NumeroDocumento,
                    Nombres = dto.Nombres,
                    Apellidos = dto.Apellidos,
                    IdArea = dto.IdArea,
                    IdCampaña = dto.IdCampaña,
                    IdEstado = 1, // Estado por defecto "Activo"
                    FechaCreacion = DateTime.UtcNow,
                    UltimaModificacion = DateTime.UtcNow
                };
                // ✅ CAMBIO: Usamos el servicio de usuarios para crear
                usuarioId = await _usuariosInformacionService.CrearUsuarioAsync(nuevoUsuario);
                Console.WriteLine($"🆕 Nuevo usuario creado con ID: {usuarioId}. Documento: {dto.NumeroDocumento}");
            }

            // --- LÓGICA PARA EL EQUIPO (UPSERT) ---
            var equipoExistente = await _equipoService.ConsultarPorSerialAsync(dto.Serial);

            if (equipoExistente != null)
            {
                // Si el equipo existe, lo actualizamos
                Console.WriteLine($"✅ Equipo existente encontrado con ID: {equipoExistente.Id}. Serial: {dto.Serial}. Actualizando...");

                equipoExistente.Marca = dto.Marca;
                equipoExistente.Modelo = dto.Modelo;
                equipoExistente.CodigoBarras = string.IsNullOrWhiteSpace(dto.CodigoBarras)
                    ? equipoExistente.CodigoBarras
                    : dto.CodigoBarras;
                equipoExistente.IdUsuarioInfo = usuarioId;
                equipoExistente.IdEstado = dto.IdEstado ?? equipoExistente.IdEstado;
                equipoExistente.IdSubEstado = dto.IdSubEstado ?? equipoExistente.IdSubEstado; // ✅ Agregado subestado
                equipoExistente.IdSede = dto.IdSede ?? equipoExistente.IdSede;
                equipoExistente.IdTipoDispositivo = dto.IdTipoDispositivo ?? equipoExistente.IdTipoDispositivo;
                equipoExistente.IdProveedor = dto.IdProveedor ?? equipoExistente.IdProveedor;
                equipoExistente.SistemaOperativo = dto.SistemaOperativo ?? equipoExistente.SistemaOperativo;
                equipoExistente.MacEquipo = dto.MacEquipo ?? equipoExistente.MacEquipo;
                equipoExistente.UltimaModificacion = DateTime.UtcNow;

                await _equipoService.ActualizarEquipoAsync(equipoExistente);

                return Ok(new { message = "Equipo y usuario actualizados correctamente." });
            }
            else
            {
                // Si el equipo no existe, lo creamos
                Console.WriteLine($"🆕 Creando nuevo equipo con Serial: {dto.Serial}...");

                var nuevoEquipo = new OUT_DOMAIN_EQUIPGO.Entities.Configuracion.Equipos
                {
                    Marca = dto.Marca,
                    Modelo = dto.Modelo,
                    Serial = dto.Serial,
                    CodigoBarras = string.IsNullOrWhiteSpace(dto.CodigoBarras)
                        ? $"SIN-ASIGNAR-{Guid.NewGuid()}"
                        : dto.CodigoBarras,
                    IdUsuarioInfo = usuarioId,
                    IdEstado = dto.IdEstado,
                    IdSubEstado = dto.IdSubEstado, // ✅ Agregado subestado
                    IdSede = dto.IdSede,
                    IdTipoDispositivo = dto.IdTipoDispositivo,
                    IdProveedor = dto.IdProveedor,
                    SistemaOperativo = dto.SistemaOperativo,
                    MacEquipo = dto.MacEquipo,
                    FechaCreacion = DateTime.UtcNow,
                    UltimaModificacion = DateTime.UtcNow
                };

                await _equipoService.CrearEquipoAsync(nuevoEquipo);
                return Ok(new { message = "Equipo y usuario creados correctamente." });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error en GuardarEquipoConUsuario: {ex.Message}");
            return BadRequest(new { error = ex.Message });
        }
    }


    //Cargue Masivo
    [HttpPost("admin/carga-masiva")]
    public async Task<ActionResult<ResultadoCargaMasivaDto>> CargaMasivaEquipos([FromBody] List<CrearEquipoDto> equiposDto)
    {
        try
        {
            var resultado = await _equipoService.CargaMasivaEquiposAsync(equiposDto);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("admin/descargar-plantilla")]
    public async Task<IActionResult> DescargarPlantilla()
    {
        try
        {
            Console.WriteLine("📥 Solicitud de descarga de plantilla recibida");

            // 🔥 Usar await para liberar el thread
            var plantillaBytes = await _equipoService.GenerarPlantillaCargaMasivaAsync();

            if (plantillaBytes == null || plantillaBytes.Length == 0)
            {
                return BadRequest(new { error = "No se pudo generar la plantilla" });
            }

            Console.WriteLine($"✅ Plantilla generada: {plantillaBytes.Length} bytes");

            // 🔥 Forzar garbage collection después de generar archivo grande
            GC.Collect();
            GC.WaitForPendingFinalizers();

            return File(plantillaBytes,
                       "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                       $"Plantilla_Carga_Masiva_Equipos.xlsx");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error en endpoint DescargarPlantilla: {ex.Message}");

            return StatusCode(500, new
            {
                error = "Error interno al generar plantilla",
                detalle = ex.Message
            });
        }
    }

    //Consultar usuario por nombres en la base local
    [HttpGet("admin/usuario-por-nombre")]
    public async Task<IActionResult> ConsultarUsuarioPorNombre([FromQuery] string nombres, [FromQuery] string apellidos)
    {
        try
        {
            if (string.IsNullOrEmpty(nombres) || string.IsNullOrEmpty(apellidos))
            {
                return BadRequest(new { error = "Los nombres y apellidos son requeridos." });
            }

            // ✅ CAMBIO: Usamos el servicio de usuarios para buscar por nombre
            var usuario = await _usuariosInformacionService.ConsultarUsuarioPorNombreAsync(nombres, apellidos);

            if (usuario == null)
            {
                return NotFound(new { message = "Usuario no encontrado en la base de datos local." });
            }

            // ❌ Si el usuario está inactivo, no devolverlo
            if (usuario.IdEstado != 1)
            {
                return BadRequest(new { message = "El usuario existe pero está inactivo." });
            }

            // Devolvemos un DTO con la información relevante para el formulario
            return Ok(new
            {
                usuario.IdTipodocumento,
                usuario.NumeroDocumento,
                usuario.IdArea,
                usuario.IdCampaña
            });
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

    [HttpGet("admin/usuarios-combinados")]
    public async Task<IActionResult> ObtenerUsuariosCombinados()
    {
        try
        {
            var usuariosCombinados = await _usuariosInformacionService.ObtenerUsuariosCombinadosAsync();
            return Ok(usuariosCombinados);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error en ObtenerUsuariosCombinados: {ex.Message}");
            return BadRequest(new { error = "Error al obtener usuarios combinados" });
        }
    }

    // Modificar el endpoint existente para incluir usuarios combinados
    [HttpGet("admin/form-data")]
    public async Task<IActionResult> ObtenerFormData()
    {
        try
        {
            var usuariosCombinados = await _usuariosInformacionService.ObtenerUsuariosCombinadosAsync();
            var estados = await _estadoService.ObtenerTodasAsync();
            var equiposPersonales = await _equipoService.ObtenerEquiposPersonalesAsync();
            var sedes = await _sedesService.ObtenerTodasAsync();
            var tiposDispositivo = await _tipoDispositivosService.ObtenerTodasAsync();
            var proveedores = await _proveedoresService.ObtenerTodasAsync();
            var tiposDocumento = await _equipoService.ObtenerTipoDocumentoAsync();
            var areas = await _equipoService.ObtenerAreasAsync();
            var campanas = await _equipoService.ObtenerCampañasAsync();

            // 🔍 DEBUG: Logs para subestados
            Console.WriteLine("🔍 Intentando obtener subestados...");
            var subestados = await _subestadoService.ObtenerTodosAsync();
            Console.WriteLine($"📊 SubEstados obtenidos: {subestados?.Count ?? 0}");

            if (subestados != null && subestados.Any())
            {
                Console.WriteLine("✅ SubEstados encontrados:");
                foreach (var sub in subestados)
                {
                    Console.WriteLine($"   - ID: {sub.Id}, Nombre: {sub.NombreSubEstado}");
                }
            }
            else
            {
                Console.WriteLine("⚠️ No se encontraron subestados o la lista es null");
            }

            // ✅ SOLUCIÓN: Crear lista con camelCase para JavaScript
            var subEstadosResponse = subestados?.Select(s => new
            {
                id = s.Id,
                nombreSubEstado = s.NombreSubEstado
            }).ToList();

            Console.WriteLine($"📦 Response preparado con {subEstadosResponse?.Count ?? 0} subestados");

            return Ok(new
            {
                usuarios = usuariosCombinados,
                estados,
                equiposPersonales,
                sedes,
                tiposDispositivo,
                proveedores,
                tiposDocumento,
                areas,
                campanas,
                subEstados = subEstadosResponse // ✅ Puede ser null sin problema
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error en ObtenerFormData: {ex.Message}");
            Console.WriteLine($"   StackTrace: {ex.StackTrace}");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("admin/asignar-usuario")]
    public async Task<IActionResult> AsignarUsuarioAEquipo([FromBody] AsignarUsuarioRequestDto request)
    {
        try
        {
            Console.WriteLine($"🔄 Asignando usuario al equipo {request.EquipoId}");

            if (request.EquipoId <= 0)
                return BadRequest(new { error = "ID de equipo inválido" });

            if (string.IsNullOrWhiteSpace(request.NumeroDocumento))
                return BadRequest(new { error = "El número de documento es obligatorio" });

            // Buscar equipo
            var equipo = await _equipoService.ObtenerPorIdAsync(request.EquipoId);
            if (equipo == null)
                return NotFound(new { error = "Equipo no encontrado" });

            int usuarioId;

            // Buscar usuario existente por documento
            var usuarioExistente = await _usuariosInformacionService.ConsultarUsuarioPorDocumentoAsync(request.NumeroDocumento);

            // --------------------------------------------------------------------
            //   🔥 CONTROL DE USUARIO INACTIVO
            // --------------------------------------------------------------------
            if (usuarioExistente != null && usuarioExistente.IdEstado != 1)
            {
                Console.WriteLine("❌ Usuario encontrado pero INACTIVO. No se puede asignar.");
                return BadRequest(new
                {
                    error = "El usuario existe pero no está activo",
                    usuario_inactivo = true
                });
            }

            // --------------------------------------------------------------------
            //   🟦 UPSERT (actualizar o crear)
            // --------------------------------------------------------------------
            if (usuarioExistente != null)
            {
                Console.WriteLine($"✅ Usuario existente encontrado: {usuarioExistente.Nombres} {usuarioExistente.Apellidos}");

                usuarioExistente.IdTipodocumento = request.IdTipoDocumento;
                usuarioExistente.Nombres = request.Nombres;
                usuarioExistente.Apellidos = request.Apellidos;
                usuarioExistente.IdArea = request.IdArea;
                usuarioExistente.IdCampaña = request.IdCampaña;
                usuarioExistente.UltimaModificacion = DateTime.UtcNow;

                await _usuariosInformacionService.ActualizarUsuarioAsync(usuarioExistente);
                usuarioId = usuarioExistente.Id;
            }
            else
            {
                // Crear usuario nuevo
                Console.WriteLine($"🆕 Creando nuevo usuario: {request.Nombres} {request.Apellidos}");

                var nuevoUsuario = new UsuariosInformacion
                {
                    IdTipodocumento = request.IdTipoDocumento,
                    NumeroDocumento = request.NumeroDocumento,
                    Nombres = request.Nombres,
                    Apellidos = request.Apellidos,
                    IdArea = request.IdArea,
                    IdCampaña = request.IdCampaña,
                    IdEstado = 1, // Activo
                    FechaCreacion = DateTime.UtcNow,
                    UltimaModificacion = DateTime.UtcNow
                };

                usuarioId = await _usuariosInformacionService.CrearUsuarioAsync(nuevoUsuario);
            }

            // --------------------------------------------------------------------
            //   🟩 ASIGNAR USUARIO AL EQUIPO
            // --------------------------------------------------------------------
            var actualizado = await _equipoService.AsignarUsuarioAEquipoAsync(request.EquipoId, usuarioId);

            if (!actualizado)
                return BadRequest(new { error = "No se pudo asignar el usuario al equipo" });

            Console.WriteLine($"✅ Usuario {usuarioId} asignado al equipo {request.EquipoId}");

            return Ok(new
            {
                message = "Usuario asignado correctamente al equipo",
                usuarioId = usuarioId
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error en AsignarUsuarioAEquipo: {ex.Message}");
            return BadRequest(new { error = ex.Message });
        }
    }


    [HttpPost("admin/desasignar-usuario/{equipoId}")]
    public async Task<IActionResult> DesasignarUsuarioDeEquipo(int equipoId)
    {
        try
        {
            Console.WriteLine($"🔄 Desasignando usuario del equipo {equipoId}");

            var desasignado = await _equipoService.DesasignarUsuarioDeEquipoAsync(equipoId);

            if (!desasignado)
                return NotFound(new { error = "Equipo no encontrado o sin usuario asignado" });

            return Ok(new { message = "Usuario desasignado correctamente del equipo" });

        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error en DesasignarUsuarioDeEquipo: {ex.Message}");
            return BadRequest(new { error = ex.Message });
        }
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