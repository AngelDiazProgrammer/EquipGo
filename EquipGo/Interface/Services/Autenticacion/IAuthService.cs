using OUT_DOMAIN_EQUIPGO.Entities.Seguridad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Services.Autenticacion
{
    public interface IAuthService
    {
        Task<UsuariosSession?> LoginAsync(string numeroDocumento, string contraseña);
        Task<bool> UsuarioTieneRolAsync(int usuarioSessionId, string nombreRol);
    }
}
