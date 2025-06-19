using Domain.Entities.Procesos;
using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Procesos
{
    [Table("EquiposVisitantes", Schema = "procesos")]
    public class EquiposVisitantes
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("marca")]
        public string Marca { get; set; } = string.Empty;

        [Column("modelo")]
        public string Modelo { get; set; } = string.Empty;

        [Column("serial")]
        public string Serial { get; set; } = string.Empty;

        [Column("id_usuarioVisitante")]
        public int IdUsuarioVisitante { get; set; }

        [Column("id_sede")]
        public int? IdSede { get; set; }

        [Column("fechaRegistro")]
        public DateTime FechaRegistro { get; set; }

        // Navegación
        [ForeignKey("IdUsuarioVisitante")]
        public UsuariosVisitantes UsuarioVisitante { get; set; } = null!;

        [ForeignKey("IdSede")]
        public Sedes? Sede { get; set; }
    }
}
