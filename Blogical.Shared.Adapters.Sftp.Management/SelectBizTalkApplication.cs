using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Blogical.Shared.Adapters.Sftp.Management
{
    public partial class SelectBizTalkApplication : Form
    {
        public SelectBizTalkApplication()
        {
            InitializeComponent();
        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SelectBizTalkApplication_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
        }
    }
}