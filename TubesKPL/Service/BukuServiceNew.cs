using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using ManajemenPerpus.Core.Models;
using System.Text.Json;
using ManajemenPerpus.Core.Helper;
using System.Text;

namespace ManajemenPerpus.CLI.Service
{
    public class BukuServiceNew
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "http://localhost:5159/api/Buku";

        private readonly string filePath;

        public BukuServiceNew()
        {
            var root = Directory.GetParent(AppContext.BaseDirectory)?.Parent?.Parent?.Parent?.Parent?.FullName;
            filePath = Path.Combine(root, "SharedData", "DataJson", "DataBuku.json");
            _httpClient = new HttpClient();
        }

        // Mengambil daftar buku dari API
        public async Task<List<FactoryBuku>> GetBukuFromApi()
        {
            try
            {
                var response = await _httpClient.GetAsync(_apiUrl);
                response.EnsureSuccessStatusCode();
                var jsonResponse = await response.Content.ReadAsStringAsync();

                var jsonList = JsonSerializer.Deserialize<List<JsonElement>>(jsonResponse);
                var bukuList = new List<FactoryBuku>();

                // Parsing JSON dan membuat objek FactoryBuku
                foreach (var item in jsonList)
                {
                    string idBuku = item.GetProperty("idBuku").GetString();
                    string judul = item.GetProperty("judul").GetString();
                    string penulis = item.GetProperty("penulis").GetString();
                    string penerbit = item.GetProperty("penerbit").GetString();
                    string kategori = item.GetProperty("kategori").GetString();
                    string sinopsis = item.GetProperty("sinopsis").GetString();
                    DateTime tanggalMasuk = item.GetProperty("tanggalMasuk").GetDateTime();

                    FactoryBuku buku;

                    if (kategori.ToLower().Contains("fiksi"))
                    {
                        buku = new BukuFiksiCreator(idBuku, judul, penulis, penerbit, kategori, sinopsis, tanggalMasuk);
                    }
                    else
                    {
                        buku = new BukuNonFiksiCreator(idBuku, judul, penulis, penerbit, kategori, sinopsis, tanggalMasuk);
                    }

                    bukuList.Add(buku);
                }

                return bukuList;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error fetching buku: {ex.Message}");
                return new List<FactoryBuku>();
            }
        }

        // Menampilkan semua buku yang ada
        public async Task ShowAllBuku()
        {
            var listBuku = await GetBukuFromApi();
            if (listBuku.Count == 0)
            {
                Console.WriteLine("Tidak ada buku yang tersedia.");
            }
            else
            {
                Console.WriteLine("Daftar Buku:");
                foreach (var buku in listBuku)
                {
                    Console.WriteLine($"ID Buku: {buku.IdBuku} | Judul: {buku.Judul} | Penulis: {buku.Penulis} | Penerbit: {buku.Penerbit} | Kategori: {buku.Kategori}");
                }
            }
            Console.WriteLine("Tekan sembarang tombol untuk melanjutkan...");
            Console.ReadKey();
        }

        // menambahkan buku baru
        public void AddBuku()
        {
            Console.WriteLine("=== Tambah Buku Baru ===");
            Console.Write("Judul Buku: ");
            string judul = Console.ReadLine();
            Console.Write("Penulis: ");
            string penulis = Console.ReadLine();
            Console.Write("Penerbit: ");
            string penerbit = Console.ReadLine();
            Console.Write("Kategori (Fiksi/Non Fiksi): ");
            string kategori = Console.ReadLine().Trim().ToLower();
            Console.Write("Sinopsis: ");
            string sinopsis = Console.ReadLine();

            BukuDTO bukuDto = new BukuDTO
            {
                IdBuku = GenerateBukuId(),
                Judul = judul,
                Penulis = penulis,
                Penerbit = penerbit,
                Kategori = kategori,
                Sinopsis = sinopsis,
                TanggalMasuk = DateTime.Now
            };

            List<BukuDTO> _listBuku = new List<BukuDTO>();
            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
                if(!string.IsNullOrEmpty(jsonData))
                {
                    _listBuku = JsonSerializer.Deserialize<List<BukuDTO>>(jsonData) ?? new List<BukuDTO>();
                }
            }

            _listBuku.Add(bukuDto);
            JsonHelper.WriteJson(filePath, _listBuku);
            Console.WriteLine("Buku berhasil ditambahkan.");
            Console.Clear();
        }

        private string GenerateBukuId()
        {
            var existingNumbers = new HashSet<int>();
            var _listBuku = JsonHelper.ReadJson<FactoryBuku>(filePath);

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
    }
}