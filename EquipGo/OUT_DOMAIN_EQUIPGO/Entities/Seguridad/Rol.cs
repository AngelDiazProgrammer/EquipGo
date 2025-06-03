using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_DOMAIN_EQUIPGO.Entities.Seguridad
{
    [Table("roles", Schema = "seguridad")]
    public class Rol
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre_rol")]
        public string NombreRol { get; set; }

        [Column("id_estado")]
        public int IdEstado { get; set; }

        [ForeignKey("IdEstado")]
        public virtual Estado Estado { get; set; }
    }
}
