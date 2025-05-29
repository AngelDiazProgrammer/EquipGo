using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_DOMAIN_EQUIPGO.Entities.Configuracion
{
    public class ZonasSedes
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitud { get; set; }
        public decimal ReadioMetros { get; set; }
        public int IdSede { get; set; }
    }
}
