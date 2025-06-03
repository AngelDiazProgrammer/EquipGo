using OUT_DOMAIN_EQUIPGO.Entities.Seguridad;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_DOMAIN_EQUIPGO.Entities.Procesos
{
    [Table("BitacoraEventos", Schema = "procesos")]
    public class BitacoraEventos
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("id_usuarioSession")]
        public int IdUsuarioSession { get; set; }

        [Column("accion")]
        public string Accion { get; set; }

        [Column("descripcion")]
        public string Descripcion { get; set; }

        [Column("fecha")]
        public DateTime Fecha { get; set; }

        [Column("tipo_evento")]
        public string TipoEvento { get; set; }

        public virtual UsuariosSession IdUsuarioSessionNavigation { get; set; }
    }
}
