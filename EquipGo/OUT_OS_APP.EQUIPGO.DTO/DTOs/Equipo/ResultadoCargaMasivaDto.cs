using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_OS_APP.EQUIPGO.DTO.DTOs.Equipo
{
    public class ResultadoCargaMasivaDto
    {
        public int TotalRegistros { get; set; }
        public int RegistrosExitosos { get; set; }
        public int RegistrosFallidos { get; set; }
        public List<ErrorCargaMasivaDto> Errores { get; set; } = new List<ErrorCargaMasivaDto>();
        public string Mensaje { get; set; } = string.Empty;
    }

    public class ErrorCargaMasivaDto
    {
        public int IndiceFila { get; set; }
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string Serial { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
    }
}
