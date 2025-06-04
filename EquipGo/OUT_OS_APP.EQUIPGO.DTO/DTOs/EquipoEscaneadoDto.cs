using System.Collections.Generic;

namespace OUT_OS_APP.EQUIPGO.DTO.DTOs
{
    public class EquipoEscaneadoDto
    {
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Serial { get; set; }
        public string Ubicacion { get; set; }
        public string CodigoBarras { get; set; }

        public string NombreUsuario { get; set; }
        public string DocumentoUsuario { get; set; }
        public string Area { get; set; }
        public string Campaña { get; set; }

        public int IdEquipoPersonal { get; set; }
        public int IdUsuarioInfo { get; set; }
        public int IdUsuarioSession { get; set; }   // ✅ Añadido para usar en scanner.razor
        public int IdSedeOs { get; set; }             // ✅ Añadido para usar en scanner.razor

        public int TipoTransaccionSugerido { get; set; } = 2; // Por defecto 'Salida'
        public List<string> HistorialTransacciones { get; set; } = new();
    }
}
