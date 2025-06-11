using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Repositories
{
    public interface IEstadosRepository : IGenericRepository<Estado>
    {
        Task<Estado?> GetByIdAsync(int id);
    }
}
