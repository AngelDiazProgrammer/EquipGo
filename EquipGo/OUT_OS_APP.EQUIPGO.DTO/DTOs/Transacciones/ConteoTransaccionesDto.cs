using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_OS_APP.EQUIPGO.DTO.DTOs.Transacciones
{
    public class ConteoTransaccionesDto
    {
        public int TotalHoy { get; set; }
        public int TotalPersonales { get; set; }
        public int TotalCorporativos { get; set; }
        public int TotalProveedores { get; set; }

    }

}
