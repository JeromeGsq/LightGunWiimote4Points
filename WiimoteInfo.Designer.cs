namespace LightGunWiimote4Points
{
	partial class WiimoteInfo
	{
		private System.ComponentModel.IContainer components = null;

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
            this.pbIR = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbIR)).BeginInit();
            this.SuspendLayout();
            // 
            // pbIR
            // 
            this.pbIR.Location = new System.Drawing.Point(0, 0);
            this.pbIR.Name = "pbIR";
            this.pbIR.Size = new System.Drawing.Size(1024, 768);
            this.pbIR.TabIndex = 0;
            this.pbIR.TabStop = false;
          
            // 
            // WiimoteInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pbIR);
            this.Name = "WiimoteInfo";
            this.Size = new System.Drawing.Size(1024, 768);
            ((System.ComponentModel.ISupportInitialize)(this.pbIR)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
		}

		#endregion

		public System.Windows.Forms.PictureBox pbIR;
    }
}
