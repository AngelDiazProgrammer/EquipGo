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
    [Table("EquiposPorPersona", Schema = "procesos")]
    public class EquiposPorPersona
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("id_usuarioInfo")]
        public int IdUsuarioInfo { get; set; }

        [Column("id_equipo")]
        public int IdEquipo { get; set; }

        [Column("id_equipoPersonal")]
        public int IdEquipoPersonal { get; set; }

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; }

        [Column("fecha_desasignacion")]
        public DateTime? FechaDesasignacion { get; set; }

        public virtual UsuariosInformacion IdUsuarioInfoNavigation { get; set; }
        public virtual Equipos IdEquipoNavigation { get; set; }
        public virtual EquiposPersonal IdEquipoPersonalNavigation { get; set; }
    }
}
