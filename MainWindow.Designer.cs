﻿namespace BitClean
{
	partial class MainWindow
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
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileMenuStripItem = new System.Windows.Forms.ToolStripMenuItem();
			this.loadImageMenuStripItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveImageMenuStripItem = new System.Windows.Forms.ToolStripMenuItem();
			this.imageMenuStripItem = new System.Windows.Forms.ToolStripMenuItem();
			this.bitCleanMenuStripItem = new System.Windows.Forms.ToolStripMenuItem();
			this.diagnosticsMenuStripItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exportMenuStripItem = new System.Windows.Forms.ToolStripMenuItem();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.BackColor = System.Drawing.Color.DarkSlateGray;
			this.pictureBox1.Location = new System.Drawing.Point(0, 27);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(800, 696);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox1.TabIndex = 1;
			this.pictureBox1.TabStop = false;
			// 
			// menuStrip1
			// 
			this.menuStrip1.BackColor = System.Drawing.Color.MintCream;
			this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenuStripItem,
            this.imageMenuStripItem,
            this.diagnosticsMenuStripItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(800, 24);
			this.menuStrip1.TabIndex = 6;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileMenuStripItem
			// 
			this.fileMenuStripItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadImageMenuStripItem,
            this.saveImageMenuStripItem});
			this.fileMenuStripItem.Name = "fileMenuStripItem";
			this.fileMenuStripItem.Size = new System.Drawing.Size(37, 20);
			this.fileMenuStripItem.Text = "File";
			// 
			// loadImageMenuStripItem
			// 
			this.loadImageMenuStripItem.Name = "loadImageMenuStripItem";
			this.loadImageMenuStripItem.Size = new System.Drawing.Size(136, 22);
			this.loadImageMenuStripItem.Text = "Load Image";
			this.loadImageMenuStripItem.Click += new System.EventHandler(this.loadImageFile_Click);
			// 
			// saveImageMenuStripItem
			// 
			this.saveImageMenuStripItem.Name = "saveImageMenuStripItem";
			this.saveImageMenuStripItem.Size = new System.Drawing.Size(136, 22);
			this.saveImageMenuStripItem.Text = "Save Image";
			this.saveImageMenuStripItem.Click += new System.EventHandler(this.saveImageFile_Click);
			// 
			// imageMenuStripItem
			// 
			this.imageMenuStripItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bitCleanMenuStripItem});
			this.imageMenuStripItem.Name = "imageMenuStripItem";
			this.imageMenuStripItem.Size = new System.Drawing.Size(52, 20);
			this.imageMenuStripItem.Text = "Image";
			// 
			// bitCleanMenuStripItem
			// 
			this.bitCleanMenuStripItem.Name = "bitCleanMenuStripItem";
			this.bitCleanMenuStripItem.Size = new System.Drawing.Size(180, 22);
			this.bitCleanMenuStripItem.Text = "Bit Clean";
			this.bitCleanMenuStripItem.Click += new System.EventHandler(this.bitCleanImage_Click);
			// 
			// diagnosticsMenuStripItem
			// 
			this.diagnosticsMenuStripItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportMenuStripItem});
			this.diagnosticsMenuStripItem.Name = "diagnosticsMenuStripItem";
			this.diagnosticsMenuStripItem.Size = new System.Drawing.Size(80, 20);
			this.diagnosticsMenuStripItem.Text = "Diagnostics";
			// 
			// exportMenuStripItem
			// 
			this.exportMenuStripItem.Name = "exportMenuStripItem";
			this.exportMenuStripItem.Size = new System.Drawing.Size(180, 22);
			this.exportMenuStripItem.Text = "Export Diagnostics...";
			this.exportMenuStripItem.Click += new System.EventHandler(this.exportDiagnostics_Click);
			// 
			// MainWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.ClientSize = new System.Drawing.Size(800, 723);
			this.Controls.Add(this.menuStrip1);
			this.Controls.Add(this.pictureBox1);
			this.ForeColor = System.Drawing.SystemColors.ControlText;
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "MainWindow";
			this.Text = "BitClean - Prototype";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.PictureBox pictureBox1;

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileMenuStripItem;
		private System.Windows.Forms.ToolStripMenuItem loadImageMenuStripItem;
		private System.Windows.Forms.ToolStripMenuItem saveImageMenuStripItem;

		private System.Windows.Forms.ToolStripMenuItem imageMenuStripItem;
		private System.Windows.Forms.ToolStripMenuItem bitCleanMenuStripItem;

		private System.Windows.Forms.ToolStripMenuItem diagnosticsMenuStripItem;
		private System.Windows.Forms.ToolStripMenuItem exportMenuStripItem;
	}
}
