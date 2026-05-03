using ManajemenPerpus.CLI.Service;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ManajemenPerpus.GUI
{
    public partial class KoleksiBuku : Form
    {
        private readonly BukuServiceNew _bukuService = new BukuServiceNew();
        private List<ManajemenPerpus.Core.Models.FactoryBuku> _allBooks = new();

        private TextBox _searchBox;
        private Panel _bookListContainer;

        // ── Theme colors ───────────────────────────────────────────────
        private static readonly Color BgColor       = Color.FromArgb(247, 248, 253);
        private static readonly Color CardBg        = Color.White;
        private static readonly Color PrimaryBlue   = Color.FromArgb(58,  90, 230);
        private static readonly Color TextDark      = Color.FromArgb(25,  30,  90);
        private static readonly Color TextMuted     = Color.FromArgb(110, 115, 160);
        private static readonly Color BorderColor   = Color.FromArgb(220, 224, 240);
        private static readonly Color CardBorder    = Color.FromArgb(230, 233, 245);

        public KoleksiBuku()
        {
            InitializeComponent();
            this.FormClosed += (s, args) => Application.Exit();
            
            BuildLayout();
        }

        private void BuildLayout()
        {
            this.BackColor = BgColor;
            this.Text = "Koleksi Buku";
            this.ClientSize = new Size(1008, 729);
            this.StartPosition = FormStartPosition.CenterScreen;

            // 1) Book list container (Dock.Fill)
            _bookListContainer = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = BgColor,
                Padding = new Padding(30, 10, 30, 30)
            };
            this.Controls.Add(_bookListContainer); // z-index 0

            // 2) Search Row
            var searchRow = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = BgColor,
                Padding = new Padding(30, 15, 30, 15)
            };

            var searchContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = CardBg,
                Padding = new Padding(15, 12, 15, 12),
                BorderStyle = BorderStyle.None
            };
            
            // Draw a border using a wrapper panel
            var searchBorder = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = BorderColor,
                Padding = new Padding(1)
            };

            var btnSearch = new Button
            {
                Text = "Cari",
                Dock = DockStyle.Right,
                Width = 100,
                FlatStyle = FlatStyle.Flat,
                BackColor = PrimaryBlue,
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.Click += async (s, e) => await FilterBooks(_searchBox.Text);

            var spacer = new Panel { Dock = DockStyle.Right, Width = 15, BackColor = CardBg };

            _searchBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 12F),
                BorderStyle = BorderStyle.None,
                BackColor = CardBg,
                ForeColor = TextDark,
                PlaceholderText = "Cari judul buku, penulis, penerbit, atau kategori..."
            };
            _searchBox.TextChanged += async (s, e) => await FilterBooks(_searchBox.Text);

            searchContainer.Controls.Add(_searchBox);
            searchContainer.Controls.Add(spacer);
            searchContainer.Controls.Add(btnSearch);
            searchBorder.Controls.Add(searchContainer);
            searchRow.Controls.Add(searchBorder);
            
            this.Controls.Add(searchRow); // z-index 1

            // 3) Header Panel
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = BgColor,
                Padding = new Padding(30, 25, 30, 0)
            };
            var lblTitle = new Label
            {
                Text = "Koleksi Buku",
                Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                ForeColor = TextDark,
                AutoSize = true,
                Location = new Point(26, 20)
            };
            var lblSubtitle = new Label
            {
                Text = "Jelajahi dan temukan buku favoritmu untuk diulas.",
                Font = new Font("Segoe UI", 11F),
                ForeColor = TextMuted,
                AutoSize = true,
                Location = new Point(30, 60)
            };
            headerPanel.Controls.Add(lblSubtitle);
            headerPanel.Controls.Add(lblTitle);
            
            this.Controls.Add(headerPanel); // z-index 2

            // 4) Navbar
            var navbar = UIHelper.BuildNavbar(this, false);
            this.Controls.Add(navbar); // z-index 3 — topmost
        }

        private async void KoleksiBuku_Load(object sender, EventArgs e)
        {
            await LoadAllBooks();
        }

        private async Task LoadAllBooks()
        {
            try
            {
                _allBooks = await _bukuService.GetBukuFromApi();
                RenderBooks(_allBooks);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat buku: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task FilterBooks(string query)
        {
            string q = query.ToLower().Trim();
            if (string.IsNullOrEmpty(q)) { RenderBooks(_allBooks); return; }

            var filtered = _allBooks.Where(b =>
                (b.Judul?.ToLower().Contains(q) ?? false) ||
                (b.Penulis?.ToLower().Contains(q) ?? false) ||
                (b.Penerbit?.ToLower().Contains(q) ?? false) ||
                (b.Kategori?.ToLower().Contains(q) ?? false) ||
                (b.Sinopsis?.ToLower().Contains(q) ?? false)
            ).ToList();
            RenderBooks(filtered);
        }

        private void RenderBooks(List<ManajemenPerpus.Core.Models.FactoryBuku> books)
        {
            _bookListContainer.Controls.Clear();

            if (books.Count == 0)
            {
                _bookListContainer.Controls.Add(new Label
                {
                    Text = "Tidak ada buku yang ditemukan.",
                    Font = new Font("Segoe UI", 12F, FontStyle.Italic),
                    ForeColor = TextMuted,
                    AutoSize = true,
                    Location = new Point(20, 20)
                });
                return;
            }

            int y = 5;
            foreach (var buku in books)
            {
                var card = BuildBookCard(buku);
                card.Location = new Point(0, y);
                card.Width = _bookListContainer.ClientSize.Width - 20; // 20 for scrollbar margin
                _bookListContainer.Controls.Add(card);
                y += card.Height + 15; // gap between cards
            }
        }

        private Panel BuildBookCard(ManajemenPerpus.Core.Models.FactoryBuku buku)
        {
            var card = new Panel
            {
                Height = 110,
                BackColor = CardBg,
                Cursor = Cursors.Hand,
                Tag = buku.IdBuku,
                Padding = new Padding(0)
            };

            // Subtle border around card
            var cardBorder = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = CardBorder,
                Padding = new Padding(1)
            };
            var cardInner = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = CardBg,
            };
            cardBorder.Controls.Add(cardInner);
            card.Controls.Add(cardBorder);

            // Left accent bar
            var accent = new Panel
            {
                Dock = DockStyle.Left,
                Width = 4,
                BackColor = PrimaryBlue
            };
            cardInner.Controls.Add(accent);

            var lblTitle = new Label
            {
                Text = buku.Judul,
                Font = new Font("Segoe UI Semibold", 13F, FontStyle.Bold),
                ForeColor = TextDark,
                Location = new Point(20, 15),
                AutoSize = true
            };
            var lblMeta = new Label
            {
                Text = $"Penulis: {buku.Penulis}   •   Penerbit: {buku.Penerbit}   •   Kategori: {buku.Kategori}",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = TextMuted,
                Location = new Point(20, 45),
                AutoSize = true
            };
            var lblSinopsis = new Label
            {
                Text = buku.Sinopsis?.Length > 120 ? buku.Sinopsis[..120] + "..." : (buku.Sinopsis ?? "-"),
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(100, 105, 120),
                Location = new Point(20, 70),
                AutoSize = true
            };

            cardInner.Controls.Add(lblSinopsis);
            cardInner.Controls.Add(lblMeta);
            cardInner.Controls.Add(lblTitle);

            // Click events
            EventHandler onClick = (s, e) =>
            {
                if (card.Tag is string idBuku)
                {
                    new UlasanPage(idBuku).Show();
                    this.Hide();
                }
            };
            
            // Attach click to all children
            lblTitle.Click += onClick;
            lblMeta.Click += onClick;
            lblSinopsis.Click += onClick;
            cardInner.Click += onClick;
            accent.Click += onClick;
            cardBorder.Click += onClick;
            card.Click += onClick;

            // Hover effects
            EventHandler onEnter = (s, e) => { cardInner.BackColor = Color.FromArgb(249, 250, 255); };
            EventHandler onLeave = (s, e) => { cardInner.BackColor = CardBg; };

            lblTitle.MouseEnter += onEnter; lblTitle.MouseLeave += onLeave;
            lblMeta.MouseEnter += onEnter; lblMeta.MouseLeave += onLeave;
            lblSinopsis.MouseEnter += onEnter; lblSinopsis.MouseLeave += onLeave;
            cardInner.MouseEnter += onEnter; cardInner.MouseLeave += onLeave;

            return card;
        }
    }
}
