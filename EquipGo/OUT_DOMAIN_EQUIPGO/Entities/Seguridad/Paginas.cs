using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_DOMAIN_EQUIPGO.Entities.Seguridad
{
    [Table("paginas", Schema = "seguridad")]
    public class Paginas
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("Nombre")]
        public string Nombre { get; set; }

        [Column("Descripcion")]
        public string Descripcion { get; set; }

        [Column("Orden")]
        public int Orden { get; set; }

        [Column("URL")]
        public string URL { get; set; }

        [Column("Icon")]
        public string Icon { get; set; }

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
    }
}
