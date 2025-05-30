using OUT_OS_APP.EQUIPGO.DTO.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Interface.Services.Equipos
{
    public interface IEquipoService
    {
        Task<EquipoEscaneadoDto?> ConsultarPorCodigoBarrasAsync(string codigoBarras);


    }
}
