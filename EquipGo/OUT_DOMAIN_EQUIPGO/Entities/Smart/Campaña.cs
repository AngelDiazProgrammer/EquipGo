using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_DOMAIN_EQUIPGO.Entities.Smart
{
    [Table("campaña", Schema = "smart")]
    public class Campaña
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre_campaña")]
        public string NombreCampaña { get; set; }
    }
}
