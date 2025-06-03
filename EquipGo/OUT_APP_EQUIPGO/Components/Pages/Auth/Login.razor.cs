using Interface.Services.Autenticacion;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Threading.Tasks;

namespace OUT_APP_EQUIPGO.Components.Pages.Auth
{
    public partial class LoginBase : ComponentBase
    {
        [Inject] public IAuthService AuthService { get; set; }
        [Inject] public AuthenticationStateProvider AuthProvider { get; set; }
        [Inject] public NavigationManager Navigation { get; set; }

        protected LoginModel loginModel { get; set; } = new();
        protected string mensaje { get; set; } = string.Empty;

        protected async Task IniciarSesion()
        {
            try
            {
                Console.WriteLine("🚨 IniciarSesion() llamado");
                mensaje = "Método ejecutado correctamente.";

                var usuario = await AuthService.LoginAsync(loginModel.NumeroDocumento, loginModel.Contraseña);
                if (usuario != null)
                {
                    if (AuthProvider is CustomAuthenticationStateProvider customAuth)
                    {
                        await customAuth.SetUsuarioAutenticadoAsync(usuario);
                    }

                    mensaje = $"Bienvenido, {usuario.Nombre} {usuario.Apellido} (Rol: {usuario.Rol.NombreRol})";

                    // 👇 Redirección según el rol
                    if (usuario.Rol.NombreRol.Equals("Guarda", StringComparison.OrdinalIgnoreCase))
                    {
                        Navigation.NavigateTo("/scanner");
                    }
                    else if (usuario.Rol.NombreRol.Equals("Administrador", StringComparison.OrdinalIgnoreCase))
                    {
                        Navigation.NavigateTo("/");
                    }
                    else
                    {
                        // Redirección por defecto
                        Navigation.NavigateTo("/");
                    }
                }
                else
                {
                    mensaje = "Usuario o contraseña incorrectos.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al iniciar sesión: {ex.Message}");
                mensaje = $"Error al iniciar sesión: {ex.Message}";
            }
        }
    }

    public class LoginModel
    {
        public string NumeroDocumento { get; set; } = string.Empty;
        public string Contraseña { get; set; } = string.Empty;
        public bool RecordarContrasena { get; set; } 
    }
}
