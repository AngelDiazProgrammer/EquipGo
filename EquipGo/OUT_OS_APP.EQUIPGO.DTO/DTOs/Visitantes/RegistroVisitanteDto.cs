using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_OS_APP.EQUIPGO.DTO.DTOs.Visitantes
{
    public class RegistroVisitanteDto
    {
        // Usuario visitante
        public string TipoDocumento { get; set; } = string.Empty;
        public string NumeroDocumento { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string TipoUsuario { get; set; } = string.Empty;
        public int? IdProveedor { get; set; }
        public string NombreProveedor { get; set; } = string.Empty;

        // Equipo visitante
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string Serial { get; set; } = string.Empty;

        //Transaccion automatica
        public int TipoTransaccionSiguiente { get; set; }
        public string TipoTransaccion { get; set; } = "";
    }
}

