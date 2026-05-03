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

        private async void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text.Trim();
            string password = textBox2.Text.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Username dan password harus diisi!");
                return;
            }

            // Periksa dari REST API
            try
            {
                using (var client = new System.Net.Http.HttpClient())
                {
                    client.BaseAddress = new Uri(ManajemenPerpus.Core.Helper.ApiConfig.BaseUrl);
                    var loginData = new { Username = username, Password = password };
                    var content = new System.Net.Http.StringContent(System.Text.Json.JsonSerializer.Serialize(loginData), System.Text.Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("api/Pengguna/login", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        var user = System.Text.Json.JsonSerializer.Deserialize<ManajemenPerpus.Core.Models.Pengguna>(responseString, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        
                        if (user != null)
                        {
                            SessionData.CurrentUser = user;
                            
                            if (user.Role == ManajemenPerpus.Core.Models.Pengguna.ROLEPENGGUNA.admin)
                            {
                                MessageBox.Show("Selamat, anda telah berhasil login sebagai Admin!");
                                MenuAdmin menuAdmin = new MenuAdmin();
                                menuAdmin.Show();
                            }
                            else
                            {
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
                            }
                            
                            this.Hide();
                            return;
                        }
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        MessageBox.Show("Password salah untuk username: " + username);
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Login gagal. Server merespon dengan: " + response.StatusCode);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saat menghubungi server API: " + ex.Message +
                    "\n\nPastikan server API sudah berjalan di http://localhost:5159/", "Koneksi Gagal");
                Console.WriteLine("API Error: " + ex.Message);
                return; // return here so we don't show a second error message below
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
