using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Services.Areas
{
    public interface IAreaService
    {
        Task<List<Area>> ObtenerTodasAsync();
    }
}
