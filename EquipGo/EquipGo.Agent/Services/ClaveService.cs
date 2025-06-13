using System;
using System.IO;

namespace EquipGo.Agent.Services
{
    public static class ClaveService
    {
        private static readonly string rutaClave = @"C:\ProgramData\EquipGo\clave.txt";

        public static void GuardarClave(string clave)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(rutaClave));
            File.WriteAllText(rutaClave, clave);
        }

        public static string? LeerClave()
        {
            if (!File.Exists(rutaClave))
                return null;

            return File.ReadAllText(rutaClave).Trim();
        }

        public static bool ExisteClave()
        {
            return File.Exists(rutaClave);
        }
    }
}
