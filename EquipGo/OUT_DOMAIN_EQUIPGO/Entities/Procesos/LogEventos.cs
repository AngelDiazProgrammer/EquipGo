using OUT_DOMAIN_EQUIPGO.Entities.Seguridad;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_DOMAIN_EQUIPGO.Entities.Procesos
{
    [Table("LogEventos", Schema = "procesos")]
    public class LogEventos
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("id_usuarioSession")]
        public int IdUsuarioSession { get; set; }

        [Column("Accion")]
        public string Accion { get; set; }

        [Column("Descripcion")]
        public string Descripcion { get; set; }

        [Column("FechaEvento")]
        public DateTime FechaEvento { get; set; }

        [Column("TipoEvento")]
        public string TipoEvento { get; set; }

        [Column("Origen")]
        public string Origen { get; set; }

        [Column("IPUsuario")]
        public string IPUsuario { get; set; }

        public virtual UsuariosSession IdUsuarioSessionNavigation { get; set; }
    }
}
