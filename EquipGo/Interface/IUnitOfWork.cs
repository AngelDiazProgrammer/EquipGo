using Interface.Repositories;
using OUT_DOMAIN_EQUIPGO.Entities.Configuracion;
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
        IAreaRepository Area { get; }
        ICampañaRepository Campaña { get; }
        IUsuariosSessionRepository UsuariosSession { get; }

        Task<int> CompleteAsync(); // Guarda cambios
    }
}
