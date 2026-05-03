namespace ManajemenPerpus.GUI
{
    public partial class MenuUtama : Form
    {
        private ManajemenPerpus.Core.Models.Pengguna _currentUser;

        public MenuUtama(ManajemenPerpus.Core.Models.Pengguna user = null)
        {
            InitializeComponent();
            this.FormClosed += (s, args) => Application.Exit();
            _currentUser = user;
            if (user != null) SessionData.CurrentUser = user;
            
            this.ClientSize = new System.Drawing.Size(1008, 729);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            
            // Hide designer navbar, inject standard one
            panel1.Visible = false;
            var navbar = UIHelper.BuildNavbar(this, true);
            this.Controls.Add(navbar); // added last = highest index = docks at very top
        }

        private void MenuUtama_Load(object sender, EventArgs e)
        {
            if (_currentUser != null)
            {
                string nama = string.IsNullOrWhiteSpace(_currentUser.Fullname) ? _currentUser.Username : _currentUser.Fullname;
                
                label3.Font = new Font("Consolas", 16F, FontStyle.Regular, GraphicsUnit.Point, 0);
                label3.Text = $"Di Aplikasi Manajemen Perpustakaan\n\nNama anggota : {nama}\nID anggota   : {_currentUser.IdPengguna}";
            }
        }

        private void customButton1_Click(object sender, EventArgs e)
        {
            KoleksiBuku ulasanGui = new KoleksiBuku();
            ulasanGui.Show();
            this.Hide();
        }

        private void LogoutButton_Click(object sender, EventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Hide();
        }

        private void customButton3_Click(object sender, EventArgs e)
        {
            KoleksiBuku ulasanGui = new KoleksiBuku();
            ulasanGui.Show();
            this.Hide();
        }

        private void customButton2_Click(object sender, EventArgs e)
        {
            Sirkulasi sirkulasi = new Sirkulasi();
            sirkulasi.Show();
            this.Hide();

        }

        private void customButton3_Click_1(object sender, EventArgs e)
        {
            string idPengguna = SessionData.CurrentUser?.IdPengguna ?? _currentUser?.IdPengguna ?? "U001";
            NotifikasiGui notifikasiGui = new NotifikasiGui(idPengguna);
            notifikasiGui.Show();
            this.Hide();
        }
    }
}
