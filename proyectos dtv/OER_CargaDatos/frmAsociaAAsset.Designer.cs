namespace OER_CargaDatos
{
    partial class frmAsociaAAsset
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
            this.tbAssetId = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.bSubmit = new System.Windows.Forms.Button();
            this.bSalir = new System.Windows.Forms.Button();
            this.tbNombre = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.bBuscar = new System.Windows.Forms.Button();
            this.lbAssets = new System.Windows.Forms.ListBox();
            this.bLimpiarAssets = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.tbAssetName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbRelacion = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // tbAssetId
            // 
            this.tbAssetId.Location = new System.Drawing.Point(66, 12);
            this.tbAssetId.Name = "tbAssetId";
            this.tbAssetId.Size = new System.Drawing.Size(86, 20);
            this.tbAssetId.TabIndex = 34;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 33;
            this.label1.Text = "Id Asset";
            // 
            // bSubmit
            // 
            this.bSubmit.Location = new System.Drawing.Point(155, 12);
            this.bSubmit.Name = "bSubmit";
            this.bSubmit.Size = new System.Drawing.Size(134, 23);
            this.bSubmit.TabIndex = 32;
            this.bSubmit.Text = "Buscar";
            this.bSubmit.UseVisualStyleBackColor = true;
            this.bSubmit.Click += new System.EventHandler(this.bSubmit_Click);
            // 
            // bSalir
            // 
            this.bSalir.Location = new System.Drawing.Point(449, 441);
            this.bSalir.Name = "bSalir";
            this.bSalir.Size = new System.Drawing.Size(134, 23);
            this.bSalir.TabIndex = 35;
            this.bSalir.Text = "Salir";
            this.bSalir.UseVisualStyleBackColor = true;
            this.bSalir.Click += new System.EventHandler(this.bSalir_Click);
            // 
            // tbNombre
            // 
            this.tbNombre.Location = new System.Drawing.Point(66, 69);
            this.tbNombre.Multiline = true;
            this.tbNombre.Name = "tbNombre";
            this.tbNombre.Size = new System.Drawing.Size(223, 125);
            this.tbNombre.TabIndex = 39;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 38;
            this.label2.Text = "Nombre ";
            // 
            // bBuscar
            // 
            this.bBuscar.Location = new System.Drawing.Point(295, 67);
            this.bBuscar.Name = "bBuscar";
            this.bBuscar.Size = new System.Drawing.Size(64, 23);
            this.bBuscar.TabIndex = 37;
            this.bBuscar.Text = "Agregar";
            this.bBuscar.UseVisualStyleBackColor = true;
            this.bBuscar.Click += new System.EventHandler(this.bBuscar_Click);
            // 
            // lbAssets
            // 
            this.lbAssets.FormattingEnabled = true;
            this.lbAssets.Location = new System.Drawing.Point(11, 200);
            this.lbAssets.Name = "lbAssets";
            this.lbAssets.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbAssets.Size = new System.Drawing.Size(348, 264);
            this.lbAssets.TabIndex = 36;
            // 
            // bLimpiarAssets
            // 
            this.bLimpiarAssets.Location = new System.Drawing.Point(363, 438);
            this.bLimpiarAssets.Name = "bLimpiarAssets";
            this.bLimpiarAssets.Size = new System.Drawing.Size(64, 23);
            this.bLimpiarAssets.TabIndex = 41;
            this.bLimpiarAssets.Text = "Limpiar";
            this.bLimpiarAssets.UseVisualStyleBackColor = true;
            this.bLimpiarAssets.Click += new System.EventHandler(this.bLimpiarAssets_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(449, 200);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(134, 23);
            this.button1.TabIndex = 40;
            this.button1.Text = "Submit";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tbAssetName
            // 
            this.tbAssetName.Location = new System.Drawing.Point(66, 38);
            this.tbAssetName.Name = "tbAssetName";
            this.tbAssetName.ReadOnly = true;
            this.tbAssetName.Size = new System.Drawing.Size(517, 20);
            this.tbAssetName.TabIndex = 42;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(383, 130);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 13);
            this.label3.TabIndex = 44;
            this.label3.Text = "Relacion";
            // 
            // cbRelacion
            // 
            this.cbRelacion.FormattingEnabled = true;
            this.cbRelacion.Location = new System.Drawing.Point(386, 146);
            this.cbRelacion.Name = "cbRelacion";
            this.cbRelacion.Size = new System.Drawing.Size(197, 21);
            this.cbRelacion.TabIndex = 43;
            // 
            // frmAsociaAAsset
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(595, 479);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbRelacion);
            this.Controls.Add(this.tbAssetName);
            this.Controls.Add(this.bLimpiarAssets);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tbNombre);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.bBuscar);
            this.Controls.Add(this.lbAssets);
            this.Controls.Add(this.bSalir);
            this.Controls.Add(this.tbAssetId);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bSubmit);
            this.Name = "frmAsociaAAsset";
            this.Text = "frmAsociaAAsset";
            this.Load += new System.EventHandler(this.frmAsociaAAsset_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbAssetId;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bSubmit;
        private System.Windows.Forms.Button bSalir;
        private System.Windows.Forms.TextBox tbNombre;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button bBuscar;
        private System.Windows.Forms.ListBox lbAssets;
        private System.Windows.Forms.Button bLimpiarAssets;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox tbAssetName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbRelacion;
    }
}