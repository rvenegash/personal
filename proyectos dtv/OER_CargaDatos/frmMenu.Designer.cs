namespace OER_CargaDatos
{
    partial class frmMenu
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
            this.bActuDatosOER = new System.Windows.Forms.Button();
            this.frmAprobaciones = new System.Windows.Forms.Button();
            this.bSalir = new System.Windows.Forms.Button();
            this.bCreaMasivo = new System.Windows.Forms.Button();
            this.frmRealName = new System.Windows.Forms.Button();
            this.bConciliador = new System.Windows.Forms.Button();
            this.bAsocia = new System.Windows.Forms.Button();
            this.bAssetXML = new System.Windows.Forms.Button();
            this.tbAsociaAAsset = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // bActuDatosOER
            // 
            this.bActuDatosOER.Location = new System.Drawing.Point(12, 17);
            this.bActuDatosOER.Name = "bActuDatosOER";
            this.bActuDatosOER.Size = new System.Drawing.Size(134, 23);
            this.bActuDatosOER.TabIndex = 0;
            this.bActuDatosOER.Text = "Actualizar Datos OER";
            this.bActuDatosOER.UseVisualStyleBackColor = true;
            this.bActuDatosOER.Click += new System.EventHandler(this.bActuDatosOER_Click);
            // 
            // frmAprobaciones
            // 
            this.frmAprobaciones.Location = new System.Drawing.Point(12, 69);
            this.frmAprobaciones.Name = "frmAprobaciones";
            this.frmAprobaciones.Size = new System.Drawing.Size(134, 23);
            this.frmAprobaciones.TabIndex = 1;
            this.frmAprobaciones.Text = "Aprobaciones";
            this.frmAprobaciones.UseVisualStyleBackColor = true;
            this.frmAprobaciones.Click += new System.EventHandler(this.button2_Click);
            // 
            // bSalir
            // 
            this.bSalir.Location = new System.Drawing.Point(292, 121);
            this.bSalir.Name = "bSalir";
            this.bSalir.Size = new System.Drawing.Size(134, 23);
            this.bSalir.TabIndex = 2;
            this.bSalir.Text = "Salir";
            this.bSalir.UseVisualStyleBackColor = true;
            this.bSalir.Click += new System.EventHandler(this.bSalir_Click);
            // 
            // bCreaMasivo
            // 
            this.bCreaMasivo.Location = new System.Drawing.Point(152, 69);
            this.bCreaMasivo.Name = "bCreaMasivo";
            this.bCreaMasivo.Size = new System.Drawing.Size(134, 23);
            this.bCreaMasivo.TabIndex = 3;
            this.bCreaMasivo.Text = "Crea masivo";
            this.bCreaMasivo.UseVisualStyleBackColor = true;
            this.bCreaMasivo.Click += new System.EventHandler(this.bCreaMasivo_Click);
            // 
            // frmRealName
            // 
            this.frmRealName.Location = new System.Drawing.Point(12, 121);
            this.frmRealName.Name = "frmRealName";
            this.frmRealName.Size = new System.Drawing.Size(134, 23);
            this.frmRealName.TabIndex = 4;
            this.frmRealName.Text = "Actualiza RealName";
            this.frmRealName.UseVisualStyleBackColor = true;
            this.frmRealName.Click += new System.EventHandler(this.frmRealName_Click);
            // 
            // bConciliador
            // 
            this.bConciliador.Location = new System.Drawing.Point(152, 121);
            this.bConciliador.Name = "bConciliador";
            this.bConciliador.Size = new System.Drawing.Size(134, 23);
            this.bConciliador.TabIndex = 5;
            this.bConciliador.Text = "Conciliador";
            this.bConciliador.UseVisualStyleBackColor = true;
            this.bConciliador.Click += new System.EventHandler(this.bConciliador_Click);
            // 
            // bAsocia
            // 
            this.bAsocia.Location = new System.Drawing.Point(292, 69);
            this.bAsocia.Name = "bAsocia";
            this.bAsocia.Size = new System.Drawing.Size(134, 23);
            this.bAsocia.TabIndex = 6;
            this.bAsocia.Text = "Asocia";
            this.bAsocia.UseVisualStyleBackColor = true;
            this.bAsocia.Click += new System.EventHandler(this.bAsocia_Click);
            // 
            // bAssetXML
            // 
            this.bAssetXML.Location = new System.Drawing.Point(292, 17);
            this.bAssetXML.Name = "bAssetXML";
            this.bAssetXML.Size = new System.Drawing.Size(134, 23);
            this.bAssetXML.TabIndex = 7;
            this.bAssetXML.Text = "Ver asset";
            this.bAssetXML.UseVisualStyleBackColor = true;
            this.bAssetXML.Click += new System.EventHandler(this.bAssetXML_Click);
            // 
            // tbAsociaAAsset
            // 
            this.tbAsociaAAsset.Location = new System.Drawing.Point(152, 17);
            this.tbAsociaAAsset.Name = "tbAsociaAAsset";
            this.tbAsociaAAsset.Size = new System.Drawing.Size(134, 23);
            this.tbAsociaAAsset.TabIndex = 3;
            this.tbAsociaAAsset.Text = "Asocia a Asset";
            this.tbAsociaAAsset.UseVisualStyleBackColor = true;
            this.tbAsociaAAsset.Click += new System.EventHandler(this.tbAsociaAAsset_Click);
            // 
            // frmMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(442, 167);
            this.ControlBox = false;
            this.Controls.Add(this.bAssetXML);
            this.Controls.Add(this.bAsocia);
            this.Controls.Add(this.bConciliador);
            this.Controls.Add(this.frmRealName);
            this.Controls.Add(this.tbAsociaAAsset);
            this.Controls.Add(this.bCreaMasivo);
            this.Controls.Add(this.bSalir);
            this.Controls.Add(this.frmAprobaciones);
            this.Controls.Add(this.bActuDatosOER);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frmMenu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmMenu";
            this.Load += new System.EventHandler(this.frmMenu_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bActuDatosOER;
        private System.Windows.Forms.Button frmAprobaciones;
        private System.Windows.Forms.Button bSalir;
        private System.Windows.Forms.Button bCreaMasivo;
        private System.Windows.Forms.Button frmRealName;
        private System.Windows.Forms.Button bConciliador;
        private System.Windows.Forms.Button bAsocia;
        private System.Windows.Forms.Button bAssetXML;
        private System.Windows.Forms.Button tbAsociaAAsset;
    }
}