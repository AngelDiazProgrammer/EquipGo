using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_DOMAIN_EQUIPGO.Entities.Procesos
{
    public class AlertaGeofencing
    {
        public int Id { get; set; }
        public int IdEquipo { get; set; }
        public string TipoAlerta { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
        public int IdEquipoOs { get; set; }
    }
}
