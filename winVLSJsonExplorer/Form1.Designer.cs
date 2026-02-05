namespace winVLSJsonExplorer
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            flowLayoutPanel1 = new FlowLayoutPanel();
            groupBox1 = new GroupBox();
            bMostrarConPrecio = new Button();
            bCerrar = new Button();
            bCargar = new Button();
            tbArchivo = new TextBox();
            bBuscar = new Button();
            treeView1 = new TreeView();
            openFileDialog1 = new OpenFileDialog();
            bAbrir = new Button();
            flowLayoutPanel1.SuspendLayout();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.AutoSize = true;
            flowLayoutPanel1.Controls.Add(groupBox1);
            flowLayoutPanel1.Controls.Add(treeView1);
            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.Location = new Point(0, 0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(979, 979);
            flowLayoutPanel1.TabIndex = 0;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(bAbrir);
            groupBox1.Controls.Add(bMostrarConPrecio);
            groupBox1.Controls.Add(bCerrar);
            groupBox1.Controls.Add(bCargar);
            groupBox1.Controls.Add(tbArchivo);
            groupBox1.Controls.Add(bBuscar);
            groupBox1.Location = new Point(3, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(964, 113);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "groupBox1";
            groupBox1.Enter += groupBox1_Enter;
            // 
            // bMostrarConPrecio
            // 
            bMostrarConPrecio.Location = new Point(234, 78);
            bMostrarConPrecio.Name = "bMostrarConPrecio";
            bMostrarConPrecio.Size = new Size(116, 29);
            bMostrarConPrecio.TabIndex = 4;
            bMostrarConPrecio.Text = "Mostrar con $";
            bMostrarConPrecio.UseVisualStyleBackColor = true;
            bMostrarConPrecio.Click += bMostrarConPrecio_Click;
            // 
            // bCerrar
            // 
            bCerrar.Location = new Point(6, 78);
            bCerrar.Name = "bCerrar";
            bCerrar.Size = new Size(94, 29);
            bCerrar.TabIndex = 3;
            bCerrar.Text = "Cerrar";
            bCerrar.UseVisualStyleBackColor = true;
            bCerrar.Click += bCerrar_Click;
            // 
            // bCargar
            // 
            bCargar.Enabled = false;
            bCargar.Location = new Point(781, 29);
            bCargar.Name = "bCargar";
            bCargar.Size = new Size(94, 29);
            bCargar.TabIndex = 2;
            bCargar.Text = "Cargar";
            bCargar.UseVisualStyleBackColor = true;
            bCargar.Click += bCargar_Click;
            // 
            // tbArchivo
            // 
            tbArchivo.Location = new Point(119, 31);
            tbArchivo.Name = "tbArchivo";
            tbArchivo.ReadOnly = true;
            tbArchivo.Size = new Size(656, 27);
            tbArchivo.TabIndex = 1;
            // 
            // bBuscar
            // 
            bBuscar.Location = new Point(9, 26);
            bBuscar.Name = "bBuscar";
            bBuscar.Size = new Size(94, 29);
            bBuscar.TabIndex = 0;
            bBuscar.Text = "Buscar";
            bBuscar.UseVisualStyleBackColor = true;
            bBuscar.Click += bBuscar_Click;
            // 
            // treeView1
            // 
            treeView1.Font = new Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            treeView1.Location = new Point(3, 122);
            treeView1.Name = "treeView1";
            treeView1.Size = new Size(964, 875);
            treeView1.TabIndex = 1;
            // 
            // openFileDialog1
            // 
            openFileDialog1.DefaultExt = "*.json";
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // bAbrir
            // 
            bAbrir.Location = new Point(119, 78);
            bAbrir.Name = "bAbrir";
            bAbrir.Size = new Size(94, 29);
            bAbrir.TabIndex = 5;
            bAbrir.Text = "Abrir";
            bAbrir.UseVisualStyleBackColor = true;
            bAbrir.Click += bAbrir_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(979, 979);
            Controls.Add(flowLayoutPanel1);
            Name = "Form1";
            Text = "Form1";
            flowLayoutPanel1.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private FlowLayoutPanel flowLayoutPanel1;
        private GroupBox groupBox1;
        private Button bBuscar;
        private OpenFileDialog openFileDialog1;
        private Button bCargar;
        private TextBox tbArchivo;
        private TreeView treeView1;
        private Button bCerrar;
        private Button bMostrarConPrecio;
        private Button bAbrir;
    }
}
