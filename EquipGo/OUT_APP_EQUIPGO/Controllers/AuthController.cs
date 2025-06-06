using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OUT_DOMAIN_EQUIPGO.Entities.Seguridad;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.AuthDTOs;
using System.Security.Claims;

namespace OUT_APP_EQUIPGO.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private static readonly AuthenticationProperties COOKIE_EXPIRE = new AuthenticationProperties()
        {
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20),
            IsPersistent = true
        };

        [HttpPost]
        [Route("api/auth/signin")]
        public async Task<ActionResult> SignInPost([FromBody] AuthRequest authRequest)
        {
            await SignOutPost();
            var claims = new List<Claim>
            {
                new Claim("Id",Convert.ToString(authRequest.Id)),
                new Claim(ClaimTypes.Name, authRequest.NombreUsuario),
                new Claim("rolId", Convert.ToString(authRequest.RolId))
            };

            var claimsIdentity = new ClaimsIdentity(claims,
                                                    CookieAuthenticationDefaults.AuthenticationScheme);
            //var authProperties = COOKIE_EXPIRES;
            var authProperties = new AuthenticationProperties()
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20),
                IsPersistent = true,
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                          new ClaimsPrincipal(claimsIdentity),
                                          authProperties);
            return this.Ok();
        }

        [HttpPost]
        [Route("api/auth/signout")]
        public async Task<ActionResult> SignOutPost()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // Limpiar todas las cookies asociadas al dominio
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
            return this.Ok();
        }


        [Authorize]
        [HttpPost]
        [Route("api/auth/getclaims")]
        public async Task<IActionResult> GetClaims()
        {
            var user = HttpContext.User;

            if (user == null || !user.Claims.Any())
            {
                return BadRequest("No claims found for the user.");
            }

            AuthResponse response = new AuthResponse()
            {
                Id = long.TryParse(user.FindFirst("Id")?.Value, out long IdValue) ? IdValue : 0,
                NombreUsuario = user.FindFirst(ClaimTypes.Name)?.Value,
                RolId = long.TryParse(user.FindFirst("rolId")?.Value, out long IdRol) ? IdRol : 0
            };

            return Ok(response);
        }

    }
}
