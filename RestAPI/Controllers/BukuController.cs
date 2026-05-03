using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using ManajemenPerpus.Core.Models;

namespace ManajemenPerpus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BukuController : ControllerBase
    {
        private string filePath = ManajemenPerpus.Core.Helper.JsonHelper.GetSharedDataPath("DataBuku.json");
        private static List<FactoryBuku> listBuku = new List<FactoryBuku>();

        [HttpGet]
        public ActionResult<List<FactoryBuku>> GetAllBuku()
        {
            if (System.IO.File.Exists(filePath))
            {
                try
                {
                    string jsonData = System.IO.File.ReadAllText(filePath);
                    var data = JsonSerializer.Deserialize<List<BukuDTO>>(jsonData, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    });
                    if (data == null)
                    {
                        return Ok(new List<BukuDTO>());
                    }

                    var result = data.Select(b => new BukuDTO
                    {
                        IdBuku = b.IdBuku,
                        Judul = b.Judul,
                        Penulis = b.Penulis,
                        Penerbit = b.Penerbit,
                        Kategori = b.Kategori,
                        Sinopsis = b.Sinopsis,
                        Status = b.Status, // Fixed the invalid initializer member declarator
                        TanggalMasuk = b.TanggalMasuk
                    }).ToList();

                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Error reading or parsing the file: {ex.Message}");
                }
            }
            else
            {
                return NotFound("Buku data file not found at: " + filePath);
            }
        }

        [HttpPost]
        public IActionResult PostBuku([FromBody] BukuDTO bukuDto)
        {
            if (bukuDto == null)
                return BadRequest("Data buku kosong.");

            // validasi kategori
            var kategori = bukuDto.Kategori.Trim().ToLower();
            if (kategori != "fiksi" && kategori != "non fiksi")
                return BadRequest("Kategori tidak valid. Harus 'Fiksi' atau 'Non Fiksi'.");

            // buat objek factory
            FactoryBuku buku = kategori == "fiksi"
                ? new BukuFiksiCreator(bukuDto.IdBuku, bukuDto.Judul, bukuDto.Penulis, bukuDto.Penerbit, bukuDto.Kategori, bukuDto.Sinopsis, bukuDto.TanggalMasuk)
                : new BukuNonFiksiCreator(bukuDto.IdBuku, bukuDto.Judul, bukuDto.Penulis, bukuDto.Penerbit, bukuDto.Kategori, bukuDto.Sinopsis, bukuDto.TanggalMasuk);

            // simpan ke file JSON atau database, sesuai logic kamu
            // (pastikan saat menyimpan, simpan dalam bentuk DTO juga)

            return Ok("Data buku berhasil ditambahkan.");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBuku(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("ID buku tidak boleh kosong.");
            // cari buku berdasarkan ID
            var buku = listBuku.FirstOrDefault(b => b.IdBuku == id);
            if (buku == null)
                return NotFound($"Buku dengan ID {id} tidak ditemukan.");
            // hapus buku dari list
            listBuku.Remove(buku);
            // simpan perubahan ke file JSON atau database, sesuai logic kamu
            return Ok($"Buku dengan ID {id} berhasil dihapus.");
        }
    }

}
