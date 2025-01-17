namespace winUtils
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tDebase = new System.Windows.Forms.Button();
            this.tbDebase = new System.Windows.Forms.TextBox();
            this.bBase = new System.Windows.Forms.Button();
            this.tbBase = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.bFormatJson = new System.Windows.Forms.Button();
            this.tbJson = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(770, 518);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tDebase);
            this.tabPage1.Controls.Add(this.tbDebase);
            this.tabPage1.Controls.Add(this.bBase);
            this.tabPage1.Controls.Add(this.tbBase);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(762, 490);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Base64";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tDebase
            // 
            this.tDebase.Location = new System.Drawing.Point(344, 212);
            this.tDebase.Name = "tDebase";
            this.tDebase.Size = new System.Drawing.Size(75, 23);
            this.tDebase.TabIndex = 1;
            this.tDebase.Text = "debase64 ↑";
            this.tDebase.UseVisualStyleBackColor = true;
            this.tDebase.Click += new System.EventHandler(this.tDebase_Click);
            // 
            // tbDebase
            // 
            this.tbDebase.Location = new System.Drawing.Point(8, 241);
            this.tbDebase.Multiline = true;
            this.tbDebase.Name = "tbDebase";
            this.tbDebase.Size = new System.Drawing.Size(746, 194);
            this.tbDebase.TabIndex = 0;
            // 
            // bBase
            // 
            this.bBase.Location = new System.Drawing.Point(8, 212);
            this.bBase.Name = "bBase";
            this.bBase.Size = new System.Drawing.Size(75, 23);
            this.bBase.TabIndex = 1;
            this.bBase.Text = "base64 ↓";
            this.bBase.UseVisualStyleBackColor = true;
            this.bBase.Click += new System.EventHandler(this.bBase_Click);
            // 
            // tbBase
            // 
            this.tbBase.Location = new System.Drawing.Point(8, 12);
            this.tbBase.Multiline = true;
            this.tbBase.Name = "tbBase";
            this.tbBase.Size = new System.Drawing.Size(746, 194);
            this.tbBase.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.bFormatJson);
            this.tabPage2.Controls.Add(this.tbJson);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(762, 490);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Json Prettify";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // bFormatJson
            // 
            this.bFormatJson.Location = new System.Drawing.Point(679, 6);
            this.bFormatJson.Name = "bFormatJson";
            this.bFormatJson.Size = new System.Drawing.Size(75, 23);
            this.bFormatJson.TabIndex = 2;
            this.bFormatJson.Text = "Format";
            this.bFormatJson.UseVisualStyleBackColor = true;
            this.bFormatJson.Click += new System.EventHandler(this.bFormatJson_Click);
            // 
            // tbJson
            // 
            this.tbJson.Location = new System.Drawing.Point(8, 31);
            this.tbJson.Multiline = true;
            this.tbJson.Name = "tbJson";
            this.tbJson.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbJson.Size = new System.Drawing.Size(746, 451);
            this.tbJson.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(770, 518);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "Form1";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button tDebase;
        private System.Windows.Forms.TextBox tbDebase;
        private System.Windows.Forms.Button bBase;
        private System.Windows.Forms.TextBox tbBase;
        private System.Windows.Forms.Button bFormatJson;
        private System.Windows.Forms.TextBox tbJson;
    }
}

