# YMH219-KeyEventLogger — Kararlar (tek doğru kaynak)

Ödev 2/5: **Key Event Logger (Eğitsel Keylogger)**. Teslim: 30.11.2026 23:59.
Ali İstanbullu — 250542019.

## 1. Ali'nin kalıcı kuralları (AliBird'den aynen)

| # | Kural |
|---|---|
| R1 | SOLID her zaman, özellikle **S** (her sınıfın tek görevi) |
| R2 | DI: `Microsoft.Extensions.DependencyInjection`. Container **sadece** `Program.cs`'te. Kendi sınıflarımızı `new` ile sadece fabrikalar ve kompozisyon kökü oluşturur |
| R3 | **Requirement her projede:** kurulum dosyası küçük olur, .NET runtime kurulum sırasında arka planda indirilip kurulur. Başarısızsa kurulum durur ve hata gösterir |
| R4 | Önemli her yerde try/catch |
| R5 | Teknik ayrıntılara Claude karar verir ve "CK" olarak yazar. Ali'ye sadece önemli sorular sorulur: kullanıcının gördüğü davranış, veri kaybı, kurallar, mimari, yeni paket |
| R6 | Uygulama **asla çökmez** |
| R7 | Her adımın sonunda Claude commit komutunu verir. Commit'leri Ali atar, Claude satırı eklenmez |
| R8 | Kod Türkçe ve ASCII (ı/ş/ğ/ü/ö/ç yok); proje, klasör, namespace ve dosya adları İngilizce. Bol Türkçe `//` yorum. Ekrandaki metinler Türkçe |

## 2. PDF gereksinimleri

| # | Gereksinim |
|---|---|
| G1 | Uygulama penceresi aktifken basılan tuş adı ve zaman ListBox/RichTextBox'ta gösterilir |
| G2 | Başlat ve Durdur butonları kayıt durumunu kontrol eder |
| G3 | Temizle butonu ekrandaki kayıtları siler |
| G4 | Kullanıcı isterse kayıtları .txt dosyasına kaydeder (StreamWriter/File) |
| G5 | Kayıt durumunu belirten açık bir gösterge bulunur |
| G6 | Kavramlar: KeyDown, KeyUp, KeyPress; dosya işlemleri; tarih-saat ve kayıt formatı; form kontrolleri, listeleme, hata yönetimi |
| G7 | **Güvenlik sınırı:** sadece kendi penceresi odaktayken yakalar. Global hook, gizli/arka plan çalışma, başlangıçta otomatik çalışma, parola yakalama, başka uygulamalardan veri, ağ üzerinden gönderme **YOK** |
| G8 | Repo `YMH219-KeyEventLogger` (public), .gitignore, README (amaç, teknolojiler, çalıştırma, ≥2 ekran görüntüsü, sınırlamalar, ad-soyad, no), anlamlı commit geçmişi |
| G9 | İsteğe bağlı: istatistik (toplam + en sık tuşlar), tarih/saat aralığına göre filtreleme |

## 3. Ali'nin kararları

| # | Karar |
|---|---|
| K1 | Liste kontrolü: **ListBox** |
| K2 | Olaylar: **KeyDown + KeyPress + KeyUp** üçü de, her biri ayrı satır |
| K3 | İsteğe bağlıların **ikisi de**: istatistik ve tarih/saat filtresi |
| K4 | Ortak yapı AliBird ile aynı: Core / Infrastructure / WinForms + 2 test projesi, DI, Serilog log (`%LocalAppData%`), Inno Setup + runtime indirme, kodla üretilen ikon, README, LICENSE (MIT), DECISIONS.md |
| K5 | Mimari onaylandı (18.11.2026): MVP (pasif görünüm), presenter Core'da; 3 adımda bitirme |

## 4. Claude kararları (CK)

| # | Karar | Durum |
|---|---|---|
| CK1 | Uygulama adı **KeyEventLogger** (namespace `KeyEventLogger.*`), pencere başlığı "Key Event Logger" | bildirildi |
| CK2 | Olay türleri kodda `OlayTuru.TusBasildi / KarakterYazildi / TusBirakildi`, ekranda dersin terimleriyle `KeyDown / KeyPress / KeyUp` | bildirildi |
| CK3 | Satır biçimi: `gg.aa.yyyy ss:dd:sn.mmm  Olay  Tuş  Ctrl+Shift+Alt`, sabit genişlikli sütunlar. Ekran ve .txt **aynı biçim** | bildirildi |
| CK4 | KeyPress'te yazılan karakter tırnak içinde gösterilir; görünmeyenlere ad verilir: Enter, Backspace, Tab, Esc, Boşluk, diğer kontrol karakterleri `U+XXXX` (ör. Ctrl+C → `U+0003`) | bildirildi |
| CK5 | İstatistik sadece **KeyDown**'ları sayar (bir basış 3 olay üretir, 3 kez sayılmasın). Basılı tutulunca Windows'un gönderdiği tekrarlar da sayılır. En sık **5** tuş, eşitlikte alfabetik | bildirildi |
| CK6 | Filtre: başlangıç ve bitiş **dahil**. Filtre açıkken yeni tuşlar oturuma kaydedilir ama aralık dışındaysa listede görünmez. İstatistik ve Kaydet **ekranda görünenler** üzerinden çalışır | bildirildi |
| CK7 | Başlangıç > bitiş ise filtre **uygulanmaz** (hepsi görünür) ve kullanıcı bir kez uyarılır | bildirildi |
| CK8 | Kaydet: kayıt yoksa "Kaydedilecek kayıt yok." uyarısı verilir. Varsayılan dosya adı `tus_kayitlari_yyyyMMdd_HHmmss.txt`. Dosyanın başında başlık, oluşturulma zamanı, filtre bilgisi ve kayıt sayısı bulunur. Yazma hatası ekrana ve loga gider | bildirildi |
| CK9 | Zaman damgasını presenter `ISaat`'ten koyar (form değil), böylece test edilebilir | bildirildi |
| CK10 | Presenter'daki her form olayı `Guvenli()` içinden geçer: beklenmeyen hata loga ve ekrana gider, uygulama kapanmaz (R6) | bildirildi |
| CK11 | **Basılan tuşlar log dosyasına yazılmaz** (gizli kayıt olmasın, G7). Logda sadece uygulama olayları (açıldı/kapandı, kayıt başladı/durdu, temizlendi, dosya kaydedildi) ve hatalar var | Ali onayladı |
| CK12 | `TusAdiCevirici`: Windows tuş kodları okunur ada çevrilir (Return→Enter, ShiftKey→Shift, ControlKey→Ctrl, Menu→Alt, D5→5 ...). Sözlükte olmayanlar Windows'un kendi adıyla yazılır | bildirildi |
| CK13 | S düzeltmesi: filtre doğrulaması `FiltreDogrulayici`'ye, biçimleyici `SatirBicimleyici` + `DosyaIcerigiOlusturucu`'ya ayrıldı. Ortak tarih biçimi `BicimAyarlari`'nda | Ali onayladı |
| CK14 | Teknoloji: .txt `StreamWriter` + UTF-8 BOM; `SaveFileDialog` (varsayılan klasör Belgeler, üzerine yazmadan önce sorar); ListBox yazı tipi Consolas; filtre = "Filtrele" onay kutusu + 2 tarih-saat seçici (varsayılan bugün 00:00:00 – 23:59:59) | Ali onayladı |
| CK15 | Butonlar odak almaz (`OdakAlmayanButon`): kayıt sırasında Space/Enter yanlışlıkla butona basmasın. Liste odaktayken Tab ve ok tuşları da kaydedilir. Tek başına Alt/F10 pencere menüsüne geçmesin diye engellenir (Alt+Space çalışır) | bildirildi |
| CK16 | Tek kopya kontrolü yok (PDF istemiyor, iki pencere birbirini etkilemiyor) | bildirildi |
| CK17 | Bilgi satırı (pencerenin altında): "Tuşlar sadece bu pencere aktifken ve kayıt açıkken yakalanır. Tuşlar log dosyasına yazılmaz." | bildirildi |
| CK18 | Infrastructure `net10.0-windows` (AliBird ile aynı). Log: `%LocalAppData%\KeyEventLogger\logs\keyeventlogger.log`, 1 MB × 5 dosya | bildirildi |
| CK19 | **Hata kataloğu** `Core/Errors/HataMesajlari.cs`: her hatanın adı tek yerde. Her hata loga yazılır (adı + hata türü + yığın izi). Kural (Ali, 18.11): **kullanıcının bilmesi gerekiyorsa ekran + log, gerekmiyorsa sadece log**. Aşağıdaki hata haritasına bakın | Ali'nin kuralı |
| CK20 | `DosyaHatasiAciklayici`: dosya yazma hatasının sebebi kullanıcıya söylenir (izin yok / klasör yok / dosya başka programda açık / disk dolu / yol çok uzun / ad geçersiz). Bilinmeyen sebepte genel mesaj gösterilir, ayrıntı loga yazılır. Mesajın altında dosya yolu da gösterilir | bildirildi |
| CK21 | **Klavye düzeni:** Windows tuş kodları ABD klavyesine göre adlandırılır (Türkçe Q'da `i` = OemQuotes, `ü` = Oem6). Ali çalıştırırken fark etti. Harf ve Oem tuşlarının adı, aktif klavye düzeninin ürettiği karakterden alınır (`MapVirtualKeyW`, MAPVK_VK_TO_CHAR) ve Türkçe kurala göre büyütülür: i→İ, ı→I, ü→Ü. Karşılığı yoksa Windows'un adı kullanılır | bildirildi |
| CK22 | Basılan tuşun kendisi ek tuşsa (Shift'e basılınca) ek tuş sütununda tekrar yazılmaz | bildirildi |

## 4b. Hata haritası (CK19)

Hepsi loga yazılır. "Ekran" sütunu kullanıcıya mesaj kutusu çıkıp çıkmadığını gösterir. Uygulama hiçbirinde kapanmaz (R6).

| Hata (HataMesajlari) | Ne zaman | Ekran | Neden |
|---|---|---|---|
| KayitBaslatilamadi / KayitDurdurulamadi / KayitlarTemizlenemedi | Butona basıldı, işlem olmadı | ✅ | Kullanıcı sonucu bekliyor |
| KaydetmePenceresiAcilamadi | Kaydet penceresi açılamadı | ✅ | Kayıt kaydedilemedi, bilmeli |
| DosyaYazilamadi (+ sebep + yol) | Dosya yazılamadı | ✅ | Veri kaybı: bilmeli ve düzeltebilmeli |
| KaydetmeBasarisiz | Kaydet sırasında beklenmeyen hata | ✅ | Kayıt kaydedilemedi |
| FiltreUygulanamadi | Filtre değişti, uygulanamadı | ✅ | Liste beklediği gibi değil |
| UygulamaHazirlanamadi / UygulamaBaslatilamadi | Açılışta | ✅ | Uygulama kullanılamaz |
| TusOlayiIslenemedi / TusOlayiIletilemedi (+ olay, tuş) | Tuş olayı işlenirken | ❌ | Saniyede onlarca gelir, mesaj kutusu yağmuru olmasın |
| ButonOlayiIletilemedi (+ buton adı) | Form → presenter aktarımı | ❌ | İkinci emniyet; presenter zaten bildirir |
| PencereIkonuYuklenemedi | Açılışta | ❌ | Uygulama ikonsuz da çalışır |
| HataMesajiGosterilemedi | Mesaj kutusu açılamadı | ❌ | Gösterilemiyor zaten |
| YakalanmamisHata | Gözden kaçan hata (son savunma hattı) | ilk kez ✅, sonra ❌ | Bir kez haber ver, yağmur olmasın |
| (log dosyası açılamadı) | Serilog açılamadı | ❌ | Yazacak yer yok; uygulama logsuz devam eder |

## 5. Mimari

```
src/
  KeyEventLogger.Core            net10.0  Models, Abstractions, Services, Presentation (KayitYoneticisi)
  KeyEventLogger.Infrastructure  net10.0-windows  Serilog, .txt yazıcı (StreamWriter), saat, yollar   (Adım 2)
  KeyEventLogger.WinForms        net10.0-windows  AnaForm, Kaydet penceresi, Program.cs (DI)         (Adım 2)
  KeyEventLogger.IconGenerator   net10.0-windows  ikon (PNG + ICO)                                    (Adım 3)
tests/
  KeyEventLogger.Core.Tests            MSTest 4.0.2 + sahte sınıflar
  KeyEventLogger.Infrastructure.Tests  (Adım 2)
installer/KeyEventLogger.iss           (Adım 3)
```

## 6. Plan

| Adım | İş | Durum |
|---|---|---|
| 1 | İskelet + Core (modeller, servisler, presenter) + Core testleri | ✅ commit 86dbfa6 |
| 2 | S düzeltmesi (CK13) + Infrastructure + WinForms + DI + testler | ⏳ yazıldı + hata kataloğu (CK19, CK20), build/test bekleniyor (Core 42 + Infra 15 = 57 test) |
| 3 | İkon + Inno Setup + README + LICENSE + ekran görüntüleri + Release | |

## 7. Claude'un hataları (tekrarlanmayacak)

AliBird'deki liste geçerli: test sayısı dosyalardan sayılır, paket API'si kurulu sürümden doğrulanır, metin değişince testlerde de aranır, "tek yer" gibi iddialar taramayla doğrulanır.
