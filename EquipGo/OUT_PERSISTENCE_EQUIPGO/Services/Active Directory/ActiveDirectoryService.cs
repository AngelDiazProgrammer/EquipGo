using Interface.Services.Active_Directory;
using Microsoft.Extensions.Configuration;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Active_Directory;
using OUT_OS_APP.EQUIPGO.DTO.DTOs.Active_Directory.OUT_OS_APP.EQUIPGO.DTO.DTOs.Active_Directory;
using System;
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
                    searcher.PropertiesToLoad.Add("displayName");      // Nombre completo
                    searcher.PropertiesToLoad.Add("givenName");        // Nombre de pila
                    searcher.PropertiesToLoad.Add("sn");               // Apellido(s) - surname
                    searcher.PropertiesToLoad.Add("mail");             // Correo electrónico
                    searcher.PropertiesToLoad.Add("sAMAccountName");   // Usuario de red
                    searcher.PropertiesToLoad.Add("description");      // Descripción

                    foreach (SearchResult result in searcher.FindAll())
                    {
                        var displayName = ObtenerPropiedad(result, "displayName");
                        var givenName = ObtenerPropiedad(result, "givenName");
                        var surname = ObtenerPropiedad(result, "sn");
                        var mail = ObtenerPropiedad(result, "mail");
                        var sam = ObtenerPropiedad(result, "sAMAccountName");
                        var description = ObtenerPropiedad(result, "description");

                        // 🔧 Procesamiento de apellidos
                        var (primerApellido, segundoApellido) = SepararApellidos(surname);

                        usuarios.Add(new UsuarioADDto
                        {
                            // Propiedades existentes
                            NombreCompleto = displayName,
                            Usuario = sam,
                            Correo = mail,
                            Descripcion = description,
                            Dominio = domain,

                            // Nuevas propiedades
                            Nombre = givenName,
                            Apellidos = surname,
                            PrimerApellido = primerApellido,
                            SegundoApellido = segundoApellido
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


        #region Metodos privados
        /// Método auxiliar para obtener una propiedad del resultado de AD
        private string ObtenerPropiedad(SearchResult result, string propertyName)
        {
            try
            {
                if (result.Properties[propertyName].Count > 0)
                {
                    var value = result.Properties[propertyName][0]?.ToString();
                    return value ?? string.Empty;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error al obtener propiedad '{propertyName}': {ex.Message}");
            }

            return string.Empty;
        }

        /// Separa los apellidos en primer y segundo apellido.
        /// Asume que el AD puede tener uno o dos apellidos separados por espacio.
        private (string primerApellido, string segundoApellido) SepararApellidos(string apellidosCompletos)
        {
            if (string.IsNullOrWhiteSpace(apellidosCompletos))
                return (string.Empty, string.Empty);

            // Eliminar espacios extras
            apellidosCompletos = apellidosCompletos.Trim();

            // Buscar el primer espacio para separar apellidos
            var partes = apellidosCompletos.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);

            if (partes.Length == 0)
                return (string.Empty, string.Empty);

            if (partes.Length == 1)
                return (partes[0], string.Empty);

            return (partes[0], partes[1]);
        }

        #endregion
    }
}