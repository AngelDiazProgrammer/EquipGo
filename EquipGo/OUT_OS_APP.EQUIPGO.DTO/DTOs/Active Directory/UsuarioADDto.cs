using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_OS_APP.EQUIPGO.DTO.DTOs.Active_Directory
{
    public class UsuarioADDto
    {
        public string NombreCompleto { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Dominio { get; set; } = string.Empty;
    }
}
