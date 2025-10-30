using System;
using System.Drawing;
using System.Windows.Forms;

namespace EquipGoAgent.Forms
{
    public partial class AlertaNoCerrableForm : Form
    {
        private Timer _blinkTimer;
        private bool _isBlinking = false;

        public AlertaNoCerrableForm(string mensaje, int contador)
        {
            InitializeComponent();
            ApplyModernStyle();
            lblMensaje.Text = mensaje;
            lblContador.Text = $"Alerta consecutiva: {contador}/5";

            // Configurar comportamiento no cerrable
            this.ControlBox = false;
            this.FormClosing += (s, e) =>
            {
                if (e.CloseReason == CloseReason.UserClosing)
                {
                    e.Cancel = true;
                    MostrarAdvertenciaNoCerrar();
                }
            };

            // Efecto de parpadeo para el panel de header
            StartBlinkingEffect();
        }

        private void InitializeComponent()
        {
            // Header
            this.panelHeader = new Panel();
            this.picIcon = new PictureBox();
            this.lblTitulo = new Label();
            this.lblContador = new Label();

            // Contenido
            this.panelContent = new Panel();
            this.lblMensaje = new Label();
            this.picWarning = new PictureBox();

            // Footer
            this.panelFooter = new Panel();
            this.btnContactarSoporte = new Button();
            this.btnRegresarSede = new Button();
            this.lblInstrucciones = new Label();

            // Timer
            this._blinkTimer = new Timer();

            SuspendLayout();

            // panelHeader
            this.panelHeader.BackColor = Color.FromArgb(192, 0, 0);
            this.panelHeader.Dock = DockStyle.Top;
            this.panelHeader.Height = 80;
            this.panelHeader.Controls.Add(picIcon);
            this.panelHeader.Controls.Add(lblTitulo);
            this.panelHeader.Controls.Add(lblContador);

            // picIcon
            this.picIcon.Image = SystemIcons.Shield.ToBitmap();
            this.picIcon.Location = new Point(20, 20);
            this.picIcon.Size = new Size(40, 40);
            this.picIcon.SizeMode = PictureBoxSizeMode.Zoom;

            // lblTitulo
            this.lblTitulo.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            this.lblTitulo.ForeColor = Color.White;
            this.lblTitulo.Location = new Point(70, 15);
            this.lblTitulo.Size = new Size(300, 30);
            this.lblTitulo.Text = "🚫 ALERTA DE SEGURIDAD - EQUIPGO";

            // lblContador
            this.lblContador.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            this.lblContador.ForeColor = Color.LightYellow;
            this.lblContador.Location = new Point(70, 45);
            this.lblContador.Size = new Size(300, 20);
            this.lblContador.Text = "Alerta consecutiva: 5/5";

            // panelContent
            this.panelContent.Dock = DockStyle.Fill;
            this.panelContent.Padding = new Padding(30);
            this.panelContent.Controls.Add(picWarning);
            this.panelContent.Controls.Add(lblMensaje);

            // picWarning
            this.picWarning.Image = SystemIcons.Warning.ToBitmap();
            this.picWarning.Location = new Point(30, 30);
            this.picWarning.Size = new Size(60, 60);
            this.picWarning.SizeMode = PictureBoxSizeMode.Zoom;

            // lblMensaje
            this.lblMensaje.Font = new Font("Segoe UI", 11F);
            this.lblMensaje.Location = new Point(100, 30);
            this.lblMensaje.Size = new Size(450, 120);
            this.lblMensaje.Text = "Mensaje de alerta";

            // panelFooter
            this.panelFooter.BackColor = Color.FromArgb(240, 240, 240);
            this.panelFooter.Dock = DockStyle.Bottom;
            this.panelFooter.Height = 140;
            this.panelFooter.Controls.Add(lblInstrucciones);
            this.panelFooter.Controls.Add(btnRegresarSede);
            this.panelFooter.Controls.Add(btnContactarSoporte);

            // lblInstrucciones
            this.lblInstrucciones.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            this.lblInstrucciones.ForeColor = Color.DarkRed;
            this.lblInstrucciones.Location = new Point(20, 15);
            this.lblInstrucciones.Size = new Size(500, 30);
            this.lblInstrucciones.Text = "⚠️ Esta alerta no se puede cerrar. El equipo debe regresar a la sede autorizada.";

            // btnContactarSoporte
            this.btnContactarSoporte.BackColor = Color.FromArgb(0, 123, 255);
            this.btnContactarSoporte.FlatStyle = FlatStyle.Flat;
            this.btnContactarSoporte.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.btnContactarSoporte.ForeColor = Color.White;
            this.btnContactarSoporte.Location = new Point(280, 60);
            this.btnContactarSoporte.Size = new Size(150, 35);
            this.btnContactarSoporte.Text = "📞 Contactar Soporte";
            this.btnContactarSoporte.UseVisualStyleBackColor = false;
            this.btnContactarSoporte.Click += BtnContactarSoporte_Click;

            // btnRegresarSede
            this.btnRegresarSede.BackColor = Color.FromArgb(40, 167, 69);
            this.btnRegresarSede.FlatStyle = FlatStyle.Flat;
            this.btnRegresarSede.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.btnRegresarSede.ForeColor = Color.White;
            this.btnRegresarSede.Location = new Point(100, 60);
            this.btnRegresarSede.Size = new Size(150, 35);
            this.btnRegresarSede.Text = "🏢 Regresar a Sede";
            this.btnRegresarSede.UseVisualStyleBackColor = false;
            this.btnRegresarSede.Click += BtnRegresarSede_Click;

            // Form principal
            this.AutoScaleDimensions = new SizeF(8F, 16F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.White;
            this.ClientSize = new Size(600, 350);
            this.Controls.Add(panelContent);
            this.Controls.Add(panelFooter);
            this.Controls.Add(panelHeader);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AlertaNoCerrableForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Alerta de Seguridad - EquipGo";
            this.TopMost = true;

            ResumeLayout(false);
        }

        private void ApplyModernStyle()
        {
            // Estilos modernos para los botones
            var buttons = new[] { btnContactarSoporte, btnRegresarSede };
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

        private void StartBlinkingEffect()
        {
            _blinkTimer = new Timer();
            _blinkTimer.Interval = 1000; // 1 segundo
            _blinkTimer.Tick += (s, e) =>
            {
                _isBlinking = !_isBlinking;
                panelHeader.BackColor = _isBlinking ?
                    Color.FromArgb(220, 0, 0) : // Rojo más oscuro
                    Color.FromArgb(192, 0, 0);   // Rojo normal
            };
            _blinkTimer.Start();
        }

        private void MostrarAdvertenciaNoCerrar()
        {
            MessageBox.Show(
                "Esta alerta de seguridad no se puede cerrar manualmente.\n\n" +
                "La alerta desaparecerá automáticamente cuando el equipo regrese a la sede autorizada.",
                "Alerta No Cerrable",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void BtnContactarSoporte_Click(object sender, EventArgs e)
        {
            try
            {
                // Abrir email o sistema de tickets
                System.Diagnostics.Process.Start("mailto:soporte@equipgo.com?subject=Alerta%20de%20Seguridad%20EquipGo");
            }
            catch
            {
                Clipboard.SetText("soporte@equipgo.com");
                MessageBox.Show("Email de soporte copiado al portapapeles: soporte@equipgo.com",
                    "Contactar Soporte",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void BtnRegresarSede_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Para desactivar esta alerta:\n\n" +
                "1. Regrese el equipo a la sede autorizada\n" +
                "2. Registre la salida en el sistema EquipGo\n" +
                "3. La alerta se desactivará automáticamente\n\n" +
                "Si necesita asistencia inmediata, contacte a soporte.",
                "Instrucciones para Regresar a Sede",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Solo permitir cerrar si no es acción del usuario
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                MostrarAdvertenciaNoCerrar();
            }
            base.OnFormClosing(e);
        }

        // Controles
        private Panel panelHeader;
        private Panel panelContent;
        private Panel panelFooter;
        private PictureBox picIcon;
        private PictureBox picWarning;
        private Label lblTitulo;
        private Label lblContador;
        private Label lblMensaje;
        private Label lblInstrucciones;
        private Button btnContactarSoporte;
        private Button btnRegresarSede;
    }
}