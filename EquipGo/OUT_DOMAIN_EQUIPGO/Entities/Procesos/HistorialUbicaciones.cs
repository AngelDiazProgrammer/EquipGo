using OUT_DOMAIN_EQUIPGO.Entities.Configuracion;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_DOMAIN_EQUIPGO.Entities.Procesos
{
    [Table("historialUbicaciones", Schema = "procesos")]
    public class HistorialUbicaciones
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("id_equipo")]
        public int IdEquipo { get; set; }

        [Column("latitud")]
        public decimal Latitud { get; set; }

        [Column("longitud")]
        public decimal Longitud { get; set; }

        [Column("fecha")]
        public DateTime Fecha { get; set; }

        public virtual Equipos IdEquipoNavigation { get; set; }
    }
}
