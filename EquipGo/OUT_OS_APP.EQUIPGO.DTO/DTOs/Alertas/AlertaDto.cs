using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_OS_APP.EQUIPGO.DTO.DTOs.Alertas
{
    public class AlertaDto
    {
        public int Id { get; set; }
        public int IdEquipo { get; set; }
        public int IdTipoAlerta { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string NombreAlerta { get; set; } = string.Empty;
        public bool FueRevisada { get; set; }
        public string SerialEquipo { get; set; } = string.Empty;
        public string MarcaEquipo { get; set; } = string.Empty;
        public string ModeloEquipo { get; set; } = string.Empty;
        public int Contador {  get; set; }
        public bool EstaBloqueado => Contador >= 5 && IdTipoAlerta == 4;

        //Navegacion para usuario asignado

        public string UsuarioAsignado {  get; set; } = string.Empty;
    }
}
