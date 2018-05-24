using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Log_File_Comparison
{


    public partial class loadingForm : Form
    {
        private DataGridView _dgv;
        Form1 check;
        long totprogress = 0;
        long sizefile = 0;
        Boolean canceled = false;
        Boolean complete = false;
        public loadingForm(DataGridView dgv, Form1 frm, long filesize, long progress)
        {
            InitializeComponent();
            CenterToScreen();
            
            _dgv = dgv;
            check = frm;
            sizefile = filesize;
            totprogress = progress;
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            canceled = true;
            CancelButton.Enabled = true;
        }
        internal void setFilterprogress(long progress)
        {
           
            int per = (int)(((double)progress / (double)sizefile) * 100);
            progressBar1.Value = per;
            if (progressBar1.Value > 99)
            {
                Close();
            }
        }

        internal bool iscanceled()
        {
            return canceled;
        }

        internal bool completed()
        {
            return complete;
        }
    }
}
