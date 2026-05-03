using ManajemenPerpus.CLI.Service;
using ManajemenPerpus.Core.Helper;
using ManajemenPerpus.Core.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ManajemenPerpus.GUI
{
    public partial class UlasanPage : Form
    {
        // ── Services & state ─────────────────────────────────────────────────
        private readonly string _idBuku;
        private readonly UlasanService  _ulasanService = new UlasanService();
        private readonly BukuServiceNew _bukuService   = new BukuServiceNew();
        private List<Ulasan> _listUlasan = new();

        private readonly string _filePath = ManajemenPerpus.Core.Helper.JsonHelper.GetSharedDataPath("DataUlasan.json");

        // ── UI references kept for runtime updates ────────────────────────────
        private Label _lblJudul;
        private Label _lblMeta;
        private Panel _ulasanListContainer;   // scrollable area that holds the cards

        // ── Card colour palette ───────────────────────────────────────────────
        private static readonly Color AccentBlue   = Color.FromArgb(58,  90, 230);
        private static readonly Color CardBg        = Color.White;
        private static readonly Color CardBorder    = Color.FromArgb(220, 224, 240);
        private static readonly Color IdColor       = Color.FromArgb(120, 130, 180);
        private static readonly Color TextColor     = Color.FromArgb(30,  35,  60);
        private static readonly Color EmptyColor    = Color.FromArgb(150, 150, 170);

        // ════════════════════════════════════════════════════════════════════
        //  Constructor
        // ════════════════════════════════════════════════════════════════════
        public UlasanPage(string idBuku)
        {
            InitializeComponent();
            _idBuku = idBuku;

            BuildLayout();

            this.Load       += UlasanGui_Load;
            this.FormClosed += (s, e) => Application.Exit();
        }

        // ════════════════════════════════════════════════════════════════════
        //  Layout
        // ════════════════════════════════════════════════════════════════════
        private void BuildLayout()
        {
            this.ClientSize    = new Size(1008, 729);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor     = Color.FromArgb(247, 248, 253);

            // Controls are added bottom-first; Dock.Top items stack top-down.

            // ── Ulasan card list (Dock.Fill — added first, occupies remainder) ──
            _ulasanListContainer = new Panel
            {
                Dock      = DockStyle.Fill,
                AutoScroll = true,
                BackColor  = Color.FromArgb(247, 248, 253),
                Padding    = new Padding(20, 10, 20, 10)
            };
            this.Controls.Add(_ulasanListContainer);

            // ── Separator below input ─────────────────────────────────────────
            this.Controls.Add(Separator());

            // ── Input section ─────────────────────────────────────────────────
            this.Controls.Add(BuildInputPanel());

            // ── Separator below book info ──────────────────────────────────────
            this.Controls.Add(Separator());

            // ── Book info strip ────────────────────────────────────────────────
            this.Controls.Add(BuildBookInfoPanel());

            // ── Navbar (topmost, added last) ───────────────────────────────────
            this.Controls.Add(UIHelper.BuildNavbar(this, false));
        }

        // ── Thin separator ────────────────────────────────────────────────────
        private static Panel Separator() => new Panel
        {
            Dock      = DockStyle.Top,
            Height    = 2,
            BackColor = Color.FromArgb(215, 218, 235)
        };

        // ── Book info strip ───────────────────────────────────────────────────
        private Panel BuildBookInfoPanel()
        {
            var panel = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 74,
                BackColor = Color.FromArgb(241, 243, 255),
                Padding   = new Padding(24, 10, 24, 10)
            };

            _lblJudul = new Label
            {
                Text      = "Memuat informasi buku…",
                Dock      = DockStyle.Top,
                AutoSize  = false,
                Height    = 28,
                Font      = new Font("Segoe UI Semibold", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 30, 90),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _lblMeta = new Label
            {
                Text      = "",
                Dock      = DockStyle.Top,
                AutoSize  = false,
                Height    = 22,
                Font      = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(110, 115, 160),
                TextAlign = ContentAlignment.MiddleLeft
            };

            panel.Controls.Add(_lblMeta);    // added first → rendered below
            panel.Controls.Add(_lblJudul);   // added second → rendered above

            return panel;
        }

        // ── "Buat Ulasanmu" input section ────────────────────────────────────
        private Panel BuildInputPanel()
        {
            var panel = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 158,
                BackColor = Color.White,
                Padding   = new Padding(24, 12, 24, 12)
            };

            var header = new Label
            {
                Text      = "Buat Ulasanmu",
                Dock      = DockStyle.Top,
                AutoSize  = false,
                Height    = 28,
                Font      = new Font("Segoe UI Semibold", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 30, 90),
                TextAlign = ContentAlignment.MiddleLeft
            };

            ulasanTextBox.Dock        = DockStyle.Top;
            ulasanTextBox.Height      = 56;
            ulasanTextBox.BackColor   = Color.FromArgb(248, 249, 255);
            ulasanTextBox.BorderStyle = BorderStyle.FixedSingle;
            ulasanTextBox.Font        = new Font("Segoe UI", 10F);

            // Button row
            var btnRow = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 50,
                BackColor = Color.White
            };

            btnSubmit.Size = new Size(130, 38);
            btnSubmit.Dock = DockStyle.Right;

            hapusBtn.Size  = new Size(130, 38);
            hapusBtn.Dock  = DockStyle.Right;

            btnRow.Controls.Add(btnSubmit);  // rightmost
            btnRow.Controls.Add(hapusBtn);   // left of Kirim

            panel.Controls.Add(btnRow);
            panel.Controls.Add(ulasanTextBox);
            panel.Controls.Add(header);

            return panel;
        }

        // ════════════════════════════════════════════════════════════════════
        //  Load
        // ════════════════════════════════════════════════════════════════════
        private async void UlasanGui_Load(object sender, EventArgs e)
        {
            await LoadBookInfo();
            await LoadUlasan();
        }

        private async Task LoadBookInfo()
        {
            try
            {
                var books = await _bukuService.GetBukuFromApi();
                var buku  = books.FirstOrDefault(b => b.IdBuku == _idBuku);
                if (buku != null)
                {
                    _lblJudul.Text = buku.Judul;
                    _lblMeta.Text  = $"Penulis: {buku.Penulis}   ·   Penerbit: {buku.Penerbit}   ·   {buku.Kategori}";
                }
                else
                {
                    _lblJudul.Text = $"Buku — {_idBuku}";
                    _lblMeta.Text  = "";
                }
            }
            catch
            {
                _lblJudul.Text = $"Buku — {_idBuku}";
                _lblMeta.Text  = "Gagal memuat info buku.";
            }
        }

        private async Task LoadUlasan()
        {
            _ulasanListContainer.Controls.Clear();

            try
            {
                var all = await _ulasanService.GetUlasanFromApi();
                var filtered = all.Where(u => u.bukuId == _idBuku).ToList();

                if (filtered.Count == 0)
                {
                    ShowEmptyState();
                    return;
                }

                RenderCards(filtered);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading ulasan: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  Card rendering  (KoleksiBuku-style)
        // ════════════════════════════════════════════════════════════════════
        private void RenderCards(List<Ulasan> list)
        {
            _ulasanListContainer.Controls.Clear();
            int y = 8;

            foreach (var u in list)
            {
                var card = BuildUlasanCard(u);
                card.Location = new Point(0, y);
                card.Width    = _ulasanListContainer.ClientSize.Width - 20;
                _ulasanListContainer.Controls.Add(card);
                y += card.Height + 8;
            }
        }

        private Panel BuildUlasanCard(Ulasan u)
        {
            // Wrap-aware height: estimate rows needed
            int contentWidth = _ulasanListContainer.ClientSize.Width - 80; // 20px container pad + 24px card pad × 2
            int charsPerRow  = Math.Max(1, contentWidth / 8);
            int textRows     = (int)Math.Ceiling((double)(u.isiUlasan?.Length ?? 0) / charsPerRow);
            int cardHeight   = Math.Max(80, 46 + textRows * 18 + 12);

            var card = new Panel
            {
                Height      = cardHeight,
                BackColor   = CardBg,
                BorderStyle = BorderStyle.None,
                Cursor      = Cursors.Default,
                Tag         = u.ulasanId,
                Padding     = new Padding(0)
            };

            // Left accent bar
            var accent = new Panel
            {
                Dock      = DockStyle.Left,
                Width     = 5,
                BackColor = AccentBlue
            };

            // Content area (right of accent bar)
            var content = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = CardBg,
                Padding   = new Padding(14, 8, 14, 8)
            };

            // ID badge row
            var lblId = new Label
            {
                Text      = $"ID Ulasan: {u.ulasanId}",
                Dock      = DockStyle.Top,
                AutoSize  = false,
                Height    = 20,
                Font      = new Font("Segoe UI", 8.5F, FontStyle.Regular),
                ForeColor = IdColor,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Review body
            var lblIsi = new Label
            {
                Text      = u.isiUlasan ?? "-",
                Dock      = DockStyle.Top,
                AutoSize  = false,
                Height    = Math.Max(20, textRows * 18),
                Font      = new Font("Segoe UI", 10.5F),
                ForeColor = TextColor,
                TextAlign = ContentAlignment.TopLeft
            };

            // Bottom separator within the card
            var innerLine = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 1,
                BackColor = CardBorder
            };

            content.Controls.Add(innerLine);
            content.Controls.Add(lblId);
            content.Controls.Add(lblIsi);

            card.Controls.Add(content);
            card.Controls.Add(accent);

            return card;
        }

        // ── Empty state ───────────────────────────────────────────────────────
        private void ShowEmptyState()
        {
            _ulasanListContainer.Controls.Add(new Label
            {
                Text      = "Belum ada ulasan untuk buku ini. Jadilah yang pertama!",
                Font      = new Font("Segoe UI", 11F, FontStyle.Italic),
                ForeColor = EmptyColor,
                AutoSize  = true,
                Location  = new Point(20, 20)
            });
        }

        // ════════════════════════════════════════════════════════════════════
        //  Button handlers
        // ════════════════════════════════════════════════════════════════════
        private void btnSubmit_Click(object sender, EventArgs e)
        {
            string isi = ulasanTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(isi))
            {
                MessageBox.Show("Isi ulasan tidak boleh kosong.", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _listUlasan = JsonHelper.ReadJson<Ulasan>(_filePath) ?? new List<Ulasan>();
            var ulasanBaru = new Ulasan(_ulasanService.GenerateUlasanId(), _idBuku, isi);
            _listUlasan.Add(ulasanBaru);

            Directory.CreateDirectory(Path.GetDirectoryName(_filePath) ?? ".");
            JsonHelper.WriteJson(_filePath, _listUlasan);

            // Append the new card to the list immediately
            // First, remove empty state label if present
            _ulasanListContainer.Controls.OfType<Label>().ToList()
                .ForEach(l => _ulasanListContainer.Controls.Remove(l));

            int nextY = _ulasanListContainer.Controls
                .OfType<Panel>()
                .Select(p => p.Bottom + 8)
                .DefaultIfEmpty(8)
                .Max();

            var newCard  = BuildUlasanCard(ulasanBaru);
            newCard.Location = new Point(0, nextY);
            newCard.Width    = _ulasanListContainer.ClientSize.Width - 20;
            _ulasanListContainer.Controls.Add(newCard);

            // Scroll to the new card
            _ulasanListContainer.ScrollControlIntoView(newCard);

            MessageBox.Show("Ulasan berhasil ditambahkan.", "Informasi",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            ulasanTextBox.Clear();
        }

        private void hapusBtn_Click(object sender, EventArgs e)
        {
            ulasanTextBox.Clear();
        }
    }
}
