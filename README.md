# Manajemen Perpustakaan (Tubes KPL)

Proyek ini adalah sistem manajemen perpustakaan yang dikembangkan dengan arsitektur multi-project menggunakan .NET/C#. Aplikasi ini dirancang untuk memisahkan antara logika bisnis, penyimpanan data, dan berbagai antarmuka pengguna (GUI, CLI, dan API).

## 📌 Struktur Solusi (Project Breakdown)

Solusi ini (`TubesKPL.sln`) dibagi menjadi beberapa project agar kode lebih terorganisir dan mudah dikelola (*Separation of Concerns*):

### 1. ManajemenPerpus.Core
Project ini adalah **Class Library** yang berfungsi sebagai "otak" dari aplikasi.
*   **Isi:** Business logic, data models, dan helper classes.
*   **Fungsi:** Menampung logika utama yang digunakan oleh semua interface (GUI, CLI, dan API).

### 2. SharedData
Menangani persistensi atau penyimpanan data aplikasi.
*   **Isi:** Folder `DataJson`.
*   **Fungsi:** Mengelola penyimpanan data (buku, user, transaksi) menggunakan format **JSON** sebagai pengganti database SQL tradisional.

### 3. RestAPI (ManajemenPerpus.API)
Project **ASP.NET Core Web API**.
*   **Fungsi:** Menyediakan *endpoint* RESTful sehingga fungsi manajemen perpustakaan bisa diakses melalui protokol HTTP oleh aplikasi eksternal atau web frontend.

### 4. ManajemenPerpus.GUI
Aplikasi desktop utama berbasis **Windows Forms (WinForms)**.
*   **Fitur Utama:** Login system, dashboard utama (`MenuUtama`), manajemen koleksi buku, sistem sirkulasi (pinjam/kembali), serta halaman ulasan dan notifikasi.

### 5. ManajemenAdminGUI
Aplikasi **WinForms** khusus untuk kebutuhan administratif.
*   **Fungsi:** Saat ini difokuskan pada fitur `LaporanStatisticGui` untuk melihat statistik dan laporan data perpustakaan.

### 6. TubesKPL (ManajemenPerpus.CLI)
Aplikasi berbasis **Console (CLI)**.
*   **Fungsi:** Antarmuka berbasis teks sebagai alternatif bagi pengguna yang ingin berinteraksi dengan sistem tanpa menggunakan aplikasi grafis.

### 7. UnitTesting
Project pengujian otomatis menggunakan **MSTest**.
*   **Fungsi:** Memastikan logika pada `ManajemenPerpus.Core` berjalan dengan benar dan meminimalisir munculnya bug saat melakukan perubahan kode (*refactoring*).

---

## 🚀 Ringkasan Peran
*   **Core Logic:** `ManajemenPerpus.Core`
*   **Data Storage:** `SharedData` (JSON based)
*   **User Interfaces:** 
    *   Desktop (User): `ManajemenPerpus.GUI`
    *   Desktop (Admin): `ManajemenAdminGUI`
    *   Terminal/Console: `TubesKPL`
*   **Web Services:** `RestAPI`
*   **Quality Assurance:** `UnitTesting`

---

## 🛠️ Persyaratan Sistem (Prerequisites)
*   **Visual Studio 2022** atau IDE yang mendukung pengembangan C# / .NET.
*   **.NET SDK** (Pastikan versi framework .NET sesuai dengan spesifikasi project).

## 💻 Cara Menjalankan Aplikasi

1.  **Buka Solution:** Buka file `TubesKPL.sln` menggunakan Visual Studio.
2.  **Jalankan API (Sangat Disarankan):** 
    Sebagian besar fitur GUI (seperti mengambil daftar buku) bergantung pada backend API. Sangat disarankan untuk menjalankan project `RestAPI` terlebih dahulu. (Klik kanan `RestAPI` -> *Set as Startup Project* -> *Start*).
3.  **Jalankan Klien:**
    Atur Visual Studio untuk menjalankan *Multiple Startup Projects* atau buka instance Visual Studio baru:
    *   Pilih `ManajemenPerpus.GUI` untuk aplikasi utama Anggota.
    *   Pilih `ManajemenAdminGUI` untuk dashboard Admin.
    *   Pilih `TubesKPL` untuk aplikasi versi Terminal/CLI.

## ⚠️ Catatan Penting
*   **Penyimpanan Data JSON:** Seluruh data aplikasi (User, Buku, Ulasan, dsb) disimpan pada folder `SharedData/DataJson`. Project ini menggunakan sistem *Dynamic Relative Pathing* sehingga file JSON akan terdeteksi otomatis pada komputer mana pun tanpa harus menyesuaikan *hardcoded path*.
*   **Navigasi Window (Memory Safe):** Sistem navigasi WinForms telah dikonfigurasi untuk mencegah proses *"zombie"* di latar belakang. Jika pengguna menutup aplikasi via tombol silang (X) di jendela manapun, seluruh aplikasi akan tertutup secara otomatis dari memori.
