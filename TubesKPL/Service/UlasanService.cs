using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ManajemenPerpus.Core.Models;
using ManajemenPerpus.Core.Helper;

namespace ManajemenPerpus.CLI.Service
{
    public class UlasanService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = ManajemenPerpus.Core.Helper.ApiConfig.BaseUrl + "api/Ulasan";
        private readonly string _filePath;

        private List<FactoryBuku> _listBuku;
        public BukuServiceNew bukuService = new BukuServiceNew();
        
        public UlasanService()
        {
            _filePath = ManajemenPerpus.Core.Helper.JsonHelper.GetSharedDataPath("DataUlasan.json");
            _httpClient = new HttpClient();
        }

        // Mengambil semua ulasan dari API
        public async Task<List<Ulasan>> GetUlasanFromApi()
        {
            try
            {
                var response = await _httpClient.GetAsync(_apiUrl);
                response.EnsureSuccessStatusCode();
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var ulasanList = JsonSerializer.Deserialize<List<Ulasan>>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return ulasanList ?? new List<Ulasan>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error fetching ulasan: {ex.Message}");
                return new List<Ulasan>();
            }
        }

        // Menampilkan semua ulasan yang ada
        public async Task ShowAllUlasan()
        {
            var listUlasan = await GetUlasanFromApi();

            if (listUlasan.Count == 0)
            {
                Console.WriteLine("Tidak ada ulasan yang tersedia.");
            }
            else
            {
                Console.WriteLine("Daftar Ulasan:");
                foreach (var u in listUlasan)
                {
                    Console.WriteLine($"ID Ulasan: {u.ulasanId}, ID Buku: {u.bukuId}, Isi Ulasan: {u.isiUlasan}");
                }
            }
            Console.WriteLine("Tekan sembarang tombol untuk melanjutkan...");
            Console.ReadKey();
            Console.Clear();
        }

        // Menampilkan semua ulasan berdasarkan ID buku
        public async Task ShowAllUlasanByBookId()
        {
            // Mengambil semua ulasan dari API
            var listUlasan = await GetUlasanFromApi();
            Console.WriteLine("Masukkan ID Buku untuk melihat ulasan: ");
            string bukuId = Console.ReadLine();
            var ulasanByBookId = listUlasan.Where(u => u.bukuId == bukuId).ToList();
            if (ulasanByBookId.Count == 0)
            {
                Console.WriteLine($"Tidak ada ulasan untuk buku dengan ID {bukuId}.");
            }
            else
            {
                Console.WriteLine($"Daftar Ulasan untuk Buku ID {bukuId}:");
                foreach (var u in ulasanByBookId)
                {
                    Console.WriteLine($"ID Ulasan: {u.ulasanId}, Isi Ulasan: {u.isiUlasan}");
                }
            }
            Console.WriteLine("Tekan sembarang tombol untuk melanjutkan...");
            Console.ReadKey();
            Console.Clear();
        }

        public string AddUlasanGUI(string isiUlasan)
        {
            var _listUlasan = JsonHelper.ReadJson<Ulasan>(_filePath) ?? new List<Ulasan>();
            var bukuId = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(bukuId))
                return "ID Buku tidak boleh kosong.";

            if (bukuId.Length > 10 || !bukuId.All(char.IsLetterOrDigit))
                return "ID Buku tidak valid. Hanya karakter alfanumerik max 10.";

            var _listBuku = bukuService.GetBukuFromApi().GetAwaiter().GetResult();
            var buku = _listBuku.FirstOrDefault(b => b.IdBuku == bukuId);
            if (buku == null)
                return "Buku dengan ID tersebut tidak ditemukan.";

            if (string.IsNullOrWhiteSpace(isiUlasan))
                return "Isi ulasan tidak boleh kosong.";

            if (isiUlasan.Length > 500)
                return "Isi ulasan terlalu panjang. Maksimal 500 karakter.";

            isiUlasan = new string(isiUlasan.Where(c => !char.IsControl(c) || c == '\n' || c == '\r').ToArray());

            var ulasan = new Ulasan(GenerateUlasanId(), bukuId, isiUlasan);
            _listUlasan.Add(ulasan);
            JsonHelper.WriteJson(_filePath, _listUlasan);

            return "success";
        }


        // Menambahkan ulasan baru
        public void AddUlasan()
        {
            var _listUlasan = JsonHelper.ReadJson<Ulasan>(_filePath) ?? new List<Ulasan>();
            try
            {
                Console.WriteLine("Masukan ID Buku: ");
                string bukuId = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(bukuId))
                {
                    Console.WriteLine("ID Buku tidak boleh kosong.");
                    return;
                }
                // validasi bukuId agar hanya alfanumerik dan panjang maksimal 10 karakter
                if (bukuId.Length > 10 || !bukuId.All(char.IsLetterOrDigit))
                {
                    Console.WriteLine("ID Buku tidak valid. Hanya karakter alfanumerik dengan panjang maksimal 20.");
                    return;
                }
                _listBuku = bukuService.GetBukuFromApi().GetAwaiter().GetResult();
                var buku = _listBuku.FirstOrDefault(b => b.IdBuku == bukuId);
                if (buku == null)
                {
                    Console.WriteLine("Buku dengan ID tersebut tidak ditemukan.");
                    return;
                }
                Console.WriteLine("Masukan Ulasan: ");
                string isiUlasan = Console.ReadLine()?.Trim();
                // validasi isi ulasan agar tidak kosong
                if (string.IsNullOrWhiteSpace(isiUlasan))
                {
                    Console.WriteLine("Isi ulasan tidak boleh kosong.");
                    return;
                }
                // Limitasi isi ulasan
                if (isiUlasan.Length > 500)
                {
                    Console.WriteLine("Isi ulasan terlalu panjang. Maksimal 500 karakter.");
                    return;
                }
                // Basic sanitization: remove control characters
                isiUlasan = new string(isiUlasan.Where(c => !char.IsControl(c) || c == '\n' || c == '\r').ToArray());
                Ulasan ulasan = new Ulasan(GenerateUlasanId(), bukuId, isiUlasan);
                _listUlasan.Add(ulasan);
                JsonHelper.WriteJson(_filePath, _listUlasan);
                Console.WriteLine("Ulasan berhasil ditambahkan.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Terjadi kesalahan saat menambahkan ulasan.");
            }
        }

        // Menghasilkan ID unik untuk ulasan baru
        public string GenerateUlasanId()
        {
            var listUlasan = JsonHelper.ReadJson<Ulasan>(_filePath);

            string id = "ULS" + (listUlasan.Count + 1).ToString("D3");
            return id;
        }
    }
}