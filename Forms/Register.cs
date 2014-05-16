
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CheckBoxStudio.WinForms {

    internal partial class Register : Form {

        public Register () {

            InitializeComponent();

        }

        private void btnOK_Click (object sender, EventArgs e) {

            int sum = 0;
            string key = txtLicenseKey.Text.ToUpper();

            for (int i = 0; i < key.Length; i++) {

                sum += (int)(char)key[i] * (i + 1);

            }

            //foreach (char chr in key) {
            //sum += (int)chr;
            //}

            MessageBox.Show(sum.ToString());

        }

    }

}
