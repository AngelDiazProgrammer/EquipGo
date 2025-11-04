using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_OS_APP.EQUIPGO.DTO.DTOs.Transacciones
{
    public class FiltrosTransaccionDto
    {
        public string CodigoBarras { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
        public string TipoTransaccion { get; set; } = string.Empty;
        public string Sede { get; set; } = string.Empty;
        public string Fecha { get; set; } = string.Empty;
    }
}
