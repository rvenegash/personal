﻿namespace revisaProyectosSCA
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
            this.bOpen = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.tbFolder = new System.Windows.Forms.TextBox();
            this.lbArtifact = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.tvParts = new System.Windows.Forms.TreeView();
            this.bParse = new System.Windows.Forms.Button();
            this.lblArtifact = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // bOpen
            // 
            this.bOpen.Location = new System.Drawing.Point(876, 12);
            this.bOpen.Name = "bOpen";
            this.bOpen.Size = new System.Drawing.Size(75, 23);
            this.bOpen.TabIndex = 0;
            this.bOpen.Text = "Open";
            this.bOpen.UseVisualStyleBackColor = true;
            this.bOpen.Click += new System.EventHandler(this.bOpen_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Folder";
            // 
            // tbFolder
            // 
            this.tbFolder.Location = new System.Drawing.Point(46, 12);
            this.tbFolder.Name = "tbFolder";
            this.tbFolder.ReadOnly = true;
            this.tbFolder.Size = new System.Drawing.Size(824, 20);
            this.tbFolder.TabIndex = 2;
            // 
            // lbArtifact
            // 
            this.lbArtifact.FormattingEnabled = true;
            this.lbArtifact.Location = new System.Drawing.Point(7, 65);
            this.lbArtifact.Name = "lbArtifact";
            this.lbArtifact.Size = new System.Drawing.Size(163, 563);
            this.lbArtifact.TabIndex = 3;
            this.lbArtifact.SelectedIndexChanged += new System.EventHandler(this.lbArtifact_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.progressBar1);
            this.panel1.Controls.Add(this.tvParts);
            this.panel1.Controls.Add(this.bParse);
            this.panel1.Controls.Add(this.lblArtifact);
            this.panel1.Location = new System.Drawing.Point(177, 61);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(774, 567);
            this.panel1.TabIndex = 4;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(288, 3);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(402, 23);
            this.progressBar1.TabIndex = 4;
            this.progressBar1.Visible = false;
            // 
            // tvParts
            // 
            this.tvParts.Location = new System.Drawing.Point(7, 33);
            this.tvParts.Name = "tvParts";
            this.tvParts.Size = new System.Drawing.Size(764, 526);
            this.tvParts.TabIndex = 3;
            // 
            // bParse
            // 
            this.bParse.Location = new System.Drawing.Point(696, 4);
            this.bParse.Name = "bParse";
            this.bParse.Size = new System.Drawing.Size(75, 23);
            this.bParse.TabIndex = 2;
            this.bParse.Text = "Parse";
            this.bParse.UseVisualStyleBackColor = true;
            this.bParse.Click += new System.EventHandler(this.bParse_Click);
            // 
            // lblArtifact
            // 
            this.lblArtifact.AutoSize = true;
            this.lblArtifact.Location = new System.Drawing.Point(4, 4);
            this.lblArtifact.Name = "lblArtifact";
            this.lblArtifact.Size = new System.Drawing.Size(39, 13);
            this.lblArtifact.TabIndex = 0;
            this.lblArtifact.Text = "Label2";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(963, 632);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lbArtifact);
            this.Controls.Add(this.tbFolder);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bOpen);
            this.Name = "Form1";
            this.Text = "SCA Analyzer";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bOpen;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TextBox tbFolder;
        private System.Windows.Forms.ListBox lbArtifact;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblArtifact;
        private System.Windows.Forms.Button bParse;
        private System.Windows.Forms.TreeView tvParts;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}