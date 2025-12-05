using Interface;
using Interface.Services.Equipos;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using OUT_DOMAIN_EQUIPGO.Entities.Configuracion;
using OUT_DOMAIN_EQUIPGO.Entities.Procesos;
using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Equipo;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Visitantes;
using OUT_PERSISTENCE_EQUIPGO.Context;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace OUT_PERSISTENCE_EQUIPGO.Services.Equipos
{
    public class EquipoService : IEquipoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly EquipGoDbContext _context;
        #region UnitofWork y Metodos propios de los equipos
        public EquipoService(IUnitOfWork unitOfWork, EquipGoDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;

        }

        public async Task<EquipoEscaneadoDto?> ConsultarPorCodigoBarrasAsync(string codigoBarras)
        {
            var query = _unitOfWork.Equipos.Query()
                .Include(e => e.IdUsuarioInfoNavigation)
                    .ThenInclude(u => u.IdAreaNavigation)
                .Include(e => e.IdUsuarioInfoNavigation)
                    .ThenInclude(u => u.IdCampañaNavigation)
                .AsNoTracking();

            var equipo = await query.FirstOrDefaultAsync(e => e.CodigoBarras == codigoBarras);

            if (equipo == null)
                return null;

            var usuario = equipo.IdUsuarioInfoNavigation;

            // ✅ Historial de transacciones
            var historial = await _unitOfWork.Transacciones.Query()
                .Where(t => t.CodigoBarras == codigoBarras)
                .OrderByDescending(t => t.FechaHora)
                .Take(5)
                .Select(t => $"🔖 [{t.FechaHora}] Tipo: {(t.IdTipoTransaccion == 1 ? "Entrada" : "Salida")}")
                .ToListAsync();

            return new EquipoEscaneadoDto
            {
                Marca = equipo.Marca,
                Modelo = equipo.Modelo,
                Serial = equipo.Serial,
                Ubicacion = equipo.Ubicacion,
                CodigoBarras = equipo.CodigoBarras,
                NombreUsuario = $"{usuario.Nombres} {usuario.Apellidos}",
                DocumentoUsuario = usuario.NumeroDocumento,
                Area = usuario.IdAreaNavigation?.NombreArea,
                Campaña = usuario.IdCampañaNavigation?.NombreCampaña,
                IdEquipoPersonal = equipo.IdEquipoPersonal ?? 0,
                IdUsuarioInfo = equipo.IdUsuarioInfo,
                IdSedeOs = equipo.IdSede ?? 0,
                HistorialTransacciones = historial
            };
        }

        public async Task<OUT_DOMAIN_EQUIPGO.Entities.Configuracion.Equipos?> ConsultarPorSerialAsync(string serial)
        {
            if (string.IsNullOrEmpty(serial))
                return null;

            // Usamos el Query del UnitOfWork para obtener la entidad directamente.
            // SIN AsNoTracking() para que Entity Framework pueda rastrearla y actualizarla.
            return await _unitOfWork.Equipos.Query()
                .FirstOrDefaultAsync(e => e.Serial == serial);
        }

        public async Task<List<EquipoDto>> ObtenerTodosLosEquiposAsync()
        {
            var equipos = await _unitOfWork.Equipos.Query()
                                       .Include(e => e.Estado)
                                       .Include(e => e.IdUsuarioInfoNavigation)
                                       .Include(e => e.IdEquipoPersonalNavigation)
                                       .Include(e => e.IdSedeNavigation)
                                       .Include(e => e.IdTipoDispositivoNavigation)
                                       .Include(e => e.IdProveedorNavigation)
                                       .AsNoTracking()
                                       .ToListAsync();

            var lista = equipos.Select(e => new EquipoDto
            {
                Id = e.Id,
                Marca = e.Marca,
                Modelo = e.Modelo,
                Serial = e.Serial,
                CodigoBarras = e.CodigoBarras,
                Ubicacion = e.Ubicacion,
                UsuarioNombreCompleto = e.IdUsuarioInfoNavigation != null ?
        $"{e.IdUsuarioInfoNavigation.Nombres} {e.IdUsuarioInfoNavigation.Apellidos}" : "",
                EstadoNombre = e.Estado?.NombreEstado,
                EquipoPersonalNombre = e.IdEquipoPersonalNavigation?.NombrePersonal,
                SedeNombre = e.IdSedeNavigation?.NombreSede,
                TipoDispositivoNombre = e.IdTipoDispositivoNavigation?.NombreTipo,
                ProveedorNombre = e.IdProveedorNavigation?.NombreProveedor,
                Latitud = e.Latitud,       // 👈 Conversión explícita
                Longitud = e.Longitud,     // 👈 Conversión explícita
                SistemaOperativo = e.SistemaOperativo,
                MacEquipo = e.MacEquipo,
                VersionSoftware = e.VersionSoftware,
                FechaCreacion = e.FechaCreacion,
                UltimaModificacion = e.UltimaModificacion,
                IdSubEstado = e.IdSubEstado // ✅ Agregado subestado
            }).ToList();

            return lista;
        }

        public async Task<bool> CrearEquipoAdminAsync(CrearEquipoDto equipoDto)
        {
            if (string.IsNullOrEmpty(equipoDto.Marca) || string.IsNullOrEmpty(equipoDto.Modelo))
                throw new Exception("La marca y el modelo son obligatorios.");

            var equipo = new OUT_DOMAIN_EQUIPGO.Entities.Configuracion.Equipos
            {
                Marca = equipoDto.Marca,
                Modelo = equipoDto.Modelo,
                Serial = equipoDto.Serial,
                CodigoBarras = equipoDto.CodigoBarras,
                Ubicacion = equipoDto.Ubicacion,
                IdUsuarioInfo = equipoDto.IdUsuarioInfo,
                IdEstado = equipoDto.IdEstado,
                IdSubEstado = equipoDto.IdSubEstado, // ✅ Agregado subestado
                IdEquipoPersonal = equipoDto.IdEquipoPersonal,
                IdSede = equipoDto.IdSede,
                IdTipoDispositivo = equipoDto.IdTipoDispositivo,
                IdProveedor = equipoDto.IdProveedor,
                Latitud = equipoDto.Latitud,
                Longitud = equipoDto.Longitud,
                SistemaOperativo = equipoDto.SistemaOperativo,
                MacEquipo = equipoDto.MacEquipo,
                VersionSoftware = equipoDto.VersionSoftware,
                FechaCreacion = DateTime.UtcNow,
                UltimaModificacion = DateTime.UtcNow
            };

            await _unitOfWork.Equipos.AddAsync(equipo);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<EquipoDto?> ObtenerPorIdAsync(int id)
        {
            var equipo = await _unitOfWork.Equipos.Query()
                .Include(e => e.IdUsuarioInfoNavigation)
                .Include(e => e.Estado)
                .Include(e => e.IdEquipoPersonalNavigation)
                .Include(e => e.IdSedeNavigation)
                .Include(e => e.IdTipoDispositivoNavigation)
                .Include(e => e.IdProveedorNavigation)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (equipo == null)
                return null;

            return new EquipoDto
            {
                Id = equipo.Id,
                Marca = equipo.Marca,
                Modelo = equipo.Modelo,
                Serial = equipo.Serial,
                CodigoBarras = equipo.CodigoBarras,
                Ubicacion = equipo.Ubicacion,
                Latitud = equipo.Latitud,
                Longitud = equipo.Longitud,
                SistemaOperativo = equipo.SistemaOperativo,
                MacEquipo = equipo.MacEquipo,
                VersionSoftware = equipo.VersionSoftware,
                IdUsuarioInfo = equipo.IdUsuarioInfo,
                IdEstado = equipo.IdEstado,
                IdSubEstado = equipo.IdSubEstado, // ✅ Agregado subestado
                IdEquipoPersonal = equipo.IdEquipoPersonal,
                IdSede = equipo.IdSede,
                IdTipoDispositivo = equipo.IdTipoDispositivo,
                IdProveedor = equipo.IdProveedor,

                // ✅ Agrega estas propiedades de texto
                UsuarioNombreCompleto = equipo.IdUsuarioInfoNavigation != null
        ? $"{equipo.IdUsuarioInfoNavigation.Nombres} {equipo.IdUsuarioInfoNavigation.Apellidos}"
        : "Sin asignar",
                EstadoNombre = equipo.Estado?.NombreEstado ?? "Sin estado",
                EquipoPersonalNombre = equipo.IdEquipoPersonalNavigation?.NombrePersonal ?? "No definido",
                TipoDispositivoNombre = equipo.IdTipoDispositivoNavigation?.NombreTipo ?? "No definido",
                ProveedorNombre = equipo.IdProveedorNavigation?.NombreProveedor ?? "Sin asignar"
            };
        }

        public async Task<bool> ActualizarEquipoAdminAsync(int id, CrearEquipoDto dto)
        {
            Console.WriteLine($"🔄 Iniciando actualización del equipo ID: {id}");
            Console.WriteLine($"📦 DTO recibido - IdSubEstado: {dto.IdSubEstado}");
            Console.WriteLine($"📦 DTO completo: {System.Text.Json.JsonSerializer.Serialize(dto)}");

            var equipo = await _unitOfWork.Equipos.Query().FirstOrDefaultAsync(e => e.Id == id);
            if (equipo == null)
            {
                Console.WriteLine("❌ Equipo no encontrado");
                return false;
            }

            Console.WriteLine($"🔍 Equipo encontrado - IdSubEstado actual: {equipo.IdSubEstado}");

            equipo.Marca = dto.Marca;
            equipo.Modelo = dto.Modelo;
            equipo.Serial = dto.Serial;
            equipo.CodigoBarras = dto.CodigoBarras;
            equipo.Ubicacion = dto.Ubicacion;
            equipo.IdEstado = dto.IdEstado;
            equipo.IdSubEstado = dto.IdSubEstado; // ✅ Esta línea es crucial
            equipo.IdEquipoPersonal = dto.IdEquipoPersonal;
            equipo.IdSede = dto.IdSede;
            equipo.IdTipoDispositivo = dto.IdTipoDispositivo;
            equipo.IdProveedor = dto.IdProveedor;
            equipo.Latitud = dto.Latitud;
            equipo.Longitud = dto.Longitud;
            equipo.SistemaOperativo = dto.SistemaOperativo;
            equipo.MacEquipo = dto.MacEquipo;
            equipo.VersionSoftware = dto.VersionSoftware;
            equipo.UltimaModificacion = DateTime.UtcNow;

            Console.WriteLine($"✅ Equipo actualizado - Nuevo IdSubEstado: {equipo.IdSubEstado}");

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

        // ✅ AÑADE ESTA NUEVA IMPLEMENTACIÓN
        public async Task<bool> ActualizarEquipoAsync(OUT_DOMAIN_EQUIPGO.Entities.Configuracion.Equipos equipo)
        {
            if (equipo == null) return false;

            // La entidad ya está siendo rastreada por el DbContext,
            // así que solo necesitamos llamar a CompleteAsync para guardar los cambios.
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            try
            {
                var equipo = await _context.Equipos.FindAsync(id);
                if (equipo == null) return false;

                _context.Equipos.Remove(equipo);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al eliminar equipo: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Cargue Masivo

        public async Task<ResultadoCargaMasivaDto> CargaMasivaEquiposAsync(List<CrearEquipoDto> equiposDto)
        {
            var resultado = new ResultadoCargaMasivaDto
            {
                TotalRegistros = equiposDto.Count
            };

            if (equiposDto == null || !equiposDto.Any())
            {
                resultado.Mensaje = "La lista de equipos está vacía";
                return resultado;
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 🔥 LISTA PARA VERIFICAR DUPLICADOS DENTRO DEL MISMO ARCHIVO
                var serialesProcesados = new HashSet<string>();
                var hayErrores = false;

                for (int i = 0; i < equiposDto.Count; i++)
                {
                    var equipoDto = equiposDto[i];

                    try
                    {
                        // Validación 1: Marca y Modelo obligatorios
                        if (string.IsNullOrWhiteSpace(equipoDto.Marca) || string.IsNullOrWhiteSpace(equipoDto.Modelo))
                        {
                            resultado.Errores.Add(new ErrorCargaMasivaDto
                            {
                                IndiceFila = i + 1,
                                Marca = equipoDto.Marca ?? "",
                                Modelo = equipoDto.Modelo ?? "",
                                Serial = equipoDto.Serial ?? "",
                                Error = "La marca y el modelo son obligatorios"
                            });
                            hayErrores = true;
                            continue;
                        }

                        // 🔥 Validación 2: Serial obligatorio
                        var serial = equipoDto.Serial?.Trim();
                        if (string.IsNullOrWhiteSpace(serial))
                        {
                            resultado.Errores.Add(new ErrorCargaMasivaDto
                            {
                                IndiceFila = i + 1,
                                Marca = equipoDto.Marca,
                                Modelo = equipoDto.Modelo,
                                Serial = "",
                                Error = "El serial es obligatorio y no puede estar vacío"
                            });
                            hayErrores = true;
                            continue;
                        }

                        // 🔥 Validación 3: Verificar duplicados dentro del mismo archivo
                        if (serialesProcesados.Contains(serial))
                        {
                            resultado.Errores.Add(new ErrorCargaMasivaDto
                            {
                                IndiceFila = i + 1,
                                Marca = equipoDto.Marca,
                                Modelo = equipoDto.Modelo,
                                Serial = serial,
                                Error = "Serial duplicado dentro del mismo archivo"
                            });
                            hayErrores = true;
                            continue;
                        }

                        // 🔥 Validación 4: Verificar si el serial ya existe en la base de datos
                        var serialExistente = await _unitOfWork.Equipos.Query()
                            .AnyAsync(e => e.Serial == serial);

                        if (serialExistente)
                        {
                            resultado.Errores.Add(new ErrorCargaMasivaDto
                            {
                                IndiceFila = i + 1,
                                Marca = equipoDto.Marca,
                                Modelo = equipoDto.Modelo,
                                Serial = serial,
                                Error = "Ya existe un equipo con este serial en el sistema"
                            });
                            hayErrores = true;
                            continue;
                        }

                        // 🔥 Validación 5: Código de barras único (si se proporciona)
                        var codigoBarras = equipoDto.CodigoBarras?.Trim();
                        if (!string.IsNullOrWhiteSpace(codigoBarras))
                        {
                            var codigoBarrasExistente = await _unitOfWork.Equipos.Query()
                                .AnyAsync(e => e.CodigoBarras == codigoBarras);

                            if (codigoBarrasExistente)
                            {
                                resultado.Errores.Add(new ErrorCargaMasivaDto
                                {
                                    IndiceFila = i + 1,
                                    Marca = equipoDto.Marca,
                                    Modelo = equipoDto.Modelo,
                                    Serial = serial,
                                    Error = "Ya existe un equipo con este código de barras en el sistema"
                                });
                                hayErrores = true;
                                continue;
                            }
                        }

                        // 🔥 SOLO si no hay errores, agregar a la lista de procesados y crear equipo
                        serialesProcesados.Add(serial);

                        // Crear el equipo
                        var equipo = new OUT_DOMAIN_EQUIPGO.Entities.Configuracion.Equipos
                        {
                            Marca = equipoDto.Marca.Trim(),
                            Modelo = equipoDto.Modelo.Trim(),
                            Serial = serial,
                            CodigoBarras = codigoBarras,
                            Ubicacion = equipoDto.Ubicacion?.Trim(),
                            IdUsuarioInfo = equipoDto.IdUsuarioInfo,
                            IdEstado = equipoDto.IdEstado,
                            IdSubEstado = equipoDto.IdSubEstado,
                            IdEquipoPersonal = equipoDto.IdEquipoPersonal,
                            IdSede = equipoDto.IdSede,
                            IdTipoDispositivo = equipoDto.IdTipoDispositivo,
                            IdProveedor = equipoDto.IdProveedor,
                            Latitud = equipoDto.Latitud,
                            Longitud = equipoDto.Longitud,
                            SistemaOperativo = equipoDto.SistemaOperativo?.Trim(),
                            MacEquipo = equipoDto.MacEquipo?.Trim(),
                            VersionSoftware = equipoDto.VersionSoftware?.Trim(),
                            FechaCreacion = DateTime.UtcNow,
                            UltimaModificacion = DateTime.UtcNow
                        };

                        await _unitOfWork.Equipos.AddAsync(equipo);
                        resultado.RegistrosExitosos++;
                    }
                    catch (Exception ex)
                    {
                        resultado.Errores.Add(new ErrorCargaMasivaDto
                        {
                            IndiceFila = i + 1,
                            Marca = equipoDto.Marca ?? "",
                            Modelo = equipoDto.Modelo ?? "",
                            Serial = equipoDto.Serial ?? "",
                            Error = $"Error interno: {ex.Message}"
                        });
                        hayErrores = true;
                    }
                }

                resultado.RegistrosFallidos = resultado.Errores.Count;

                // 🔥 DECISIÓN CRÍTICA: Commit o Rollback
                if (resultado.RegistrosExitosos > 0 && !hayErrores)
                {
                    // ✅ CASO IDEAL: Todos los registros son válidos
                    await _unitOfWork.CompleteAsync();
                    await transaction.CommitAsync();
                    resultado.Mensaje = $"Carga masiva completada exitosamente: {resultado.RegistrosExitosos} equipos registrados";
                }
                else if (resultado.RegistrosExitosos > 0 && hayErrores)
                {
                    // 🔥 CASO CON ERRORES: Hacemos ROLLBACK - No se guarda NADA
                    await transaction.RollbackAsync();
                    resultado.RegistrosExitosos = 0; // Reiniciamos porque no se guardó nada
                    resultado.Mensaje = "❌ Carga cancelada: Se detectaron errores en el archivo. No se registró ningún equipo. Revise los detalles abajo.";
                }
                else
                {
                    // ❌ CASO SIN ÉXITOS: Rollback automático
                    await transaction.RollbackAsync();
                    resultado.Mensaje = "No se pudo procesar ningún registro. Revise los errores.";
                }

                return resultado;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                resultado.Mensaje = $"Error general en la carga masiva: {ex.Message}";
                resultado.RegistrosExitosos = 0;
                resultado.RegistrosFallidos = resultado.TotalRegistros;

                return resultado;
            }
        }

        public async Task<byte[]> GenerarPlantillaCargaMasivaAsync()
        {
            try
            {
                Console.WriteLine("🔄 Generando plantilla simple...");

                // Versión mínima para evitar problemas de memoria
                var workbook = new XLWorkbook();
                var worksheet = workbook.AddWorksheet("PlantillaEquipos");

                // Solo encabezados básicos
                worksheet.Cell("A1").Value = "Marca";
                worksheet.Cell("B1").Value = "Modelo";
                worksheet.Cell("C1").Value = "Serial";
                worksheet.Cell("D1").Value = "CodigoBarras";

                // Un solo ejemplo
                worksheet.Cell("A2").Value = "Dell";
                worksheet.Cell("B2").Value = "Latitude 5510";
                worksheet.Cell("C2").Value = "ABC123456";
                worksheet.Cell("D2").Value = "CB001";

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);

                var bytes = stream.ToArray();
                workbook.Dispose(); // 🔥 Liberar recursos explícitamente

                Console.WriteLine($"✅ Plantilla simple generada: {bytes.Length} bytes");
                return bytes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
                throw;
            }
        }
        #endregion

        #region Asigancion de usuarios

        public async Task<bool> AsignarUsuarioAEquipoAsync(int equipoId, int usuarioId)
        {
            try
            {
                Console.WriteLine($"🔄 Asignando usuario {usuarioId} al equipo {equipoId}");

                var equipo = await _unitOfWork.Equipos.Query()
                    .FirstOrDefaultAsync(e => e.Id == equipoId);

                if (equipo == null)
                {
                    Console.WriteLine("❌ Equipo no encontrado");
                    return false;
                }

                // Verificar que el usuario existe
                var usuario = await _unitOfWork.UsuariosInformacion.Query()
                    .FirstOrDefaultAsync(u => u.Id == usuarioId);

                if (usuario == null)
                {
                    Console.WriteLine("❌ Usuario no encontrado");
                    return false;
                }

                // Verificar si está activo
                if (usuario.IdEstado != 1) 
                {
                    Console.WriteLine("❌ El usuario existe pero no está activo. No se puede asignar el equipo.");
                    return false;
                }

                // Asignar usuario al equipo
                equipo.IdUsuarioInfo = usuarioId;
                equipo.UltimaModificacion = DateTime.UtcNow;

                await _unitOfWork.CompleteAsync();

                Console.WriteLine($"✅ Usuario {usuarioId} asignado al equipo {equipoId}");
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en AsignarUsuarioAEquipoAsync: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> DesasignarUsuarioDeEquipoAsync(int equipoId)
        {
            try
            {
                Console.WriteLine($"🔄 Desasignando usuario del equipo {equipoId}");

                var equipo = await _unitOfWork.Equipos.Query()
                    .FirstOrDefaultAsync(e => e.Id == equipoId);

                if (equipo == null)
                {
                    Console.WriteLine("❌ Equipo no encontrado");
                    return false;
                }

                if (equipo.IdUsuarioInfo == null)
                {
                    Console.WriteLine("⚠️ El equipo no tiene usuario asignado");
                    return true; // Consideramos éxito porque ya está desasignado
                }

                // Desasignar usuario
                equipo.IdUsuarioInfo = null;
                equipo.UltimaModificacion = DateTime.UtcNow;

                await _unitOfWork.CompleteAsync();

                Console.WriteLine($"✅ Usuario desasignado del equipo {equipoId}");
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en DesasignarUsuarioDeEquipoAsync: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Visitantes

        public async Task<RegistroVisitanteDto?> ConsultarVisitantePorDocumentoAsync(string numeroDocumento)
        {
            var visitante = await _context.UsuariosVisitantes
                .Include(v => v.EquiposVisitantes)
                .FirstOrDefaultAsync(v => v.NumeroDocumento == numeroDocumento);

            if (visitante == null) return null;

            var equipo = visitante.EquiposVisitantes.FirstOrDefault();

            // Obtener el nombre del proveedor si existe
            string nombreProveedor = "Sin proveedor";

            if (visitante.IdProveedor.HasValue)
            {
                nombreProveedor = await _context.Proveedores
                    .Where(p => p.Id == visitante.IdProveedor)
                    .Select(p => p.NombreProveedor)
                    .FirstOrDefaultAsync() ?? "Sin proveedor";
            }

            // Obtener la última transacción del visitante
            var ultimaTransaccion = await _context.TransaccionesVisitantes
                .Where(t => t.IdUsuarioVisitante == visitante.Id)
                .OrderByDescending(t => t.FechaTransaccion)
                .FirstOrDefaultAsync();

            // Determinar tipo de transacción automáticamente
            int tipoTransaccionSiguiente = (ultimaTransaccion == null || ultimaTransaccion.IdTipoTransaccion == 2) ? 1 : 2;

            return new RegistroVisitanteDto
            {
                TipoDocumento = visitante.TipoDocumento,
                NumeroDocumento = visitante.NumeroDocumento,
                Nombres = visitante.Nombres,
                Apellidos = visitante.Apellidos,
                IdProveedor = visitante.IdProveedor,
                NombreProveedor = nombreProveedor,
                Marca = equipo?.Marca ?? "",
                Modelo = equipo?.Modelo ?? "",
                Serial = equipo?.Serial ?? "",
                FotoBase64 = equipo.Foto,
                TipoTransaccionSiguiente = tipoTransaccionSiguiente,
                TipoTransaccion = tipoTransaccionSiguiente == 1 ? "Entrada" : "Salida"
            };
        }

        #endregion

        #region Sincronizar equipos - agente de escritorio

        public async Task<string> SincronizarEquipoAsync(EquipoSyncRequestDto dto)
        {
            // 🔍 Validaciones mínimas
            if (string.IsNullOrWhiteSpace(dto.Serial))
                return "El campo 'Serial' es obligatorio.";

            if (string.IsNullOrWhiteSpace(dto.Marca) || string.IsNullOrWhiteSpace(dto.Modelo))
                return "Los campos 'Marca' y 'Modelo' son obligatorios.";

            if (string.IsNullOrWhiteSpace(dto.MacEquipo))
                return "El campo 'MacEquipo' es obligatorio.";

            if (string.IsNullOrWhiteSpace(dto.SistemaOperativo))
                return "El campo 'SistemaOperativo' es obligatorio.";

            if (dto.Latitud < -90 || dto.Latitud > 90 || dto.Longitud < -180 || dto.Longitud > 180)
                return "Las coordenadas de ubicación son inválidas.";

            // 🔍 Buscar si ya existe un equipo con ese serial
            var equipoExistente = await _context.Equipos
                .FirstOrDefaultAsync(e => e.Serial == dto.Serial);

            if (equipoExistente != null)
            {
                // 🔁 Actualizar equipo existente
                equipoExistente.Marca = dto.Marca;
                equipoExistente.Modelo = dto.Modelo;
                equipoExistente.MacEquipo = dto.MacEquipo;
                equipoExistente.SistemaOperativo = dto.SistemaOperativo;
                equipoExistente.VersionSoftware = dto.VersionSoftware;
                equipoExistente.Latitud = dto.Latitud;
                equipoExistente.Longitud = dto.Longitud;
                equipoExistente.UltimaModificacion = DateTime.Now;

                await _unitOfWork.CompleteAsync();
                return "Equipo actualizado con éxito.";
            }

            // 🆕 Crear nuevo equipo
            var nuevoCodigo = string.IsNullOrWhiteSpace(dto.CodigoBarras)
                ? $"SIN-ASIGNAR-{Guid.NewGuid()}"
                : dto.CodigoBarras;

            var nuevoEquipo = new OUT_DOMAIN_EQUIPGO.Entities.Configuracion.Equipos
            {
                Serial = dto.Serial,
                Marca = dto.Marca,
                Modelo = dto.Modelo,
                MacEquipo = dto.MacEquipo,
                SistemaOperativo = dto.SistemaOperativo,
                VersionSoftware = dto.VersionSoftware,
                Latitud = dto.Latitud,
                Longitud = dto.Longitud,
                CodigoBarras = nuevoCodigo,
                IdUsuarioInfo = dto.IdUsuarioInfo, // Por ahora 0 como "sin asignar"
                IdEstado = dto.IdEstado,
                IdSubEstado = dto.IdSubEstado, // ✅ Agregado subestado
                IdSede = dto.IdSede,
                IdEquipoPersonal = dto.IdEquipoPersonal,
                IdTipoDispositivo = dto.IdTipoDispositivo,
                IdProveedor = dto.IdProveedor,
                FechaCreacion = DateTime.Now,
                UltimaModificacion = DateTime.Now
            };

            await _unitOfWork.Equipos.AddAsync(nuevoEquipo);
            await _unitOfWork.CompleteAsync();

            return "Equipo registrado automáticamente.";
        }
        #endregion

        #region Resgitros de equipos no corporativos
        public async Task<UsuariosInformacion?> ConsultarUsuarioPorDocumentoAsync(string documento)
        {
            if (!string.IsNullOrEmpty(documento))
            {
                return await _unitOfWork.UsuariosInformacion.Query().FirstOrDefaultAsync(u => u.NumeroDocumento == documento);
            }
            return null;  // ✅ agregado
        }
        public async Task<List<TipoDocumento>> ObtenerTipoDocumentoAsync()
        {
            return await _unitOfWork.TipoDocumento.Query().ToListAsync();
        }
        public async Task<int> CrearUsuarioAsync(UsuariosInformacion usuario)
        {
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

        public async Task<List<Area>> ObtenerAreasAsync()
        {
            return await _unitOfWork.Areas.Query().ToListAsync();
        }

        public async Task<List<Campaña>> ObtenerCampañasAsync()
        {
            return await _unitOfWork.Campaña.Query().ToListAsync();
        }

        public async Task<List<EquiposPersonal>> ObtenerEquiposPersonalesAsync()
        {
            return await _unitOfWork.EquiposPersonal.Query().ToListAsync();
        }

        public async Task<EquiposPersonal?> ObtenerEquipoPersonalPorIdAsync(int id)
        {
            return await _unitOfWork.EquiposPersonal.Query().FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<bool> CrearEquipoAsync(OUT_DOMAIN_EQUIPGO.Entities.Configuracion.Equipos equipo)
        {
            await _unitOfWork.Equipos.AddAsync(equipo);
            await _unitOfWork.CompleteAsync();
            return true;
        }
        #endregion
    }
}