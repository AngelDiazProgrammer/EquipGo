using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_OS_APP.EQUIPGO.DTO.DTOs
{
    public class EquipoEscaneadoDto
    {
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Serial { get; set; }
        public string Ubicacion { get; set; }
        public string CodigoBarras { get; set; }

        public string NombreUsuario { get; set; }
        public string DocumentoUsuario { get; set; }
        public string Area { get; set; }
        public string Campaña { get; set; }

        public List<string> HistorialTransacciones { get; set; } = new();
    }
}
