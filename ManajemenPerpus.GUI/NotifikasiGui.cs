using ManajemenPerpus.CLI.Service;
using ManajemenPerpus.Core.Models;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ManajemenPerpus.GUI
{
    public partial class NotifikasiGui : Form
    {
        private readonly NotifikasiService _notifikasiService;
        private readonly string _idPengguna;
        private Panel _listContainer;

        // ── Modern Sleek Light Theme ───────────────────────────────────────────────
        private static readonly Color BgColor       = Color.FromArgb(249, 250, 252);
        private static readonly Color CardBg        = Color.White;
        private static readonly Color AccentColor   = Color.FromArgb(59, 130, 246); // Modern Blue
        private static readonly Color TextPrimary   = Color.FromArgb(30, 41, 59);
        private static readonly Color TextSecondary = Color.FromArgb(100, 116, 139);
        private static readonly Color BorderColor   = Color.FromArgb(226, 232, 240);
        private static readonly Color DangerColor   = Color.FromArgb(239, 68, 68);
        private static readonly Color HoverColor    = Color.FromArgb(241, 245, 249);

        public NotifikasiGui(string idPengguna)
        {
            InitializeComponent();
            _notifikasiService = new NotifikasiService();
            _idPengguna = idPengguna;

            this.FormClosed += (s, args) => Application.Exit();
            
            BuildModernLayout();
            LoadNotifikasiData();
        }

        private void BuildModernLayout()
        {
            this.ClientSize = new Size(1024, 768);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BgColor;
            this.Text = "Pusat Notifikasi";

            // 1) List container for notif cards
            _listContainer = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = BgColor,
                Padding = new Padding(40, 20, 40, 40)
            };
            this.Controls.Add(_listContainer); // z-index 0

            // 2) Header panel
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 130,
                BackColor = BgColor,
                Padding = new Padding(40, 40, 40, 0)
            };
            
            var lblTitle = new Label
            {
                Text = "Notifikasi Anda",
                Font = new Font("Segoe UI Variable Display", 26F, FontStyle.Bold),
                ForeColor = TextPrimary,
                AutoSize = true,
                Location = new Point(36, 30)
            };
            
            var lblSubtitle = new Label
            {
                Text = "Tetap terhubung dengan aktivitas dan peringatan terbaru Anda.",
                Font = new Font("Segoe UI", 12F, FontStyle.Regular),
                ForeColor = TextSecondary,
                AutoSize = true,
                Location = new Point(42, 80)
            };
            
            // Subtle top border gradient effect simulation
            var topBorder = new Panel
            {
                Dock = DockStyle.Top,
                Height = 4,
                BackColor = AccentColor
            };

            headerPanel.Controls.Add(lblSubtitle);
            headerPanel.Controls.Add(lblTitle);
            headerPanel.Controls.Add(topBorder);
            this.Controls.Add(headerPanel); // z-index 1

            // 3) Navbar
            var navbar = UIHelper.BuildNavbar(this, false);
            this.Controls.Add(navbar); // z-index 2
        }

        private void LoadNotifikasiData()
        {
            _listContainer.Controls.Clear();
            var notifikasiList = _notifikasiService.GetNotifikasiByPengguna(_idPengguna);

            if (notifikasiList.Count == 0)
            {
                var emptyStatePanel = new Panel
                {
                    Size = new Size(400, 200),
                    Location = new Point((_listContainer.Width - 400) / 2, 100),
                    BackColor = Color.Transparent
                };

                var lblEmpty = new Label
                {
                    Text = "Belum Ada Notifikasi",
                    Font = new Font("Segoe UI Semibold", 16F, FontStyle.Bold),
                    ForeColor = TextPrimary,
                    AutoSize = true,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Top,
                    Padding = new Padding(0, 20, 0, 10)
                };

                var lblEmptySub = new Label
                {
                    Text = "Anda telah membaca semua pemberitahuan. \nTidak ada peringatan baru saat ini.",
                    Font = new Font("Segoe UI", 11F),
                    ForeColor = TextSecondary,
                    AutoSize = true,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Top
                };

                emptyStatePanel.Controls.Add(lblEmptySub);
                emptyStatePanel.Controls.Add(lblEmpty);

                _listContainer.Resize += (s, e) => {
                    emptyStatePanel.Location = new Point((_listContainer.Width - emptyStatePanel.Width) / 2, 100);
                };

                _listContainer.Controls.Add(emptyStatePanel);
                return;
            }

            int y = 10;
            foreach (var notif in notifikasiList.OrderByDescending(n => n.TanggalNotifikasi))
            {
                var card = BuildModernNotifCard(notif);
                card.Location = new Point(0, y);
                card.Width = _listContainer.ClientSize.Width - 20; // Scrollbar margin
                
                _listContainer.Resize += (s, e) => {
                    card.Width = _listContainer.ClientSize.Width - 20;
                };

                _listContainer.Controls.Add(card);
                y += card.Height + 20;
            }
        }

        private Panel BuildModernNotifCard(Notifikasi notif)
        {
            var card = new Panel
            {
                Height = 115,
                BackColor = CardBg,
                Padding = new Padding(1), // for border
                Cursor = Cursors.Hand
            };

            var cardBorder = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = BorderColor,
                Padding = new Padding(1)
            };
            
            var cardInner = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = CardBg,
            };
            
            cardBorder.Controls.Add(cardInner);
            card.Controls.Add(cardBorder);

            // Left accent bar - thick and vibrant
            var accent = new Panel
            {
                Dock = DockStyle.Left,
                Width = 6,
                BackColor = AccentColor
            };
            cardInner.Controls.Add(accent);

            // Notification Icon (simulated with a colored panel/label)
            var iconPanel = new Panel
            {
                Size = new Size(40, 40),
                Location = new Point(25, 25),
                BackColor = Color.FromArgb(40, AccentColor.R, AccentColor.G, AccentColor.B),
            };
            var iconLabel = new Label
            {
                Text = "🔔",
                Font = new Font("Segoe UI", 14F),
                ForeColor = AccentColor,
                AutoSize = true,
                Location = new Point(6, 6),
                BackColor = Color.Transparent
            };
            iconPanel.Controls.Add(iconLabel);

            var lblDate = new Label
            {
                Text = notif.TanggalNotifikasi.ToString("dd MMM yyyy, HH:mm"),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = TextSecondary,
                Location = new Point(80, 25),
                AutoSize = true
            };

            var lblContent = new Label
            {
                Text = notif.IsiNotifikasi,
                Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold),
                ForeColor = TextPrimary,
                Location = new Point(78, 50),
                AutoSize = true,
                MaximumSize = new Size(650, 50)
            };

            var btnBaca = new Button
            {
                Text = "Detail",
                Size = new Size(100, 40),
                BackColor = Color.FromArgb(40, AccentColor.R, AccentColor.G, AccentColor.B),
                ForeColor = AccentColor,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnBaca.FlatAppearance.BorderSize = 0;
            btnBaca.Click += (s, e) => MessageBox.Show($"Detail Notifikasi:\n\n{notif.IsiNotifikasi}\n\nTanggal: {notif.TanggalNotifikasi:yyyy-MM-dd HH:mm}", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

            var btnHapus = new Button
            {
                Text = "Hapus",
                Size = new Size(100, 40),
                BackColor = Color.FromArgb(30, DangerColor.R, DangerColor.G, DangerColor.B),
                ForeColor = DangerColor,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnHapus.FlatAppearance.BorderSize = 0;
            btnHapus.Click += (s, e) =>
            {
                if (MessageBox.Show("Hapus notifikasi ini dari riwayat?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (_notifikasiService.DeleteNotifikasi(notif.IdNotifikasi))
                    {
                        LoadNotifikasiData();
                    }
                }
            };

            // Positioning buttons dynamically relative to parent size
            cardInner.Resize += (s, e) =>
            {
                btnHapus.Location = new Point(cardInner.Width - 130, 37);
                btnBaca.Location = new Point(cardInner.Width - 245, 37);
            };

            // Hover effects
            cardInner.MouseEnter += (s, e) => { cardInner.BackColor = HoverColor; };
            cardInner.MouseLeave += (s, e) => { 
                // Only reset if mouse left the entire control bounds
                var pt = cardInner.PointToClient(Cursor.Position);
                if (!cardInner.ClientRectangle.Contains(pt))
                    cardInner.BackColor = CardBg; 
            };

            cardInner.Controls.Add(iconPanel);
            cardInner.Controls.Add(lblDate);
            cardInner.Controls.Add(lblContent);
            cardInner.Controls.Add(btnBaca);
            cardInner.Controls.Add(btnHapus);

            return card;
        }
    }
}