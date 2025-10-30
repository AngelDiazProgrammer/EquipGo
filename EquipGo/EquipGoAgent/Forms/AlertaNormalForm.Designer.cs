namespace EquipGo.Agent
{
    partial class AlertaNormalForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Panel panelContent;
        private System.Windows.Forms.Panel panelFooter;
        private System.Windows.Forms.PictureBox picIcon;
        private System.Windows.Forms.PictureBox picWarning;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblContador;
        private System.Windows.Forms.Label lblMensaje;
        private System.Windows.Forms.Label lblInstrucciones;
        private System.Windows.Forms.Button btnEntendido;
        private System.Windows.Forms.Button btnMasInformacion;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelHeader = new System.Windows.Forms.Panel();
            this.picIcon = new System.Windows.Forms.PictureBox();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblContador = new System.Windows.Forms.Label();
            this.panelContent = new System.Windows.Forms.Panel();
            this.picWarning = new System.Windows.Forms.PictureBox();
            this.lblMensaje = new System.Windows.Forms.Label();
            this.panelFooter = new System.Windows.Forms.Panel();
            this.lblInstrucciones = new System.Windows.Forms.Label();
            this.btnMasInformacion = new System.Windows.Forms.Button();
            this.btnEntendido = new System.Windows.Forms.Button();

            // panelHeader
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(193)))), ((int)(((byte)(7)))));
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(550, 70);
            this.panelHeader.TabIndex = 0;

            // picIcon
            this.picIcon.Image = System.Drawing.SystemIcons.Warning.ToBitmap();
            this.picIcon.Location = new System.Drawing.Point(20, 15);
            this.picIcon.Name = "picIcon";
            this.picIcon.Size = new System.Drawing.Size(35, 35);
            this.picIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picIcon.TabIndex = 0;
            this.picIcon.TabStop = false;

            // lblTitulo
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.Location = new System.Drawing.Point(65, 12);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(262, 21);
            this.lblTitulo.TabIndex = 1;
            this.lblTitulo.Text = "⚠️ ALERTA DE SEGURIDAD - EQUIPGO";

            // lblContador
            this.lblContador.AutoSize = true;
            this.lblContador.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContador.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblContador.Location = new System.Drawing.Point(65, 37);
            this.lblContador.Name = "lblContador";
            this.lblContador.Size = new System.Drawing.Size(118, 15);
            this.lblContador.TabIndex = 2;
            this.lblContador.Text = "Alerta consecutiva: 1/5";

            this.panelHeader.Controls.Add(this.lblContador);
            this.panelHeader.Controls.Add(this.lblTitulo);
            this.panelHeader.Controls.Add(this.picIcon);

            // panelContent
            this.panelContent.BackColor = System.Drawing.Color.White;
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.Location = new System.Drawing.Point(0, 70);
            this.panelContent.Name = "panelContent";
            this.panelContent.Padding = new System.Windows.Forms.Padding(25);
            this.panelContent.Size = new System.Drawing.Size(550, 110);
            this.panelContent.TabIndex = 1;

            // picWarning
            this.picWarning.Image = System.Drawing.SystemIcons.Exclamation.ToBitmap();
            this.picWarning.Location = new System.Drawing.Point(25, 25);
            this.picWarning.Name = "picWarning";
            this.picWarning.Size = new System.Drawing.Size(50, 50);
            this.picWarning.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picWarning.TabIndex = 0;
            this.picWarning.TabStop = false;

            // lblMensaje
            this.lblMensaje.AutoSize = false;
            this.lblMensaje.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMensaje.Location = new System.Drawing.Point(90, 25);
            this.lblMensaje.Name = "lblMensaje";
            this.lblMensaje.Size = new System.Drawing.Size(400, 60);
            this.lblMensaje.TabIndex = 1;
            this.lblMensaje.Text = "Mensaje de alerta";

            this.panelContent.Controls.Add(this.lblMensaje);
            this.panelContent.Controls.Add(this.picWarning);

            // panelFooter
            this.panelFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panelFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelFooter.Location = new System.Drawing.Point(0, 180);
            this.panelFooter.Name = "panelFooter";
            this.panelFooter.Size = new System.Drawing.Size(550, 120);
            this.panelFooter.TabIndex = 2;

            // lblInstrucciones
            this.lblInstrucciones.AutoSize = true;
            this.lblInstrucciones.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInstrucciones.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.lblInstrucciones.Location = new System.Drawing.Point(20, 15);
            this.lblInstrucciones.Name = "lblInstrucciones";
            this.lblInstrucciones.Size = new System.Drawing.Size(510, 30);
            this.lblInstrucciones.TabIndex = 0;
            this.lblInstrucciones.Text = "Puede cerrar esta ventana. La alerta se mostrará nuevamente en 25 horas si persiste la condición.";

            // btnEntendido
            this.btnEntendido.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.btnEntendido.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEntendido.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEntendido.ForeColor = System.Drawing.Color.White;
            this.btnEntendido.Location = new System.Drawing.Point(150, 55);
            this.btnEntendido.Name = "btnEntendido";
            this.btnEntendido.Size = new System.Drawing.Size(120, 35);
            this.btnEntendido.TabIndex = 1;
            this.btnEntendido.Text = "✅ Entendido";
            this.btnEntendido.UseVisualStyleBackColor = false;
            this.btnEntendido.Click += new System.EventHandler(this.BtnEntendido_Click);

            // btnMasInformacion
            this.btnMasInformacion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(123)))), ((int)(((byte)(255)))));
            this.btnMasInformacion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMasInformacion.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMasInformacion.ForeColor = System.Drawing.Color.White;
            this.btnMasInformacion.Location = new System.Drawing.Point(280, 55);
            this.btnMasInformacion.Name = "btnMasInformacion";
            this.btnMasInformacion.Size = new System.Drawing.Size(140, 35);
            this.btnMasInformacion.TabIndex = 2;
            this.btnMasInformacion.Text = "📖 Más Información";
            this.btnMasInformacion.UseVisualStyleBackColor = false;
            this.btnMasInformacion.Click += new System.EventHandler(this.BtnMasInformacion_Click);

            this.panelFooter.Controls.Add(this.btnMasInformacion);
            this.panelFooter.Controls.Add(this.btnEntendido);
            this.panelFooter.Controls.Add(this.lblInstrucciones);

            // AlertaNormalForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(550, 300);
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.panelFooter);
            this.Controls.Add(this.panelHeader);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = true;
            this.Name = "AlertaNormalForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Alerta de Seguridad - EquipGo";
            this.TopMost = true;

            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picIcon)).EndInit();
            this.panelContent.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picWarning)).EndInit();
            this.panelFooter.ResumeLayout(false);
            this.panelFooter.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}