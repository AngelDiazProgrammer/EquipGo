using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace OUT_DOMAIN_EQUIPGO.Entities.Procesos
{
    [Table("ContadoresGeofencing", Schema = "procesos")]
    public class ContadorGeofencing
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("serial")]
        public string Serial { get; set; }

        [Column("contador")]
        public int Contador { get; set; }

        [Column("fecha_ultima_notificacion")]
        public DateTime? FechaUltimaNotificacion { get; set; }

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; }

        [Column("fecha_actualizacion")]
        public DateTime FechaActualizacion { get; set; }
    }
}