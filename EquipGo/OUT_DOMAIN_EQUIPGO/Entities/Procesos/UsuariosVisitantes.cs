using System.ComponentModel.DataAnnotations.Schema;
using OUT_DOMAIN_EQUIPGO.Entities.Smart;

namespace Domain.Entities.Procesos
{
    [Table("UsuariosVisitantes", Schema = "procesos")]
    public class UsuariosVisitantes
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("tipoDocumento")]
        public string TipoDocumento { get; set; } = string.Empty;

        [Column("numeroDocumento")]
        public string NumeroDocumento { get; set; } = string.Empty;

        [Column("nombres")]
        public string Nombres { get; set; } = string.Empty;

        [Column("apellidos")]
        public string Apellidos { get; set; } = string.Empty;

        [Column("tipoUsuario")]
        public string TipoUsuario { get; set; } = string.Empty;

        [Column("id_proveedor")]
        public int? IdProveedor { get; set; }

        [Column("fechaRegistro")]
        public DateTime FechaRegistro { get; set; }

        // Navegación
        [ForeignKey("IdProveedor")]
        public Proveedores? Proveedor { get; set; }

        public ICollection<EquiposVisitantes>? Equipos { get; set; }
    }
}
