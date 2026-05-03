using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManajemenPerpus.Core.Models
{
    public class BukuDTO
    {
        public string IdBuku { get; set; } = string.Empty;
        public string Judul { get; set; } = string.Empty;
        public string Penulis { get; set; } = string.Empty;
        public string Penerbit { get; set; } = string.Empty;
        public string Kategori { get; set; } = string.Empty;
        public string Sinopsis { get; set; } = string.Empty;
        public STATUSBUKU Status { get; set; }
        public DateTime TanggalMasuk { get; set; }
    }
}
