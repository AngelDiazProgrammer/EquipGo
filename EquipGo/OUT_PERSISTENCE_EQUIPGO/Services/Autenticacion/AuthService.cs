using Interface;
using Interface.Services.Autenticacion ;
using Microsoft.EntityFrameworkCore;
using OUT_DOMAIN_EQUIPGO.Entities.Seguridad;
using System.Threading.Tasks;

namespace OUT_PERSISTENCE_EQUIPGO.Services.Seguridad
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UsuariosSession?> LoginAsync(string numeroDocumento, string contraseña)
        {
            var usuario = await _unitOfWork.UsuariosSession
                .Query()
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.NumeroDocumento == numeroDocumento && u.IdEstado == 1);

            if (usuario != null && usuario.Contraseña == contraseña)
            {
                return usuario;
            }

            return null;
        }

        public async Task<bool> UsuarioTieneRolAsync(int usuarioSessionId, string nombreRol)
        {
            var usuario = await _unitOfWork.UsuariosSession
                .Query()
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Id == usuarioSessionId);

            return usuario != null &&
                   usuario.Rol != null &&
                   usuario.Rol.NombreRol == nombreRol;
        }
    }
}
