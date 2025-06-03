using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_DOMAIN_EQUIPGO.Entities.Smart
{
    [Table("tiposDispositivos", Schema = "smart")]
    public class TiposDispositivos
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre_tipo")]
        public string NombreTipo { get; set; }
    }
}
