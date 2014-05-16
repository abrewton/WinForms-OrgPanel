
namespace CheckBoxStudio.WinForms {

    public partial class OrgPanel {

        private System.ComponentModel.IContainer components = null;

        /// <summary>Clean up any resources being used.</summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose (bool disposing) {

            if (disposing) {

                if (shadowBrush != null)
                    shadowBrush.Dispose();

                if (linkBackPen != null)
                    linkBackPen.Dispose();

                if (linkForePen != null)
                    linkForePen.Dispose();

                if (components != null)
                    components.Dispose();

                if (license != null)
                    license.Dispose();
                
            }

            base.Dispose(disposing);

        }

        #region Component Designer generated code

        private void InitializeComponent () {

            components = new System.ComponentModel.Container();
            
            this.DoubleBuffered = true;
            this.Size = new System.Drawing.Size(200, 100);
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            base.BackColor = System.Drawing.SystemColors.Control;
            this.Padding = new System.Windows.Forms.Padding(12);
            this.AutoScrollMargin = new System.Drawing.Size(16, 16);

        }

        #endregion

    }

}
