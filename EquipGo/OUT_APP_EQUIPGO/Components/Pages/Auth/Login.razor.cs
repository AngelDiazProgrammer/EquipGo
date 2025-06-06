using Interface.Services.Autenticacion;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using OUT_DOMAIN_EQUIPGO.Entities.Seguridad;


namespace OUT_APP_EQUIPGO.Components.Pages.Auth
{
    public partial class LoginBase : ComponentBase
    {
        [Inject] public IAuthService AuthService { get; set; }
        [Inject] public AuthenticationStateProvider AuthProvider { get; set; }
        [Inject] public NavigationManager Navigation { get; set; }
        [Inject] public IJSRuntime _Js { get; set; }

        protected LoginModel loginModel { get; set; } = new();
        protected string mensaje { get; set; } = string.Empty;


        protected override async Task OnAfterRenderAsync(bool firtsRender)
        {
            if (!firtsRender)
            {
                return;
            }
            await _Js.InvokeVoidAsync("SignOutLogin");
        }

        protected async Task IniciarSesion()
        {
            try
            {
                Console.WriteLine("🚨 IniciarSesion() llamado");
                mensaje = "Método ejecutado correctamente.";

                UsuariosSession usuario = await AuthService.LoginAsync(loginModel.NumeroDocumento, loginModel.Contraseña);

                if(usuario == null)
                {
                    mensaje = "Usuario o contraseña incorrectos.";
                    return;
                }


                await IniciarSesionApp(usuario);

                mensaje = $"Bienvenido, {usuario.Nombre} {usuario.Apellido} (Rol: {usuario.Rol.NombreRol})";

                // 👇 Redirección según el rol
                if (usuario.Rol.NombreRol.Equals("Guarda", StringComparison.OrdinalIgnoreCase))
                {
                    Navigation.NavigateTo("/scanner");
                }
                else if (usuario.Rol.NombreRol.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                {
                    Navigation.NavigateTo("/AdminDashboard");
                }
                else
                {
                    // Redirección por defecto
                    Navigation.NavigateTo("/AdminDashboard");
                }



            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al iniciar sesión: {ex.Message}");
                mensaje = $"Error al iniciar sesión: {ex.Message}";
            }
        }

        private async Task IniciarSesionApp(UsuariosSession User)
        {
            await _Js.InvokeAsync<string>("SignIn", User.Id,User.Nombre, User.IdRol);
        }
    }

    public class LoginModel
    {
        public string NumeroDocumento { get; set; } = string.Empty;
        public string Contraseña { get; set; } = string.Empty;
        public bool RecordarContrasena { get; set; } 
    }
}
