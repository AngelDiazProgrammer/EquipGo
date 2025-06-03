using OUT_DOMAIN_EQUIPGO.Entities.Seguridad;
using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_DOMAIN_EQUIPGO.Entities.Procesos
{
    [Table("Transacciones", Schema = "procesos")]
    public class Transacciones
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("codigoBarras")]
        public string CodigoBarras { get; set; }

        [Column("id_tipoTransaccion")]
        public int IdTipoTransaccion { get; set; }

        [Column("id_equipoPersonal")]
        public int IdEquipoPersonal { get; set; }

        [Column("id_usuarioInfo")]
        public int IdUsuarioInfo { get; set; }

        [Column("id_usuarioSession")]
        public int IdUsuarioSession { get; set; }

        [Column("sedeOs")]
        public int SedeOs { get; set; }

        [Column("fechaHora")]
        public DateTime FechaHora { get; set; }

        public virtual TiposTransaccion IdTipoTransaccionNavigation { get; set; }
        public virtual EquiposPersonal IdEquipoPersonalNavigation { get; set; }
        public virtual UsuariosInformacion IdUsuarioInfoNavigation { get; set; }
        public virtual UsuariosSession IdUsuarioSessionNavigation { get; set; }
        public virtual Sedes SedeOsNavigation { get; set; }
    }
}
