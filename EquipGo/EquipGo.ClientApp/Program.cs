using EquipGo.ClientApp.Services;
using System;
using System.Windows.Forms;

namespace EquipGoAgent
{
    static void Main()
    {
        Log("Iniciando recolección de datos...");

        var equipoService = new EquipoInfoService();
        var datos = equipoService.ObtenerDatosEquipo();

        if (datos == null)
        {
            Log("❌ Error al recolectar datos.");
            return;
        }

        Log("✅ Datos recolectados correctamente.");

        var resultado = ApiClient.EnviarDatosAServidor(datos);

        Log($"📡 Resultado del envío: {resultado}");
    }
}