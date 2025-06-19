using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Repositories
{
    public interface IProveedoresRepository : IGenericRepository<Proveedores>
    {
        Task<Proveedores?> GetByIdAsync(int id);
    }
}
