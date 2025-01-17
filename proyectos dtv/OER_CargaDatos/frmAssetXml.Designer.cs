namespace OER_CargaDatos
{
    partial class frmAssetXml
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
            this.bSubmit = new System.Windows.Forms.Button();
            this.bSalir = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbAssetId = new System.Windows.Forms.TextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.bFontMas = new System.Windows.Forms.Button();
            this.btnFontMenos = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // bSubmit
            // 
            this.bSubmit.Location = new System.Drawing.Point(203, 12);
            this.bSubmit.Name = "bSubmit";
            this.bSubmit.Size = new System.Drawing.Size(134, 23);
            this.bSubmit.TabIndex = 29;
            this.bSubmit.Text = "Buscar";
            this.bSubmit.UseVisualStyleBackColor = true;
            this.bSubmit.Click += new System.EventHandler(this.bSubmit_Click);
            // 
            // bSalir
            // 
            this.bSalir.Location = new System.Drawing.Point(508, 12);
            this.bSalir.Name = "bSalir";
            this.bSalir.Size = new System.Drawing.Size(134, 23);
            this.bSalir.TabIndex = 28;
            this.bSalir.Text = "Salir";
            this.bSalir.UseVisualStyleBackColor = true;
            this.bSalir.Click += new System.EventHandler(this.bSalir_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 30;
            this.label1.Text = "Id Asset";
            // 
            // tbAssetId
            // 
            this.tbAssetId.Location = new System.Drawing.Point(63, 14);
            this.tbAssetId.Name = "tbAssetId";
            this.tbAssetId.Size = new System.Drawing.Size(134, 20);
            this.tbAssetId.TabIndex = 31;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox1.Location = new System.Drawing.Point(15, 40);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(627, 435);
            this.richTextBox1.TabIndex = 32;
            this.richTextBox1.Text = "";
            // 
            // bFontMas
            // 
            this.bFontMas.Location = new System.Drawing.Point(368, 12);
            this.bFontMas.Name = "bFontMas";
            this.bFontMas.Size = new System.Drawing.Size(32, 23);
            this.bFontMas.TabIndex = 33;
            this.bFontMas.Text = "A+";
            this.bFontMas.UseVisualStyleBackColor = true;
            this.bFontMas.Click += new System.EventHandler(this.bFontMas_Click);
            // 
            // btnFontMenos
            // 
            this.btnFontMenos.Location = new System.Drawing.Point(406, 12);
            this.btnFontMenos.Name = "btnFontMenos";
            this.btnFontMenos.Size = new System.Drawing.Size(32, 23);
            this.btnFontMenos.TabIndex = 34;
            this.btnFontMenos.Text = "A-";
            this.btnFontMenos.UseVisualStyleBackColor = true;
            this.btnFontMenos.Click += new System.EventHandler(this.btnFontMenos_Click);
            // 
            // frmAssetXml
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(654, 487);
            this.ControlBox = false;
            this.Controls.Add(this.btnFontMenos);
            this.Controls.Add(this.bFontMas);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.tbAssetId);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bSubmit);
            this.Controls.Add(this.bSalir);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAssetXml";
            this.Text = "frmAssetXml";
            this.Load += new System.EventHandler(this.frmAssetXml_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bSubmit;
        private System.Windows.Forms.Button bSalir;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbAssetId;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button bFontMas;
        private System.Windows.Forms.Button btnFontMenos;
    }
}