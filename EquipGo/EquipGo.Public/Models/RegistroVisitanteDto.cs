namespace EquipGo.Public.Models
{
    public class RegistroVisitanteDto
    {
        public string TipoDocumento { get; set; } = string.Empty;
        public string NumeroDocumento { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string TipoUsuario { get; set; } = string.Empty;
        public int? IdProveedor { get; set; }
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string Serial { get; set; } = string.Empty;
        public string FotoBase64 { get; set; } = string.Empty;

    }

    public class RegistroVisitanteViewModel
    {
        public RegistroVisitanteDto Visitante { get; set; } = new();
        public List<TipoDocumentoDto> TiposDocumento { get; set; } = new();
        public int PasoActual { get; set; } = 0;
        public bool InicioFormulario { get; set; } = false;
        public string Mensaje { get; set; } = "";
        public bool RegistroCompletado { get; set; } = false;
        public List<ProveedorDto> Proveedores { get; set; } = new();
    }
}