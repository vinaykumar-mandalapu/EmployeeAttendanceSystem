namespace EAS_QRScanner
{
    partial class EmployeeAttendance
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
            this.videoPanel = new System.Windows.Forms.Panel();
            this.checkInButton = new System.Windows.Forms.Button();
            this.scanCardButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // videoPanel
            // 
            this.videoPanel.Location = new System.Drawing.Point(112, 22);
            this.videoPanel.Name = "videoPanel";
            this.videoPanel.Size = new System.Drawing.Size(1028, 735);
            this.videoPanel.TabIndex = 0;
            this.videoPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.OnvideoPanelPaint);
            // 
            // checkInButton
            // 
            this.checkInButton.Location = new System.Drawing.Point(735, 794);
            this.checkInButton.Name = "checkInButton";
            this.checkInButton.Size = new System.Drawing.Size(288, 118);
            this.checkInButton.TabIndex = 0;
            this.checkInButton.Text = "Check In";
            this.checkInButton.UseVisualStyleBackColor = true;
            this.checkInButton.Click += new System.EventHandler(this.checkInButton_Click);
            // 
            // scanCardButton
            // 
            this.scanCardButton.Location = new System.Drawing.Point(261, 794);
            this.scanCardButton.Name = "scanCardButton";
            this.scanCardButton.Size = new System.Drawing.Size(294, 118);
            this.scanCardButton.TabIndex = 1;
            this.scanCardButton.Text = "Scan Card";
            this.scanCardButton.UseVisualStyleBackColor = true;
            this.scanCardButton.Click += new System.EventHandler(this.scanCardButton_Click);
            // 
            // EmployeeAttendance
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 30F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1282, 983);
            this.Controls.Add(this.scanCardButton);
            this.Controls.Add(this.checkInButton);
            this.Controls.Add(this.videoPanel);
            this.Name = "EmployeeAttendance";
            this.Text = "EmployeeAttendance";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnClosing);
            this.Load += new System.EventHandler(this.OnLoad);
            this.Resize += new System.EventHandler(this.OnResize);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Panel videoPanel;
        private Button checkInButton;
        private Button scanCardButton;
    }
}