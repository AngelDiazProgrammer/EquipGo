using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_OS_APP.EQUIPGO.DTO.DTOs.AuthDTOs
{
    public class AuthResponse
    {
        public long Id { get; set; }
        public string NombreUsuario { get; set; }
        public long RolId { get; set; }
    }
}
