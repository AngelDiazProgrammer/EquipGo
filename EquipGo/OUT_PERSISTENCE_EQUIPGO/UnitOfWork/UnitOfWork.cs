using Interface;
using Interface.Repositories;
using OUT_DOMAIN_EQUIPGO.Entities.Smart;
using OUT_PERSISTENCE_EQUIPGO.Context;
using OUT_PERSISTENCE_EQUIPGO.Repositories;

namespace OUT_PERSISTENCE_EQUIPGO.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EquipGoDbContext _context;

        public UnitOfWork(EquipGoDbContext context)
        {
            _context = context;
            Equipos = new EquiposRepository(_context);
            UsuariosInformacion = new UsuariosInformacionRepository(_context);
            Transacciones = new TransaccionesRepository(_context);
            Areas = new AreaRepository(_context);
            Campaña = new CampañaRepository(_context);
            UsuariosSession = new UsuariosSessionRepository(_context);
            EquiposPersonal = new EquipoPersonalRepository(_context); 
            TipoDocumento = new TipoDocumentoRepository(_context);
            Estados = new EstadosRepository(_context);
            SubEstados = new SubEstadoRepository(_context);
            Sedes = new SedesRepository(_context);
            TipoDispositivo = new TipoDispositivoRepository(_context);
        }

        public IEquiposRepository Equipos { get; }
        public IUsuariosInformacionRepository UsuariosInformacion { get; }
        public ITransaccionesRepository Transacciones { get; }
        public IAreaRepository Areas { get; }
        public ICampañaRepository Campaña { get; }
        public IUsuariosSessionRepository UsuariosSession { get; }
        public IEquipoPersonalRepositoy EquiposPersonal { get; }
        public ITipoDocumentoRepository TipoDocumento { get; }
        public IEstadosRepository Estados { get; }
        public ISubEstadoRepository SubEstados { get; }
        public ISedesRepository Sedes { get; }
        public ITipoDispositivoRepository TipoDispositivo { get; }
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
