using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EquipGo.Agent
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 🚀 Iniciar worker en background (versión async)
            Worker worker = new Worker();

            // ✅ Usar Task.Run para métodos async
            Thread workerThread = new Thread(async () =>
            {
                try
                {
                    await worker.StartAsync();
                }
                catch (Exception ex)
                {
                    // Log del error
                    MessageBox.Show($"Error iniciando Worker: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });

            workerThread.IsBackground = true;
            workerThread.Start();

            // Mantener la app viva sin mostrar ventanas
            Application.Run(new ApplicationContext());
        }
    }
}