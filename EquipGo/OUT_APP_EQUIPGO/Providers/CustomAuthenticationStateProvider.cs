using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using OUT_DOMAIN_EQUIPGO.Entities.Seguridad;
using System.Security.Claims;
using System.Text.Json;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IJSRuntime _jsRuntime;
    private const string UserSessionKey = "UsuarioSession";
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

    public CustomAuthenticationStateProvider(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task SetUsuarioAutenticadoAsync(UsuariosSession usuario)
    {
        // Serializa el usuario en LocalStorage
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", UserSessionKey, JsonSerializer.Serialize(usuario));

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task LimpiarUsuarioAsync()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", UserSessionKey);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var usuarioJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", UserSessionKey);
            if (!string.IsNullOrWhiteSpace(usuarioJson))
            {
                var usuario = JsonSerializer.Deserialize<UsuariosSession>(usuarioJson);

                if (usuario != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, $"{usuario.Nombre} {usuario.Apellido}"),
                        new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                        new Claim(ClaimTypes.Role, usuario.Rol?.NombreRol ?? "Usuario")
                    };

                    var identity = new ClaimsIdentity(claims, "CustomAuth");
                    var user = new ClaimsPrincipal(identity);

                    return await Task.FromResult(new AuthenticationState(user));
                }
            }
        }
        catch
        {
            // Manejo de errores si quieres
        }

        return await Task.FromResult(new AuthenticationState(_anonymous));
    }
}
