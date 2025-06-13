using System;

namespace EquipGoAgent
{
    public class BackgroundAgent
    {
        public void Iniciar()
        {
            // Aquí irá toda la lógica del agente
            // Por ahora solo probamos con un log temporal
            try
            {
                string rutaLog = @"C:\ProgramData\EquipGo\Logs\log.txt";
                //Crear directorio
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(rutaLog));
                //Agregar log
                System.IO.File.AppendAllText(rutaLog, $"[INFO] Agente iniciado correctamente: {DateTime.Now}\n");
            }
            catch (Exception ex)
            {
                // En caso de error al guardar log
                System.IO.File.AppendAllText(@"C:\ProgramData\EquipGo\error.txt", ex.ToString());
            }
        }
    }
}
