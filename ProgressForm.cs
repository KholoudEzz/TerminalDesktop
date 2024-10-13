using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using ProgressBar = System.Windows.Forms.ProgressBar;

namespace VPrinterMonitor
{
    public partial class ProgressForm : Form
    {
        public ProgressForm()
        {
            InitializeComponent();

        }

        private void ProgressForm_Load(object sender, EventArgs e)
        {
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(workingArea.Right - this.Width, workingArea.Bottom - (this.Height*2) );
        }
    
        public ProgressBar ProgressBar
        {
            get { return PrintProgressBar; }
        }
        public void UpdateProgress(int progress)
        {
            if (PrintProgressBar.InvokeRequired)
            {
                PrintProgressBar.Invoke(new Action<int>(UpdateProgress), progress);
            }
            else
            {
                PrintProgressBar.Value = progress;
            }
        }

    }

}   
