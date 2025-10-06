using Interface.Services.Active_Directory;
using Microsoft.Extensions.Configuration;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Active_Directory;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ActiveDirectoryService : IActiveDirectoryService
    {
        private readonly IConfiguration _configuration;

        public ActiveDirectoryService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<UsuarioADDto>> ObtenerUsuariosAsync()
        {
            var usuarios = new List<UsuarioADDto>();

            return await Task.Run(() =>
            {
                var domain = _configuration["ActiveDirectory:Domain"];
                var username = _configuration["ActiveDirectory:Username"];
                var password = _configuration["ActiveDirectory:Password"];

                Console.WriteLine($"🟢 Conectando a LDAP: {domain} con usuario {username}");

                try
                {
                    using var entry = new DirectoryEntry($"LDAP://{domain}", username, password);

                    using var searcher = new DirectorySearcher(entry)
                    {
                        // 🔍 Solo usuarios activos (no grupos, no deshabilitados)
                        Filter = "(&(objectCategory=person)(objectClass=user)(!(userAccountControl:1.2.840.113556.1.4.803:=2)))"
                    };

                    // 🔧 Aumenta el límite de resultados (por defecto es 1000)
                    searcher.PageSize = 10000;

                    // 🔹 Propiedades que queremos obtener
                    searcher.PropertiesToLoad.Add("displayName");
                    searcher.PropertiesToLoad.Add("mail");
                    searcher.PropertiesToLoad.Add("sAMAccountName");

                    foreach (SearchResult result in searcher.FindAll())
                    {
                        var displayName = result.Properties["displayName"].Count > 0
                            ? result.Properties["displayName"][0].ToString()
                            : "";

                        var mail = result.Properties["mail"].Count > 0
                            ? result.Properties["mail"][0].ToString()
                            : "";

                        var sam = result.Properties["sAMAccountName"].Count > 0
                            ? result.Properties["sAMAccountName"][0].ToString()
                            : "";

                        usuarios.Add(new UsuarioADDto
                        {
                            NombreCompleto = displayName,
                            Correo = mail,
                            Usuario = sam
                        });
                    }

                    Console.WriteLine($"✅ Se encontraron {usuarios.Count} usuarios en AD.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error al consultar AD: {ex.Message}");
                }

                return usuarios;
            });
        }
    }
}
