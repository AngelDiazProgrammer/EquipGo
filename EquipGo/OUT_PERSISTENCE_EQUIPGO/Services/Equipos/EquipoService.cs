using Interface;
using Interface.Services.Equipos;
using Microsoft.EntityFrameworkCore;
using OUT_DOMAIN_EQUIPGO.Entities.Configuracion;
using OUT_OS_APP.EQUIPGO.DTO.DTOs;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Equipo;
using System.Linq;
using System.Threading.Tasks;


namespace OUT_PERSISTENCE_EQUIPGO.Services.Equipos
{
    public class EquipoService : IEquipoService
    {
        private readonly IUnitOfWork _unitOfWork;

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
                .Take(5) // Últimas 5 transacciones
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
                IdEquipoPersonal = equipo.IdEquipoPersonal,
                IdUsuarioInfo = equipo.IdUsuarioInfo,
                IdSedeOs = equipo.IdSede,
                HistorialTransacciones = historial
            };
        }


        public async Task<List<EquipoDto>> ObtenerTodosLosEquiposAsync()
        {
            var equipos = await _unitOfWork.Equipos
                                           .Query()
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
                Latitud = e.Latitud,
                Longitud = e.Longitud,
                SistemaOperativo = e.SistemaOperativo,
                MacEquipo = e.MacEquipo,
                VersionSoftware = e.VersionSoftware,
                FechaCreacion = e.FechaCreacion,
                UltimaModificacion = e.UltimaModificacion
            }).ToList();

            return lista;
        }

    }
}
