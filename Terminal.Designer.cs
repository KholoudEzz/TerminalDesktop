namespace VPrinterMonitor
{
    partial class Terminal
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
            SendBtn = new Button();
            label1 = new Label();
            PrintersCBox = new ComboBox();
            RichTBox = new RichTextBox();
            label2 = new Label();
            label3 = new Label();
            BalanceUdbBtn = new Button();
            sqlConnection1 = new Microsoft.Data.SqlClient.SqlConnection();
            XMLPrintBtn = new Button();
            SuspendLayout();
            // 
            // SendBtn
            // 
            SendBtn.Location = new Point(330, 43);
            SendBtn.Margin = new Padding(3, 4, 3, 4);
            SendBtn.Name = "SendBtn";
            SendBtn.Size = new Size(183, 36);
            SendBtn.TabIndex = 0;
            SendBtn.Text = "Send To Physical Printer";
            SendBtn.UseVisualStyleBackColor = true;
            SendBtn.Click += SendBtn_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(37, 47);
            label1.Name = "label1";
            label1.Size = new Size(91, 20);
            label1.TabIndex = 2;
            label1.Text = "Printers List :";
            // 
            // PrintersCBox
            // 
            PrintersCBox.FormattingEnabled = true;
            PrintersCBox.Location = new Point(125, 43);
            PrintersCBox.Margin = new Padding(3, 4, 3, 4);
            PrintersCBox.Name = "PrintersCBox";
            PrintersCBox.Size = new Size(198, 28);
            PrintersCBox.TabIndex = 3;
            PrintersCBox.SelectedIndexChanged += PrintersCBox_SelectedIndexChanged;
            // 
            // RichTBox
            // 
            RichTBox.Location = new Point(37, 152);
            RichTBox.Margin = new Padding(3, 4, 3, 4);
            RichTBox.Name = "RichTBox";
            RichTBox.RightToLeft = RightToLeft.No;
            RichTBox.Size = new Size(476, 219);
            RichTBox.TabIndex = 4;
            RichTBox.Text = "";
            RichTBox.Visible = false;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(37, 407);
            label2.Name = "label2";
            label2.Size = new Size(0, 20);
            label2.TabIndex = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(37, 456);
            label3.Name = "label3";
            label3.Size = new Size(0, 20);
            label3.TabIndex = 7;
            // 
            // BalanceUdbBtn
            // 
            BalanceUdbBtn.Location = new Point(37, 99);
            BalanceUdbBtn.Margin = new Padding(3, 4, 3, 4);
            BalanceUdbBtn.Name = "BalanceUdbBtn";
            BalanceUdbBtn.Size = new Size(183, 36);
            BalanceUdbBtn.TabIndex = 8;
            BalanceUdbBtn.Text = "Check for New Balance";
            BalanceUdbBtn.UseVisualStyleBackColor = true;
            BalanceUdbBtn.Visible = false;
            BalanceUdbBtn.Click += DBalance_UpdateDB_Click;
            // 
            // sqlConnection1
            // 
            sqlConnection1.AccessTokenCallback = null;
            sqlConnection1.FireInfoMessageEventOnUserErrors = false;
            // 
            // XMLPrintBtn
            // 
            XMLPrintBtn.Location = new Point(330, 99);
            XMLPrintBtn.Margin = new Padding(3, 4, 3, 4);
            XMLPrintBtn.Name = "XMLPrintBtn";
            XMLPrintBtn.Size = new Size(183, 36);
            XMLPrintBtn.TabIndex = 9;
            XMLPrintBtn.Text = "Rashid Printer";
            XMLPrintBtn.UseVisualStyleBackColor = true;
            XMLPrintBtn.Click += XMLPrintBtn_Click;
            // 
            // Terminal
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(573, 459);
            Controls.Add(XMLPrintBtn);
            Controls.Add(BalanceUdbBtn);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(PrintersCBox);
            Controls.Add(label1);
            Controls.Add(SendBtn);
            Controls.Add(RichTBox);
            Margin = new Padding(3, 4, 3, 4);
            Name = "Terminal";
            Text = "Terminal     -Version 1.0.3";
            FormClosing += Terminal_FormClosing;
            Load += Terminal_Load;
            Shown += Terminal_Shown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button SendBtn;
        private Label label1;
        private ComboBox PrintersCBox;
        private RichTextBox RichTBox;
        private Label label2;
        private Label label3;
        private Button BalanceUdbBtn;
        private Microsoft.Data.SqlClient.SqlConnection sqlConnection1;
        private Button XMLPrintBtn;
    }
}
