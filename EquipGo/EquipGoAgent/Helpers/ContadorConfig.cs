using System;
using System.IO;
using System.Windows.Forms;

namespace EquipGo.Agent
{
    public static class ContadorPersistente
    {
        private static readonly string ConfigDirectory =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "EquipGo");

        private static readonly string ConfigPath =
            Path.Combine(ConfigDirectory, "contador.txt");

        public static int ObtenerContador()
        {
            try
            {
                FileLogger logger = new FileLogger();
                logger.Log($"🔍 Buscando archivo de configuración en: {ConfigPath}");

                if (!File.Exists(ConfigPath))
                {
                    logger.Log("📄 Archivo de configuración no encontrado, usando contador: 0");
                    return 0;
                }

                string contenido = File.ReadAllText(ConfigPath);
                if (int.TryParse(contenido.Trim(), out int contador))
                {
                    logger.Log($"📄 Contador recuperado del archivo: {contador}");
                    return contador;
                }
                else
                {
                    logger.Log("⚠️ Contenido del archivo no válido, usando contador: 0");
                    return 0;
                }
            }
            catch (Exception ex)
            {
                FileLogger logger = new FileLogger();
                logger.Log($"❌ Error recuperando contador: {ex.Message}");
                return 0;
            }
        }

        public static void GuardarContador(int contador)
        {
            try
            {
                FileLogger logger = new FileLogger();
                logger.Log($"💾 Intentando guardar contador: {contador}");

                // Asegurar que el directorio existe
                if (!Directory.Exists(ConfigDirectory))
                {
                    Directory.CreateDirectory(ConfigDirectory);
                    logger.Log($"📁 Directorio creado: {ConfigDirectory}");
                }

                // Guardar solo el número
                File.WriteAllText(ConfigPath, contador.ToString());

                logger.Log($"✅ Contador guardado exitosamente: {contador}");
            }
            catch (Exception ex)
            {
                FileLogger logger = new FileLogger();
                logger.Log($"❌ Error guardando contador: {ex.Message}");
                // No lanzar excepción para no interrumpir el flujo
            }
        }

        public static void ReiniciarContador()
        {
            try
            {
                FileLogger logger = new FileLogger();
                logger.Log("🔄 Reiniciando contador a 0");

                GuardarContador(0);

                // Opcional: eliminar el archivo si quieres reset completo
                if (File.Exists(ConfigPath))
                {
                    File.Delete(ConfigPath);
                    logger.Log("🗑️ Archivo de contador eliminado");
                }
            }
            catch (Exception ex)
            {
                FileLogger logger = new FileLogger();
                logger.Log($"❌ Error reiniciando contador: {ex.Message}");
            }
        }

        public static DateTime ObtenerFechaUltimaNotificacion()
        {
            try
            {
                string fechaPath = Path.Combine(ConfigDirectory, "fecha_ultima.txt");
                if (!File.Exists(fechaPath))
                    return DateTime.Now;

                string fechaStr = File.ReadAllText(fechaPath);
                if (DateTime.TryParse(fechaStr, out DateTime fecha))
                    return fecha;
                else
                    return DateTime.Now;
            }
            catch
            {
                return DateTime.Now;
            }
        }

        public static void GuardarFechaUltimaNotificacion()
        {
            try
            {
                if (!Directory.Exists(ConfigDirectory))
                    Directory.CreateDirectory(ConfigDirectory);

                string fechaPath = Path.Combine(ConfigDirectory, "fecha_ultima.txt");
                File.WriteAllText(fechaPath, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            catch
            {
                // Ignorar errores de fecha
            }
        }
    }
}