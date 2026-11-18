using System.Text;
using KeyEventLogger.Core.Abstractions;

namespace KeyEventLogger.Infrastructure.Files;

// IDosyaYazici'nin gercek uygulamasi: StreamWriter ile metin dosyasi yazar (PDF: StreamWriter/File).
// Tek sorumlulugu: verilen metni verilen yola yazmak (S).
//
// - Dosya varsa USTUNE yazilir (SaveFileDialog zaten "uzerine yazilsin mi?" diye sorar).
// - UTF-8 BOM'lu: Turkce karakterler (ı, ş, ğ) Notepad'de dogru gorunur (teknoloji karari, Ali onayladi).
// - Hata (klasor yok, yetki yok, dosya baska programda acik) YAKALANMAZ: cagiran presenter
//   yakalar ve kullaniciya "Kayıtlar dosyaya yazılamadı." der (CK8). Burada yutulsaydi
//   kullanici dosyanin kaydedildigini sanardi (veri kaybi).
public sealed class MetinDosyasiYazici : IDosyaYazici
{
    // true = dosyanin basina BOM (EF BB BF) yaz.
    private static readonly Encoding Utf8Bom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);

    public void Yaz(string yol, string icerik)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(yol);
        ArgumentNullException.ThrowIfNull(icerik);

        // using: is bitince (hata olsa bile) dosya kapatilir, kilitli kalmaz.
        using var yazici = new StreamWriter(yol, append: false, Utf8Bom);
        yazici.Write(icerik);
    }
}
