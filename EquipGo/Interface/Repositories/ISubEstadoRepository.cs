using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Repositories
{
    public interface ISubEstadoRepository : IGenericRepository<SubEstado>
    {
        Task<SubEstado?> GetByIdAsync(int id);
    }
}
