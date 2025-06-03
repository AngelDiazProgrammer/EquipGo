using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_DOMAIN_EQUIPGO.Entities.Smart
{
    [Table("Sedes", Schema = "smart")]
    public class Sedes
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre_sede")]
        public string NombreSede { get; set; }
    }
}
