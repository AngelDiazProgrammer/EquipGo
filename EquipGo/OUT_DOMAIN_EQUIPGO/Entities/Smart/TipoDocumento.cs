using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_DOMAIN_EQUIPGO.Entities.Smart
{
    [Table("tipoDocumento", Schema = "smart")]
    public class TipoDocumento
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre_documento")]
        public string NombreDocumento { get; set; }
    }
}
