namespace winMantCarpetas
{
    partial class Form1
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
            this.rbFront = new System.Windows.Forms.RadioButton();
            this.tbBack = new System.Windows.Forms.RadioButton();
            this.lbCarpetas = new System.Windows.Forms.ListBox();
            this.bCerrar = new System.Windows.Forms.Button();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.bActualizado = new System.Windows.Forms.Button();
            this.cbRevisar = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // rbFront
            // 
            this.rbFront.AutoSize = true;
            this.rbFront.Location = new System.Drawing.Point(22, 13);
            this.rbFront.Name = "rbFront";
            this.rbFront.Size = new System.Drawing.Size(49, 17);
            this.rbFront.TabIndex = 0;
            this.rbFront.TabStop = true;
            this.rbFront.Text = "Front";
            this.rbFront.UseVisualStyleBackColor = true;
            this.rbFront.CheckedChanged += new System.EventHandler(this.rbFront_CheckedChanged);
            // 
            // tbBack
            // 
            this.tbBack.AutoSize = true;
            this.tbBack.Location = new System.Drawing.Point(170, 13);
            this.tbBack.Name = "tbBack";
            this.tbBack.Size = new System.Drawing.Size(50, 17);
            this.tbBack.TabIndex = 1;
            this.tbBack.TabStop = true;
            this.tbBack.Text = "Back";
            this.tbBack.UseVisualStyleBackColor = true;
            this.tbBack.CheckedChanged += new System.EventHandler(this.tbBack_CheckedChanged);
            // 
            // lbCarpetas
            // 
            this.lbCarpetas.DisplayMember = "Nombre";
            this.lbCarpetas.FormattingEnabled = true;
            this.lbCarpetas.Location = new System.Drawing.Point(22, 51);
            this.lbCarpetas.Name = "lbCarpetas";
            this.lbCarpetas.Size = new System.Drawing.Size(558, 329);
            this.lbCarpetas.TabIndex = 2;
            this.lbCarpetas.SelectedIndexChanged += new System.EventHandler(this.lbCarpetas_SelectedIndexChanged);
            // 
            // bCerrar
            // 
            this.bCerrar.Location = new System.Drawing.Point(586, 357);
            this.bCerrar.Name = "bCerrar";
            this.bCerrar.Size = new System.Drawing.Size(75, 23);
            this.bCerrar.TabIndex = 3;
            this.bCerrar.Text = "Cerrar";
            this.bCerrar.UseVisualStyleBackColor = true;
            this.bCerrar.Click += new System.EventHandler(this.bCerrar_Click);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker1.Location = new System.Drawing.Point(586, 282);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(93, 20);
            this.dateTimePicker1.TabIndex = 4;
            // 
            // bActualizado
            // 
            this.bActualizado.Location = new System.Drawing.Point(586, 308);
            this.bActualizado.Name = "bActualizado";
            this.bActualizado.Size = new System.Drawing.Size(75, 23);
            this.bActualizado.TabIndex = 5;
            this.bActualizado.Text = "Actualizar";
            this.bActualizado.UseVisualStyleBackColor = true;
            this.bActualizado.Click += new System.EventHandler(this.bActualizado_Click);
            // 
            // cbRevisar
            // 
            this.cbRevisar.AutoSize = true;
            this.cbRevisar.Location = new System.Drawing.Point(587, 259);
            this.cbRevisar.Name = "cbRevisar";
            this.cbRevisar.Size = new System.Drawing.Size(62, 17);
            this.cbRevisar.TabIndex = 6;
            this.cbRevisar.Text = "Revisar";
            this.cbRevisar.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(691, 459);
            this.Controls.Add(this.cbRevisar);
            this.Controls.Add(this.bActualizado);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.bCerrar);
            this.Controls.Add(this.lbCarpetas);
            this.Controls.Add(this.tbBack);
            this.Controls.Add(this.rbFront);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rbFront;
        private System.Windows.Forms.RadioButton tbBack;
        private System.Windows.Forms.ListBox lbCarpetas;
        private System.Windows.Forms.Button bCerrar;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Button bActualizado;
        private System.Windows.Forms.CheckBox cbRevisar;
    }
}

