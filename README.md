# Memory Loop
A source code for my own original game, a visual novel game kinda thing with the style of 2D pixel art. 

<div align="center">

# 🏠 Hari yang Terus Berulang

[Download source code disini! 👋](https://drive.google.com/file/d/1w-Y3Wjmb8MIVBdesMe1MyjM75DPAJzlK/view?usp=sharing)

### *Setiap hari yang sama, adalah caramu untuk tidak melupakan.*

[![Bahasa](https://img.shields.io/badge/🇮🇩_Bahasa-Indonesia-red?style=for-the-badge)](README.md)
[![Language](https://img.shields.io/badge/🇬🇧_Switch_to-English-blue?style=for-the-badge)](README.en.md)

![Unity](https://img.shields.io/badge/Unity-6000.4.0f1-000000?style=flat-square&logo=unity)
![Genre](https://img.shields.io/badge/Genre-2D_Pixel_Art_Narrative-8B5E3C?style=flat-square)
![Status](https://img.shields.io/badge/Status-UAS_Project-6A4C93?style=flat-square)
![Platform](https://img.shields.io/badge/Platform-PC-1e88e5?style=flat-square)

</div>

---

<p align="center">
  <img width="1577" height="881" alt="Screenshot 2026-07-25 231738" src="https://github.com/user-attachments/assets/516bd0d4-5c56-47fc-9e31-585685d5cb2f" />
  <img width="1578" height="888" alt="Screenshot 2026-07-25 201436" src="https://github.com/user-attachments/assets/a2cb30c2-1e31-4565-bdfe-a1394ecbe792" />
</p>

<details>
<summary><b>🔍 Tutorial setup</b></p></summary>

## ✅ Sebelum Mulai

Pastikan kamu sudah punya:

- **[Unity Hub](https://unity.com/download)** terinstall
- **Unity Editor `6000.4.0f1`** (atau versi 6.x yang mendekati) install lewat Unity Hub
  - Buka Unity Hub → tab **Installs** → **Install Editor** → pilih versi yang sesuai
- Ruang penyimpanan yang cukup (beberapa GB untuk Editor + file project)

---

## 🗂️ [Metode 1] Download ZIP (Paling Mudah, Tanpa Git)

**Langkah 1: Download**

Buka halaman repository GitHub [(link ada di README)]((https://drive.google.com/file/d/1w-Y3Wjmb8MIVBdesMe1MyjM75DPAJzlK/view?usp=sharing)), klik tombol hijau **`<> Code`**, lalu klik **Download ZIP**.

**Langkah 2: Extract File ZIP**

Klik kanan file `.zip` yang sudah terdownload → **Extract All** (Windows) atau double-click (Mac). Bisa pakai tool apa saja (WinRAR, 7-Zip, atau bawaan sistem).

> ⚠️ Setelah di-extract, biasanya akan muncul folder seperti `HariYangTerusBerulang-main`. **Buka folder itu** — di dalamnya seharusnya ada subfolder bernama `Assets`, `Packages`, dan `ProjectSettings`. Itulah folder utama project Unity yang kamu butuhkan untuk langkah berikutnya.

**Langkah 3: Buka Unity Hub**

1. Buka **Unity Hub**
2. Masuk ke tab **Projects**
3. Klik **Add** → **Add project from disk**
4. Cari dan pilih folder hasil extract tadi (yang berisi `Assets`, `Packages`, `ProjectSettings`)
5. Klik **Add Project** / **Select Folder**

**Langkah 4: Buka Project**

Klik project yang baru saja muncul di daftar Unity Hub. Unity akan mulai meng-import semua asset — **proses ini bisa memakan waktu beberapa menit saat pertama kali dibuka**, terutama saat compile shader. Tunggu sampai selesai, jangan ditutup paksa.

**Langkah 5: Main!**

Setelah Editor terbuka:
1. Di jendela **Project**, buka `Assets/Scenes/`
2. Double-click **`Main Menu`** untuk membuka scene tersebut
3. Tekan tombol ▶️ **Play** di bagian atas Editor

🎉 Selesai kamu sudah masuk ke dalam game!

---

## 🌱 [Metode 2] Clone lewat Git (Untuk Update & Kontributor)

Kalau kamu ingin project-mu mudah di-*update*, atau berencana ikut berkontribusi, clone dengan Git lebih baik dibanding download ZIP. Berikut panduan lengkapnya walau kamu belum pernah pakai Git sebelumnya:

**Langkah 1: Install Git**

Download dan install Git dari **[git-scm.com](https://git-scm.com/downloads)**. Cukup klik "Next" terus sampai selesai dengan opsi default.

**Langkah 2: Buka Terminal**

- **Windows:** Klik kanan di Desktop atau di dalam folder → **Open in Terminal** (atau cari "Command Prompt" / "Git Bash" di Start Menu)
- **Mac:** Buka aplikasi **Terminal** (cari lewat Spotlight)

**Langkah 3: Clone Repository**

Masuk ke folder tempat kamu ingin menyimpan project, lalu jalankan:

```bash
git clone https://github.com/AceBreaker-Cell/Memory-Loop.git
```

Ini akan membuat folder baru berisi seluruh project.

**Langkah 4: Tambahkan ke Unity Hub**

Sama seperti Metode 1, Langkah 3–5:
1. Buka **Unity Hub** → **Projects** → **Add** → **Add project from disk**
2. Pilih folder hasil clone tadi
3. Buka project, tunggu proses import
4. Buka scene `Main Menu` → tekan **Play**

**Bonus Cara update project di kemudian hari:**
Kalau repository sudah di-update, cukup buka terminal di dalam folder hasil clone-mu dan jalankan:
```bash
git pull
```
Ini akan mengambil perubahan terbaru tanpa perlu download ulang dari awal.

---

## 🛠️ Mengedit Project

Silakan jelajahi, ubah, dan bereksperimen dengan project ini! Beberapa tips:

- Semua script gameplay ada di `Assets/Scripts/`
- Semua scene (Main Menu, Loop 0–3, Final Loop, Ending) ada di `Assets/Scenes/`
- Pastikan versi Unity Editor kamu sama (atau mendekati) `6000.4.0f1` biar tidak muncul peringatan kompatibilitas

---

## ❗ Troubleshooting

| Masalah | Solusi |
|---|---|
| Unity Hub bilang "Unity version not found" | Install versi yang sesuai lewat Unity Hub → Installs → Install Editor |
| Tekstur pink/hilang setelah dibuka | Biarkan project selesai import sepenuhnya, lalu restart Unity |
| Project tidak mau terbuka / stuck loading | Pastikan kamu memilih folder yang langsung berisi `Assets`, bukan folder di atas atau di dalamnya |
| Semuanya sangat lambat saat pertama dibuka | Normal Unity sedang compile shader dan import asset untuk pertama kalinya. Pembukaan berikutnya akan jauh lebih cepat |

---

## 📜 Credits & Lisensi

Project ini dibuat oleh **Muhammad Aziz Syah Dani** sebagai tugas Ujian Akhir Semester mata kuliah *Pengenalan Pemrograman Game* Dan Fajri Aulia sebagai Konsep cerita dari game ini, dengan aset visual oleh **Nagita Syahira Putri** dan **Muhammad Zaki Daisa Ammar**.

Kamu bebas download, main, dan modifikasi project ini untuk keperluan belajar **mohon cantumkan kredit ke pembuat aslinya jika kamu membagikan, menampilkan, atau mengembangkan karya ini lebih lanjut.**

Beberapa file asset di project ini ukurannya melebihi batas upload GitHub sebesar 25 MB, jadi source code lengkap (beserta semua asset) di-host di Google Drive, bukan langsung di repo ini.

➡️ [Download project lengkap di sini:](https://drive.google.com/file/d/1w-Y3Wjmb8MIVBdesMe1MyjM75DPAJzlK/view?usp=sharing)

Silakan pakai link Drive ini dan ikuti Metode 1 (Download ZIP) di panduan setup repo GitHub-nya saja mungkin ada beberapa file besar yang hilang.

<div align="center">

**Copyright © Albatany 2026**

*Selamat bermain! 🎮*

</div>
</details>

---

## 📖 Tentang Game

**Hari yang Terus Berulang** adalah game *2D pixel art narrative adventure* dengan sentuhan eksplorasi ringan dan *emotional puzzle*. Dibangun di Unity sebagai proyek Ujian Akhir Semester untuk mata kuliah **Pengenalan Pemrograman Game**.

Game ini mengangkat tema tentang **pulang, penyesalan, ingatan, dan menerima kenyataan** dibungkus dalam gaya visual yang hangat namun perlahan berubah menjadi menghantui.

> Kamu berperan sebagai **Mono**, seorang pekerja kantoran yang akhirnya pulang ke rumah masa kecilnya setelah sekian lama tidak berkunjung karena kesibukan. Ibu menyambutnya dengan hangat seperti biasa.
>
> Tapi keesokan harinya... adalah hari yang sama.
> Dan keesokan harinya lagi... juga sama.

Seiring hari yang terus berulang, Mono mulai menyadari ada yang tidak beres jam yang berhenti, foto yang rusak, dan Ibu yang mulai bertingkah aneh. Pemain harus menjelajah, berbicara, dan memilih dengan hati-hati, karena setiap pilihan membentuk bagaimana kisah ini akan berakhir.

---

## 🎮 Mekanisme Gameplay

### Eksplorasi & Interaksi
Jelajahi rumah masa kecil Mono secara bebas dari halaman depan, ruang keluarga, dapur, hingga kamar tidur. Gunakan **Arrow Keys / A-D** untuk berjalan, dan **E / Spasi** untuk berinteraksi dengan objek maupun berbicara dengan karakter lain.

### Dialog Bercabang
Setiap percakapan dengan Ibu punya pilihan respons yang berbeda. Kata-kata yang kamu pilih tidak hanya mengubah reaksi karakter tapi juga diam-diam **mempengaruhi arah cerita dan akhir yang akan kamu dapatkan**.

### Sistem Loop
Cerita berjalan melalui beberapa *loop* (pengulangan hari) yang masing-masing punya suasana dan intensitas berbeda:

| Loop | Suasana | Yang Terjadi |
|---|---|---|
| **Loop 0** | Hangat, normal | Hari pertama pulang semuanya terasa baik-baik saja |
| **Loop 1** | Sedikit suram | Déjà vu mulai muncul, jam dinding berhenti |
| **Loop 2** | Lebih gelap | Foto keluarga rusak, muncul *puzzle* mencari kepingan foto |
| **Loop 3** | Sangat suram | Retakan mulai muncul, Ibu bertingkah semakin aneh |
| **Final Loop** | Puncak cerita | Kebenaran akhirnya terungkap |

### Puzzle Kepingan Foto
Di Loop 2, kamu harus menjelajahi rumah untuk mengumpulkan **kepingan foto keluarga** yang tersebar di berbagai ruangan. Semakin banyak kepingan terkumpul, semakin banyak pula ruangan tersembunyi yang terbuka.

### Sistem Emosi Tersembunyi
Di balik layar, game ini melacak tiga nilai emosi yang tidak terlihat oleh pemain: **Denial (Penyangkalan)**, **Regret (Penyesalan)**, dan **Acceptance (Penerimaan)**. Setiap dialog yang kamu pilih menambah salah satu nilai ini dan nilai tertinggi di akhir game akan menentukan *ending* mana yang kamu dapatkan.

### Item Kenangan
Objek-objek yang kamu temukan dan periksa akan tersimpan sebagai *memory item* sepotong kecil dari masa lalu Mono dan Ibu yang perlahan menyusun potongan kebenaran.

---

## 🌅 [Ending] Empat Akhir Berbeda

<details>
<summary><b>⚠️ Klik untuk membuka mengandung SPOILER cerita</b></summary>

<br>

Akhir yang kamu dapatkan ditentukan oleh pola pilihan dialog sepanjang permainan, bukan oleh satu keputusan besar di akhir.

### 🌤️ [Ending A] *Acceptance (Penerimaan)*
Mono akhirnya melepaskan. Ibu tidak benar-benar hilang ia menjadi kenangan yang bisa Mono bawa tanpa terjebak di dalamnya. Rumah perlahan menjadi terang, *loop* berhenti, dan pagi yang sesungguhnya akhirnya datang.

*Didapat dengan: sering memilih dialog yang jujur dan terbuka, serta mengumpulkan mayoritas item kenangan.*

### 🌫️ [Ending B] *Denial (Penyangkalan)*
Mono menolak menerima kenyataan. Ia kembali terbangun di depan rumah namun kali ini bahkan menu utama pun terlihat rusak. Kesan yang tertinggal: *loop* ini belum, dan mungkin tidak akan pernah, selesai.

*Didapat dengan: sering memilih dialog yang menghindar dan mengabaikan tanda-tanda kebenaran.*

### 🍂 [Ending C] *Regret (Penyesalan)*
Mono sadar akan kebenarannya, tapi belum bisa sepenuhnya berdamai. Ia meninggalkan rumah itu dengan foto keluarga yang belum lengkap — akhir yang terasa pahit dan menggantung.

*Didapat dengan: pilihan dialog yang bercampur antara jujur dan menghindar, kepingan foto tidak terkumpul penuh.*

### ✨ [Secret Ending] *Memory Album Complete*
Jika seluruh kepingan kenangan berhasil dikumpulkan, sebuah epilog singkat terbuka: Mono kembali ke rumah itu di hari yang berbeda — bukan untuk terjebak lagi, namun untuk merapikan rumah dan menyimpan kenangan Ibunya dengan tenang. Kali ini, ia datang untuk berpamitan.

*Didapat dengan: mengumpulkan seluruh item kenangan dan condong pada dialog penerimaan di momen-momen akhir.*

</details>

---

## 💭 Pesan di Balik Cerita

Game ini lahir dari sebuah pertanyaan sederhana: *apa yang tersisa ketika kita terlalu sibuk untuk pulang?*

**Hari yang Terus Berulang** bukan tentang kutukan atau horor semata tapi tentang bagaimana penyangkalan bisa membuat seseorang terjebak mengulang hari yang sama, alih-alih menghadapi kehilangan. Ia mengingatkan bahwa waktu bersama orang-orang yang kita sayangi tidak pernah benar-benar bisa "ditunda sampai nanti."

Kadang, cara paling berani untuk menyayangi seseorang adalah dengan berani mengucapkan selamat tinggal.

---

## 🕹️ Kontrol

| Aksi | Tombol |
|---|---|
| Bergerak Kiri / Kanan | `←` `→` atau `A` `D` |
| Interaksi / Bicara | `E` atau `Spasi` |
| Lanjut Dialog | `E` / `Spasi` / `Enter` |
| Buka Inventaris | Tombol UI (pojok kanan atas) |
| Pause | Tombol UI (pojok kanan atas) |

---

## 🛠️ Dibangun Dengan

- **Engine:** Unity 6 (6000.4.0f1)
- **Bahasa:** C#
- **Rendering:** Universal Render Pipeline (URP), 2D
- **UI:** TextMeshPro
- **Gaya Visual:** 2D Pixel Art, terinspirasi dari *A Space for the Unbound*

---

## 👥 Tim Pengembang

Proyek ini dikerjakan sebagai tugas Ujian Akhir Semester mata kuliah **Pengenalan Pemrograman Game** oleh **Kelompok Ganjil**.

| Peran | Nama |
|---|---|
| 🧠 **Game Concept** | Fajri Aulia |
| 🎮 **Game Design, Programming, Narrative & Direction** | Muhammad Aziz Syah Dani |
| 🎨 **Visual Assets** | Nagita Syahira Putri |
| 🎨 **Visual Assets** | Muhammad Zaki Daisa Ammar |

<div align="center">

*Dikerjakan dengan sepenuh hati oleh satu orang yang tidak pernah berhenti mencoba.*

</div>

---

<div align="center">

**"Aku pulang, Bu."**

</div>
