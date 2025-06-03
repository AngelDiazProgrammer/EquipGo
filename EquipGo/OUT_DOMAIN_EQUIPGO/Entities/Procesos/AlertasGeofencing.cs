using OUT_DOMAIN_EQUIPGO.Entities.Configuracion;
using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_DOMAIN_EQUIPGO.Entities.Procesos
{
    [Table("AlertasGeofencing", Schema = "procesos")]
    public class AlertasGeofencing
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("id_equipo")]
        public int IdEquipo { get; set; }

        [Column("id_tipoAlerta")]
        public int IdTipoAlerta { get; set; }

        [Column("descripcion")]
        public string Descripcion { get; set; }

        [Column("fecha")]
        public DateTime Fecha { get; set; }

        public virtual Equipos IdEquipoNavigation { get; set; }
        public virtual TipoAlerta IdTipoAlertaNavigation { get; set; }
    }
}
