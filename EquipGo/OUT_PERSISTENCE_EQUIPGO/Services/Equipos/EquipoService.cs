using Interface;
using Interface.Services.Equipos;
using Microsoft.EntityFrameworkCore;
using OUT_DOMAIN_EQUIPGO.Entities.Configuracion;
using OUT_DOMAIN_EQUIPGO.Entities.Procesos;
using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Equipo;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OUT_PERSISTENCE_EQUIPGO.Services.Equipos
{
    public class EquipoService : IEquipoService
    {
        private readonly IUnitOfWork _unitOfWork;
        #region UnitofWork y Metodos propios de los equipos
        public EquipoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
                Latitud = e.Latitud ?? 0,       // 👈 Conversión explícita
                Longitud = e.Longitud ?? 0,     // 👈 Conversión explícita
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
            return await _unitOfWork.Area.Query().ToListAsync();
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
