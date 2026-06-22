#  Medipanel - Hastane Otomasyon Sistemi

Medipanel, modern hastane süreçlerini (hasta yönetimi, doktor takip, randevu planlama, tıbbi dosya arşivleme ve istatistik takibi) dijitalleştirmek ve kolaylaştırmak amacıyla geliştirilmiş, şık arayüze sahip ASP.NET Core MVC tabanlı bir hastane yönetim sistemidir.

---

##  Özellikler

*   ** Yönetici Paneli (Dashboard):** 
    *   Toplam hasta, aktif doktor ve bugünkü randevu sayıları.
    *   **Chart.js** entegrasyonu ile son 7 günün randevu grafiği.
    *   En son eklenen/güncellenen randevuların listesi.
*   ** Hasta Yönetimi:**
    *   Hasta kaydı ekleme, güncelleme ve silme.
    *   Hasta listesinde gelişmiş arama ve filtreleme.
    *   Tıbbi dosya yönetimi (pdf yükleme, listeleme, pdf önizleme/indirme).
*   ** Doktor Yönetimi:**
    *   Doktor ekleme, branş, unvan, iletişim ve çalışma durumu yönetimi.
    *   Doktor randevularına hızlı filtreleme.
*   ** Randevu Yönetimi:**
    *   Randevu oluşturma, durum güncelleme (Onaylandı, Beklemede, Tamamlandı, İptal).
    *   Randevuya ait detaylı notlar ekleyebilme.
*   ** Gelişmiş Bildirim Sistemi:**
    *   Navbar üzerinde bekleyen randevuların anlık sayısını gösteren bildirim zili ve dropdown menüsü.
    *   İşlemler sonrasında dinamik olarak beliren ve otomatik kaybolan **Toast** uyarı mesajları.
*   ** Güvenli Giriş Sistemi (Auth):**
    *   Session tabanlı kimlik doğrulama.
    *   Yetkisiz erişim denemelerinde otomatik yönlendirme (`SessionCheck` filtresi).

---

##  Teknolojiler & Kütüphaneler

*   **Backend:** ASP.NET Core 9.0 / .NET 9.0 (MVC mimarisi)
*   **Veritabanı:** SQLite & Entity Framework Core (Code-First)
*   **Frontend & Tasarım:** HTML5, CSS3, Tailwind CSS (CDN), FontAwesome (İkonlar)
*   **Grafik & İstatistik:** Chart.js
*   **Oturum Yönetimi:** ASP.NET Core Session (`AddDistributedMemoryCache`)

---

##  Başlangıç & Kurulum

Projeyi yerel bilgisayarınızda çalıştırmak için aşağıdaki adımları izleyin:

### 1. Gereksinimler
*   [.NET SDK 9.0](https://dotnet.microsoft.com/download/dotnet/9.0) veya üzeri.
*   Tercihen VS Code veya Visual Studio 2022.

### 2. Projeyi Çalıştırma
Proje ana dizinindeyken terminalde veya Powershell üzerinde aşağıdaki komutu çalıştırın:

```bash
dotnet run
```

Uygulama varsayılan olarak şu adreste çalışmaya başlayacaktır:
 **`http://localhost:5036`** (veya `https://localhost:7072`)

### 3. Yönetici Giriş Bilgileri
Sisteme giriş yapmak için `appsettings.json` dosyasında tanımlı olan varsayılan kimlik bilgilerini kullanabilirsiniz:

*   **E-posta:** `admin@medipanel.com`
*   **Şifre:** `123456`

---

##  Proje Klasör Yapısı

*   `Controllers/` : İş mantığının ve yönlendirmelerin yapıldığı sınıflar (Patients, Doctors, Appointments, Auth vb.).
*   `Models/` : Veritabanı tablolarını ve ViewModellerini temsil eden sınıflar.
*   `Views/` : Kullanıcı arayüz tasarımları (Razor Pages).
*   `Services/` : PDF servisleri vb. yardımcı servisler.
*   `wwwroot/` : Statik dosyalar (CSS, JS, resimler).
*   `hospital.db` : SQLite veritabanı dosyası.
