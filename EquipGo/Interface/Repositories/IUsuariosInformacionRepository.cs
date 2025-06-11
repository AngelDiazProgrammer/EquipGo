using OUT_DOMAIN_EQUIPGO.Entities.Procesos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Repositories
{
    public interface IUsuariosInformacionRepository : IGenericRepository<UsuariosInformacion>
    {
        IQueryable<UsuariosInformacion> Query();
        Task AddAsync(UsuariosInformacion entity);
    }
}
