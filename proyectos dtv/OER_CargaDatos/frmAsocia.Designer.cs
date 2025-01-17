namespace OER_CargaDatos
{
    partial class frmAsocia
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
            this.label3 = new System.Windows.Forms.Label();
            this.cbAplicacion = new System.Windows.Forms.ComboBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.bSubmit = new System.Windows.Forms.Button();
            this.bSalir = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cbServicios = new System.Windows.Forms.ComboBox();
            this.lbAssets = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbTipoAssets = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(374, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 30;
            this.label3.Text = "Asset";
            // 
            // cbAplicacion
            // 
            this.cbAplicacion.FormattingEnabled = true;
            this.cbAplicacion.Location = new System.Drawing.Point(377, 60);
            this.cbAplicacion.Name = "cbAplicacion";
            this.cbAplicacion.Size = new System.Drawing.Size(268, 21);
            this.cbAplicacion.TabIndex = 29;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(377, 166);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(268, 23);
            this.progressBar1.TabIndex = 28;
            // 
            // bSubmit
            // 
            this.bSubmit.Location = new System.Drawing.Point(459, 123);
            this.bSubmit.Name = "bSubmit";
            this.bSubmit.Size = new System.Drawing.Size(134, 23);
            this.bSubmit.TabIndex = 27;
            this.bSubmit.Text = "Submit";
            this.bSubmit.UseVisualStyleBackColor = true;
            this.bSubmit.Click += new System.EventHandler(this.bSubmit_Click);
            // 
            // bSalir
            // 
            this.bSalir.Location = new System.Drawing.Point(511, 455);
            this.bSalir.Name = "bSalir";
            this.bSalir.Size = new System.Drawing.Size(134, 23);
            this.bSalir.TabIndex = 26;
            this.bSalir.Text = "Salir";
            this.bSalir.UseVisualStyleBackColor = true;
            this.bSalir.Click += new System.EventHandler(this.bSalir_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 32;
            this.label1.Text = "Servicios";
            // 
            // cbServicios
            // 
            this.cbServicios.FormattingEnabled = true;
            this.cbServicios.Location = new System.Drawing.Point(88, 25);
            this.cbServicios.Name = "cbServicios";
            this.cbServicios.Size = new System.Drawing.Size(268, 21);
            this.cbServicios.TabIndex = 31;
            this.cbServicios.SelectedIndexChanged += new System.EventHandler(this.cbTipoAssets_SelectedIndexChanged);
            // 
            // lbAssets
            // 
            this.lbAssets.FormattingEnabled = true;
            this.lbAssets.Location = new System.Drawing.Point(8, 71);
            this.lbAssets.Name = "lbAssets";
            this.lbAssets.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbAssets.Size = new System.Drawing.Size(348, 407);
            this.lbAssets.TabIndex = 33;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(374, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 35;
            this.label2.Text = "Tipo de Assets";
            // 
            // cbTipoAssets
            // 
            this.cbTipoAssets.FormattingEnabled = true;
            this.cbTipoAssets.Location = new System.Drawing.Point(377, 22);
            this.cbTipoAssets.Name = "cbTipoAssets";
            this.cbTipoAssets.Size = new System.Drawing.Size(268, 21);
            this.cbTipoAssets.TabIndex = 34;
            this.cbTipoAssets.SelectedIndexChanged += new System.EventHandler(this.cbTipoAssets_SelectedIndexChanged_1);
            // 
            // frmAsocia
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(669, 494);
            this.ControlBox = false;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbTipoAssets);
            this.Controls.Add(this.lbAssets);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbServicios);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbAplicacion);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.bSubmit);
            this.Controls.Add(this.bSalir);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAsocia";
            this.ShowInTaskbar = false;
            this.Text = "frmAsocia";
            this.Load += new System.EventHandler(this.frmAsocia_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbAplicacion;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button bSubmit;
        private System.Windows.Forms.Button bSalir;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbServicios;
        private System.Windows.Forms.ListBox lbAssets;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbTipoAssets;
    }
}