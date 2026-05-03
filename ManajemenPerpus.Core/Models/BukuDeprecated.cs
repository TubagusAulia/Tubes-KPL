using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ManajemenPerpus.Core.Models
{

    public class BukuDeprecated
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum STATUSBUKU
        {
            TERSEDIA,
            DIPINJAM,
            RUSAK
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum KATEGORIBUKU
        {
            FIKSI,
            NONFIKSI
        }

        // Data Buku  
        public string IdBuku { get; set; }
        public string Judul { get; set; }
        public string Penulis { get; set; }
        public string Penerbit { get; set; }
        public KATEGORIBUKU Kategori { get; set; }
        public string Sinopsis { get; set; }
        public STATUSBUKU Status { get; set; }

        public DateTime TanggalMasuk { get; set; } // Tanggal buku masuk ke perpustakaan

        public BukuDeprecated(string idBuku, string judul, string penulis, string penerbit, KATEGORIBUKU kategori, string sinopsis)
        {
            IdBuku = idBuku;
            Judul = judul;
            Penulis = penulis;
            Penerbit = penerbit;
            Kategori = kategori;
            Sinopsis = sinopsis;
            Status = STATUSBUKU.TERSEDIA; // status default  
            TanggalMasuk = DateTime.Now; // Set tanggal masuk ke waktu sekarang
        }

        // Parameterless constructor needed for System.Text.Json deserialization
        public BukuDeprecated()
        {
            IdBuku = string.Empty;
            Judul = string.Empty;
            Penulis = string.Empty;
            Penerbit = string.Empty;
            Sinopsis = string.Empty;
            Status = STATUSBUKU.TERSEDIA;
            TanggalMasuk = DateTime.Now;
        }
    }
}
