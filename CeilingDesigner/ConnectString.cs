using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CeilingDesigner
{
    public partial class ConnectString : Form
    {
        public ConnectString()
        {
            InitializeComponent();
        }

        private void OKbutton_Click(object sender, EventArgs e)
        {
            ShareData.Server = this.hostTextBox.Text;
            ShareData.ConnectString = "server=" + this.hostTextBox.Text + ";User Id=" + this.unameTextBox.Text + ";database=" + this.dbNameTextBox.Text + ";password=" + this.pwordTextBox.Text + ";Character Set=utf8";
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
