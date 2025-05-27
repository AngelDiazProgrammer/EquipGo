using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_DOMAIN_EQUIPGO.Entities.Procesos
{
    public class LogEvento
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Accion { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaEvento { get; set; }
        public string TipoEvento { get; set; }
        public string Origen { get; set; }
        public string IPUsuario { get; set; }
    }
}
