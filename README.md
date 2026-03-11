# 📬 MAB Mailer

Toplu ve kişiselleştirilmiş e-posta gönderimi için geliştirilmiş masaüstü uygulaması.

![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.7.2-blue)
![WPF](https://img.shields.io/badge/UI-WPF-blueviolet)
![License](https://img.shields.io/badge/License-MIT-green)
![Platform](https://img.shields.io/badge/Platform-Windows%20x64-lightgrey)

---

## ✨ Özellikler

- 📋 **Excel'den toplu kişi aktarımı** — Hazır şablon oluştur, doldur, sisteme yükle
- 👥 **Grup yönetimi** — Kişileri gruplara ayır, filtreleyerek gönderim yap
- ✉️ **Kişiselleştirilmiş mail** — `{Ad}`, `{Soyad}`, `{Email}` ve dinamik Excel sütunlarıyla her maili özelleştir
- 📝 **Hazır mesaj şablonları** — Sık kullandığın mesajları kaydet ve tekrar kullan
- 🔗 **HTML link ekleme** — Mail içeriğine tıklanabilir linkler ekle
- ✒️ **İmza desteği** — Otomatik imza ekleme
- 📮 **Kuyruk sistemi** — Mailleri kuyruğa ekle, topluca gönder, durumları takip et
- 🔄 **Çoklu hesap** — Birden fazla e-posta hesabı kaydet ve aralarında geçiş yap
- 🌗 **Otomatik tema** — Windows sistem temasına (Açık/Koyu) otomatik uyum
- 🔀 **Spoofing desteği** — Gönderen adresini özelleştirme (SMTP izin veriyorsa)

---

## 📸 Ekran Görüntüleri

> Ekran görüntülerini `screenshots/` klasörüne ekleyip aşağıdaki satırları açabilirsiniz:

<!--
![Login](screenshots/login.png)
![Main](screenshots/main.png)
-->

---

## 🛠️ Gereksinimler

- **İşletim Sistemi:** Windows 10/11 (x64)
- **Runtime:** [.NET Framework 4.7.2](https://dotnet.microsoft.com/download/dotnet-framework/net472)
- **IDE (geliştirme için):** Visual Studio 2022+

---

## 🚀 Kurulum ve Çalıştırma

### Kullanıcı olarak
1. [Releases](../../releases) sayfasından en güncel `.exe` veya Setup dosyasını indir
2. Çalıştır — uygulama ilk açılışta veritabanını otomatik oluşturur

### Geliştirici olarak
```bash
git clone https://github.com/KULLANICI_ADIN/MAB-Mailer.git
cd MAB-Mailer
```
1. `MAB Mailer.slnx` dosyasını Visual Studio ile aç
2. NuGet paketlerinin geri yüklenmesini bekle (otomatik)
3. **Build → Start** ile çalıştır

---

## 📦 Kullanılan Teknolojiler

| Paket | Açıklama |
|---|---|
| **WPF** | Masaüstü arayüz framework'ü |
| **SQLite** (Microsoft.Data.Sqlite) | Yerel veritabanı |
| **ClosedXML** | Excel okuma/yazma |
| **Newtonsoft.Json** | JSON serileştirme |
| **Inno Setup** | Windows installer oluşturma |

---

## 📁 Proje Yapısı

```
MAB Mailer/
├── App.xaml / App.xaml.cs          # Uygulama giriş noktası, tema yükleme
├── LoginWindow.xaml/.cs            # Giriş ekranı, hesap yönetimi
├── MainWindow.xaml/.cs             # Ana ekran, mail kompozisyon
├── DatabaseService.cs              # SQLite CRUD işlemleri
├── MailService.cs                  # SMTP ile mail gönderimi
├── ExcelService.cs                 # Excel şablon oluşturma ve okuma
├── TemplateEngine.cs               # {Ad}, {Soyad} gibi değişken çözümleme
├── ThemeService.cs                 # Windows tema algılama
├── GlobalSettings.cs               # Oturum bilgileri (runtime)
├── CustomAlert.xaml/.cs            # Özel uyarı/onay penceresi
├── LinkWindow.xaml/.cs             # HTML link ekleme penceresi
├── TemplateNameWindow.xaml/.cs     # Şablon kaydetme penceresi
├── EmailAccount.cs                 # Hesap modeli
├── Recipient.cs                    # Kişi modeli
├── RecipientGroup.cs               # Grup modeli
├── MailDraft.cs                    # Kuyruk öğesi modeli
├── MessageTemplate.cs              # Mesaj şablonu modeli
└── InputBox.cs                     # Sayı girdi penceresi
```

---

## 📄 Lisans

Bu proje [MIT Lisansı](LICENSE) ile lisanslanmıştır. Detaylar için `LICENSE` dosyasına bakın.
