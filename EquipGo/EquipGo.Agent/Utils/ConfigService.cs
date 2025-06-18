// Utils/ConfigService.cs
using Microsoft.Extensions.Configuration;

namespace EquipGo.Agent.Utils
{
    public static class ConfigService
    {
        private static IConfigurationRoot _configuration;

        static ConfigService()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
        }

        public static string GetGoogleApiKey()
        {
            return _configuration["GoogleApi:Key"] ?? "";
        }
    }
}
