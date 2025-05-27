using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_DOMAIN_EQUIPGO.Entities.Configuracion
{
    public class Equipo
    {
        public int Id { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Serial { get; set; }
        public string CodigoBarras { get; set; }
        public int Propietario { get; set; }
        public string Ubicacion { get; set; }
        public int EstadoId { get; set; }
        public bool EquipoPersonal { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public string TipoDispositivo { get; set; }
        public string SistemaOperativo { get; set; }
        public string MacEquipo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime UltimaModificacion { get; set; }
        public string VersionSoftware { get; set; }
        public int SedeOs { get; set; }
    }
}
