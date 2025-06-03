using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace OUT_DOMAIN_EQUIPGO.Entities.Procesos
{
    [Table("usuariosInformacion", Schema = "procesos")]
    public class UsuariosInformacion
    {
        public int Id { get; set; }

        [Column("id_Tipodocumento")]
        public int IdTipodocumento { get; set; }

        [Column("numeroDocumento")]
        public string NumeroDocumento { get; set; }

        public string Nombres { get; set; }
        public string Apellidos { get; set; }

        [Column("id_area")]
        public int IdArea { get; set; }

        [Column("id_campaña")]
        public int IdCampaña { get; set; }

        [Column("id_estado")]
        public int IdEstado { get; set; }

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; }

        [Column("ultima_modificacion")]
        public DateTime UltimaModificacion { get; set; }

        // 🔥 Propiedades de navegación para relaciones foráneas:
        [ForeignKey("IdTipodocumento")]
        public virtual TipoDocumento IdTipodocumentoNavigation { get; set; }

        [ForeignKey("IdArea")]
        public virtual Area IdAreaNavigation { get; set; }

        [ForeignKey("IdCampaña")]
        public virtual Campaña IdCampañaNavigation { get; set; }

        [ForeignKey("IdEstado")]
        public virtual Estado Estado { get; set; }
    }
}
