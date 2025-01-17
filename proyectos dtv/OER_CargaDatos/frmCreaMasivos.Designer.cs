namespace OER_CargaDatos
{
    partial class frmCreaMasivos
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.bSubmit = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cbTipoAssets = new System.Windows.Forms.ComboBox();
            this.bBuscar = new System.Windows.Forms.Button();
            this.bSalir = new System.Windows.Forms.Button();
            this.lbAssets = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cbServicios = new System.Windows.Forms.ComboBox();
            this.tbNombre = new System.Windows.Forms.TextBox();
            this.bLimpiarAssets = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.cbVersion = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(374, 190);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(285, 23);
            this.progressBar1.TabIndex = 18;
            // 
            // bSubmit
            // 
            this.bSubmit.Location = new System.Drawing.Point(460, 148);
            this.bSubmit.Name = "bSubmit";
            this.bSubmit.Size = new System.Drawing.Size(134, 23);
            this.bSubmit.TabIndex = 17;
            this.bSubmit.Text = "Submit";
            this.bSubmit.UseVisualStyleBackColor = true;
            this.bSubmit.Click += new System.EventHandler(this.bSubmit_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Tipo de Assets";
            // 
            // cbTipoAssets
            // 
            this.cbTipoAssets.FormattingEnabled = true;
            this.cbTipoAssets.Location = new System.Drawing.Point(90, 12);
            this.cbTipoAssets.Name = "cbTipoAssets";
            this.cbTipoAssets.Size = new System.Drawing.Size(268, 21);
            this.cbTipoAssets.TabIndex = 15;
            // 
            // bBuscar
            // 
            this.bBuscar.Location = new System.Drawing.Point(294, 66);
            this.bBuscar.Name = "bBuscar";
            this.bBuscar.Size = new System.Drawing.Size(64, 23);
            this.bBuscar.TabIndex = 14;
            this.bBuscar.Text = "Agregar";
            this.bBuscar.UseVisualStyleBackColor = true;
            this.bBuscar.Click += new System.EventHandler(this.bBuscar_Click);
            // 
            // bSalir
            // 
            this.bSalir.Location = new System.Drawing.Point(525, 440);
            this.bSalir.Name = "bSalir";
            this.bSalir.Size = new System.Drawing.Size(134, 23);
            this.bSalir.TabIndex = 13;
            this.bSalir.Text = "Salir";
            this.bSalir.UseVisualStyleBackColor = true;
            this.bSalir.Click += new System.EventHandler(this.bSalir_Click);
            // 
            // lbAssets
            // 
            this.lbAssets.FormattingEnabled = true;
            this.lbAssets.Location = new System.Drawing.Point(10, 199);
            this.lbAssets.Name = "lbAssets";
            this.lbAssets.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbAssets.Size = new System.Drawing.Size(348, 264);
            this.lbAssets.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 19;
            this.label2.Text = "Nombre ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(399, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(116, 13);
            this.label3.TabIndex = 21;
            this.label3.Text = "Contenidos en Servicio";
            // 
            // cbServicios
            // 
            this.cbServicios.FormattingEnabled = true;
            this.cbServicios.Location = new System.Drawing.Point(402, 49);
            this.cbServicios.Name = "cbServicios";
            this.cbServicios.Size = new System.Drawing.Size(268, 21);
            this.cbServicios.TabIndex = 20;
            // 
            // tbNombre
            // 
            this.tbNombre.Location = new System.Drawing.Point(65, 68);
            this.tbNombre.Multiline = true;
            this.tbNombre.Name = "tbNombre";
            this.tbNombre.Size = new System.Drawing.Size(223, 125);
            this.tbNombre.TabIndex = 22;
            // 
            // bLimpiarAssets
            // 
            this.bLimpiarAssets.Location = new System.Drawing.Point(374, 440);
            this.bLimpiarAssets.Name = "bLimpiarAssets";
            this.bLimpiarAssets.Size = new System.Drawing.Size(64, 23);
            this.bLimpiarAssets.TabIndex = 23;
            this.bLimpiarAssets.Text = "Limpiar";
            this.bLimpiarAssets.UseVisualStyleBackColor = true;
            this.bLimpiarAssets.Click += new System.EventHandler(this.bLimpiarAssets_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(399, 83);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 13);
            this.label4.TabIndex = 25;
            this.label4.Text = "Versión";
            // 
            // cbVersion
            // 
            this.cbVersion.FormattingEnabled = true;
            this.cbVersion.Items.AddRange(new object[] {
            "1.0",
            "6.2"});
            this.cbVersion.Location = new System.Drawing.Point(402, 99);
            this.cbVersion.Name = "cbVersion";
            this.cbVersion.Size = new System.Drawing.Size(268, 21);
            this.cbVersion.TabIndex = 24;
            // 
            // frmCreaMasivos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(682, 475);
            this.ControlBox = false;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cbVersion);
            this.Controls.Add(this.bLimpiarAssets);
            this.Controls.Add(this.tbNombre);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbServicios);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.bSubmit);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbTipoAssets);
            this.Controls.Add(this.bBuscar);
            this.Controls.Add(this.bSalir);
            this.Controls.Add(this.lbAssets);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frmCreaMasivos";
            this.ShowInTaskbar = false;
            this.Text = "frmCreaMasivos";
            this.Load += new System.EventHandler(this.frmCreaMasivos_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button bSubmit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbTipoAssets;
        private System.Windows.Forms.Button bBuscar;
        private System.Windows.Forms.Button bSalir;
        private System.Windows.Forms.ListBox lbAssets;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbServicios;
        private System.Windows.Forms.TextBox tbNombre;
        private System.Windows.Forms.Button bLimpiarAssets;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbVersion;
    }
}