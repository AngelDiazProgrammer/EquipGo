using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_OS_APP.EQUIPGO.DTO.DTOs.Transacciones
{
    public class TransaccionVisitanteDashboardDto
    {
        public string NombresVisitante { get; set; } = string.Empty;
        public string MarcaEquipo { get; set; } = string.Empty;
        public string NombreAprobador { get; set; } = string.Empty;
        public DateTime FechaTransaccion { get; set; }
        public string NombreSede { get; set; } = string.Empty;
        public string TipoTransaccion { get; set; } = string.Empty;
    }
}
