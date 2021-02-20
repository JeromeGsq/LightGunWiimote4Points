namespace WiimoteTest
{
	partial class WiimoteInfo
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.clbButtons = new System.Windows.Forms.CheckedListBox();
            this.pbIR = new System.Windows.Forms.PictureBox();
            this.resolution_x = new System.Windows.Forms.TextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.resolution_y = new System.Windows.Forms.TextBox();
            this.groupBox8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbIR)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.clbButtons);
            this.groupBox8.Location = new System.Drawing.Point(3, 3);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(72, 220);
            this.groupBox8.TabIndex = 37;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Wiimote";
            // 
            // clbButtons
            // 
            this.clbButtons.FormattingEnabled = true;
            this.clbButtons.Items.AddRange(new object[] {
            "A",
            "B",
            "-",
            "Home",
            "+",
            "1",
            "2",
            "Up",
            "Down",
            "Left",
            "Right"});
            this.clbButtons.Location = new System.Drawing.Point(8, 16);
            this.clbButtons.Name = "clbButtons";
            this.clbButtons.Size = new System.Drawing.Size(56, 184);
            this.clbButtons.TabIndex = 1;
            // 
            // pbIR
            // 
            this.pbIR.Location = new System.Drawing.Point(81, 11);
            this.pbIR.Name = "pbIR";
            this.pbIR.Size = new System.Drawing.Size(940, 754);
            this.pbIR.TabIndex = 28;
            this.pbIR.TabStop = false;
            // 
            // resolution_x
            // 
            this.resolution_x.Location = new System.Drawing.Point(3, 229);
            this.resolution_x.Name = "resolution_x";
            this.resolution_x.Size = new System.Drawing.Size(72, 20);
            this.resolution_x.TabIndex = 38;
            this.resolution_x.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // resolution_y
            // 
            this.resolution_y.Location = new System.Drawing.Point(3, 255);
            this.resolution_y.Name = "resolution_y";
            this.resolution_y.Size = new System.Drawing.Size(72, 20);
            this.resolution_y.TabIndex = 39;
            // 
            // WiimoteInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.resolution_y);
            this.Controls.Add(this.resolution_x);
            this.Controls.Add(this.groupBox8);
            this.Controls.Add(this.pbIR);
            this.Name = "WiimoteInfo";
            this.Size = new System.Drawing.Size(1024, 768);
            this.groupBox8.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbIR)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		public System.Windows.Forms.GroupBox groupBox8;
		public System.Windows.Forms.CheckedListBox clbButtons;
		public System.Windows.Forms.PictureBox pbIR;
        private System.Windows.Forms.TextBox resolution_x;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.TextBox resolution_y;
    }
}
