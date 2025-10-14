// En OUT_PERSISTENCE_EQUIPGO/Services/Usuarios/UsuariosInformacionService.cs

using Interface;
using Interface.Services.Usuarios;
using Microsoft.EntityFrameworkCore;
using OUT_DOMAIN_EQUIPGO.Entities.Procesos;
using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_PERSISTENCE_EQUIPGO.Services.Usuarios
{
    public class UsuariosInformacionService : IUsuariosInformacionService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UsuariosInformacionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Método existente (sin cambios)
        public async Task<List<UsuarioInformacionDto>> ObtenerTodosLosUsuariosInformacionAsync()
        {
            // ... tu código existente ...
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

        // ✅ NUEVAS IMPLEMENTACIONES

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
    }
}