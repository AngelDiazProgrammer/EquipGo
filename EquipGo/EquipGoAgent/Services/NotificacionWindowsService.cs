using System.Threading;
using System.Windows.Forms;

namespace EquipGo.Agent
{
    public static class NotificacionWindowsService
    {
        public static void MostrarAlerta(string mensaje)
        {
            new Thread(() =>
            {
                MessageBox.Show(
                    mensaje,
                    "EquipGo Agent",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }).Start();
        }
    }
}
