# 🏢 Room Booking API

Backend API untuk sistem peminjaman ruangan berbasis ASP.NET Core dengan PostgreSQL.

## 📋 Deskripsi

Aplikasi ini adalah REST API untuk mengelola peminjaman ruangan di institusi pendidikan. Sistem ini mendukung tiga peran pengguna (Mahasiswa, Staf, Admin) dengan fitur autentikasi JWT, manajemen ruangan, dan alur persetujuan untuk peminjaman.

## ✨ Fitur Utama

- 🔐 **Autentikasi & Otorisasi** - Autentikasi JWT Bearer dengan kontrol akses berbasis peran
- 👥 **Sistem Multi-Peran** - Mahasiswa, Staf, dan Admin dengan hak akses berbeda
- 🏛️ **Manajemen Ruangan** - Operasi CRUD untuk data ruangan
- 📅 **Sistem Peminjaman** - Pembuatan peminjaman dengan validasi tumpang tindih dan alur persetujuan
- ✅ **Alur Persetujuan** - Admin/Staf dapat menyetujui/menolak peminjaman
- 🔍 **Pencarian & Filter** - Pencarian ruangan, penyaringan, dan paginasi
- 📊 **Pelacakan Status** - Pelacakan status peminjaman (Menunggu, Disetujui, Ditolak, Dibatalkan)

## 🛠️ Teknologi yang Digunakan

- **.NET 10.0** - Framework utama
- **ASP.NET Core Web API** - Framework REST API
- **Entity Framework Core 9.0** - ORM (Object-Relational Mapping)
- **PostgreSQL** - Database
- **JWT Bearer** - Autentikasi
- **BCrypt** - Hashing password
- **Swagger/OpenAPI** - Dokumentasi API

## 📦 Dependensi

```xml
<PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="10.0.3" />
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.2" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.1" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="9.0.2" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.8.1" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.3.1" />
```

## 🚀 Memulai (Getting Started)

### Prasyarat

- .NET 10 SDK atau .NET 9 SDK
- PostgreSQL 12+
- Git

### Instalasi

1. **Clone repository**

```bash
git clone https://github.com/Fairanova/2026-room-booking-backend.git
cd 2026-room-booking-backend
```

2. **Setup Database**

Buat database PostgreSQL:

```sql
CREATE DATABASE room_booking_db;
```

3. **Konfigurasi Connection String**

Update `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=room_booking_db;Username=postgres;Password=password_anda"
  },
  "Jwt": {
    "Key": "kunci-rahasia-anda-minimal-32-karakter",
    "Issuer": "RoomBookingApi",
    "Audience": "RoomBookingClient",
    "ExpiryInMinutes": "1440"
  }
}
```

4. **Jalankan Migrasi**

```bash
dotnet ef database update
```

5. **Jalankan Aplikasi**

```bash
dotnet run
```

API akan berjalan di `http://localhost:5001`

### Data Awal (Seeded Data)

Aplikasi akan otomatis mengisi data berikut saat pertama kali dijalankan:

**Pengguna (Users):**

- Admin: `username: admin`, `password: Admin123`
- Staf: `username: staff001`, `password: Staff123`
- Mahasiswa: `username: student001`, `password: Student123`

**Ruangan (Rooms):**

- 3 ruangan contoh (Lab Komputer, Ruang Meeting, Auditorium)

## 📖 Dokumentasi API

### URL Dasar

```
http://localhost:5001/api
```

### Autentikasi

Semua endpoint yang dilindungi memerlukan token JWT di header:

```
Authorization: Bearer {token-jwt-anda}
```

---

## 🔐 Endpoint Autentikasi

### 1. Registrasi

**POST** `/api/auth/register`

**Body Request:**

```json
{
  "username": "budi_santoso",
  "email": "budi@example.com",
  "password": "PasswordAman123!",
  "fullName": "Budi Santoso",
  "role": 1
}
```

**Nilai Peran (Role):**

- `1` = Mahasiswa (Student)
- `2` = Staf (Staff)
- `3` = Admin

**Response:** `201 Created`

```json
{
  "id": "uuid",
  "username": "budi_santoso",
  "email": "budi@example.com",
  "fullName": "Budi Santoso",
  "role": 1
}
```

---

### 2. Login

**POST** `/api/auth/login`

**Body Request:**

```json
{
  "username": "admin",
  "password": "Admin123"
}
```

**Response:** `200 OK`

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "uuid",
    "username": "admin",
    "email": "admin@example.com",
    "fullName": "Administrator",
    "role": 3
  }
}
```

---

### 3. Lihat Profil

**GET** `/api/auth/profile`

**Headers:** `Authorization: Bearer {token}`

**Response:** `200 OK`

```json
{
  "id": "uuid",
  "username": "admin",
  "email": "admin@example.com",
  "fullName": "Administrator",
  "role": 3
}
```

---

## 🏛️ Endpoint Ruangan (Rooms)

### 1. Lihat Semua Ruangan (Publik)

**GET** `/api/rooms?page=1&pageSize=10&search=Lab&location=Gedung A`

**Parameter Query:**

- `page` (opsional, default: 1)
- `pageSize` (opsional, default: 10, maks: 50)
- `search` (opsional) - Cari berdasarkan nama atau deskripsi
- `location` (opsional) - Filter berdasarkan lokasi

**Response:** `200 OK`

```json
{
  "data": [
    {
      "id": "uuid",
      "name": "Lab Komputer 1",
      "location": "Gedung A Lt. 2",
      "capacity": 40,
      "description": "Lab dengan 40 komputer",
      "amenities": "Proyektor, AC, WiFi",
      "isAvailable": true
    }
  ],
  "page": 1,
  "pageSize": 10,
  "totalCount": 3,
  "totalPages": 1
}
```

---

### 2. Lihat Ruangan berdasarkan ID (Publik)

**GET** `/api/rooms/{id}`

**Response:** `200 OK`

---

### 3. Cek Ketersediaan Ruangan (Publik)

**GET** `/api/rooms/available?date=2026-02-20&startTime=09:00&endTime=11:00`

**Parameter Query:**

- `date` (wajib, format: YYYY-MM-DD)
- `startTime` (wajib, format: HH:mm)
- `endTime` (wajib, format: HH:mm)

**Response:** `200 OK`

```json
[
  {
    "id": "uuid",
    "name": "Ruang Meeting A",
    "location": "Gedung B Lt. 3",
    "capacity": 20
  }
]
```

---

### 4. Tambah Ruangan (Khusus Admin)

**POST** `/api/rooms`

**Headers:** `Authorization: Bearer {token-admin}`

**Body Request:**

```json
{
  "name": "Lab Komputer 2",
  "location": "Gedung A Lt. 3",
  "capacity": 30,
  "description": "Lab pemrograman",
  "amenities": "Proyektor, AC, WiFi"
}
```

**Response:** `201 Created`

---

### 5. Update Ruangan (Khusus Admin)

**PUT** `/api/rooms/{id}`

**Headers:** `Authorization: Bearer {token-admin}`

---

### 6. Hapus Ruangan (Khusus Admin)

**DELETE** `/api/rooms/{id}`

**Headers:** `Authorization: Bearer {token-admin}`

**Response:** `204 No Content`

---

## 📅 Endpoint Peminjaman (Bookings)

### 1. Lihat Peminjaman

**GET** `/api/bookings?page=1&pageSize=10&status=1`

**Headers:** `Authorization: Bearer {token}`

**Parameter Query:**

- `page` (opsional, default: 1)
- `pageSize` (opsional, default: 10)
- `status` (opsional) - Filter berdasarkan status

**Kontrol Akses:**

- **Mahasiswa**: Hanya melihat booking milik sendiri
- **Admin/Staf**: Melihat semua booking

**Response:** `200 OK`

```json
{
  "data": [
    {
      "id": "uuid",
      "bookingDate": "2026-02-20",
      "startTime": "09:00:00",
      "endTime": "11:00:00",
      "purpose": "Diskusi Kelompok",
      "description": "Diskusi tugas akhir",
      "status": 1,
      "rejectionReason": null,
      "createdAt": "2026-02-16T10:00:00Z",
      "room": {
        "id": "uuid",
        "name": "Lab Komputer 1",
        "location": "Gedung A Lt. 2"
      },
      "user": {
        "id": "uuid",
        "username": "student001",
        "fullName": "Student One"
      }
    }
  ],
  "page": 1,
  "pageSize": 10,
  "totalCount": 5,
  "totalPages": 1
}
```

---

### 2. Buat Peminjaman

**POST** `/api/bookings`

**Headers:** `Authorization: Bearer {token}`

**Body Request:**

```json
{
  "roomId": "uuid",
  "bookingDate": "2026-02-20",
  "startTime": "09:00",
  "endTime": "11:00",
  "purpose": "Diskusi Kelompok",
  "description": "Diskusi tugas akhir"
}
```

**Validasi:**

- Tanggal booking harus hari ini atau di masa depan
- Waktu mulai harus sebelum waktu selesai
- Durasi minimal 30 menit
- Tidak boleh bentrok dengan booking lain yang sudah disetujui

**Response:** `201 Created`

---

### 3. Update Status Peminjaman (Khusus Admin/Staf)

**PUT** `/api/bookings/{id}/status`

**Headers:** `Authorization: Bearer {token-admin-atau-staf}`

**Body Request (Setujui):**

```json
{
  "status": 2
}
```

**Body Request (Tolak):**

```json
{
  "status": 3,
  "rejectionReason": "Waktu bentrok dengan kegiatan lain"
}
```

**Nilai Status:**

- `2` = Disetujui (Approved)
- `3` = Ditolak (Rejected) - wajib sertakan rejectionReason

**Response:** `200 OK`

---

### 4. Batalkan Peminjaman

**DELETE** `/api/bookings/{id}`

**Headers:** `Authorization: Bearer {token}`

**Kontrol Akses:**

- **Mahasiswa**: Hanya bisa membatalkan booking sendiri yang statusnya Masih Menunggu (Pending)
- **Admin/Staf**: Bisa membatalkan booking apa saja

**Response:** `204 No Content`

---

## 📊 Alur Status Peminjaman

```
Pengguna membuat booking
        ↓
   [1: MENUNGGU] ━━━━━━━━━━━━━━━┐
        ↓                       ↓
  Admin/Staf Review      Pengguna Batal
        ↓                       ↓
   ┌────┴────┐          [4: DIBATALKAN]
   ↓         ↓
[2: DISETUJUI] [3: DITOLAK]
```

**Kode Status:**

1. **Pending** - Menunggu persetujuan
2. **Approved** - Disetujui oleh Admin/Staf
3. **Rejected** - Ditolak (dengan alasan penolakan)
4. **Cancelled** - Dibatalkan oleh pengguna

---

## 🧪 Pengujian dengan Swagger

1. Buka browser ke `http://localhost:5001/swagger`
2. Login via endpoint `POST /api/auth/login`
3. Salin token dari response
4. Klik tombol **"Authorize"** (🔒 di pojok kanan atas)
5. Masukkan: `Bearer {token-anda}`
6. Klik **Authorize** → **Close**
7. Uji semua endpoint!

---

## 🗄️ Skema Database

### Tabel Users

```
- Id (UUID, PK)
- Username (string, unik)
- Email (string, unik)
- PasswordHash (string)
- FullName (string)
- Role (int: 1=Mahasiswa, 2=Staf, 3=Admin)
- CreatedAt (datetime)
- UpdatedAt (datetime)
```

### Tabel Rooms

```
- Id (UUID, PK)
- Name (string)
- Location (string)
- Capacity (int)
- Description (string, nullable)
- Amenities (string, nullable)
- IsAvailable (bool)
- CreatedAt (datetime)
- UpdatedAt (datetime)
```

### Tabel Bookings

```
- Id (UUID, PK)
- UserId (UUID, FK)
- RoomId (UUID, FK)
- BookingDate (date)
- StartTime (time)
- EndTime (time)
- Purpose (string)
- Description (string, nullable)
- Status (int: 1-4)
- RejectionReason (string, nullable)
- CreatedAt (datetime)
- UpdatedAt (datetime)
```

---

## 🔒 Keamanan

- **Hashing Password**: BCrypt dengan work factor 12
- **Kedaluwarsa JWT**: Default 24 jam (dapat dikonfigurasi)
- **Akses Berbasis Peran**: Perlindungan endpoint menggunakan `[Authorize(Roles = "...")]`
- **Validasi Input**: Data Annotations di semua DTO

---

## 📝 Lisensi

Lisensi MIT - silakan gunakan untuk tujuan pendidikan.

---

## 👤 Pembuat

Dikembangkan oleh **Faira** - [GitHub](https://github.com/Fairanova)

---

## 🙏 Ucapan Terima Kasih

- Dokumentasi ASP.NET Core
- Dokumentasi Entity Framework Core
- JWT.io untuk debugging JWT
