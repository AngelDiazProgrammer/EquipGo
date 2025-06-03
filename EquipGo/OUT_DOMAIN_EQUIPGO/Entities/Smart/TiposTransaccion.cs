using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_DOMAIN_EQUIPGO.Entities.Smart
{
    [Table("tiposTransaccion", Schema = "smart")]
    public class TiposTransaccion
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("nombreTransaccion")]
        public string NombreTransaccion { get; set; }
    }
}
