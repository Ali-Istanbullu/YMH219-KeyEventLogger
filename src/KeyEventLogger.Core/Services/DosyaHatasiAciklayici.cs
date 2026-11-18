using KeyEventLogger.Core.Abstractions;
using KeyEventLogger.Core.Errors;

namespace KeyEventLogger.Core.Services;

// Tek sorumlulugu: dosya hatasinin turune bakip kullaniciya NE OLDUGUNU ve NE YAPMASI
// GEREKTIGINI soylemek (S). CK20.
// "Kayıtlar dosyaya yazılamadı." tek basina yetmez: kullanici sebebini bilirse duzeltebilir.
public sealed class DosyaHatasiAciklayici : IDosyaHatasiAciklayici
{
    // Windows hata kodlari (HRESULT). Kaynak: Win32 hata kodlari ERROR_SHARING_VIOLATION (32),
    // ERROR_LOCK_VIOLATION (33), ERROR_DISK_FULL (112), ERROR_HANDLE_DISK_FULL (39); 0x8007xxxx = Win32 kodu.
    private const int PaylasimIhlali = unchecked((int)0x80070020);
    private const int KilitIhlali = unchecked((int)0x80070021);
    private const int DiskDolu = unchecked((int)0x80070070);
    private const int DiskDoluTanitici = unchecked((int)0x80070027);

    public string Acikla(Exception hata)
    {
        ArgumentNullException.ThrowIfNull(hata);

        string? sebep = hata switch
        {
            UnauthorizedAccessException => "bu konuma yazma izniniz yok. Başka bir klasör seçin.",
            DirectoryNotFoundException => "seçilen klasör bulunamadı.",
            PathTooLongException => "dosya yolu çok uzun. Daha kısa bir yol seçin.",
            IOException io when io.HResult is PaylasimIhlali or KilitIhlali
                => "dosya başka bir programda açık. Kapatıp tekrar deneyin.",
            IOException io when io.HResult is DiskDolu or DiskDoluTanitici
                => "diskte yeterli boş yer yok.",
            ArgumentException => "dosya adı geçersiz.",
            NotSupportedException => "dosya adı geçersiz.",
            _ => null
        };

        // Sebebi bilinmiyorsa genel mesaj; ayrinti (hata turu, yigin izi) zaten loga gidiyor.
        string genel = HataMesajlari.DosyaYazilamadi;
        return sebep is null ? genel : $"{genel.TrimEnd('.')}: {sebep}";
    }
}
