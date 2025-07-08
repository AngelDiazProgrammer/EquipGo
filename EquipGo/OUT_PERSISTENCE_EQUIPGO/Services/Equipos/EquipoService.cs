using Interface;
using Interface.Services.Equipos;
using Microsoft.EntityFrameworkCore;
using OUT_DOMAIN_EQUIPGO.Entities.Configuracion;
using OUT_DOMAIN_EQUIPGO.Entities.Procesos;
using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Equipo;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Visitantes;
using OUT_PERSISTENCE_EQUIPGO.Context;
using System.Collections.Generic;
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
                UltimaModificacion = e.UltimaModificacion
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
            var equipo = await _unitOfWork.Equipos.Query().FirstOrDefaultAsync(e => e.Id == id);
            if (equipo == null)
                return false;

            equipo.Marca = dto.Marca;
            equipo.Modelo = dto.Modelo;
            equipo.Serial = dto.Serial;
            equipo.CodigoBarras = dto.CodigoBarras;
            equipo.Ubicacion = dto.Ubicacion;
            equipo.IdUsuarioInfo = dto.IdUsuarioInfo;
            equipo.IdEstado = dto.IdEstado;
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
