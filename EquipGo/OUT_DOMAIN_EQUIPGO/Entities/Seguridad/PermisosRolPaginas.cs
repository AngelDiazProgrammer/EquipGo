using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_DOMAIN_EQUIPGO.Entities.Seguridad
{
    [Table("PermisosRolPaginas", Schema = "seguridad")]
    public class PermisosRolPaginas
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("id_rol")]
        public int IdRol { get; set; }

        [Column("id_Pagina")]
        public int IdPagina { get; set; }

        [Column("Estado")]
        public int Estado { get; set; }

        [Column("FechaRegistro")]
        public DateTime FechaRegistro { get; set; }

        [Column("UsuarioRegistroId")]
        public int UsuarioRegistroId { get; set; }

        [Column("FechaActualizacion")]
        public DateTime FechaActualizacion { get; set; }

        [Column("UsuarioActualizacionId")]
        public int UsuarioActualizacionId { get; set; }

        public virtual Rol IdRolNavigation { get; set; }
        public virtual Paginas IdPaginaNavigation { get; set; }
    }
}
