using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

// Importa todas las entidades agrupadas por esquema
using OUT_DOMAIN_EQUIPGO.Entities.Configuracion;
using OUT_DOMAIN_EQUIPGO.Entities.Procesos;
using OUT_DOMAIN_EQUIPGO.Entities.Seguridad;
using OUT_DOMAIN_EQUIPGO.Entities.Smart;

namespace OUT_PERSISTENCE_EQUIPGO
{
    public class EquipGoDbContext : DbContext
    {
        public EquipGoDbContext(DbContextOptions<EquipGoDbContext> options)
            : base(options)
        {
        }

        // DbSets - representan las tablas
        public DbSet<Equipo> Equipos { get; set; }
        public DbSet<ZonaSede> ZonasSedes { get; set; }
        public DbSet<Sede> Sedes { get; set; }

        public DbSet<Transaccion> Transacciones { get; set; }
        public DbSet<UsuarioInformacion> UsuariosInformacion { get; set; }
        public DbSet<BitacoraEventos> BitacoraEventos { get; set; }
        public DbSet<HistorialUbicacion> HistorialUbicaciones { get; set; }
        public DbSet<AlertaGeofencing> AlertasGeofencing { get; set; }
        public DbSet<EquipoPorPersona> EquiposPorPersona { get; set; }
        public DbSet<LogEvento> LogEventos { get; set; }

        public DbSet<UsuarioSession> UsuariosSession { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Pagina> Paginas { get; set; }
        public DbSet<PermisosRolPaginas> PermisosRolPaginas { get; set; }

        public DbSet<TipoDocumento> TiposDocumento { get; set; }
        public DbSet<TipoDispositivo> TiposDispositivos { get; set; }
        public DbSet<Estado> Estados { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)

        {
            // Esquema: configuracion
            modelBuilder.Entity<Equipo>().ToTable("equipos", "configuracion");
            modelBuilder.Entity<ZonaSede>().ToTable("zonasSedes", "configuracion");

            // Esquema: smart
            modelBuilder.Entity<Sede>().ToTable("Sedes", "smart");
            modelBuilder.Entity<TipoDocumento>().ToTable("tipoDocumento", "smart");
            modelBuilder.Entity<TipoDispositivo>().ToTable("tiposDispositivos", "smart");
            modelBuilder.Entity<Estado>().ToTable("Estado", "smart");

            // Esquema: procesos
            modelBuilder.Entity<Transaccion>().ToTable("Transacciones", "procesos");
            modelBuilder.Entity<UsuarioInformacion>().ToTable("usuariosInformacion", "procesos");
            modelBuilder.Entity<BitacoraEventos>().ToTable("BitacoraEventos", "procesos");
            modelBuilder.Entity<HistorialUbicacion>().ToTable("historialUbicaciones", "procesos");
            modelBuilder.Entity<AlertaGeofencing>().ToTable("AlertasGeofencing", "procesos");
            modelBuilder.Entity<EquipoPorPersona>().ToTable("EquiposPorPersona", "procesos");
            modelBuilder.Entity<LogEvento>().ToTable("LogEventos", "procesos");

            // Esquema: seguridad
            modelBuilder.Entity<UsuarioSession>().ToTable("usuariosSession", "seguridad");
            modelBuilder.Entity<Rol>().ToTable("roles", "seguridad");
            modelBuilder.Entity<Pagina>().ToTable("paginas", "seguridad");
            modelBuilder.Entity<PermisosRolPaginas>().ToTable("PermisosRolPaginas", "seguridad");

            base.OnModelCreating(modelBuilder);
        }
    }
}
