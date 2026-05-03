using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ManajemenPerpus.Core.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum STATUSBUKU
    {
        TERSEDIA,
        DIPINJAM,
        RUSAK
    }

    // Factory Method Pattern untuk Buku
    public abstract class FactoryBuku
    {

        public string IdBuku { get; set; } = string.Empty;
        public string Judul { get; set; } = string.Empty;
        public string Penulis { get; set; } = string.Empty;
        public string Penerbit { get; set; } = string.Empty;
        public string Kategori { get; set; } = string.Empty;
        public string Sinopsis { get; set; } = string.Empty;
        public STATUSBUKU Status { get; set; } // Status default buku adalah TERSEDIA
        public DateTime TanggalMasuk { get; set; }

        public FactoryBuku() { }

        public FactoryBuku(string idBuku, string judul, string penulis, string penerbit,
            string kategori, string sinopsis, STATUSBUKU status, DateTime tanggalMasuk)
        {
            IdBuku = idBuku;
            Judul = judul;
            Penulis = penulis;
            Penerbit = penerbit;
            Kategori = kategori;
            Sinopsis = sinopsis;
            Status = STATUSBUKU.TERSEDIA; // Status default buku adalah TERSEDIA
            TanggalMasuk = tanggalMasuk;
        }

        // Implementasi Factory Method
        public abstract IBuku FactoryMethod();
    }

    // Interface Buku
    public interface IBuku
    {
        void DisplayInfo();
    }

    // Concrete Creator Buku Fiksi
    public class BukuFiksiCreator : FactoryBuku
    {
        public BukuFiksiCreator(string idBuku, string judul, string penulis, string penerbit,
            string kategori, string sinopsis, DateTime tanggalMasuk)
            : base(idBuku, judul, penulis, penerbit, "Fiksi", sinopsis, STATUSBUKU.TERSEDIA, DateTime.Now)
        {
        }
        public override IBuku FactoryMethod()
        {
            return new BukuFiksi(this);
        }
    }

    // Concrete Creator Buku Non-Fiksi
    public class BukuNonFiksiCreator : FactoryBuku
    {
        public BukuNonFiksiCreator(string idBuku, string judul, string penulis, string penerbit,
            string kategori, string sinopsis, DateTime tanggalMasuk)
            : base(idBuku, judul, penulis, penerbit, "Non Fiksi", sinopsis, STATUSBUKU.TERSEDIA, DateTime.Now)
        {
        }

        public override IBuku FactoryMethod()
        {
            return new BukuNonFiksi(this);
        }
    }

    // Concrete Produk (Buku Fiksi)
    class BukuFiksi : IBuku
    {
        private FactoryBuku _data;

        public BukuFiksi(FactoryBuku data)
        {
            _data = data;
        }

        public void DisplayInfo()
        {
            Console.WriteLine($"Buku Fiksi: {_data.Judul}, Penulis: {_data.Penulis}, " +
                $"Penerbit: {_data.Penerbit}, Kategori: {_data.Kategori}, Sinopsis: {_data.Sinopsis}");
        }
    }

    // Concrete Produk (Buku Non-Fiksi)
    class BukuNonFiksi : IBuku
    {
        private FactoryBuku _data;
        public BukuNonFiksi(FactoryBuku data)
        {
            _data = data;
        }
        public void DisplayInfo()
        {
            Console.WriteLine($"Buku Non-Fiksi: {_data.Judul}, Penulis: {_data.Penulis}, " +
                $"Penerbit: {_data.Penerbit}, Kategori: {_data.Kategori}, Sinopsis: {_data.Sinopsis}");
        }
    }
}
