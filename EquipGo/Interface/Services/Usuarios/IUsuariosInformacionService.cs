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


    }
}
