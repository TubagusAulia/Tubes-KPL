namespace ManajemenPerpus.GUI
{
    partial class UlasanPage
    {
        /// <summary>Required designer variable.</summary>
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            ulasanControllerBindingSource1 = new BindingSource(components);
            ulasanServiceBindingSource     = new BindingSource(components);
            ulasanTextBox = new TextBox();
            btnSubmit     = new ManajemenPerpus.GUI.CustomControl.CustomButton();
            hapusBtn      = new ManajemenPerpus.GUI.CustomControl.CustomButton();

            ((System.ComponentModel.ISupportInitialize)ulasanControllerBindingSource1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ulasanServiceBindingSource).BeginInit();
            SuspendLayout();

            ulasanControllerBindingSource1.DataSource = typeof(API.Controllers.UlasanController);

            // ulasanTextBox
            ulasanTextBox.Multiline       = true;
            ulasanTextBox.PlaceholderText = "Bagaimana pendapatmu tentang buku ini?";
            ulasanTextBox.Name            = "ulasanTextBox";
            ulasanTextBox.TabIndex        = 0;

            // btnSubmit (Kirim)
            btnSubmit.Text                      = "Kirim";
            btnSubmit.Name                      = "btnSubmit";
            btnSubmit.TabIndex                  = 1;
            btnSubmit.BackColor                 = Color.RoyalBlue;
            btnSubmit.BackgroundColor           = Color.RoyalBlue;
            btnSubmit.BorderColor               = Color.PaleVioletRed;
            btnSubmit.BorderRadius              = 10;
            btnSubmit.BorderSize                = 0;
            btnSubmit.FlatStyle                 = FlatStyle.Flat;
            btnSubmit.FlatAppearance.BorderSize = 0;
            btnSubmit.Font                      = new Font("Segoe UI Semibold", 10.125F, FontStyle.Bold);
            btnSubmit.ForeColor                 = Color.White;
            btnSubmit.TextColor                 = Color.White;
            btnSubmit.UseVisualStyleBackColor    = false;
            btnSubmit.Click                    += btnSubmit_Click;

            // hapusBtn (Hapus)
            hapusBtn.Text                      = "Hapus";
            hapusBtn.Name                      = "hapusBtn";
            hapusBtn.TabIndex                  = 2;
            hapusBtn.BackColor                 = Color.White;
            hapusBtn.BackgroundColor           = Color.White;
            hapusBtn.BorderColor               = Color.PaleVioletRed;
            hapusBtn.BorderRadius              = 10;
            hapusBtn.BorderSize                = 0;
            hapusBtn.FlatStyle                 = FlatStyle.Flat;
            hapusBtn.FlatAppearance.BorderSize = 0;
            hapusBtn.Font                      = new Font("Segoe UI Semibold", 10.125F, FontStyle.Bold);
            hapusBtn.ForeColor                 = Color.Red;
            hapusBtn.TextColor                 = Color.Red;
            hapusBtn.UseVisualStyleBackColor    = false;
            hapusBtn.Click                    += hapusBtn_Click;

            // Form
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode       = AutoScaleMode.Font;
            BackColor           = Color.White;
            ClientSize          = new Size(1008, 729);
            Name                = "UlasanPage";
            StartPosition       = FormStartPosition.CenterScreen;
            Text                = "Ulasan";

            ((System.ComponentModel.ISupportInitialize)ulasanControllerBindingSource1).EndInit();
            ((System.ComponentModel.ISupportInitialize)ulasanServiceBindingSource).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private BindingSource ulasanControllerBindingSource1;
        private BindingSource ulasanServiceBindingSource;
        private ManajemenPerpus.GUI.CustomControl.CustomButton btnSubmit;
        private ManajemenPerpus.GUI.CustomControl.CustomButton hapusBtn;
        private TextBox ulasanTextBox;
    }
}