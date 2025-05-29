using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_DOMAIN_EQUIPGO.Entities.Seguridad
{
    public class PermisosRolPaginas
    {
        public int Id { get; set; }
        public int IdRol { get; set; }
        public int IdPagina { get; set; }
        public int Estado { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int UsuarioRegistroId { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public int UsuarioActualizacionId { get; set; }
    }
}
