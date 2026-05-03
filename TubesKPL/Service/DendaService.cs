using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using ManajemenPerpus.Core.Models;

namespace ManajemenPerpus.CLI.Service
{
    public class DendaService
    {
        private readonly string _jsonFilePath;
        private readonly List<Denda> _dendaList;
        private readonly BukuService _bukuService;
        private readonly PenggunaService _penggunaService;

        public DendaService()
        {
            _jsonFilePath = ManajemenPerpus.Core.Helper.JsonHelper.GetSharedDataPath("DataDenda.json");
            _dendaList = new List<Denda>();
            _bukuService = new BukuService();
            _penggunaService = new PenggunaService();

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                if (!File.Exists(_jsonFilePath)) return;

                var jsonData = File.ReadAllText(_jsonFilePath);
                if (!string.IsNullOrWhiteSpace(jsonData))
                {
                    _dendaList.AddRange(JsonSerializer.Deserialize<List<Denda>>(jsonData) ?? new List<Denda>());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
        }

        private void SaveData()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var jsonData = JsonSerializer.Serialize(_dendaList, options);
                File.WriteAllText(_jsonFilePath, jsonData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving data: {ex.Message}");
            }
        }

        /// Mengembalikan semua data denda.
        public List<Denda> GetAllDenda() => _dendaList;

        /// Mengambil data denda berdasarkan ID.
        public Denda? GetDendaById(string id) =>
            _dendaList.FirstOrDefault(d => d.IdDenda == id);

        /// Mengambil semua denda milik pengguna tertentu.
        public List<Denda> GetDendaByPengguna(string idPengguna) =>
            _dendaList.Where(d => d.IdPengguna == idPengguna).ToList();

        /// Menambahkan denda baru.
        public void AddDenda(Denda newDenda)
        {
            _dendaList.Add(newDenda);
            SaveData();
        }

        /// Memperbarui data denda.
        public void UpdateDenda(Denda updatedDenda)
        {
            var denda = GetDendaById(updatedDenda.IdDenda);
            if (denda is null) return;

            denda.IdPengguna = updatedDenda.IdPengguna;
            denda.IdBuku = updatedDenda.IdBuku;
            denda.IdPeminjaman = updatedDenda.IdPeminjaman;
            denda.StatusDenda = updatedDenda.StatusDenda;
            denda.JumlahDenda = updatedDenda.JumlahDenda;
            denda.JumlahHariTerlambat = updatedDenda.JumlahHariTerlambat;

            SaveData();
        }

        /// Menghapus denda berdasarkan ID.
        public void DeleteDenda(string id)
        {
            var denda = GetDendaById(id);
            if (denda is null) return;

            _dendaList.Remove(denda);
            SaveData();
        }

        /// Menandai denda sebagai lunas.
        public void BayarDenda(string id)
        {
            var denda = GetDendaById(id);
            if (denda is null) return;

            denda.StatusDenda = Denda.STATUSDENDA.LUNAS;
            SaveData();
        }

        /// Menampilkan menu untuk menghitung dan membayar denda yang belum lunas.
        public void HitungDenda()
        {
            Console.Clear();
            Console.WriteLine("=== PERHITUNGAN DENDA ===");

            var dendaBelumLunas = _dendaList
                .Where(d => d.StatusDenda == Denda.STATUSDENDA.BELUMLUNAS)
                .ToList();

            if (!dendaBelumLunas.Any())
            {
                Console.WriteLine("Tidak ada denda yang belum dibayar.");
                Console.ReadKey();
                return;
            }

            TampilkanDendaBelumLunas(dendaBelumLunas);

            int selectedIndex = PilihDendaDariList(dendaBelumLunas.Count);
            if (selectedIndex == 0) return;

            var selectedDenda = dendaBelumLunas[selectedIndex - 1];
            BayarDenda(selectedDenda.IdDenda);

            Console.WriteLine($"\nDenda {selectedDenda.IdDenda} telah dibayar.");
            Console.ReadKey();
        }

        private void TampilkanDendaBelumLunas(List<Denda> dendaList)
        {
            Console.WriteLine("\nDaftar Denda Belum Lunas:");
            for (int i = 0; i < dendaList.Count; i++)
            {
                var denda = dendaList[i];
                var buku = _bukuService.GetBukuById(denda.IdBuku);
                var pengguna = _penggunaService.GetPenggunaById(denda.IdPengguna);

                Console.WriteLine($"{i + 1}. ID Denda: {denda.IdDenda}");
                Console.WriteLine($"   Buku: {buku?.Judul ?? "Tidak Diketahui"}");
                Console.WriteLine($"   Anggota: {pengguna?.Fullname ?? "Tidak Diketahui"}");
                Console.WriteLine($"   Hari Keterlambatan: {denda.JumlahHariTerlambat}");
                Console.WriteLine($"   Jumlah Denda: Rp{denda.JumlahDenda:N0}\n");
            }
        }

        private int PilihDendaDariList(int maxPilihan)
        {
            Console.Write("Pilih nomor denda yang akan dibayar (0 untuk batal): ");
            if (!int.TryParse(Console.ReadLine(), out int pilihan) ||
                pilihan < 0 || pilihan > maxPilihan)
            {
                Console.WriteLine("Pilihan tidak valid!");
                Console.WriteLine("Pilih nomor denda yang akan dibayar (0 untuk batal): ");
                Console.ReadKey();
                return 0;
            }

            return pilihan;
        }
    }
}
