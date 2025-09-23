using System;
using System.Threading;
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

            // 🚀 Iniciar worker en background
            Worker worker = new Worker();
            Thread workerThread = new Thread(new ThreadStart(worker.Start));
            workerThread.IsBackground = true;
            workerThread.Start();

            // Mantener la app viva sin mostrar ventanas
            Application.Run(new ApplicationContext());
        }
    }
}
