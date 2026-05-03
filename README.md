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

---

## 🚀 Panduan Pengoperasian (How to Run)

### 1. Persiapan Awal
*   **Clone Repository:** Pastikan Anda telah meng-clone repository ini ke komputer lokal[cite: 1].
*   **Restore Dependencies:** Buka terminal di folder root solusi dan jalankan perintah:
    ```bash
    dotnet restore
    ```
*   **Data Initialization:** Tidak perlu membuat file JSON manual. Aplikasi akan otomatis membuat folder `SharedData/DataJson` dan akun default saat pertama kali dijalankan[cite: 1].

### 2. Menjalankan via Visual Studio (Rekomendasi)
Untuk fungsionalitas penuh, API dan GUI harus berjalan bersamaan:
1.  Klik kanan pada **Solution 'TubesKPL'** di Solution Explorer.
2.  Pilih **Properties** > **Startup Project**.
3.  Pilih **Multiple startup projects**.
4.  Atur `RestAPI` dan `ManajemenPerpus.GUI` (atau client lain yang diinginkan) ke aksi **Start**.
5.  Tekan **F5** atau klik tombol **Start**.

### 3. Menjalankan via Terminal (CLI)
Jika Anda bekerja di lingkungan tanpa Visual Studio lengkap, gunakan dua jendela terminal terpisah:

**Terminal 1 (Jalankan API Terlebih Dahulu):**
```bash
cd RestAPI
dotnet run
```
*Catatan: Pastikan API berjalan di port `5159`. Jika port berbeda, sesuaikan di `ManajemenPerpus.Core/Helper/ApiConfig.cs`[cite: 1].*

**Terminal 2 (Jalankan Client):**
```bash
cd ManajemenPerpus.GUI
dotnet run --no-build
```
*Gunakan flag `--no-build` untuk menghindari error **File Locking (MSB3021)** jika API sedang berjalan[cite: 1].*

---

### 3. Menjalankan via Terminal
Gunakan PowerShell atau Command Prompt
```bash
Start-Process cmd -ArgumentList '/k title API && cd /d "c:\Kuliah\Pengujian PL\Refrensi Tubes KPL\Tubes-KPL\RestAPI" && dotnet run --no-build' ; Start-Process cmd -ArgumentList '/k title GUI && cd /d "c:\Kuliah\Pengujian PL\Refrensi Tubes KPL\Tubes-KPL\ManajemenPerpus.GUI" && dotnet run --no-build'
```
*Catatan: akan menjalankan API dan GUI secara bersamaan*
---

## 🔐 Akun Akses Default
Sistem secara otomatis menyediakan akun berikut untuk keperluan pengujian[cite: 1]:

| Peran | Username | Password |
| :--- | :--- | :--- |
| **Administrator** | `admin` | `admin` |
| **Anggota** | `user` | `user` |

---

## 🛠️ Troubleshooting & Tips Lab
*   **Error File Lock (MSB3021):** Jika muncul pesan file sedang digunakan oleh proses lain, matikan aplikasi yang sedang berjalan (Stop Debugging) atau gunakan perintah `dotnet run --no-build`[cite: 1].
*   **Gagal Login:** Pastikan project `RestAPI` sudah dalam status *Running*. GUI tidak bisa melakukan autentikasi tanpa layanan dari API[cite: 1].
*   **Konfigurasi Port:** Jika port default `5159` di komputer lab sudah terpakai, Anda hanya perlu mengubah satu baris kode di `ApiConfig.cs` agar seluruh layanan kembali sinkron[cite: 1].

---


## ⚠️ Catatan Penting
*   **Penyimpanan Data JSON:** Seluruh data aplikasi (User, Buku, Ulasan, dsb) disimpan pada folder `SharedData/DataJson`. Project ini menggunakan sistem *Dynamic Relative Pathing* sehingga file JSON akan terdeteksi otomatis pada komputer mana pun tanpa harus menyesuaikan *hardcoded path*.
*   **Navigasi Window (Memory Safe):** Sistem navigasi WinForms telah dikonfigurasi untuk mencegah proses *"zombie"* di latar belakang. Jika pengguna menutup aplikasi via tombol silang (X) di jendela manapun, seluruh aplikasi akan tertutup secara otomatis dari memori.
