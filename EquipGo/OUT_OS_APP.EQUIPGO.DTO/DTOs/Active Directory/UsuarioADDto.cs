using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_OS_APP.EQUIPGO.DTO.DTOs.Active_Directory
{
    namespace OUT_OS_APP.EQUIPGO.DTO.DTOs.Active_Directory
    {
        public class UsuarioADDto
        {
            // Propiedades existentes
            public string NombreCompleto { get; set; } = string.Empty;
            public string Usuario { get; set; } = string.Empty;
            public string Correo { get; set; } = string.Empty;
            public string Descripcion { get; set; } = string.Empty;
            public string Dominio { get; set; } = string.Empty;

            // Nuevas propiedades para nombre y apellidos
            public string Nombre { get; set; } = string.Empty;
            public string Apellidos { get; set; } = string.Empty;
            public string PrimerApellido { get; set; } = string.Empty;
            public string SegundoApellido { get; set; } = string.Empty;
        }
    }
}
