using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interface;
using Interface.Services.Usuarios;
using Microsoft.EntityFrameworkCore;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Usuarios;
namespace OUT_PERSISTENCE_EQUIPGO.Services.Usuarios
{
    public class UsuariosInformacionService : IUsuariosInformacionService{

        private readonly IUnitOfWork _unitOfWork;
        public UsuariosInformacionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

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
    }


}

