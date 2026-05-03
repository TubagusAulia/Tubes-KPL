using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ManajemenAdminGUI;
using ManajemenAdminGUI.Forms;

namespace ManajemenPerpus.GUI
{
    public partial class LoginForm : Form
    {
        MenuAdmin MenuAdmin;
        public LoginForm()
        {
            InitializeComponent();
            this.FormClosed += (s, args) => Application.Exit();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text.Trim();
            string password = textBox2.Text.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Username dan password harus diisi!");
                return;
            }

            if (username == "admin" && password == "admin1234")
            {
                MessageBox.Show("Selamat, anda telah berhasil login sebagai Admin!");
                MenuAdmin menuAdmin = new MenuAdmin();
                menuAdmin.Show();
                this.Hide();
                return;
            }
            
            // Periksa dari JSON file
            try
            {
                string root = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.Parent?.FullName;
                string filePath = System.IO.Path.Combine(root, "SharedData", "DataJson", "DataPengguna.json");
                var penggunaList = ManajemenPerpus.Core.Helper.JsonHelper.ReadJson<ManajemenPerpus.Core.Models.Pengguna>(filePath) ?? new List<ManajemenPerpus.Core.Models.Pengguna>();
                
                var usersWithSameName = penggunaList.Where(p => p.Username == username).ToList();
                if (usersWithSameName.Count > 0)
                {
                    var user = usersWithSameName.FirstOrDefault(p => p.Password == password);
                    if (user != null)
                    {
                        SessionData.CurrentUser = user;
                        MessageBox.Show("Selamat, anda telah berhasil login sebagai Anggota!");
                        
                        // Testing: Send welcome back notification
                        var notifService = new ManajemenPerpus.CLI.Service.NotifikasiService();
                        notifService.AddNotifikasi(new ManajemenPerpus.Core.Models.Notifikasi(
                            idNotifikasi: "N" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                            idPengguna: user.IdPengguna,
                            isiNotifikasi: "Welcome back",
                            tanggalNotifikasi: DateTime.Now
                        ));

                        MenuUtama menuUtama = new MenuUtama(user);
                        menuUtama.Show();
                        this.Hide();
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Password salah untuk username: " + username);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saat membaca data: " + ex.Message);
                Console.WriteLine("Error reading data: " + ex.Message);
            }

            // Fallback hardcoded user
            if (username == "user123" && password == "user1234")
            {
                MessageBox.Show("Selamat, anda telah berhasil login sebagai Anggota!");
                MenuUtama menuUtama = new MenuUtama();
                menuUtama.Show();
                this.Hide();
                return;
            }

            MessageBox.Show("Login gagal, silakan cek kembali username dan password!");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var registerForm = new RegisterForm();
            registerForm.ShowDialog();
        }
    }
}
