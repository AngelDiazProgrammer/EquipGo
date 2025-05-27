using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_DOMAIN_EQUIPGO.Entities.Procesos
{
    public class EquipoPorPersona
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public int IdEquipoOs { get; set; }
        public int IdEquipoPersonal { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaDesasignacion { get; set; }
    }
}
