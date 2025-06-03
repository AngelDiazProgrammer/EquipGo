using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_DOMAIN_EQUIPGO.Entities.Smart
{
    [Table("Estado", Schema = "smart")]
    public class Estado
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre_estado")]
        public string NombreEstado { get; set; }
    }
}
