<div align="center">

# 📬 MAB Mailer

**Bulk & personalized email sending desktop application**
**Toplu ve kişiselleştirilmiş e-posta gönderim uygulaması**

[![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.7.2-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/download/dotnet-framework/net472)
[![WPF](https://img.shields.io/badge/WPF-Desktop_UI-6C3483?style=for-the-badge&logo=windows&logoColor=white)]()
[![SQLite](https://img.shields.io/badge/SQLite-Local_DB-003B57?style=for-the-badge&logo=sqlite&logoColor=white)]()
[![License](https://img.shields.io/badge/License-MIT-27AE60?style=for-the-badge)](LICENSE)
[![Platform](https://img.shields.io/badge/Platform-Windows_x64-0078D6?style=for-the-badge&logo=windows&logoColor=white)]()

</div>

---

## 📸 Screenshots / Ekran Görüntüleri

<div align="center">

| Login | Main — Dark | Main — Light |
|:---:|:---:|:---:|
| ![Login](screenshots/login.png) | ![Main Dark](screenshots/main.png) | ![Main Light](screenshots/mainLight.png) |

</div>

---

## ✨ Features / Özellikler

| | English | Türkçe |
|---|---|---|
| 📋 | **Bulk import from Excel** — Generate template, fill, import | **Excel'den toplu aktarım** — Şablon oluştur, doldur, yükle |
| 👥 | **Group management** — Organize contacts, filter & send | **Grup yönetimi** — Kişileri grupla, filtrele, gönder |
| ✉️ | **Personalized emails** — Use `{Name}`, `{Surname}`, `{Email}` and dynamic columns | **Kişiselleştirilmiş mail** — `{Ad}`, `{Soyad}`, `{Email}` ve dinamik sütunlar |
| 📝 | **Message templates** — Save & reuse frequent messages | **Mesaj şablonları** — Sık mesajları kaydet ve tekrar kullan |
| 🔗 | **HTML links** — Insert clickable links into email body | **HTML link** — Mail içeriğine tıklanabilir link ekle |
| ✒️ | **Signature support** — Auto-append email signature | **İmza desteği** — Otomatik imza ekleme |
| 📮 | **Queue system** — Queue drafts, bulk send, track status | **Kuyruk sistemi** — Kuyruğa ekle, toplu gönder, takip et |
| 🔄 | **Multi-account** — Store multiple SMTP accounts, switch easily | **Çoklu hesap** — Birden fazla hesap kaydet, geçiş yap |
| 🌗 | **Auto theme** — Follows Windows Light/Dark theme | **Otomatik tema** — Windows Açık/Koyu temasına uyum |
| 🔀 | **Spoofing** — Custom sender address (if SMTP allows) | **Spoofing** — Gönderen adresini özelleştir (SMTP izinliyse) |

---

## 🛠️ Requirements / Gereksinimler

| | |
|---|---|
| **OS** | Windows 10 / 11 (x64) |
| **Runtime** | [.NET Framework 4.7.2](https://dotnet.microsoft.com/download/dotnet-framework/net472) |
| **IDE** *(dev only)* | Visual Studio 2022+ |

---

## 🚀 Getting Started / Başlangıç

### 📥 End User / Kullanıcı

> Download the latest installer from [**Releases**](../../releases) and run it.
> The app creates its database automatically on first launch.

### 👨‍💻 Developer / Geliştirici

```bash
git clone https://github.com/KULLANICI_ADIN/MAB-Mailer.git
cd MAB-Mailer
```

1. Open `MAB Mailer.slnx` in **Visual Studio**
2. Wait for **NuGet Restore** to complete
3. **Build → Start** ▶️

---

## 📦 Tech Stack / Teknolojiler

| Technology | Purpose |
|---|---|
| **WPF** | Desktop UI framework |
| **Microsoft.Data.Sqlite** | Local database (SQLite) |
| **ClosedXML** | Excel read/write |
| **Newtonsoft.Json** | JSON serialization |
| **Inno Setup** | Windows installer packaging |

---

## 📁 Project Structure / Proje Yapısı

```
MAB Mailer/
│
├── App.xaml/.cs                    # Entry point, theme loading
├── LoginWindow.xaml/.cs            # Login screen, account management
├── MainWindow.xaml/.cs             # Main screen, mail composition
│
├── DatabaseService.cs              # SQLite CRUD operations
├── MailService.cs                  # SMTP email sending
├── ExcelService.cs                 # Excel template & import
├── TemplateEngine.cs               # Variable parsing ({Name}, {Surname}...)
├── ThemeService.cs                 # Windows theme detection
├── GlobalSettings.cs               # Runtime session settings
│
├── CustomAlert.xaml/.cs            # Custom alert/confirm dialog
├── LinkWindow.xaml/.cs             # HTML link insertion dialog
├── TemplateNameWindow.xaml/.cs     # Template save dialog
├── InputBox.cs                     # Numeric input dialog
│
├── EmailAccount.cs                 # Account model
├── Recipient.cs                    # Recipient model
├── RecipientGroup.cs               # Group model
├── MailDraft.cs                    # Queue item model
└── MessageTemplate.cs              # Message template model
```

---

## 🤝 Contributing / Katkıda Bulunma

Contributions are welcome! Feel free to open an **Issue** or submit a **Pull Request**.

Katkılarınızı bekliyoruz! **Issue** açabilir veya **Pull Request** gönderebilirsiniz.

---

## 📄 License / Lisans

This project is licensed under the [MIT License](LICENSE).

Bu proje [MIT Lisansı](LICENSE) ile lisanslanmıştır.
