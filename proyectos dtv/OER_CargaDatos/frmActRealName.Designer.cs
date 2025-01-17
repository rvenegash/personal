namespace OER_CargaDatos
{
    partial class frmActRealName
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
            this.cbServicios = new System.Windows.Forms.ComboBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.bSubmit = new System.Windows.Forms.Button();
            this.bSalir = new System.Windows.Forms.Button();
            this.lbAssets = new System.Windows.Forms.ListBox();
            this.lbPortType = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(116, 13);
            this.label3.TabIndex = 23;
            this.label3.Text = "Contenidos en Servicio";
            // 
            // cbServicios
            // 
            this.cbServicios.FormattingEnabled = true;
            this.cbServicios.Location = new System.Drawing.Point(12, 34);
            this.cbServicios.Name = "cbServicios";
            this.cbServicios.Size = new System.Drawing.Size(268, 21);
            this.cbServicios.TabIndex = 22;
            this.cbServicios.SelectedIndexChanged += new System.EventHandler(this.cbServicios_SelectedIndexChanged);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(318, 88);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(285, 23);
            this.progressBar1.TabIndex = 26;
            // 
            // bSubmit
            // 
            this.bSubmit.Location = new System.Drawing.Point(400, 48);
            this.bSubmit.Name = "bSubmit";
            this.bSubmit.Size = new System.Drawing.Size(134, 23);
            this.bSubmit.TabIndex = 25;
            this.bSubmit.Text = "Asociar";
            this.bSubmit.UseVisualStyleBackColor = true;
            this.bSubmit.Click += new System.EventHandler(this.bSubmit_Click);
            // 
            // bSalir
            // 
            this.bSalir.Location = new System.Drawing.Point(469, 350);
            this.bSalir.Name = "bSalir";
            this.bSalir.Size = new System.Drawing.Size(134, 23);
            this.bSalir.TabIndex = 24;
            this.bSalir.Text = "Salir";
            this.bSalir.UseVisualStyleBackColor = true;
            this.bSalir.Click += new System.EventHandler(this.bSalir_Click);
            // 
            // lbAssets
            // 
            this.lbAssets.FormattingEnabled = true;
            this.lbAssets.Location = new System.Drawing.Point(12, 120);
            this.lbAssets.Name = "lbAssets";
            this.lbAssets.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbAssets.Size = new System.Drawing.Size(268, 303);
            this.lbAssets.TabIndex = 27;
            // 
            // lbPortType
            // 
            this.lbPortType.AutoSize = true;
            this.lbPortType.Location = new System.Drawing.Point(12, 57);
            this.lbPortType.Name = "lbPortType";
            this.lbPortType.Size = new System.Drawing.Size(116, 13);
            this.lbPortType.TabIndex = 28;
            this.lbPortType.Text = "Contenidos en Servicio";
            // 
            // frmActRealName
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(618, 435);
            this.Controls.Add(this.lbPortType);
            this.Controls.Add(this.lbAssets);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.bSubmit);
            this.Controls.Add(this.bSalir);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbServicios);
            this.Name = "frmActRealName";
            this.Text = "frmActRealName";
            this.Load += new System.EventHandler(this.frmActRealName_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbServicios;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button bSubmit;
        private System.Windows.Forms.Button bSalir;
        private System.Windows.Forms.ListBox lbAssets;
        private System.Windows.Forms.Label lbPortType;
    }
}