using Interface.Repositories;
using System;
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
        IEquipoPersonalRepositoy EquiposPersonal { get; }
        ITipoDocumentoRepository TipoDocumento { get; }
        IEstadosRepository Estados { get; }
        ISedesRepository Sedes { get; }
        Task<int> CompleteAsync(); // Guarda cambios
    }
}
