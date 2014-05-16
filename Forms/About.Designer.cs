
namespace CheckBoxStudio.WinForms {

    partial class About {

        private System.ComponentModel.IContainer components = null;

        protected override void Dispose (bool disposing) {

            if (disposing && (components != null)) {

                components.Dispose();

            }

            base.Dispose(disposing);

        }

        #region Windows Form Designer generated code

        private void InitializeComponent () {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
            this.btnOK = new System.Windows.Forms.Button();
            this.lblProduct = new System.Windows.Forms.Label();
            this.lnkCompany = new System.Windows.Forms.LinkLabel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tpProduct = new System.Windows.Forms.TabPage();
            this.tpAgreement = new System.Windows.Forms.TabPage();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.tpProduct.SuspendLayout();
            this.tpAgreement.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(348, 293);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // lblProduct
            // 
            this.lblProduct.AutoSize = true;
            this.lblProduct.Location = new System.Drawing.Point(6, 12);
            this.lblProduct.Name = "lblProduct";
            this.lblProduct.Size = new System.Drawing.Size(228, 75);
            this.lblProduct.TabIndex = 4;
            this.lblProduct.Text = "CheckBox Studio - WinForm Components\r\nVersion 2.1.1.0\r\n\r\nCopyright © 2013 CheckBo" +
    "x Studio\r\nAll Rights Reserved.";
            // 
            // lnkCompany
            // 
            this.lnkCompany.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lnkCompany.AutoSize = true;
            this.lnkCompany.Location = new System.Drawing.Point(4, 225);
            this.lnkCompany.Margin = new System.Windows.Forms.Padding(4);
            this.lnkCompany.Name = "lnkCompany";
            this.lnkCompany.Size = new System.Drawing.Size(147, 15);
            this.lnkCompany.TabIndex = 4;
            this.lnkCompany.TabStop = true;
            this.lnkCompany.Text = "www.checkboxstudio.com";
            this.lnkCompany.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkCompany_LinkClicked);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tpProduct);
            this.tabControl1.Controls.Add(this.tpAgreement);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(411, 275);
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl1.TabIndex = 8;
            // 
            // tpProduct
            // 
            this.tpProduct.Controls.Add(this.lblProduct);
            this.tpProduct.Controls.Add(this.lnkCompany);
            this.tpProduct.Location = new System.Drawing.Point(4, 24);
            this.tpProduct.Name = "tpProduct";
            this.tpProduct.Padding = new System.Windows.Forms.Padding(3);
            this.tpProduct.Size = new System.Drawing.Size(403, 247);
            this.tpProduct.TabIndex = 0;
            this.tpProduct.Text = "Product";
            this.tpProduct.UseVisualStyleBackColor = true;
            // 
            // tpAgreement
            // 
            this.tpAgreement.Controls.Add(this.textBox1);
            this.tpAgreement.Location = new System.Drawing.Point(4, 24);
            this.tpAgreement.Name = "tpAgreement";
            this.tpAgreement.Padding = new System.Windows.Forms.Padding(3);
            this.tpAgreement.Size = new System.Drawing.Size(403, 247);
            this.tpAgreement.TabIndex = 1;
            this.tpAgreement.Text = "Agreement";
            this.tpAgreement.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(6, 6);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(391, 235);
            this.textBox1.TabIndex = 0;
            this.textBox1.Text = resources.GetString("textBox1.Text");
            // 
            // About
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(435, 328);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnOK);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "About";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About CheckBox Studio WinForm Components";
            this.tabControl1.ResumeLayout(false);
            this.tpProduct.ResumeLayout(false);
            this.tpProduct.PerformLayout();
            this.tpAgreement.ResumeLayout(false);
            this.tpAgreement.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lblProduct;
        private System.Windows.Forms.LinkLabel lnkCompany;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tpProduct;
        private System.Windows.Forms.TabPage tpAgreement;
        private System.Windows.Forms.TextBox textBox1;

    }

}