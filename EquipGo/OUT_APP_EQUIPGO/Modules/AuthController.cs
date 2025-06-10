// Controllers/AuthController.cs
using Interface.Services.Autenticacion;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using OUT_APP_EQUIPGO.Components.Pages.Auth;
using OUT_DOMAIN_EQUIPGO.Entities.Seguridad;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var usuario = await _authService.LoginAsync(model.NumeroDocumento, model.Contraseña);
        if (usuario != null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, $"{usuario.Nombre} {usuario.Apellido}"),
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim("id_usuarioSession", usuario.Id.ToString()),  // llamada al id del usuario!
                new Claim(ClaimTypes.Role, usuario.Rol?.NombreRol ?? "Usuario")
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties
                {
                    IsPersistent = model.RecordarContrasena
                });

            return Ok(new
            {
                mensaje = "Autenticación exitosa",
                rol = usuario.Rol?.NombreRol // 👈 Agrega esto
            });
        }

        return Unauthorized(new { mensaje = "Usuario o contraseña incorrectos." });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(new { mensaje = "Sesión cerrada" });
    }
}
