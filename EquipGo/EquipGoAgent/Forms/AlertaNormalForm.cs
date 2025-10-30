using System;
using System.Drawing;
using System.Windows.Forms;

namespace EquipGo.Agent
{
    public partial class AlertaNormalForm : Form
    {
        public AlertaNormalForm(string mensaje, int contador)
        {
            InitializeComponent();
            ApplyModernStyle();
            lblMensaje.Text = mensaje;
            lblContador.Text = $"Alerta consecutiva: {contador}/5";

            // ✅ Esta ventana SÍ se puede cerrar
            this.ControlBox = true;

            // Configurar título según el contador
            string severidad = contador >= 3 ? "ALTA" : "MEDIA";
            this.Text = $"⚠️ Alerta de Seguridad ({severidad}) - EquipGo";
        }

        private void ApplyModernStyle()
        {
            var buttons = new[] { btnEntendido, btnMasInformacion };
            foreach (var btn in buttons)
            {
                btn.FlatAppearance.BorderSize = 0;
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(
                    Math.Min(btn.BackColor.R + 20, 255),
                    Math.Min(btn.BackColor.G + 20, 255),
                    Math.Min(btn.BackColor.B + 20, 255)
                );
                btn.Cursor = Cursors.Hand;
            }
        }

        private void BtnMasInformacion_Click(object sender, EventArgs e)
        {
            string info =
                "INFORMACIÓN SOBRE LA ALERTA:\n\n" +
                "• Esta alerta indica que el equipo se encuentra fuera de la sede autorizada\n" +
                "• Si esta situación es normal, puede ignorar esta advertencia\n" +
                "• Si no debería estar fuera de sede, regrese inmediatamente\n" +
                "• Alertas consecutivas incrementan el nivel de severidad\n" +
                "• Después de 5 alertas consecutivas, la ventana no podrá cerrarse\n\n" +
                "Para asistencia contacte a: soporte@equipgo.com";

            MessageBox.Show(info, "Información de la Alerta",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnEntendido_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}