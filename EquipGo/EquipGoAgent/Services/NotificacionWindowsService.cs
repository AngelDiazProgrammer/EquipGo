using EquipGoAgent.Forms;
using System.Threading;
using System.Windows.Forms;

namespace EquipGo.Agent
{
    public static class NotificacionWindowsService
    {
        public static void MostrarAlerta(string mensaje, bool noCerrable = false, int contador = 0)
        {
            new Thread(() =>
            {
                if (noCerrable && contador >= 5)
                {
                    // 🚫 Alerta NO cerrable (nivel 4-5)
                    Application.Run(new AlertaNoCerrableForm(mensaje, contador));
                }
                else
                {
                    // ✅ Alerta normal cerrable (nivel 1-3)
                    Application.Run(new AlertaNormalForm(mensaje, contador));
                }
            }).Start();
        }
    }
}