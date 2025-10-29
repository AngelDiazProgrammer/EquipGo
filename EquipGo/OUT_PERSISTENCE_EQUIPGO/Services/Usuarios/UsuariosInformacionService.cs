using Interface;
using Interface.Services.Usuarios;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using OUT_DOMAIN_EQUIPGO.Entities.Procesos;
using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Usuarios;
using OUT_PERSISTENCE_EQUIPGO.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Interface.Services.Active_Directory;

namespace OUT_PERSISTENCE_EQUIPGO.Services.Usuarios
{
    public class UsuariosInformacionService : IUsuariosInformacionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly EquipGoDbContext _context;
        private readonly IActiveDirectoryService _activeDirectoryService;

        public UsuariosInformacionService(IUnitOfWork unitOfWork, EquipGoDbContext context, IActiveDirectoryService activeDirectoryService)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _activeDirectoryService = activeDirectoryService;
        }

        #region Métodos CRUD Básicos

        public async Task<List<UsuarioInformacionDto>> ObtenerTodosLosUsuariosInformacionAsync()
        {
            var usuarios = await _unitOfWork.UsuariosInformacion
                                           .Query()
                                           .Include(u => u.IdTipodocumentoNavigation)
                                           .Include(u => u.IdAreaNavigation)
                                           .Include(u => u.IdCampañaNavigation)
                                           .Include(u => u.Estado)
                                           .AsNoTracking()
                                           .ToListAsync();

            var lista = usuarios.Select(u => new UsuarioInformacionDto
            {
                Id = u.Id,
                TipoDocumento = u.IdTipodocumentoNavigation?.NombreDocumento ?? "",
                NumeroDocumento = long.TryParse(u.NumeroDocumento, out var numero) ? numero : 0,
                Nombres = u.Nombres,
                Apellidos = u.Apellidos,
                Area = u.IdAreaNavigation?.NombreArea ?? "",
                Campana = u.IdCampañaNavigation?.NombreCampaña ?? "",
                Estado = u.Estado?.NombreEstado ?? "",
                FechaCreacion = u.FechaCreacion,
                UltimaModificacion = u.UltimaModificacion
            }).ToList();

            return lista;
        }

        public async Task<UsuarioInformacionDto?> ObtenerPorIdAsync(int id)
        {
            var usuario = await _unitOfWork.UsuariosInformacion.Query()
                .Include(u => u.IdTipodocumentoNavigation)
                .Include(u => u.IdAreaNavigation)
                .Include(u => u.IdCampañaNavigation)
                .Include(u => u.Estado)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
                return null;

            return new UsuarioInformacionDto
            {
                Id = usuario.Id,
                TipoDocumento = usuario.IdTipodocumentoNavigation?.NombreDocumento ?? "",
                NumeroDocumento = long.TryParse(usuario.NumeroDocumento, out var numero) ? numero : 0,
                Nombres = usuario.Nombres,
                Apellidos = usuario.Apellidos,
                Area = usuario.IdAreaNavigation?.NombreArea ?? "",
                Campana = usuario.IdCampañaNavigation?.NombreCampaña ?? "",
                Estado = usuario.Estado?.NombreEstado ?? "",
                FechaCreacion = usuario.FechaCreacion,
                UltimaModificacion = usuario.UltimaModificacion,
                // Propiedades para edición
                IdTipoDocumento = usuario.IdTipodocumento,
                IdArea = usuario.IdArea,
                IdCampaña = usuario.IdCampaña,
                IdEstado = usuario.IdEstado
            };
        }


        public async Task<UsuariosInformacion?> ConsultarUsuarioPorDocumentoAsync(string documento)
        {
            if (string.IsNullOrEmpty(documento))
                return null;

            return await _unitOfWork.UsuariosInformacion.Query()
                .FirstOrDefaultAsync(u => u.NumeroDocumento == documento);
        }

        public async Task<UsuariosInformacion?> ConsultarUsuarioPorNombreAsync(string nombres, string apellidos)
        {
            if (string.IsNullOrEmpty(nombres) || string.IsNullOrEmpty(apellidos))
                return null;

            return await _unitOfWork.UsuariosInformacion.Query()
                .FirstOrDefaultAsync(u =>
                    u.Nombres.Trim().ToLower() == nombres.Trim().ToLower() &&
                    u.Apellidos.Trim().ToLower() == apellidos.Trim().ToLower());
        }

        public async Task<int> CrearUsuarioAsync(UsuariosInformacion usuario)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));

            // Validar que el tipo de documento existe
            var tipoDoc = await _unitOfWork.TipoDocumento.Query()
                .FirstOrDefaultAsync(t => t.Id == usuario.IdTipodocumento);
            if (tipoDoc == null)
            {
                throw new Exception("El tipo de documento seleccionado no existe.");
            }

            await _unitOfWork.UsuariosInformacion.AddAsync(usuario);
            await _unitOfWork.CompleteAsync();
            return usuario.Id;
        }

        public async Task<bool> ActualizarUsuarioAsync(UsuariosInformacion usuario)
        {
            if (usuario == null)
                return false;

            // La entidad ya está siendo rastreada por el DbContext,
            // así que solo necesitamos llamar a CompleteAsync para guardar los cambios.
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> ActualizarUsuarioAdminAsync(int id, UsuarioCrearDto dto)
        {
            Console.WriteLine($"🔄 Iniciando actualización del usuario ID: {id}");
            Console.WriteLine($"📦 DTO recibido: {System.Text.Json.JsonSerializer.Serialize(dto)}");

            var usuario = await _unitOfWork.UsuariosInformacion.Query().FirstOrDefaultAsync(u => u.Id == id);
            if (usuario == null)
            {
                Console.WriteLine("❌ Usuario no encontrado");
                return false;
            }

            Console.WriteLine($"🔍 Usuario encontrado: {usuario.Nombres} {usuario.Apellidos}");

            usuario.IdTipodocumento = dto.IdTipoDocumento;
            usuario.NumeroDocumento = dto.NumeroDocumento;
            usuario.Nombres = dto.Nombres;
            usuario.Apellidos = dto.Apellidos;
            usuario.IdArea = dto.IdArea;
            usuario.IdCampaña = dto.IdCampaña;
            usuario.IdEstado = dto.IdEstado;
            usuario.UltimaModificacion = DateTime.UtcNow;

            Console.WriteLine($"✅ Usuario actualizado");

            try
            {
                await _unitOfWork.CompleteAsync();
                Console.WriteLine("💾 Cambios guardados en la base de datos");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al guardar cambios: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> EliminarAsync(int id)
        {
            try
            {
                var usuario = await _context.UsuariosInformacion.FindAsync(id);
                if (usuario == null) return false;

                _context.UsuariosInformacion.Remove(usuario);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al eliminar usuario: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Cargue Masiva
        //public async Task<ResultadoCargaMasivaDto> CargaMasivaUsuariosAsyncSinuso(List<UsuarioCrearDto> usuariosDto)
        //{
        //    var resultado = new ResultadoCargaMasivaDto
        //    {
        //        TotalRegistros = usuariosDto.Count
        //    };

        //    if (usuariosDto == null || !usuariosDto.Any())
        //    {
        //        resultado.Mensaje = "La lista de usuarios está vacía";
        //        return resultado;
        //    }

        //    using var transaction = await _context.Database.BeginTransactionAsync();

        //    try
        //    {
        //        // 🔥 LISTA PARA VERIFICAR DUPLICADOS DENTRO DEL MISMO ARCHIVO
        //        var documentosProcesados = new HashSet<string>();
        //        var hayErrores = false;

        //        // 🔥 PRECARGAR CAMPAÑAS PARA OPTIMIZACIÓN
        //        var nombresCampañas = usuariosDto
        //            .Where(u => !string.IsNullOrEmpty(u.NombreCampaña))
        //            .Select(u => u.NombreCampaña.Trim())
        //            .Distinct()
        //            .ToList();

        //        var campañasExistentes = await _context.Campañas
        //            .Where(c => nombresCampañas.Contains(c.NombreCampaña))
        //            .ToDictionaryAsync(c => c.NombreCampaña, c => c.Id);

        //        for (int i = 0; i < usuariosDto.Count; i++)
        //        {
        //            var usuarioDto = usuariosDto[i];

        //            try
        //            {
        //                // 🔥 Validación 1: CAMPAÑA obligatoria por nombre
        //                if (string.IsNullOrWhiteSpace(usuarioDto.NombreCampaña))
        //                {
        //                    resultado.Errores.Add(new ErrorCargaMasivaDto
        //                    {
        //                        IndiceFila = i + 1,
        //                        Nombres = usuarioDto.Nombres ?? "",
        //                        Apellidos = usuarioDto.Apellidos ?? "",
        //                        NumeroDocumento = usuarioDto.NumeroDocumento ?? "",
        //                        Error = "El nombre de la campaña es obligatorio"
        //                    });
        //                    hayErrores = true;
        //                    continue;
        //                }

        //                var nombreCampaña = usuarioDto.NombreCampaña.Trim();

        //                // 🔥 Validación 2: Verificar si la campaña existe
        //                if (!campañasExistentes.TryGetValue(nombreCampaña, out var idCampaña))
        //                {
        //                    resultado.Errores.Add(new ErrorCargaMasivaDto
        //                    {
        //                        IndiceFila = i + 1,
        //                        Nombres = usuarioDto.Nombres ?? "",
        //                        Apellidos = usuarioDto.Apellidos ?? "",
        //                        NumeroDocumento = usuarioDto.NumeroDocumento ?? "",
        //                        Error = $"La campaña '{nombreCampaña}' no existe en el sistema"
        //                    });
        //                    hayErrores = true;
        //                    continue;
        //                }

        //                // 🔥 Validación 3: Verificar duplicados dentro del mismo archivo
        //                var documento = usuarioDto.NumeroDocumento?.Trim();
        //                if (!string.IsNullOrEmpty(documento))
        //                {
        //                    if (documentosProcesados.Contains(documento))
        //                    {
        //                        resultado.Errores.Add(new ErrorCargaMasivaDto
        //                        {
        //                            IndiceFila = i + 1,
        //                            Nombres = usuarioDto.Nombres,
        //                            Apellidos = usuarioDto.Apellidos,
        //                            NumeroDocumento = documento,
        //                            Error = "Número de documento duplicado dentro del mismo archivo"
        //                        });
        //                        hayErrores = true;
        //                        continue;
        //                    }
        //                    documentosProcesados.Add(documento);
        //                }

        //                // 🔥 Validación 4: Verificar si el documento ya existe en la base de datos
        //                if (!string.IsNullOrEmpty(documento))
        //                {
        //                    var documentoExistente = await _unitOfWork.UsuariosInformacion.Query()
        //                        .AnyAsync(u => u.NumeroDocumento == documento);

        //                    if (documentoExistente)
        //                    {
        //                        resultado.Errores.Add(new ErrorCargaMasivaDto
        //                        {
        //                            IndiceFila = i + 1,
        //                            Nombres = usuarioDto.Nombres,
        //                            Apellidos = usuarioDto.Apellidos,
        //                            NumeroDocumento = documento,
        //                            Error = "Ya existe un usuario con este número de documento en el sistema"
        //                        });
        //                        hayErrores = true;
        //                        continue;
        //                    }
        //                }

        //                // 🔥 SOLO si no hay errores, crear usuario
        //                var usuario = new UsuariosInformacion
        //                {
        //                    IdTipodocumento = usuarioDto.IdTipoDocumento > 0 ? usuarioDto.IdTipoDocumento : null,
        //                    NumeroDocumento = documento,
        //                    Nombres = usuarioDto.Nombres?.Trim(),
        //                    Apellidos = usuarioDto.Apellidos?.Trim(),
        //                    IdArea = usuarioDto.IdArea > 0 ? usuarioDto.IdArea : null,
        //                    IdCampaña = idCampaña, // ✅ Usamos el ID encontrado por el nombre
        //                    IdEstado = 1,
        //                    FechaCreacion = DateTime.UtcNow,
        //                    UltimaModificacion = DateTime.UtcNow
        //                };

        //                await _unitOfWork.UsuariosInformacion.AddAsync(usuario);
        //                resultado.RegistrosExitosos++;
        //            }
        //            catch (Exception ex)
        //            {
        //                resultado.Errores.Add(new ErrorCargaMasivaDto
        //                {
        //                    IndiceFila = i + 1,
        //                    Nombres = usuarioDto.Nombres ?? "",
        //                    Apellidos = usuarioDto.Apellidos ?? "",
        //                    NumeroDocumento = usuarioDto.NumeroDocumento ?? "",
        //                    Error = $"Error interno: {ex.Message}"
        //                });
        //                hayErrores = true;
        //            }
        //        }

        //        resultado.RegistrosFallidos = resultado.Errores.Count;

        //        // 🔥 DECISIÓN CRÍTICA: Commit o Rollback
        //        if (resultado.RegistrosExitosos > 0 && !hayErrores)
        //        {
        //            await _unitOfWork.CompleteAsync();
        //            await transaction.CommitAsync();
        //            resultado.Mensaje = $"Carga masiva completada exitosamente: {resultado.RegistrosExitosos} usuarios registrados";
        //        }
        //        else if (resultado.RegistrosExitosos > 0 && hayErrores)
        //        {
        //            await transaction.RollbackAsync();
        //            resultado.RegistrosExitosos = 0;
        //            resultado.Mensaje = "❌ Carga cancelada: Se detectaron errores en el archivo. No se registró ningún usuario. Revise los detalles abajo.";
        //        }
        //        else
        //        {
        //            await transaction.RollbackAsync();
        //            resultado.Mensaje = "No se pudo procesar ningún registro. Revise los errores.";
        //        }

        //        return resultado;
        //    }
        //    catch (Exception ex)
        //    {
        //        await transaction.RollbackAsync();

        //        resultado.Mensaje = $"Error general en la carga masiva: {ex.Message}";
        //        resultado.RegistrosExitosos = 0;
        //        resultado.RegistrosFallidos = resultado.TotalRegistros;

        //        return resultado;
        //    }
        //}

        public async Task<ResultadoCargaMasivaDto> CargaMasivaUsuariosAsync(List<UsuarioCrearDto> usuariosDto,Dictionary<string, int> campañasExistentes)
        {
            var resultado = new ResultadoCargaMasivaDto
            {
                TotalRegistros = usuariosDto.Count
            };

            if (usuariosDto == null || !usuariosDto.Any())
            {
                resultado.Mensaje = "La lista de usuarios está vacía";
                return resultado;
            }

            try
            {
                var documentosProcesados = new HashSet<string>();
                var hayErrores = false;
                var usuariosCreados = new List<UsuariosInformacion>();

                // 🔥 PRECARGAR DOCUMENTOS EXISTENTES EN BD (MÁS ROBUSTO)
                var documentosEnArchivo = usuariosDto
                    .Where(u => !string.IsNullOrEmpty(u.NumeroDocumento))
                    .Select(u => u.NumeroDocumento.Trim())
                    .Distinct()
                    .ToList();

                Console.WriteLine($"🔍 Buscando {documentosEnArchivo.Count} documentos en BD...");

                var documentosExistentesEnBD = await _unitOfWork.UsuariosInformacion.Query()
                    .Where(u => documentosEnArchivo.Contains(u.NumeroDocumento))
                    .Select(u => new { u.NumeroDocumento, u.Id })
                    .ToDictionaryAsync(u => u.NumeroDocumento, u => u.Id);

                Console.WriteLine($"🔍 Documentos existentes encontrados: {documentosExistentesEnBD.Count}");

                for (int i = 0; i < usuariosDto.Count; i++)
                {
                    var usuarioDto = usuariosDto[i];

                    try
                    {
                        // Validación 1: CAMPAÑA obligatoria
                        if (string.IsNullOrWhiteSpace(usuarioDto.NombreCampaña))
                        {
                            resultado.Errores.Add(new ErrorCargaMasivaDto
                            {
                                IndiceFila = i + 1,
                                Nombres = usuarioDto.Nombres ?? "",
                                Apellidos = usuarioDto.Apellidos ?? "",
                                NumeroDocumento = usuarioDto.NumeroDocumento ?? "",
                                Error = "El nombre de la campaña es obligatorio"
                            });
                            hayErrores = true;
                            continue;
                        }

                        var nombreCampaña = usuarioDto.NombreCampaña.Trim();

                        // Validación 2: Verificar si la campaña existe
                        if (!campañasExistentes.TryGetValue(nombreCampaña, out var idCampaña))
                        {
                            resultado.Errores.Add(new ErrorCargaMasivaDto
                            {
                                IndiceFila = i + 1,
                                Nombres = usuarioDto.Nombres ?? "",
                                Apellidos = usuarioDto.Apellidos ?? "",
                                NumeroDocumento = usuarioDto.NumeroDocumento ?? "",
                                Error = $"La campaña '{nombreCampaña}' no existe en el sistema"
                            });
                            hayErrores = true;
                            continue;
                        }

                        // Validación 3: Verificar duplicados dentro del mismo archivo
                        var documento = usuarioDto.NumeroDocumento?.Trim();
                        if (!string.IsNullOrEmpty(documento))
                        {
                            if (documentosProcesados.Contains(documento))
                            {
                                resultado.Errores.Add(new ErrorCargaMasivaDto
                                {
                                    IndiceFila = i + 1,
                                    Nombres = usuarioDto.Nombres,
                                    Apellidos = usuarioDto.Apellidos,
                                    NumeroDocumento = documento,
                                    Error = "Número de documento duplicado dentro del mismo archivo"
                                });
                                hayErrores = true;
                                continue;
                            }
                            documentosProcesados.Add(documento);
                        }

                        // 🔥 Validación 4: Verificar si el documento ya existe en la base de datos
                        if (!string.IsNullOrEmpty(documento) && documentosExistentesEnBD.ContainsKey(documento))
                        {
                            resultado.Errores.Add(new ErrorCargaMasivaDto
                            {
                                IndiceFila = i + 1,
                                Nombres = usuarioDto.Nombres,
                                Apellidos = usuarioDto.Apellidos,
                                NumeroDocumento = documento,
                                Error = $"Ya existe un usuario (ID: {documentosExistentesEnBD[documento]}) con este número de documento en el sistema"
                            });
                            hayErrores = true;
                            continue;
                        }

                        // 🔥 SOLO si no hay errores, crear usuario
                        var usuario = new UsuariosInformacion
                        {
                            IdTipodocumento = usuarioDto.IdTipoDocumento > 0 ? usuarioDto.IdTipoDocumento : null,
                            NumeroDocumento = documento,
                            Nombres = usuarioDto.Nombres?.Trim(),
                            Apellidos = usuarioDto.Apellidos?.Trim(),
                            IdArea = usuarioDto.IdArea > 0 ? usuarioDto.IdArea : null,
                            IdCampaña = idCampaña,
                            IdEstado = 1,
                            FechaCreacion = DateTime.UtcNow,
                            UltimaModificacion = DateTime.UtcNow
                        };

                        usuariosCreados.Add(usuario);
                        resultado.RegistrosExitosos++;
                    }
                    catch (Exception ex)
                    {
                        resultado.Errores.Add(new ErrorCargaMasivaDto
                        {
                            IndiceFila = i + 1,
                            Nombres = usuarioDto.Nombres ?? "",
                            Apellidos = usuarioDto.Apellidos ?? "",
                            NumeroDocumento = usuarioDto.NumeroDocumento ?? "",
                            Error = $"Error interno: {ex.Message}"
                        });
                        hayErrores = true;
                    }
                }

                resultado.RegistrosFallidos = resultado.Errores.Count;

                // 🔥 GUARDAR SOLO SI HAY USUARIOS VÁLIDOS
                if (usuariosCreados.Any())
                {
                    foreach (var usuario in usuariosCreados)
                    {
                        await _unitOfWork.UsuariosInformacion.AddAsync(usuario);
                    }

                    var cambios = await _unitOfWork.CompleteAsync();
                    Console.WriteLine($"✅ Usuarios guardados exitosamente: {cambios}");

                    if (!hayErrores)
                    {
                        resultado.Mensaje = $"Carga masiva completada exitosamente: {resultado.RegistrosExitosos} usuarios registrados";
                    }
                    else
                    {
                        resultado.Mensaje = $"⚠️ Carga completada con advertencias: {resultado.RegistrosExitosos} exitosos, {resultado.RegistrosFallidos} fallidos";
                    }
                }
                else
                {
                    resultado.Mensaje = "No se pudo procesar ningún registro. Revise los errores.";
                }

                return resultado;
            }
            catch (Exception ex)
            {
                resultado.Mensaje = $"Error general en la carga masiva: {ex.Message}";
                resultado.RegistrosExitosos = 0;
                resultado.RegistrosFallidos = resultado.TotalRegistros;

                Console.WriteLine($"❌ ERROR: {ex.Message}");
                Console.WriteLine($"❌ INNER: {ex.InnerException?.Message}");

                return resultado;
            }
        }

        public async Task<byte[]> GenerarPlantillaCargaMasivaAsync()
        {
            try
            {
                Console.WriteLine("🔄 Generando plantilla de usuarios...");

                var workbook = new XLWorkbook();
                var worksheet = workbook.AddWorksheet("PlantillaUsuarios");

                // Encabezados
                worksheet.Cell("A1").Value = "NumeroDocumento";
                worksheet.Cell("B1").Value = "Nombres";
                worksheet.Cell("C1").Value = "Apellidos";
                worksheet.Cell("D1").Value = "NombreCampaña"; // ✅ Cambiado a nombre

                // Obtener campañas existentes para ejemplos
                var campañasEjemplo = await _context.Campañas
                    .Take(3)
                    .Select(c => c.NombreCampaña)
                    .ToListAsync();

                // Ejemplos
                worksheet.Cell("A2").Value = "123456789";
                worksheet.Cell("B2").Value = "Juan";
                worksheet.Cell("C2").Value = "Pérez";
                worksheet.Cell("D2").Value = campañasEjemplo.FirstOrDefault() ?? "Campaña Ejemplo";

                // Formato y estilos
                var headerRange = worksheet.Range("A1:D1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);

                var bytes = stream.ToArray();
                workbook.Dispose();

                Console.WriteLine($"✅ Plantilla de usuarios generada: {bytes.Length} bytes");
                return bytes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
                throw;
            }
        }

        #endregion

        #region Métodos para Combos/Selects

        public async Task<List<TipoDocumento>> ObtenerTipoDocumentoAsync()
        {
            return await _unitOfWork.TipoDocumento.Query().ToListAsync();
        }

        public async Task<List<Area>> ObtenerAreasAsync()
        {
            return await _unitOfWork.Areas.Query().ToListAsync();
        }

        public async Task<List<Campaña>> ObtenerCampañasAsync()
        {
            return await _unitOfWork.Campaña.Query().ToListAsync();
        }

        public async Task<List<Estado>> ObtenerEstadosAsync()
        {
            return await _unitOfWork.Estados.Query().ToListAsync();
        }

        #endregion

        #region Usuarios Combinados (AD + Local)

        public async Task<List<object>> ObtenerUsuariosCombinadosAsync()
        {
            Console.WriteLine("🔄 [ObtenerUsuariosCombinadosAsync] Iniciando combinación de usuarios...");

            var listaCombinada = new List<object>();
            var usuariosProcesados = new HashSet<string>();

            // 1. Añadir la opción para crear un nuevo usuario
            listaCombinada.Add(new
            {
                usuario = "nuevo",
                nombreCompleto = "➕ Crear nuevo usuario",
                origen = "sistema"
            });

            // 2. Obtener TODOS los usuarios locales de una sola vez
            var usuariosLocales = await _unitOfWork.UsuariosInformacion
                                           .Query()
                                           .Include(u => u.IdTipodocumentoNavigation)
                                           .Include(u => u.IdAreaNavigation)
                                           .Include(u => u.IdCampañaNavigation)
                                           .AsNoTracking()
                                           .ToListAsync();

            Console.WriteLine($"📊 Usuarios locales encontrados: {usuariosLocales.Count}");

            // 3. Crear Lookup para búsqueda por nombre
            var usuariosLocalesPorNombre = usuariosLocales
                .ToLookup(
                    u => $"{u.Nombres?.Trim().ToLower()}_{u.Apellidos?.Trim().ToLower()}",
                    u => u,
                    StringComparer.OrdinalIgnoreCase
                );

            // 4. Procesar TODOS los usuarios locales individualmente
            foreach (var usuario in usuariosLocales)
            {
                var claveNombre = $"{usuario.Nombres?.Trim().ToLower()}_{usuario.Apellidos?.Trim().ToLower()}";
                var claveDocumento = usuario.NumeroDocumento?.Trim().ToLower() ?? "";

                // Crear clave única que combine nombre + documento
                var claveUnica = $"{claveNombre}_{claveDocumento}";

                if (!usuariosProcesados.Contains(claveUnica))
                {
                    usuariosProcesados.Add(claveUnica);

                    // Verificar si hay duplicados por nombre
                    var usuariosMismoNombre = usuariosLocalesPorNombre[claveNombre].ToList();
                    var tieneDuplicadosPorNombre = usuariosMismoNombre.Count > 1;

                    // CORREGIDO: Solo mostrar documento en nombreCompleto si hay duplicados
                    // y usar un identificador único para el campo "usuario"
                    listaCombinada.Add(new
                    {
                        usuario = $"local_{usuario.NumeroDocumento}", // Identificador único
                        nombreCompleto = tieneDuplicadosPorNombre ?
                            $"{usuario.Nombres} {usuario.Apellidos} N.º doc. " :
                            $"{usuario.Nombres} {usuario.Apellidos}",
                        nombres = usuario.Nombres,
                        apellidos = usuario.Apellidos,
                        numeroDocumento = usuario.NumeroDocumento.ToString(),
                        area = usuario.IdAreaNavigation?.NombreArea,
                        campana = usuario.IdCampañaNavigation?.NombreCampaña,
                        tipoDocumento = usuario.IdTipodocumentoNavigation?.NombreDocumento,
                        idTipoDocumento = usuario.IdTipodocumento,
                        idArea = usuario.IdArea,
                        idCampaña = usuario.IdCampaña,
                        origen = "local",
                        tieneDatosCompletos = true,
                        tieneDuplicadosPorNombre = tieneDuplicadosPorNombre
                    });
                }
            }

            // 5. Obtener usuarios de AD y buscar coincidencias en la base local
            var usuariosAD = await _activeDirectoryService.ObtenerUsuariosAsync();
            Console.WriteLine($" Usuarios AD encontrados: {usuariosAD.Count}");

            foreach (var usuarioAD in usuariosAD)
            {
                var claveNombreAD = $"{usuarioAD.Nombre?.Trim().ToLower()}_{usuarioAD.Apellidos?.Trim().ToLower()}";

                // Buscar coincidencias en usuarios locales por NOMBRE
                var usuariosCoincidentes = usuariosLocalesPorNombre[claveNombreAD].ToList();

                // SOLO agregar usuario AD si NO existe en local
                if (!usuariosCoincidentes.Any())
                {
                    var claveUnicaAD = $"{claveNombreAD}_ad_{usuarioAD.Usuario?.ToLower()}";

                    if (!usuariosProcesados.Contains(claveUnicaAD))
                    {
                        usuariosProcesados.Add(claveUnicaAD);

                        listaCombinada.Add(new
                        {
                            usuario = $"ad_{usuarioAD.Usuario}", // Identificador único para AD
                            nombreCompleto = $"{usuarioAD.NombreCompleto} (AD)",
                            nombres = usuarioAD.Nombre,
                            apellidos = usuarioAD.Apellidos,
                            numeroDocumento = "",
                            area = "",
                            campana = "",
                            tipoDocumento = "",
                            idTipoDocumento = (int?)null,
                            idArea = (int?)null,
                            idCampaña = (int?)null,
                            origen = "ad",
                            tieneDatosCompletos = false,
                            correo = usuarioAD.Correo,
                            coincidenciaLocal = false
                        });
                    }
                }
            }

            Console.WriteLine($"✅ [ObtenerUsuariosCombinadosAsync] Combinación finalizada. Total: {listaCombinada.Count - 1}");

            return listaCombinada;
        }

        #endregion

        #region Filtros
        public async Task<IEnumerable<object>> FiltrarAsync(Dictionary<string, string> filtros)
        {
            string filtroTexto = filtros.TryGetValue("nombre", out var valor) ? valor.ToLower() : "";

            var query = _context.UsuariosInformacion.AsQueryable();

            if (!string.IsNullOrEmpty(filtroTexto))
            {
                query = query.Where(u =>
                    u.Nombres.ToLower().Contains(filtroTexto) ||
                    u.Apellidos.ToLower().Contains(filtroTexto) ||
                    u.NumeroDocumento.Contains(filtroTexto)
                );
            }

            var lista = await query
                .Select(u => new
                {
                    id = u.Id,
                    tipoDocumento = u.IdTipodocumento,
                    numeroDocumento = u.NumeroDocumento,
                    nombreCompleto = $"{u.Nombres} {u.Apellidos}",
                    area = u.IdArea,
                    campana = u.IdCampaña,
                    estado = u.Estado
                })
                .ToListAsync();

            return lista;
        }
        #endregion
    }
}