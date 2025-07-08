using Microsoft.Win32;
using System;

namespace EquipGo.Agent
{
    public static class AutoStartManager
    {
        public static void RegistrarInicioAutomaticoParaTodos()
        {
            string rutaExe = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            key?.SetValue("EquipGoAgent", $"\"{rutaExe}\"");
        }
    }
}
