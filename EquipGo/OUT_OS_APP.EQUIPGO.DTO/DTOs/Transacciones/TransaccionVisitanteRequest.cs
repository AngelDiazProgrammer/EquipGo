using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_OS_APP.EQUIPGO.DTO.DTOs.Transacciones
{
    public class TransaccionVisitanteRequest
    {
        public string NumeroDocumento { get; set; } = string.Empty;
        public int TipoTransaccion { get; set; }
        public int IdUsuarioVisitante { get; set; }
        public int IdEquipoVisitante { get; set; }
        public int IdUsuarioSession { get; set; }
        public int? SedeOs { get; set; }

        public DateTime FechaTransaccion { get; set; }

        public int IdUsuarioAprobador { get; set; }
    }
}
