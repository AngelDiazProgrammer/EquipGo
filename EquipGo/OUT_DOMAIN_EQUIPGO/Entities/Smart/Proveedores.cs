using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_DOMAIN_EQUIPGO.Entities.Smart
{
    [Table("Proveedores", Schema = "smart")]
    public class Proveedores
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre")]
        public string NombreProveedor { get; set; }
    }
}
