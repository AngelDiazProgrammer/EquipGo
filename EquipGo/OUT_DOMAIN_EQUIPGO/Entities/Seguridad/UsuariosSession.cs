using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using OUT_DOMAIN_EQUIPGO.Entities.Seguridad;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_DOMAIN_EQUIPGO.Entities.Seguridad
{
    [Table("usuariosSession", Schema = "seguridad")]
    public class UsuariosSession
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("id_Tipodocumento")]
        public int IdTipodocumento { get; set; }

        [Column("numeroDocumento")]
        public string NumeroDocumento { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("apellido")]
        public string Apellido { get; set; }

        [Column("contraseña")]
        public string Contraseña { get; set; }

        [Column("id_estado")]
        public int IdEstado { get; set; }

        [Column("id_rol")]
        public int IdRol { get; set; }

        [Column("id_sede")]
        public int IdSede { get; set; }

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; }

        [Column("ultima_modificacion")]
        public DateTime UltimaModificacion { get; set; }

        // 🔥 Propiedades de navegación
        [ForeignKey("IdTipodocumento")]
        public virtual TipoDocumento TipoDocumento { get; set; }

        [ForeignKey("IdEstado")]
        public virtual Estado Estado { get; set; }

        [ForeignKey("IdRol")]
        public virtual Rol Rol { get; set; }

        [ForeignKey("IdSede")]
        public virtual Sedes Sede { get; set; }
    }
}