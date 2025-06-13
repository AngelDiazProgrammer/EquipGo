using Microsoft.Win32;
using System.Reflection;

namespace EquipGo.Agent.Services
{
    public static class AutoStartManager
    {
        private const string AppName = "EquipGoAgent";

        public static void RegistrarInicioAutomatico()
        {
            string rutaExe = Assembly.GetExecutingAssembly().Location;

            using (RegistryKey clave = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Run", true))
            {
                if (clave.GetValue(AppName) == null || clave.GetValue(AppName)?.ToString() != rutaExe)
                {
                    clave.SetValue(AppName, rutaExe);
                }
            }
        }

        public static void QuitarInicioAutomatico()
        {
            using (RegistryKey clave = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Run", true))
            {
                clave.DeleteValue(AppName, false);
            }
        }
    }
}
