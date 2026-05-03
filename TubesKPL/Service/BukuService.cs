using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using ManajemenPerpus.Core.Models;
using System.Text.Json;

namespace ManajemenPerpus.CLI.Service
{
    public class BukuService
    {
        private readonly string _jsonFilePath;
        private List<BukuDeprecated> _listBuku;

        public BukuService()
        {
            _jsonFilePath = ManajemenPerpus.Core.Helper.JsonHelper.GetSharedDataPath("DataBuku.json");
            LoadData();
        }

        private void LoadData()
        {
            if (File.Exists(_jsonFilePath))
            {
                string json = File.ReadAllText(_jsonFilePath);
                try
                {
                    _listBuku = JsonSerializer.Deserialize<List<BukuDeprecated>>(json, new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<BukuDeprecated>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading buku data: {ex.Message}");
                    _listBuku = new List<BukuDeprecated>();
                }
            }
            else
            {
                _listBuku = new List<BukuDeprecated>();
                SaveData();
            }
        }

        private void SaveData()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(GetAllBuku(), options);
            File.WriteAllText(_jsonFilePath, json);
        }

        public void AddBuku(string judul, string penulis, string penerbit,
                            BukuDeprecated.KATEGORIBUKU kategori, string sinopsis)
        {
            string id = GenerateBukuId();
            BukuDeprecated newBuku = new BukuDeprecated(id, judul, penulis, penerbit, kategori, sinopsis);
            _listBuku.Add(newBuku);
            SaveData();
        }

        private string GenerateBukuId()
        {
            var existingNumbers = new HashSet<int>();

            foreach (var buku in _listBuku)
            {
                if (buku.IdBuku.StartsWith("B") && buku.IdBuku.Length == 4)
                {
                    if (int.TryParse(buku.IdBuku.Substring(1), out int num))
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

            return $"B{newNumber:D3}";
        }

        public List<BukuDeprecated> GetAllBuku()
        {
            return _listBuku.OrderBy(b => b.IdBuku).ToList();
        }

        public BukuDeprecated GetBukuById(string id)
        {
            return _listBuku.FirstOrDefault(b => b.IdBuku.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        public bool DeleteBuku(string id)
        {
            var buku = GetBukuById(id);
            if (buku != null)
            {
                _listBuku.Remove(buku);
                SaveData();
                return true;
            }
            return false;
        }

        public bool UpdateBuku(BukuDeprecated updatedBuku)
        {
            var existingBuku = GetBukuById(updatedBuku.IdBuku);
            if (existingBuku != null)
            {
                existingBuku.Judul = updatedBuku.Judul;
                existingBuku.Penulis = updatedBuku.Penulis;
                existingBuku.Penerbit = updatedBuku.Penerbit;
                existingBuku.Kategori = updatedBuku.Kategori;
                existingBuku.Sinopsis = updatedBuku.Sinopsis;
                SaveData();
                return true;
            }
            return false;
        }
    }
}