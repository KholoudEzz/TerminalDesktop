namespace VPrinterMonitor
{
    partial class ProgressForm
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
            PrintProgressBar = new ProgressBar();
            SuspendLayout();
            // 
            // PrintProgressBar
            // 
            PrintProgressBar.Location = new Point(2, 12);
            PrintProgressBar.Name = "PrintProgressBar";
            PrintProgressBar.Size = new Size(494, 30);
            PrintProgressBar.TabIndex = 0;
            // 
            // ProgressForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(498, 49);
            Controls.Add(PrintProgressBar);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ProgressForm";
            Load += ProgressForm_Load;
            ResumeLayout(false);
        }

        #endregion

        private ProgressBar PrintProgressBar;
    }
}