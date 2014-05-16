
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace CheckBoxStudio.WinForms {

    internal partial class About : Form {

        public About () {

            InitializeComponent();

        }

        private void lnkCompany_LinkClicked (object sender, LinkLabelLinkClickedEventArgs e) {

            Process.Start("www.checkboxstudio.com");

        }

        private void txtInfo_TextChanged (object sender, EventArgs e) {

        }

    }

}
