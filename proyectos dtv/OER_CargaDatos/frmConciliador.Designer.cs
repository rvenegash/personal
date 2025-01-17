namespace OER_CargaDatos
{
    partial class frmConciliador
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbOperacion = new System.Windows.Forms.TextBox();
            this.cbOperacion = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.bActualizar = new System.Windows.Forms.Button();
            this.bCerrar = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.bCrearRel = new System.Windows.Forms.Button();
            this.bEliminarRel = new System.Windows.Forms.Button();
            this.lbOerNoGit = new System.Windows.Forms.ListBox();
            this.lbGitNoOer = new System.Windows.Forms.ListBox();
            this.lbEnGit = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lbEnOer = new System.Windows.Forms.ListBox();
            this.label5 = new System.Windows.Forms.Label();
            this.bCrearAssets = new System.Windows.Forms.Button();
            this.rbEnOer = new System.Windows.Forms.RadioButton();
            this.rbEnGit = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbEnGit);
            this.groupBox1.Controls.Add(this.rbEnOer);
            this.groupBox1.Controls.Add(this.tbOperacion);
            this.groupBox1.Controls.Add(this.cbOperacion);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.bActualizar);
            this.groupBox1.Controls.Add(this.bCerrar);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1054, 247);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // tbOperacion
            // 
            this.tbOperacion.Location = new System.Drawing.Point(74, 211);
            this.tbOperacion.Name = "tbOperacion";
            this.tbOperacion.ReadOnly = true;
            this.tbOperacion.Size = new System.Drawing.Size(827, 20);
            this.tbOperacion.TabIndex = 9;
            // 
            // cbOperacion
            // 
            this.cbOperacion.DisplayMember = "Nombre";
            this.cbOperacion.FormattingEnabled = true;
            this.cbOperacion.Location = new System.Drawing.Point(74, 19);
            this.cbOperacion.Name = "cbOperacion";
            this.cbOperacion.Size = new System.Drawing.Size(827, 186);
            this.cbOperacion.TabIndex = 8;
            this.cbOperacion.SelectedIndexChanged += new System.EventHandler(this.cbOperacion_SelectedIndexChanged_1);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Operacion";
            // 
            // bActualizar
            // 
            this.bActualizar.Location = new System.Drawing.Point(917, 16);
            this.bActualizar.Name = "bActualizar";
            this.bActualizar.Size = new System.Drawing.Size(121, 25);
            this.bActualizar.TabIndex = 1;
            this.bActualizar.Text = "Actualizar";
            this.bActualizar.UseVisualStyleBackColor = true;
            this.bActualizar.Click += new System.EventHandler(this.bActualizar_Click);
            // 
            // bCerrar
            // 
            this.bCerrar.Location = new System.Drawing.Point(917, 216);
            this.bCerrar.Name = "bCerrar";
            this.bCerrar.Size = new System.Drawing.Size(121, 25);
            this.bCerrar.TabIndex = 2;
            this.bCerrar.Text = "Cerrar";
            this.bCerrar.UseVisualStyleBackColor = true;
            this.bCerrar.Click += new System.EventHandler(this.bCerrar_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 276);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "En OER no en GIT";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(348, 276);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "En GIT no en OER";
            // 
            // bCrearRel
            // 
            this.bCrearRel.Location = new System.Drawing.Point(551, 270);
            this.bCrearRel.Name = "bCrearRel";
            this.bCrearRel.Size = new System.Drawing.Size(121, 25);
            this.bCrearRel.TabIndex = 10;
            this.bCrearRel.Text = "Crear Relaciones";
            this.bCrearRel.UseVisualStyleBackColor = true;
            this.bCrearRel.Click += new System.EventHandler(this.bCrearRel_Click);
            // 
            // bEliminarRel
            // 
            this.bEliminarRel.Location = new System.Drawing.Point(212, 266);
            this.bEliminarRel.Name = "bEliminarRel";
            this.bEliminarRel.Size = new System.Drawing.Size(121, 25);
            this.bEliminarRel.TabIndex = 11;
            this.bEliminarRel.Text = "Eliminar Relaciones";
            this.bEliminarRel.UseVisualStyleBackColor = true;
            this.bEliminarRel.Click += new System.EventHandler(this.bEliminarRel_Click);
            // 
            // lbOerNoGit
            // 
            this.lbOerNoGit.DisplayMember = "Nombre";
            this.lbOerNoGit.FormattingEnabled = true;
            this.lbOerNoGit.Location = new System.Drawing.Point(12, 301);
            this.lbOerNoGit.Name = "lbOerNoGit";
            this.lbOerNoGit.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbOerNoGit.Size = new System.Drawing.Size(321, 407);
            this.lbOerNoGit.TabIndex = 9;
            this.lbOerNoGit.ValueMember = "Id";
            // 
            // lbGitNoOer
            // 
            this.lbGitNoOer.DisplayMember = "Nombre";
            this.lbGitNoOer.FormattingEnabled = true;
            this.lbGitNoOer.Location = new System.Drawing.Point(351, 327);
            this.lbGitNoOer.Name = "lbGitNoOer";
            this.lbGitNoOer.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbGitNoOer.Size = new System.Drawing.Size(321, 381);
            this.lbGitNoOer.TabIndex = 12;
            this.lbGitNoOer.ValueMember = "Id";
            // 
            // lbEnGit
            // 
            this.lbEnGit.DisplayMember = "Nombre";
            this.lbEnGit.FormattingEnabled = true;
            this.lbEnGit.Location = new System.Drawing.Point(687, 301);
            this.lbEnGit.Name = "lbEnGit";
            this.lbEnGit.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbEnGit.Size = new System.Drawing.Size(321, 407);
            this.lbEnGit.TabIndex = 14;
            this.lbEnGit.ValueMember = "Id";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(684, 276);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "En GIT";
            // 
            // lbEnOer
            // 
            this.lbEnOer.DisplayMember = "Nombre";
            this.lbEnOer.FormattingEnabled = true;
            this.lbEnOer.Location = new System.Drawing.Point(1014, 301);
            this.lbEnOer.Name = "lbEnOer";
            this.lbEnOer.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbEnOer.Size = new System.Drawing.Size(321, 407);
            this.lbEnOer.TabIndex = 16;
            this.lbEnOer.ValueMember = "Id";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(1011, 276);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "En OER";
            // 
            // bCrearAssets
            // 
            this.bCrearAssets.Location = new System.Drawing.Point(551, 296);
            this.bCrearAssets.Name = "bCrearAssets";
            this.bCrearAssets.Size = new System.Drawing.Size(121, 25);
            this.bCrearAssets.TabIndex = 17;
            this.bCrearAssets.Text = "Crear Assets";
            this.bCrearAssets.UseVisualStyleBackColor = true;
            this.bCrearAssets.Click += new System.EventHandler(this.bCrearAssets_Click);
            // 
            // rbEnOer
            // 
            this.rbEnOer.AutoSize = true;
            this.rbEnOer.Checked = true;
            this.rbEnOer.Location = new System.Drawing.Point(917, 58);
            this.rbEnOer.Name = "rbEnOer";
            this.rbEnOer.Size = new System.Drawing.Size(118, 17);
            this.rbEnOer.TabIndex = 10;
            this.rbEnOer.TabStop = true;
            this.rbEnOer.Text = "En OER, no en GIT";
            this.rbEnOer.UseVisualStyleBackColor = true;
            // 
            // rbEnGit
            // 
            this.rbEnGit.AutoSize = true;
            this.rbEnGit.Location = new System.Drawing.Point(917, 81);
            this.rbEnGit.Name = "rbEnGit";
            this.rbEnGit.Size = new System.Drawing.Size(118, 17);
            this.rbEnGit.TabIndex = 11;
            this.rbEnGit.Text = "En GIT, no en OER";
            this.rbEnGit.UseVisualStyleBackColor = true;
            // 
            // frmConciliador
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1358, 730);
            this.Controls.Add(this.bCrearAssets);
            this.Controls.Add(this.lbEnOer);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lbEnGit);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lbGitNoOer);
            this.Controls.Add(this.lbOerNoGit);
            this.Controls.Add(this.bEliminarRel);
            this.Controls.Add(this.bCrearRel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label3);
            this.Name = "frmConciliador";
            this.Text = "frmConciliador";
            this.Load += new System.EventHandler(this.frmConciliador_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button bCerrar;
        private System.Windows.Forms.Button bActualizar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox cbOperacion;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button bCrearRel;
        private System.Windows.Forms.Button bEliminarRel;
        private System.Windows.Forms.ListBox lbOerNoGit;
        private System.Windows.Forms.ListBox lbGitNoOer;
        private System.Windows.Forms.TextBox tbOperacion;
        private System.Windows.Forms.ListBox lbEnGit;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox lbEnOer;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button bCrearAssets;
        private System.Windows.Forms.RadioButton rbEnGit;
        private System.Windows.Forms.RadioButton rbEnOer;
    }
}