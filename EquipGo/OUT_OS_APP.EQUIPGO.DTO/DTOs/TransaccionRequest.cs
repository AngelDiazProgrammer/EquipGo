using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_OS_APP.EQUIPGO.DTO.DTOs
{
    public class TransaccionRequest
    {
        public string CodigoBarras { get; set; }
        public int TipoTransaccion { get; set; }
        public int IdEquipoPersonal { get; set; }
        public int IdUsuarioInfo { get; set; }
        public int IdUsuarioSession { get; set; }
        public int SedeOs { get; set; }
        public string Usuario { get; set; }
    }
}
