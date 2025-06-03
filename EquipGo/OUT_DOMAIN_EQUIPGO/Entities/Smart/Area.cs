using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_DOMAIN_EQUIPGO.Entities.Smart
{
    [Table("area", Schema = "smart")]
    public class Area
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre_area")]
        public string NombreArea { get; set; }
    }
}
