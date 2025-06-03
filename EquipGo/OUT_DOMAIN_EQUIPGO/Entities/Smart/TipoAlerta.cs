using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_DOMAIN_EQUIPGO.Entities.Smart
{
    [Table("tipoAlerta", Schema = "smart")]
    public class TipoAlerta
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre_alerta")]
        public string NombreAlerta { get; set; }
    }
}
