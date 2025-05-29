using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_DOMAIN_EQUIPGO.Entities.Procesos
{
    public class Transacciones
    {
        public int Id { get; set; }
        public string CodigoBarras { get; set; }
        public int IdTipoTransaccion { get; set; }
        public int IdEquipoPersonal { get; set; }
        public int IdUsuarioInfo { get; set; }
        public int IdUsuarioSession { get; set; }
        public int SedeOs { get; set; }
        public DateTime FechaHora { get; set; }
        public string Usuario { get; set; }
    }
}
