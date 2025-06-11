using Interface.Repositories;
using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Services.Sedes
{
    public interface ISedesService
    {
        Task<List<OUT_DOMAIN_EQUIPGO.Entities.Smart.Sedes>> ObtenerTodasAsync();
    }
}
