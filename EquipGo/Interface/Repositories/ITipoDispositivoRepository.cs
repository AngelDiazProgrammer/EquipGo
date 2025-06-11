using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Repositories
{
    public interface ITipoDispositivoRepository : IGenericRepository<TiposDispositivos>
    {
        Task<TiposDispositivos?> GetByIdAsync(int id);
    }
}
