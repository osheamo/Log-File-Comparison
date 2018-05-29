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
    public partial class ClusterDetailsForm : Form
    {
        public string logName = "";
        public ClusterDetailsForm()
        {
            InitializeComponent();
            CenterToParent();
        }

        private void backToMainButton_Click(object sender, EventArgs e)
        {
            
            this.Close();
            
        }
       
        public void LogName(string name)
        {
            logName = name.ToString();
            mainLogNameLabel.Text = logName.ToString();
        }

      
    }
}
