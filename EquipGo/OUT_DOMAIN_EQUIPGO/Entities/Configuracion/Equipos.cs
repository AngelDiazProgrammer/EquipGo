using OUT_DOMAIN_EQUIPGO.Entities.Procesos;
using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_DOMAIN_EQUIPGO.Entities.Configuracion
{
    [Table("Equipos", Schema = "configuracion")]
    public class Equipos
    {
        public int Id { get; set; }
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public string? Serial { get; set; }
        public string? CodigoBarras { get; set; }
        public string? Ubicacion { get; set; }

        [Column("id_usuarioInfo")]
        public int? IdUsuarioInfo { get; set; }

        [Column("id_estado")]
        public int? IdEstado { get; set; }

        [Column("id_equipoPersonal")]
        public int? IdEquipoPersonal { get; set; }

        [Column("id_Sede")]
        public int? IdSede { get; set; }

        [Column("id_tipoDispositivo")]
        public int? IdTipoDispositivo { get; set; }

        public double? Latitud { get; set; }
        public double? Longitud { get; set; }

        [Column("sistema_operativo")]
        public string? SistemaOperativo { get; set; }

        [Column("mac_equipo")]
        public string? MacEquipo { get; set; }

        [Column("version_software")]
        public string? VersionSoftware { get; set; }

        [Column("fecha_creacion")]
        public DateTime? FechaCreacion { get; set; }

        [Column("ultima_modificacion")]
        public DateTime? UltimaModificacion { get; set; }

        // 🔥 AGREGAR ESTAS PROPIEDADES DE NAVEGACIÓN:
        [ForeignKey("IdUsuarioInfo")]
        public virtual UsuariosInformacion IdUsuarioInfoNavigation { get; set; }

        [ForeignKey("IdEstado")]
        public virtual Estado Estado { get; set; }

        [ForeignKey("IdEquipoPersonal")]
        public virtual EquiposPersonal IdEquipoPersonalNavigation { get; set; }

        [ForeignKey("IdSede")]
        public virtual Sedes IdSedeNavigation { get; set; }

        [ForeignKey("IdTipoDispositivo")]
        public virtual TiposDispositivos IdTipoDispositivoNavigation { get; set; }
    }
}
