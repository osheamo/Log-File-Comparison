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
        public ClusterDetailsForm()
        {
            InitializeComponent();
            CenterToParent();
        }

        private void backToMainButton_Click(object sender, EventArgs e)
        {
            
            this.Close();
            
        }
    }
}
