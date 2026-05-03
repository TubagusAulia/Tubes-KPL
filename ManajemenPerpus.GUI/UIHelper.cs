using System;
using System.Drawing;
using System.Windows.Forms;

namespace ManajemenPerpus.GUI
{
    // Stores the currently logged-in user globally
    public static class SessionData
    {
        public static ManajemenPerpus.Core.Models.Pengguna CurrentUser { get; set; }
    }

    public static class UIHelper
    {
        private static readonly Color NavBg = Color.RoyalBlue;
        private static readonly Color NavFg = Color.White;
        private static readonly Font NavFont = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);

        /// <summary>
        /// Builds and returns a standardized 58px Dock.Top navbar panel.
        ///
        /// Layout (left → right):
        ///   [Logout/Back]  [App Name]  [Koleksi Buku] [Ulasan] [Pinjaman] [Notifikasi]
        /// </summary>
        public static Panel BuildNavbar(Form owner, bool isHomepage)
        {
            var navbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 58,
                BackColor = NavBg
            };

            // ── LEFT side ────────────────────────────────────────────────────────
            // Dock.Left controls stack left → right in the ORDER they are added.

            // 1) Logout / Back button
            var leftBtn = new Button
            {
                Text = isHomepage ? "Logout" : "◀ Back",
                Dock = DockStyle.Left,
                Width = 110,
                FlatStyle = FlatStyle.Flat,
                ForeColor = NavFg,
                Font = NavFont,
                Cursor = Cursors.Hand,
                BackColor = Color.FromArgb(40, 0, 0, 0)
            };
            leftBtn.FlatAppearance.BorderSize = 0;
            leftBtn.Click += (s, e) =>
            {
                if (isHomepage)
                {
                    SessionData.CurrentUser = null;
                    new LoginForm().Show();
                    owner.Hide();
                }
                else
                {
                    new MenuUtama(SessionData.CurrentUser).Show();
                    owner.Hide();
                }
            };
            // 2) App Name label
            var appLabel = new Label
            {
                Text = "Manajemen Perpustakaan",
                ForeColor = NavFg,
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                Dock = DockStyle.Left,
                AutoSize = false,
                Width = 260,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };

            // Left side order: App Name first (leftmost), then Logout/Back
            navbar.Controls.Add(appLabel);
            navbar.Controls.Add(leftBtn);

            // ── RIGHT side ───────────────────────────────────────────────────────
            // Dock.Right controls stack right → left in the ORDER they are added.
            // Visual goal (left → right): Koleksi Buku | Ulasan | Pinjaman | Notifikasi
            // So we must add them in reverse: Notifikasi first → Koleksi Buku last.
            var rightPages = new[] { "Koleksi Buku", "Pinjaman", "Notifikasi" };
            foreach (var page in rightPages)
            {
                var btn = new Button
                {
                    Text = page,
                    Dock = DockStyle.Right,
                    AutoSize = false,
                    Width = 130,
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = NavFg,
                    Font = NavFont,
                    Cursor = Cursors.Hand,
                    BackColor = NavBg
                };
                btn.FlatAppearance.BorderSize = 0;
                string p = page;
                btn.Click += (s, e) => NavigateTo(p, owner);
                navbar.Controls.Add(btn);
            }

            return navbar;
        }

        private static void NavigateTo(string page, Form current)
        {
            Form next = null;
            switch (page)
            {
                case "Koleksi Buku": next = new KoleksiBuku(); break;
                case "Ulasan":       next = new KoleksiBuku(); break; // go to book list first to pick a book
                case "Pinjaman":     next = new Sirkulasi(); break;
                case "Notifikasi":   next = new NotifikasiGui(SessionData.CurrentUser?.IdPengguna ?? "P001"); break;
            }
            if (next != null)
            {
                next.Show();
                current.Hide();
            }
        }
    }
}
