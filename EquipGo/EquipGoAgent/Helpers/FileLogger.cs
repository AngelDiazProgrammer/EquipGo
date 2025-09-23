using System;
using System.IO;

namespace EquipGo.Agent
{
    public class FileLogger
    {
        private readonly string _logPath;

        public FileLogger()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string folder = Path.Combine(appData, "EquipGo.Agent");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            _logPath = Path.Combine(folder, "logs.txt");
        }

        public void Log(string message)
        {
            string line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            File.AppendAllText(_logPath, line + Environment.NewLine);
        }
    }
}
