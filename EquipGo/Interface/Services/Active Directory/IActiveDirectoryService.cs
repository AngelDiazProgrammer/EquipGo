using OUT_OS_APP.EQUIPGO.DTO.DTOs.Active_Directory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Services.Active_Directory
{
    public interface IActiveDirectoryService
    {
        Task<List<UsuarioADDto>> ObtenerUsuariosAsync();
    }
}
