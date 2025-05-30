using Interface.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface
{
    public interface IUnitOfWork : IDisposable
    {
        IEquiposRepository Equipos { get; }
        IUsuariosInformacionRepository UsuariosInformacion { get; }
        ITransaccionesRepository Transacciones { get; }
        Task<int> CompleteAsync(); // Guarda cambios
    }
}
