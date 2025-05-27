using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_DOMAIN_EQUIPGO.Entities.Procesos
{
    public class Transaccion
    {
        public int Id { get; set; }
        public string CodigoBarra { get; set; }
        public DateTime FechaHora { get; set; }
        public string Tipo { get; set; } // entrada / salida
        public int Usuario { get; set; }
        public bool EquipoPersonal { get; set; }
        public int Propietario { get; set; }
        public int RolAprobador { get; set; }
        public int SedeOs { get; set; }
    }
}
