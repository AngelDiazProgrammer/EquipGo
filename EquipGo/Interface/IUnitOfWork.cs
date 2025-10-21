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
        IAreaRepository Areas { get; }
        ICampañaRepository Campaña { get; }
        IUsuariosSessionRepository UsuariosSession { get; }
        IEquipoPersonalRepositoy EquiposPersonal { get; }
        ITipoDocumentoRepository TipoDocumento { get; }
        IEstadosRepository Estados { get; }
        ISubEstadoRepository SubEstados { get; }
        ISedesRepository Sedes { get; }
        Task<int> CompleteAsync(); // Guarda cambios
    }
}
