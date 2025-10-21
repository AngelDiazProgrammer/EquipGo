using OUT_DOMAIN_EQUIPGO.Entities.Procesos;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Services.Usuarios
{
    public interface IUsuariosInformacionService
    {
        public Task<List<UsuarioInformacionDto?>> ObtenerTodosLosUsuariosInformacionAsync();

        Task<UsuariosInformacion?> ConsultarUsuarioPorDocumentoAsync(string documento);
        Task<UsuariosInformacion?> ConsultarUsuarioPorNombreAsync(string nombres, string apellidos);
        Task<int> CrearUsuarioAsync(UsuariosInformacion usuario);
        Task<bool> ActualizarUsuarioAsync(UsuariosInformacion usuario);

        Task<List<object>> ObtenerUsuariosCombinadosAsync();
    }
}
