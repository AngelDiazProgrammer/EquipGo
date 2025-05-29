using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_DOMAIN_EQUIPGO.Entities.Procesos
{
    public class AlertasGeofencing
    {
        public int Id { get; set; }
        public int IdEquipo { get; set; }
        public int IdTipoAlerta { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
    }
}
