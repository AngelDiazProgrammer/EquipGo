﻿namespace EquipGo.Agent.Models
{
    public class EquipoSyncRequestDto
    {
        public string Serial { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string MacEquipo { get; set; }
        public string SistemaOperativo { get; set; }
        public string VersionSoftware { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }

        public string? CodigoBarras { get; set; }
        public int? IdUsuarioInfo { get; set; }
        public int? IdEstado { get; set; }
        public int? IdSede { get; set; }
        public int? IdEquipoPersonal { get; set; }
        public int? IdTipoDispositivo { get; set; }
        public int? IdProveedor { get; set; }
    }
}
