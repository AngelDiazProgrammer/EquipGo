using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_DOMAIN_EQUIPGO.Entities.Procesos
{
    public class EquiposPorPersona
    {
        public int Id { get; set; }
        public int IdUsuarioInfo { get; set; }
        public int IdEquipo { get; set; }
        public int IdEquipoPersonal { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaDesasignacion { get; set; }
    }
}
