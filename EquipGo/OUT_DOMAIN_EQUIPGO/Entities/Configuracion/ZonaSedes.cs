using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_DOMAIN_EQUIPGO.Entities.Configuracion
{
    [Table("zonasSedes", Schema = "configuracion")]
    public class ZonasSedes
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("latitude")]
        public decimal Latitude { get; set; }

        [Column("longitud")]
        public decimal Longitud { get; set; }

        [Column("readioMetros")]
        public decimal ReadioMetros { get; set; }

        [Column("id_Sede")]
        public int IdSede { get; set; }

        public virtual Sedes IdSedeNavigation { get; set; }
    }
}
