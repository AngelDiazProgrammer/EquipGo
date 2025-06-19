using OUT_OS_APP.EQUIPGO.DTO.DTOs.Visitantes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Services.Visitantes
{
    public interface IRegistroVisitanteService
    {
        Task RegistrarVisitanteAsync(RegistroVisitanteDto dto);
    }
}