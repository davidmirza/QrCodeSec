# QrSec Designer

Aplikasi Windows (WinForms, .NET Framework 4.8) untuk membuat QR **qrsec**. Satu QR berisi dua lapis data:

- **Main**: teks yang terbaca semua aplikasi scanner (nomor seri, URL, dll).
- **Secret**: data tersembunyi setelah terminator QR. Hanya aplikasi ZUQrCode (APK) yang menampilkannya.

Secret bisa **ditandatangani** dengan signing key (ECDSA P-256), sehingga APK bisa membuktikan QR dibuat oleh Anda.
Signing juga bisa dimatikan; secret tetap terbaca, tetapi keasliannya tidak dijamin.

| Komponen | Lokasi |
|---|---|
| Aplikasi ini | `QrSecDesigner\` di `qrsec.sln` |
| DLL pembuat QR | `qrsec\` → `mrzha.dll` (lihat `PANDUAN_APK.md` di folder induk) |
| APK pembaca | `C:\CLD\qrsec\androidzarda-zuqrcode-cf60f3d6806a` (lihat `CATATAN_QRSEC.md` di sana) |

---

## 1. Menjalankan

**Kebutuhan:** Windows 10/11 dengan .NET Framework 4.8 (sudah bawaan Windows 10 1903 ke atas).

**Dari Visual Studio:** buka `qrsec.sln`, klik kanan **QrSecDesigner** → *Set as Startup Project*, lalu tekan F5.

**Dari command line** (butuh .NET SDK):

```powershell
dotnet build qrsec.sln -c Release
```

Hasilnya ada di `QrSecDesigner\bin\Release\net48\`. Untuk dipakai di PC lain, salin isi folder itu (minimal 4 file ini):

```
QrSecDesigner.exe   QrSecDesigner.exe.config   mrzha.dll   zxing.dll
```

`zxing.dll` hanya dipakai untuk verifikasi (membaca ulang QR yang baru dibuat), bukan untuk membuat QR.

---

## 2. Tampilan

| Kontrol | Fungsi |
|---|---|
| **Isi QR / main** | Teks yang terlihat semua scanner. Wajib diisi. |
| **Secret** | Data tersembunyi. Penghitung di bawahnya menunjukkan pemakaian byte UTF-8 (merah bila lewat batas). |
| **Aktifkan signing key** | ✔ = QR ditandatangani (`Buat.CetakAman`). ☐ = main + secret polos (`Buat.Cetak`); kontrol kunci ikut nonaktif. |
| **Error correction** | L 7% (QR paling kecil), M 15%, **Q 25% (disarankan)**, H 30% (paling tahan rusak/kotor). |
| **Ukuran gambar (px)** | 100 - 2000. Gambar bisa sedikit lebih besar bila QR butuh lebih banyak modul. |
| **File kunci (.key) / Pilih...** | Memuat signing key yang sudah ada. |
| **KeyId kunci baru / Buat kunci baru...** | Membuat signing key baru dengan keyId 1 - 255. |
| **Salin public key untuk APK** | Menyalin baris Java `PUBLIC_KEYS.put(keyId, "...");` ke clipboard. |
| **Buat QR** (Enter) | Membuat QR lalu memverifikasinya otomatis. |
| **Simpan PNG... / Salin gambar** | Menyimpan ke file atau menyalin ke clipboard (untuk ditempel ke Word, desain label, dll). |
| **Kotak verifikasi** | Hasil baca ulang QR (lihat bagian 5). Hijau = siap cetak. |

### Batas panjang secret

| Signing | Maks secret | Keterangan |
|---|---|---|
| Aktif | **156 byte** | 66 byte dipakai untuk versi, keyId, dan tanda tangan |
| Mati | **222 byte** | |

Satu huruf latin = 1 byte. Huruf beraksen, simbol, atau emoji memakai 2 - 4 byte.
Semakin panjang main + secret, semakin rapat QR-nya. Untuk label kecil, gunakan secret sependek mungkin.

---

## 3. Alur kerja

### Setup awal (sekali per signing key)

1. Isi **KeyId kunci baru** (mulai dari `1`), klik **Buat kunci baru...**, lalu simpan file `.key`.
2. Aplikasi menyimpan:
   - `qrsec_01.key`: kunci privat (terenkripsi DPAPI).
   - `qrsec_01.key.pub.txt`: public key.
   - Baris Java untuk APK, **otomatis tersalin ke clipboard**.
3. Di Android Studio, buka `QrSec.java` dan tempel baris itu di blok `PUBLIC_KEYS`:
   ```java
   static {
       PUBLIC_KEYS.put(1, "MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAE....");
   }
   ```
   Hapus baris **kunci uji** yang masih ada di sana.
4. Build ulang APK dan pasang di HP.

### Pemakaian sehari-hari

1. Buka QrSec Designer. File kunci terakhir dimuat otomatis.
2. Isi main dan secret, pilih setup, lalu klik **Buat QR**.
3. Pastikan kotak verifikasi **hijau**, lalu **Simpan PNG...** dan cetak.

Project Android **tidak perlu diubah** untuk setiap QR. Semua QR yang ditandatangani kunci yang sama otomatis sah di APK.

### Hasil di APK

| Cara cetak | Tampilan di APK |
|---|---|
| Signing aktif, public key sudah ada di APK | 🔑 Signing key: **DITEMUKAN** (keyId N), ✔ ASLI (hijau) |
| Signing mati | 🔓 Signing key: **TIDAK AKTIF**, secret tetap tampil (abu-abu) |
| Signing aktif, public key belum/salah di APK | 🔑 Signing key: DITEMUKAN, ✖ TIDAK SAH (merah) |

---

## 4. Signing key: aturan penting

- **File `.key` adalah kunci utama keamanan.** Siapa pun yang memilikinya (dan bisa membukanya) bisa membuat QR "asli".
  Jangan dikirim lewat chat/email, dan jangan dimasukkan ke repository.
- **DPAPI:** file `.key` hanya bisa dibuka oleh **user Windows yang sama di komputer yang sama**.
  Setelah Windows diinstal ulang atau pindah laptop, file itu tidak bisa dibuka lagi.
  Simpan cadangan file `.key` (misalnya di flashdisk khusus), tetapi cadangan pun hanya berguna di user/PC yang sama.
- **Public key boleh disebar.** Isinya memang ditanam di APK.
- **Membuat kunci pengganti** (hilang, pindah PC, dicurigai bocor):
  1. Gunakan **keyId baru** (2, 3, ...), jangan memakai keyId lama.
  2. Di `QrSec.java`, **tambahkan** barisnya dan **biarkan baris lama tetap ada**:
     ```java
     PUBLIC_KEYS.put(1, "MFkw...kunci lama...");
     PUBLIC_KEYS.put(2, "MFkw...kunci baru...");
     ```
     Label lama dan baru sama-sama sah.
  3. Bila kunci lama **bocor**, hapus baris keyId lama. Label lama akan tampil "TIDAK SAH" dan perlu dicetak ulang.
- **Jangan** membuat kunci baru dengan keyId yang sama lalu mengganti barisnya di APK. Semua label lama dengan
  keyId itu akan langsung "TIDAK SAH". Pengecualian satu-satunya: mengganti kunci uji, yang belum dipakai untuk label sungguhan.

---

## 5. Kotak verifikasi

Setiap kali **Buat QR** diklik, gambar dibaca ulang dengan decoder ZXing (seperti scanner HP):

```
VERIFIKASI OK
✔ Scanner biasa membaca: BBBHWF0504841
✔ Signing key: DITEMUKAN (keyId 1) - tanda tangan sah
  Secret: SN-0504841|BATCH-2026-10|ID-77
  Bagian tersembunyi: 96 byte
```

| Baris | Arti |
|---|---|
| Scanner biasa membaca | Teks yang akan dilihat aplikasi scanner lain (harus sama dengan main) |
| Signing key | DITEMUKAN + sah (signing aktif) atau TIDAK AKTIF (signing mati) |
| Secret | Isi secret yang terbaca dari QR |
| Bagian tersembunyi | Ukuran data setelah terminator (secret + tanda tangan) |

Jika setup atau isi diubah setelah QR dibuat, kotak berubah abu-abu ("klik Buat QR lagi"), supaya QR lama tidak tersimpan tanpa sengaja.

---

## 6. Pengaturan tersimpan

Setup terakhir disimpan saat aplikasi ditutup, di:

```
%APPDATA%\QrSecDesigner\pengaturan.txt
```

Isinya: path file kunci, signing aktif/mati, level EC, ukuran, dan folder simpan terakhir. File ini aman dihapus
untuk kembali ke setup awal. Kunci privat **tidak** disimpan di file ini, hanya path-nya.

---

## 7. Masalah umum

| Gejala | Penyebab & solusi |
|---|---|
| "Kunci tidak bisa dibuka ... DPAPI" | File `.key` dibuat oleh user Windows atau PC lain. Buat kunci baru dengan keyId baru, lalu tambahkan public key-nya ke APK. |
| APK: signing key DITEMUKAN tetapi **TIDAK SAH** | Public key kunci ini belum ada / masih kunci lama di `QrSec.java`. Klik **Salin public key untuk APK**, tempel, build ulang APK. Pesan "Signing key N tidak dikenal" = keyId N belum didaftarkan. |
| "Signing aktif tetapi file kunci belum dimuat" | Klik **Pilih...** atau **Buat kunci baru...**, atau matikan signing. |
| "Secret terlalu panjang" | Lewat 156 byte (signing) / 222 byte (tanpa signing). Persingkat secret, atau matikan signing bila tanda tangan tidak diperlukan. |
| "Saat signing dimatikan, secret tidak boleh diawali karakter kontrol U+0001" | Byte 0x01 di awal adalah penanda tanda tangan, dan APK akan salah mengira ada signing key. Hapus karakter itu (biasanya terbawa dari copy-paste). |
| "QR gagal dibuat: ... too big / data bits cannot fit" | Main + secret melebihi kapasitas QR. Turunkan error correction (H → Q → M → L) atau persingkat isinya. |
| Kotak verifikasi merah | Jangan dicetak. Biasanya karena gambar terlalu kecil untuk QR yang rapat; naikkan **Ukuran gambar**, lalu Buat QR lagi. |
| QR sulit terbaca HP setelah dicetak | Cetak minimal ±2 × 2 cm; gunakan EC Q/H; hindari secret yang sangat panjang; pastikan ada margin putih di sekeliling QR. |
