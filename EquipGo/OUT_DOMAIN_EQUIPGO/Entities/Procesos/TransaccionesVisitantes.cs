using Domain.Entities.Procesos;
using OUT_DOMAIN_EQUIPGO.Entities.Seguridad;
using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_DOMAIN_EQUIPGO.Entities.Procesos
{
    [Table("TransaccionesVisitantes", Schema = "procesos")]
    public class TransaccionesVisitantes
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("fechaTransaccion")]
        public DateTime FechaTransaccion { get; set; }

        [Column("id_equipoVisitante")]
        public int? IdEquipoVisitante { get; set; }

        [Column("tipoTransaccion")]
        public int IdTipoTransaccion { get; set; }

        [Column("id_usuarioSession")]
        public int IdUsuarioSession { get; set; }

        [Column("id_usuarioVisitante")]
        public int IdUsuarioVisitante { get; set; }

        [Column("sedeOs")]
        public int? SedeOs { get; set; }

        // Propiedades de navegación con ForeignKey explícito
        [ForeignKey(nameof(IdTipoTransaccion))]
        public virtual TiposTransaccion IdTipoTransaccionNavigation { get; set; }

        [ForeignKey(nameof(IdUsuarioVisitante))]
        public virtual UsuariosVisitantes IdUsuarioVisitanteNavigation { get; set; }

        [ForeignKey(nameof(IdEquipoVisitante))]
        public virtual EquiposVisitantes IdEquipoVisitanteNavigation { get; set; }

        [ForeignKey(nameof(IdUsuarioSession))]
        public virtual UsuariosSession IdUsuarioSessionNavigation { get; set; }

        [ForeignKey(nameof(SedeOs))]
        public virtual Sedes SedeOsNavigation { get; set; }
    }
}
