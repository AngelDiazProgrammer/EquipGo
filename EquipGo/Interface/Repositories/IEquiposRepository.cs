using OUT_DOMAIN_EQUIPGO.Entities.Configuracion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Repositories
{
    public interface IEquiposRepository : IGenericRepository<Equipos>
    {
        Task<Equipos?> GetByCodigoBarrasAsync(string codigoBarras);
    }
}
