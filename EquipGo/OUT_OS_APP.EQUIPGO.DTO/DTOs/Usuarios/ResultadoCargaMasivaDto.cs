using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_OS_APP.EQUIPGO.DTO.DTOs.Usuarios
{
    public class ResultadoCargaMasivaDto
    {
        public int TotalRegistros { get; set; }
        public int RegistrosExitosos { get; set; }
        public int RegistrosFallidos { get; set; }
        public string Mensaje { get; set; }
        public List<ErrorCargaMasivaDto> Errores { get; set; } = new List<ErrorCargaMasivaDto>();
    }

    public class ErrorCargaMasivaDto
    {
        public int IndiceFila { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string NumeroDocumento { get; set; }
        public string Error { get; set; }
    }
}
