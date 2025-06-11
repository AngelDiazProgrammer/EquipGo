using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Repositories
{
    public interface IEquipoPersonalRepositoy
    {
        IQueryable<EquiposPersonal> Query();
        Task AddAsync(EquiposPersonal entity);
        // Aquí podrías incluir otros métodos como Update, Delete, etc.
    }
}
