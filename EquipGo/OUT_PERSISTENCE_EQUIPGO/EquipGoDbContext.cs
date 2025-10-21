using Domain.Entities.Procesos;
using Microsoft.EntityFrameworkCore;
using OUT_DOMAIN_EQUIPGO.Entities.Configuracion;
using OUT_DOMAIN_EQUIPGO.Entities.Procesos;
using OUT_DOMAIN_EQUIPGO.Entities.Seguridad;
using OUT_DOMAIN_EQUIPGO.Entities.Smart;

namespace OUT_PERSISTENCE_EQUIPGO.Context
{
    public class EquipGoDbContext : DbContext
    {
        public EquipGoDbContext(DbContextOptions<EquipGoDbContext> options)
            : base(options)
        {
        }

        // SMART
        public DbSet<Estado> Estados { get; set; }
        public DbSet<SubEstado> SubEstados { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Campaña> Campañas { get; set; }
        public DbSet<EquiposPersonal> EquiposPersonales { get; set; }
        public DbSet<Sedes> Sedes { get; set; }
        public DbSet<TipoAlerta> TiposAlerta { get; set; }
        public DbSet<TipoDocumento> TipoDocumento { get; set; }
        public DbSet<TiposDispositivos> TiposDispositivos { get; set; }
        public DbSet<Proveedores> Proveedores { get; set; }
        public DbSet<TiposTransaccion> TiposTransaccion { get; set; }

        // PROCESOS
        public DbSet<UsuariosInformacion> UsuariosInformacion { get; set; }
        public DbSet<Transacciones> Transacciones { get; set; }
        public DbSet<AlertasGeofencing> AlertasGeofencing { get; set; }
        public DbSet<BitacoraEventos> BitacoraEventos { get; set; }
        public DbSet<HistorialUbicaciones> HistorialUbicaciones { get; set; }
        public DbSet<LogEventos> LogEventos { get; set; }
        public DbSet<EquiposPorPersona> EquiposPorPersona { get; set; }
        public DbSet<UsuariosVisitantes> UsuariosVisitantes { get; set; }
        public DbSet<EquiposVisitantes> EquiposVisitantes { get; set; }
        public DbSet<TransaccionesVisitantes> TransaccionesVisitantes { get; set; }


        // CONFIGURACION
        public DbSet<ZonasSedes> ZonasSedes { get; set; }
        public DbSet<Equipos> Equipos { get; set; }

        // SEGURIDAD
        public DbSet<UsuariosSession> UsuariosSession { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Paginas> Paginas { get; set; }
        public DbSet<PermisosRolPaginas> PermisosRolPaginas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ejemplo especial: código de barras como clave foránea
            modelBuilder.Entity<Equipos>()
    .HasIndex(e => e.CodigoBarras)
    .IsUnique();
        }
    }
}
