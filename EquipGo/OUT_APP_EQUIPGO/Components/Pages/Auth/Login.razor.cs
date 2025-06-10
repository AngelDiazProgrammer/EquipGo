using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace OUT_APP_EQUIPGO.Components.Pages.Auth
{
    public partial class Login : ComponentBase
    {
        [Inject] public IJSRuntime JS { get; set; }
        [Inject] public NavigationManager Navigation { get; set; }

        protected LoginModel loginModel { get; set; } = new();
        protected string mensaje { get; set; } = string.Empty;

        private bool _circuitReady = false;

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                _circuitReady = true;
            }
        }

        protected async Task IniciarSesion()
        {
            if (!_circuitReady)
            {
                mensaje = "⚠️ Por favor espera a que la conexión con el servidor esté lista.";
                return;
            }

            try
            {
                mensaje = "Procesando...";

                var resultado = await JS.InvokeAsync<LoginRespuesta>("authInterop.login", loginModel);

                mensaje = resultado.Mensaje;

                // Redirección según el rol
                if (resultado.Rol?.Equals("Admin", StringComparison.OrdinalIgnoreCase) == true)
                {
                    Navigation.NavigateTo("/AdminDashboard", forceLoad: true); // Forced
                }
                else if (resultado.Rol?.Equals("Guarda", StringComparison.OrdinalIgnoreCase) == true)
                {
                    Navigation.NavigateTo("/scanner", forceLoad: true); // Forced
                }
                else
                {
                    Navigation.NavigateTo("/AdminDashboard", forceLoad: true); // Forced
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Error al iniciar sesión: {ex.Message}";
            }
        }

        public class LoginRespuesta
        {
            public string Mensaje { get; set; }
            public string Rol { get; set; }
        }
    }

    public class LoginModel
    {
        public string NumeroDocumento { get; set; } = string.Empty;
        public string Contraseña { get; set; } = string.Empty;
        public bool RecordarContrasena { get; set; }
    }
}
