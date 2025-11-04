using OUT_DOMAIN_EQUIPGO.Entities.Seguridad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Services.Usuarios
{
    public interface IUsuariosSessionService
    {
        Task<UsuariosSession> GetByIdAsync(int id);
        Task<UsuariosSession> GetWithSedeAsync(int usuarioId);
        Task<string> GetNombreSedeByUsuarioIdAsync(int usuarioId);
        Task<int> GetSedeByUsuarioIdAsync(int usuarioId);
    }
}
