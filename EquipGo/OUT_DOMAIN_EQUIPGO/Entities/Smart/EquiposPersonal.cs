using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_DOMAIN_EQUIPGO.Entities.Smart
{
    [Table("equiposPersonal", Schema = "smart")]
    public class EquiposPersonal
    {
        public readonly string TipoEquipo;

        [Column("id")]
        public int Id { get; set; }

        [Column("nombre_personal")]
        public string NombrePersonal { get; set; }
    }
}
