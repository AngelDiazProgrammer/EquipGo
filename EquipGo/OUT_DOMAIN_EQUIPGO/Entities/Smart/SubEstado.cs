using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_DOMAIN_EQUIPGO.Entities.Smart
{
    [Table("SubEstados", Schema = "smart")] // o el schema que uses
    public class SubEstado
    {
        [Column("id")]
        public int Id { get; set; }
        
        [Column("nombreSubEstado")]
        public string NombreSubEstado { get; set; } = string.Empty;
    }
}
