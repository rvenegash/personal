namespace OER_CargaDatos
{
    partial class frmAprobaciones
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
            this.lbAssets = new System.Windows.Forms.ListBox();
            this.bSalir = new System.Windows.Forms.Button();
            this.bBuscar = new System.Windows.Forms.Button();
            this.cbTipoAssets = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cboEstAssets = new System.Windows.Forms.ComboBox();
            this.bSubmit = new System.Windows.Forms.Button();
            this.bRegister = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.bAccept = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbAssets
            // 
            this.lbAssets.FormattingEnabled = true;
            this.lbAssets.Location = new System.Drawing.Point(12, 111);
            this.lbAssets.Name = "lbAssets";
            this.lbAssets.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbAssets.Size = new System.Drawing.Size(348, 368);
            this.lbAssets.TabIndex = 0;
            // 
            // bSalir
            // 
            this.bSalir.Location = new System.Drawing.Point(517, 459);
            this.bSalir.Name = "bSalir";
            this.bSalir.Size = new System.Drawing.Size(134, 23);
            this.bSalir.TabIndex = 3;
            this.bSalir.Text = "Salir";
            this.bSalir.UseVisualStyleBackColor = true;
            this.bSalir.Click += new System.EventHandler(this.bSalir_Click);
            // 
            // bBuscar
            // 
            this.bBuscar.Location = new System.Drawing.Point(226, 76);
            this.bBuscar.Name = "bBuscar";
            this.bBuscar.Size = new System.Drawing.Size(134, 23);
            this.bBuscar.TabIndex = 4;
            this.bBuscar.Text = "Buscar";
            this.bBuscar.UseVisualStyleBackColor = true;
            this.bBuscar.Click += new System.EventHandler(this.bBuscar_Click);
            // 
            // cbTipoAssets
            // 
            this.cbTipoAssets.FormattingEnabled = true;
            this.cbTipoAssets.Location = new System.Drawing.Point(92, 13);
            this.cbTipoAssets.Name = "cbTipoAssets";
            this.cbTipoAssets.Size = new System.Drawing.Size(268, 21);
            this.cbTipoAssets.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Tipo de Assets";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Estado Assets";
            // 
            // cboEstAssets
            // 
            this.cboEstAssets.FormattingEnabled = true;
            this.cboEstAssets.Location = new System.Drawing.Point(92, 40);
            this.cboEstAssets.Name = "cboEstAssets";
            this.cboEstAssets.Size = new System.Drawing.Size(268, 21);
            this.cboEstAssets.TabIndex = 7;
            // 
            // bSubmit
            // 
            this.bSubmit.Location = new System.Drawing.Point(445, 170);
            this.bSubmit.Name = "bSubmit";
            this.bSubmit.Size = new System.Drawing.Size(134, 23);
            this.bSubmit.TabIndex = 9;
            this.bSubmit.Text = "Submit";
            this.bSubmit.UseVisualStyleBackColor = true;
            this.bSubmit.Click += new System.EventHandler(this.bSubmit_Click);
            // 
            // bRegister
            // 
            this.bRegister.Location = new System.Drawing.Point(445, 228);
            this.bRegister.Name = "bRegister";
            this.bRegister.Size = new System.Drawing.Size(134, 23);
            this.bRegister.TabIndex = 10;
            this.bRegister.Text = "Register";
            this.bRegister.UseVisualStyleBackColor = true;
            this.bRegister.Click += new System.EventHandler(this.bRegister_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(366, 257);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(285, 23);
            this.progressBar1.TabIndex = 11;
            // 
            // bAccept
            // 
            this.bAccept.Location = new System.Drawing.Point(445, 199);
            this.bAccept.Name = "bAccept";
            this.bAccept.Size = new System.Drawing.Size(134, 23);
            this.bAccept.TabIndex = 12;
            this.bAccept.Text = "Accept";
            this.bAccept.UseVisualStyleBackColor = true;
            this.bAccept.Click += new System.EventHandler(this.bAccept_Click);
            // 
            // frmAprobaciones
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(663, 494);
            this.Controls.Add(this.bAccept);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.bRegister);
            this.Controls.Add(this.bSubmit);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cboEstAssets);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbTipoAssets);
            this.Controls.Add(this.bBuscar);
            this.Controls.Add(this.bSalir);
            this.Controls.Add(this.lbAssets);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmAprobaciones";
            this.Text = "Aprobaciones";
            this.Load += new System.EventHandler(this.frmAprobaciones_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbAssets;
        private System.Windows.Forms.Button bSalir;
        private System.Windows.Forms.Button bBuscar;
        private System.Windows.Forms.ComboBox cbTipoAssets;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboEstAssets;
        private System.Windows.Forms.Button bSubmit;
        private System.Windows.Forms.Button bRegister;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button bAccept;
    }
}