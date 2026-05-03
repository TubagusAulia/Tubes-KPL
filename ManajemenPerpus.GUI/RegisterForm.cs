using ManajemenPerpus.Core.Helper;
using ManajemenPerpus.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ManajemenPerpus.GUI
{
    public partial class RegisterForm : Form
    {
        List<Pengguna> penggunaList;
        string filePath = ManajemenPerpus.Core.Helper.JsonHelper.GetSharedDataPath("DataPengguna.json");

        public RegisterForm()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text.Trim();
            string password = textBox2.Text.Trim();
            string confirmPassword = textBox3.Text.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Username dan password tidak tepat, silakan coba kembali");
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Password yang dimasukkan tidak tepat");
                return;
            }

            try 
            {
                penggunaList = JsonHelper.ReadJson<Pengguna>(filePath) ?? new List<Pengguna>();
                if (penggunaList.Any(p => p.Username == username))
                {
                    MessageBox.Show("Username sudah digunakan. Silakan pilih username lain.");
                    return;
                }

                string newId = "USR" + (penggunaList.Count + 1).ToString("D3");
                Pengguna newUser = new Pengguna(newId, username, password, Pengguna.ROLEPENGGUNA.anggota, "", "", "", "");
                penggunaList.Add(newUser);

                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                JsonHelper.WriteJson(filePath, penggunaList);

                MessageBox.Show("Registrasi berhasil!");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saat registrasi: " + ex.Message);
            }
        }
    }
}
