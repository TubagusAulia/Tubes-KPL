using ManajemenPerpus.CLI.Service;
using ManajemenPerpus.Core.Models;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ManajemenPerpus.GUI
{
    public partial class Sirkulasi : Form
    {
        private readonly PinjamanService pinjamanService;

        // ── Theme colors ───────────────────────────────────────────────
        private static readonly Color BgColor       = Color.FromArgb(247, 248, 253);
        private static readonly Color CardBg        = Color.White;
        private static readonly Color PrimaryBlue   = Color.FromArgb(58,  90, 230);
        private static readonly Color TextDark      = Color.FromArgb(25,  30,  90);
        private static readonly Color TextMuted     = Color.FromArgb(110, 115, 160);
        private static readonly Color BorderColor   = Color.FromArgb(220, 224, 240);
        
        public Sirkulasi()
        {
            InitializeComponent();
            
            pinjamanService = new PinjamanService();
            
            BuildLayout();
            
            this.FormClosed += (s, args) => Application.Exit();
            this.Load += Sirkulasi_Load;
        }

        private void Sirkulasi_Load(object sender, EventArgs e)
        {
            LoadComboBoxBuku();
            SetDefaultTanggalKembali();
            
            textBoxIdPeminjamanReturn.Text = string.Empty;
            buttonResetPeminjaman_Click(null, null);
            buttonResetPengembalian_Click(null, null);
        }

        // ════════════════════════════════════════════════════════════════════
        //  Layout
        // ════════════════════════════════════════════════════════════════════
        private void BuildLayout()
        {
            this.ClientSize    = new Size(1008, 729);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor     = BgColor;

            // 1. Container for the two forms side-by-side
            var tableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(30, 40, 30, 40),
                BackColor = BgColor
            };
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            var panelPeminjaman = BuildPeminjamanCard();
            panelPeminjaman.Margin = new Padding(0, 0, 15, 0); // Spacing on the right

            var panelPengembalian = BuildPengembalianCard();
            panelPengembalian.Margin = new Padding(15, 0, 0, 0); // Spacing on the left

            tableLayout.Controls.Add(panelPeminjaman, 0, 0);
            tableLayout.Controls.Add(panelPengembalian, 1, 0);

            this.Controls.Add(tableLayout);

            // 2. Page Title Header
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = BgColor,
                Padding = new Padding(30, 25, 30, 0)
            };
            var lblTitle = new Label
            {
                Text = "Sirkulasi Buku",
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = TextDark,
                AutoSize = true,
                Location = new Point(30, 25)
            };
            var lblSubtitle = new Label
            {
                Text = "Kelola peminjaman dan pengembalian koleksi perpustakaan.",
                Font = new Font("Segoe UI", 10F),
                ForeColor = TextMuted,
                AutoSize = true,
                Location = new Point(32, 60)
            };
            headerPanel.Controls.Add(lblSubtitle);
            headerPanel.Controls.Add(lblTitle);
            
            this.Controls.Add(headerPanel);

            // 3. Navbar
            this.Controls.Add(UIHelper.BuildNavbar(this, false));
        }

        private Panel BuildPeminjamanCard()
        {
            var card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = CardBg,
                Padding = new Padding(30),
                BorderStyle = BorderStyle.None,
            };
            
            int currentY = 30;
            
            var lblHeader = new Label { Text = "Peminjaman Baru", Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, Location = new Point(30, currentY) };
            currentY += 50;

            var lblId = new Label { Text = "ID Anggota", Font = new Font("Segoe UI", 10F), ForeColor = TextMuted, AutoSize = true, Location = new Point(30, currentY) };
            currentY += 25;
            textBoxIdAnggota.Location = new Point(30, currentY);
            textBoxIdAnggota.Width = 370;
            textBoxIdAnggota.Font = new Font("Segoe UI", 11F);
            textBoxIdAnggota.BackColor = BgColor;
            textBoxIdAnggota.BorderStyle = BorderStyle.FixedSingle;
            currentY += 55;

            var lblBuku = new Label { Text = "Pilih Buku", Font = new Font("Segoe UI", 10F), ForeColor = TextMuted, AutoSize = true, Location = new Point(30, currentY) };
            currentY += 25;
            comboBoxBuku.Location = new Point(30, currentY);
            comboBoxBuku.Width = 370;
            comboBoxBuku.Font = new Font("Segoe UI", 11F);
            comboBoxBuku.BackColor = BgColor;
            comboBoxBuku.DropDownStyle = ComboBoxStyle.DropDownList;
            currentY += 55;

            var lblBatas = new Label { Text = "Batas Pengembalian", Font = new Font("Segoe UI", 10F), ForeColor = TextMuted, AutoSize = true, Location = new Point(30, currentY) };
            currentY += 25;
            labelTanggalKembali.Location = new Point(30, currentY);
            labelTanggalKembali.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            labelTanggalKembali.ForeColor = PrimaryBlue;
            labelTanggalKembali.AutoSize = true;
            currentY += 70;

            buttonPinjam.Text = "Pinjam Buku";
            buttonPinjam.Location = new Point(30, currentY);
            buttonPinjam.Size = new Size(160, 45);
            buttonPinjam.BackColor = PrimaryBlue;
            buttonPinjam.ForeColor = Color.White;
            buttonPinjam.FlatStyle = FlatStyle.Flat;
            buttonPinjam.FlatAppearance.BorderSize = 0;
            buttonPinjam.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);

            buttonResetPeminjaman.Text = "Reset";
            buttonResetPeminjaman.Location = new Point(200, currentY);
            buttonResetPeminjaman.Size = new Size(100, 45);
            buttonResetPeminjaman.BackColor = Color.White;
            buttonResetPeminjaman.ForeColor = TextMuted;
            buttonResetPeminjaman.FlatStyle = FlatStyle.Flat;
            buttonResetPeminjaman.FlatAppearance.BorderColor = BorderColor;
            buttonResetPeminjaman.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);

            card.Controls.Add(lblHeader);
            card.Controls.Add(lblId);
            card.Controls.Add(textBoxIdAnggota);
            card.Controls.Add(lblBuku);
            card.Controls.Add(comboBoxBuku);
            card.Controls.Add(lblBatas);
            card.Controls.Add(labelTanggalKembali);
            card.Controls.Add(buttonPinjam);
            card.Controls.Add(buttonResetPeminjaman);

            return card;
        }

        private Panel BuildPengembalianCard()
        {
            var card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = CardBg,
                Padding = new Padding(30),
            };

            int currentY = 30;

            var lblHeader = new Label { Text = "Pengembalian Buku", Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold), ForeColor = TextDark, AutoSize = true, Location = new Point(30, currentY) };
            currentY += 50;

            var lblId = new Label { Text = "ID Peminjaman", Font = new Font("Segoe UI", 10F), ForeColor = TextMuted, AutoSize = true, Location = new Point(30, currentY) };
            currentY += 25;
            
            textBoxIdPeminjamanReturn.Location = new Point(30, currentY);
            textBoxIdPeminjamanReturn.Width = 260;
            textBoxIdPeminjamanReturn.Font = new Font("Segoe UI", 11F);
            textBoxIdPeminjamanReturn.BackColor = BgColor;
            textBoxIdPeminjamanReturn.BorderStyle = BorderStyle.FixedSingle;

            buttonCek.Text = "Cek Data";
            buttonCek.Location = new Point(300, currentY);
            buttonCek.Size = new Size(100, 27);
            buttonCek.BackColor = Color.FromArgb(230, 235, 255);
            buttonCek.ForeColor = PrimaryBlue;
            buttonCek.FlatStyle = FlatStyle.Flat;
            buttonCek.FlatAppearance.BorderSize = 0;
            buttonCek.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            
            currentY += 45;

            // Details panel
            var detailsPanel = new Panel
            {
                Location = new Point(30, currentY),
                Width = 370,
                Height = 180,
                BackColor = BgColor,
                Padding = new Padding(20)
            };

            int dy = 20;
            var fontLbl = new Font("Segoe UI", 9F);
            var fontVal = new Font("Segoe UI Semibold", 10F);
            
            var lbl1 = new Label { Text = "Buku:", Font = fontLbl, ForeColor = TextMuted, Location = new Point(20, dy), AutoSize = true };
            labelDisplayBukuReturn.Font = fontVal; labelDisplayBukuReturn.ForeColor = TextDark; labelDisplayBukuReturn.Location = new Point(100, dy); labelDisplayBukuReturn.AutoSize = true; labelDisplayBukuReturn.Text = "-";
            dy += 30;

            var lbl2 = new Label { Text = "Anggota:", Font = fontLbl, ForeColor = TextMuted, Location = new Point(20, dy), AutoSize = true };
            labelDisplayIdAnggotaReturn.Font = fontVal; labelDisplayIdAnggotaReturn.ForeColor = TextDark; labelDisplayIdAnggotaReturn.Location = new Point(100, dy); labelDisplayIdAnggotaReturn.AutoSize = true; labelDisplayIdAnggotaReturn.Text = "-";
            dy += 30;

            var lbl3 = new Label { Text = "Batas:", Font = fontLbl, ForeColor = TextMuted, Location = new Point(20, dy), AutoSize = true };
            labelDisplayBatasPengembalian.Font = fontVal; labelDisplayBatasPengembalian.ForeColor = TextDark; labelDisplayBatasPengembalian.Location = new Point(100, dy); labelDisplayBatasPengembalian.AutoSize = true; labelDisplayBatasPengembalian.Text = "-";
            dy += 30;

            var lbl4 = new Label { Text = "Status:", Font = fontLbl, ForeColor = TextMuted, Location = new Point(20, dy), AutoSize = true };
            labelDisplayStatus.Font = fontVal; labelDisplayStatus.ForeColor = TextDark; labelDisplayStatus.Location = new Point(100, dy); labelDisplayStatus.AutoSize = true; labelDisplayStatus.Text = "-";
            dy += 30;

            var lbl5 = new Label { Text = "Denda:", Font = fontLbl, ForeColor = TextMuted, Location = new Point(20, dy), AutoSize = true };
            labelDisplayDenda.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold); labelDisplayDenda.ForeColor = Color.Crimson; labelDisplayDenda.Location = new Point(100, dy); labelDisplayDenda.AutoSize = true; labelDisplayDenda.Text = "-";

            detailsPanel.Controls.AddRange(new Control[] { lbl1, labelDisplayBukuReturn, lbl2, labelDisplayIdAnggotaReturn, lbl3, labelDisplayBatasPengembalian, lbl4, labelDisplayStatus, lbl5, labelDisplayDenda });
            
            currentY += 215;

            buttonKembalikan.Text = "Kembalikan";
            buttonKembalikan.Location = new Point(30, currentY);
            buttonKembalikan.Size = new Size(160, 45);
            buttonKembalikan.BackColor = Color.FromArgb(46, 204, 113); // Emerald green
            buttonKembalikan.ForeColor = Color.White;
            buttonKembalikan.FlatStyle = FlatStyle.Flat;
            buttonKembalikan.FlatAppearance.BorderSize = 0;
            buttonKembalikan.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);

            buttonResetPengembalian.Text = "Reset";
            buttonResetPengembalian.Location = new Point(200, currentY);
            buttonResetPengembalian.Size = new Size(100, 45);
            buttonResetPengembalian.BackColor = Color.White;
            buttonResetPengembalian.ForeColor = TextMuted;
            buttonResetPengembalian.FlatStyle = FlatStyle.Flat;
            buttonResetPengembalian.FlatAppearance.BorderColor = BorderColor;
            buttonResetPengembalian.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);

            card.Controls.Add(lblHeader);
            card.Controls.Add(lblId);
            card.Controls.Add(textBoxIdPeminjamanReturn);
            card.Controls.Add(buttonCek);
            card.Controls.Add(detailsPanel);
            card.Controls.Add(buttonKembalikan);
            card.Controls.Add(buttonResetPengembalian);

            return card;
        }

        // ════════════════════════════════════════════════════════════════════
        //  Logic
        // ════════════════════════════════════════════════════════════════════

        private void LoadComboBoxBuku()
        {
            var bukuTersedia = pinjamanService.bukuService.GetAllBuku()
                .Where(b => b.Status == BukuDeprecated.STATUSBUKU.TERSEDIA)
                .ToList();

            comboBoxBuku.Items.Clear();
            if (!bukuTersedia.Any())
            {
                return;
            }

            bukuTersedia.ForEach(b => comboBoxBuku.Items.Add(b.Judul));
            if(comboBoxBuku.Items.Count > 0)
                comboBoxBuku.SelectedIndex = 0;
        }

        private void SetDefaultTanggalKembali()
        {
            DateTime tanggalBatas = DateTime.Today.AddDays(7);
            labelTanggalKembali.Text = tanggalBatas.ToString("dd/MM/yyyy");
        }

        private void buttonPinjam_Click(object sender, EventArgs e)
        {
            string idAnggota = textBoxIdAnggota.Text.Trim();
            if (comboBoxBuku.SelectedItem == null) return;
            string judulBuku = comboBoxBuku.SelectedItem.ToString();

            if (string.IsNullOrEmpty(idAnggota))
            {
                MessageBox.Show("ID Anggota harus diisi.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var anggota = pinjamanService.penggunaService.GetPenggunaById(idAnggota);
            if (anggota == null || anggota.Role != Pengguna.ROLEPENGGUNA.anggota)
            {
                MessageBox.Show("Anggota tidak ditemukan atau ID tidak valid!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var bukuDipinjam = pinjamanService.bukuService.GetAllBuku()
                .FirstOrDefault(b => b.Judul == judulBuku);
            DateTime batasPengembalian = DateTime.Now.AddDays(7);

            string idPeminjamanBaru = pinjamanService.GeneratePinjamanId();

            MessageBox.Show("Peminjaman berhasil:\nID Peminjaman: " + idPeminjamanBaru + "\nBatas Kembali: " + labelTanggalKembali.Text,
                "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

            pinjamanService.TambahPinjaman(bukuDipinjam.IdBuku, idAnggota, batasPengembalian);
            pinjamanService.LoadData();

            LoadComboBoxBuku();
        }

        private void buttonResetPeminjaman_Click(object sender, EventArgs e)
        {
            textBoxIdAnggota.Clear();
            if (comboBoxBuku.Items.Count > 0) comboBoxBuku.SelectedIndex = 0;
            SetDefaultTanggalKembali();
        }

        private void buttonCek_Click(object sender, EventArgs e)
        {
            var pinjaman = pinjamanService.GetPinjamanById(textBoxIdPeminjamanReturn.Text.Trim());
            if (pinjaman == null)
            {
                MessageBox.Show("Pinjaman tidak ditemukan!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (DateTime.Now > pinjaman.BatasPengembalian)
            {
                TimeSpan keterlambatan = DateTime.Now - pinjaman.BatasPengembalian;
                labelDisplayStatus.Text = "Terlambat";
                labelDisplayStatus.ForeColor = Color.Crimson;
                labelDisplayDenda.Text = "Rp " + (keterlambatan.Days * 5000).ToString("N0");
            }
            else
            {
                labelDisplayStatus.Text = "Tepat waktu";
                labelDisplayStatus.ForeColor = Color.FromArgb(46, 204, 113);
                labelDisplayDenda.Text = "-";
            }

            labelDisplayBukuReturn.Text = pinjamanService.bukuService.GetBukuById(pinjaman.IdBuku).Judul;
            labelDisplayIdAnggotaReturn.Text = pinjamanService.penggunaService.GetPenggunaById(pinjaman.IdAnggota).Username;
            labelDisplayBatasPengembalian.Text = pinjaman.BatasPengembalian.ToString("dd/MM/yyyy");
        }

        private void buttonKembalikan_Click(object sender, EventArgs e)
        {
            string idPeminjamanInput = textBoxIdPeminjamanReturn.Text.Trim();
            if (string.IsNullOrEmpty(idPeminjamanInput))
            {
                MessageBox.Show("Silakan masukkan ID Peminjaman terlebih dahulu.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var pinjaman = pinjamanService.GetPinjamanById(idPeminjamanInput);
            if(pinjaman == null)
            {
                 MessageBox.Show("Pinjaman tidak ditemukan!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                 return;
            }

            DateTime tanggalSekarang = DateTime.Today;
            DateTime tanggalBatas = DateTime.Parse(labelDisplayBatasPengembalian.Text);

            if (tanggalSekarang > tanggalBatas)
            {
                TimeSpan terlambat = tanggalSekarang - tanggalBatas;
                int jumlahDenda = terlambat.Days * 5000;
                string idDenda = $"D{DateTime.Now:yyyyMMddHHmmss}";
                var denda = new Denda(pinjaman.IdAnggota, pinjaman.IdBuku, pinjaman.IdPinjaman, Denda.STATUSDENDA.BELUMLUNAS)
                {
                    IdDenda = idDenda,
                    JumlahHariTerlambat = terlambat.Days,
                    JumlahDenda = jumlahDenda
                };
                pinjamanService.dendaService.AddDenda(denda);
                MessageBox.Show($"Proses pengembalian selesai.\nStatus: {labelDisplayStatus.Text}\nDenda: Rp {denda.JumlahDenda:N0}", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                labelDisplayStatus.Text = "Tepat waktu";
                MessageBox.Show($"Proses pengembalian selesai.\nStatus: {labelDisplayStatus.Text}", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (pinjamanService.HapusPinjaman(idPeminjamanInput))
            {
                LoadComboBoxBuku();
                buttonResetPengembalian_Click(null, null);
            }
            else
            {
                MessageBox.Show("Pengembalian gagal. Silakan coba lagi.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonResetPengembalian_Click(object sender, EventArgs e)
        {
            textBoxIdPeminjamanReturn.Clear();
            labelDisplayBukuReturn.Text = "-";
            labelDisplayIdAnggotaReturn.Text = "-";
            labelDisplayBatasPengembalian.Text = "-";
            labelDisplayStatus.Text = "-";
            labelDisplayStatus.ForeColor = TextDark;
            labelDisplayDenda.Text = "-";
        }
    }
}
