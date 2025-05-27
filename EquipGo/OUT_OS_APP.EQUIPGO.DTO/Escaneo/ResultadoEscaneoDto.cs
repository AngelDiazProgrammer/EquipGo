using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_OS_APP.EQUIPGO.DTO.Escaneo
{
    public class ResultadoEscaneoDto
    {
        public bool Encontrado { get; set; }
        public string TipoTransaccion { get; set; } = "";
        public UsuarioDto? Usuario { get; set; }
        public EquipoDto? Equipo { get; set; }
        public List<HistorialTransaccionDto> Historial { get; set; } = new();
    }

    public class UsuarioDto
    {
        public string Nombre { get; set; } = "";
        public string Documento { get; set; } = "";
        public string Area { get; set; } = "";
        public string Campania { get; set; } = "";
    }

    public class EquipoDto
    {
        public string Marca { get; set; } = "";
        public string Modelo { get; set; } = "";
    }

    public class HistorialTransaccionDto
    {
        public string FechaHora { get; set; } = "";
        public string Tipo { get; set; } = "";
        public string Usuario { get; set; } = "";
    }
}
