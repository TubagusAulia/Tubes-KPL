namespace ManajemenPerpus.GUI
{
    partial class Sirkulasi
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
            // ── Peminjaman form controls ──────────────────────────────────────
            textBoxIdAnggota        = new TextBox();
            comboBoxBuku            = new ComboBox();
            labelTanggalKembali     = new Label();
            buttonPinjam            = new Button();
            buttonResetPeminjaman   = new Button();

            // ── Pengembalian form controls ────────────────────────────────────
            textBoxIdPeminjamanReturn       = new TextBox();
            labelDisplayBukuReturn          = new Label();
            labelDisplayIdAnggotaReturn     = new Label();
            labelDisplayBatasPengembalian   = new Label();
            labelDisplayStatus              = new Label();
            labelDisplayDenda               = new Label();
            buttonCek                       = new Button();
            buttonKembalikan                = new Button();
            buttonResetPengembalian         = new Button();

            // ── Deprecated header (hidden at runtime) ─────────────────────────
            panelHeader = new Panel();

            SuspendLayout();

            // All layout and styling is done in BuildLayout() / code-behind.
            // Only event wiring that the designer must know about:
            buttonPinjam.Click              += buttonPinjam_Click;
            buttonResetPeminjaman.Click     += buttonResetPeminjaman_Click;
            buttonCek.Click                 += buttonCek_Click;
            buttonKembalikan.Click          += buttonKembalikan_Click;
            buttonResetPengembalian.Click   += buttonResetPengembalian_Click;

            // Form defaults
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode       = AutoScaleMode.Font;
            BackColor           = Color.FromArgb(247, 248, 253);
            ClientSize          = new Size(1008, 729);
            FormBorderStyle     = FormBorderStyle.Sizable;
            MaximizeBox         = true;
            Name                = "Sirkulasi";
            StartPosition       = FormStartPosition.CenterScreen;
            Text                = "Pinjaman — Sirkulasi";

            ResumeLayout(false);
        }

        #endregion

        // ── Designer-required field declarations ──────────────────────────────
        private Panel    panelHeader;
        private TextBox  textBoxIdAnggota;
        private ComboBox comboBoxBuku;
        private Label    labelTanggalKembali;
        private Button   buttonPinjam;
        private Button   buttonResetPeminjaman;

        private TextBox textBoxIdPeminjamanReturn;
        private Label   labelDisplayBukuReturn;
        private Label   labelDisplayIdAnggotaReturn;
        private Label   labelDisplayBatasPengembalian;
        private Label   labelDisplayStatus;
        private Label   labelDisplayDenda;
        private Button  buttonCek;
        private Button  buttonKembalikan;
        private Button  buttonResetPengembalian;
    }
}
