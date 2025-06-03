using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Services.Campañas
{
    public interface ICampañaService
    {
        Task<List<Campaña>> ObtenerTodasAsync();
    }
}
