using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ManajemenPerpus.Core.Models;

namespace ManajemenPerpus.CLI.Service
{
    public class PenggunaService
    {
        private readonly string filePath;
        private List<Pengguna> _listPengguna;

        public PenggunaService()
        {
            filePath = ManajemenPerpus.Core.Helper.JsonHelper.GetSharedDataPath("DataPengguna.json");
            _listPengguna = new List<Pengguna>();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string jsonData = File.ReadAllText(filePath);
                    _listPengguna = JsonSerializer.Deserialize<List<Pengguna>>(jsonData) ?? new List<Pengguna>();
                }
                else
                {
                    // Create file if it doesn't exist
                    File.WriteAllText(filePath, "[]");
                    _listPengguna = new List<Pengguna>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading user data: {ex.Message}");
                _listPengguna = new List<Pengguna>();
            }
        }

        private void SaveData()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonData = JsonSerializer.Serialize(_listPengguna, options);
                File.WriteAllText(filePath, jsonData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving user data: {ex.Message}");
            }
        }

        public void AddPengguna(string userClass, string username, string password, Pengguna.ROLEPENGGUNA role, string fullname, string email, string phone, string address)
        {
            if (userClass == "Admin" || userClass == "Anggota")
            {
                string id = GeneratorIdPengguna(role);
                Pengguna newUser = new Pengguna(id, username, password, role, fullname, email, phone, address);
                _listPengguna.Add(newUser);
                SaveData();
            }
            else
            {
                Console.WriteLine("Invalid user class");
            }
        }

        public string GeneratorIdPengguna(Pengguna.ROLEPENGGUNA role)
        {
            string prefix = role == Pengguna.ROLEPENGGUNA.admin ? "A" : "P";

            var existingNumbers = new HashSet<int>();

            foreach (var pengguna in _listPengguna)
            {
                if (pengguna.Role == role &&
                    pengguna.IdPengguna.StartsWith(prefix) &&
                    pengguna.IdPengguna.Length == 4)
                {
                    if (int.TryParse(pengguna.IdPengguna.Substring(1), out int num))
                    {
                        existingNumbers.Add(num);
                    }
                }
            }

            int newNumber = 1;
            while (existingNumbers.Contains(newNumber))
            {
                newNumber++;
            }

            return $"{prefix}{newNumber:D3}";
        }

        public List<Pengguna> GetAllPengguna()
        {
            return _listPengguna;
        }

        public Pengguna GetPenggunaById(string id)
        {
            return _listPengguna.FirstOrDefault(p => p.IdPengguna == id);
        }

        public bool DeletePengguna(string id)
        {
            var pengguna = GetPenggunaById(id);
            if (pengguna != null)
            {
                _listPengguna.Remove(pengguna);
                SaveData();
                return true;
            }
            return false;
        }
    }
}

