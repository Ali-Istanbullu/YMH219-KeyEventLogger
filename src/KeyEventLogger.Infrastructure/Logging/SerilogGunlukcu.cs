using KeyEventLogger.Core.Abstractions;
using KeyEventLogger.Infrastructure.FileSystem;
using Serilog;
using Serilog.Core;   // Logger tipi

namespace KeyEventLogger.Infrastructure.Logging;

// IGunlukcu'nun Serilog ile calisan uygulamasi (AliBird ile ayni).
// Tek sorumlulugu: Core'dan gelen log isteklerini Serilog'a aktarmak (S).
//
// 1 MB'a ulasan dosya kapanir, yenisi acilir; en fazla 5 dosya tutulur, en eskisi Serilog
// tarafindan OTOMATIK silinir. Her satirda tarih ve saat var.
// CK11: BASILAN TUSLAR LOGA YAZILMAZ (gizli kayit olmasin, PDF guvenlik siniri). Logda sadece
//       uygulama olaylari (acildi, kayit basladi/durdu, dosya kaydedildi) ve hatalar bulunur.
// R6 : Bu sinif ASLA exception firlatmaz. Log yazilamazsa uygulama sessizce devam eder.
public sealed class SerilogGunlukcu : IGunlukcu, IDisposable
{
    private const long DosyaBoyutuSiniri = 1024 * 1024;   // 1 MB
    private const int TutulacakDosyaSayisi = 5;           // toplam ~5 MB

    // "2026-11-18 14:50:03.123 +03:00 [ERR] mesaj" + alt satirda hata ayrintisi.
    private const string SatirBicimi =
        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}";

    private readonly Logger _logger;

    public SerilogGunlukcu(IUygulamaYollari yollar)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(yollar);

            // Serilog'un kendi kurulumu: kutuphane nesnesi adaptorun icinde olusturulur.
            _logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(
                    path: yollar.GunlukDosyasi(),
                    outputTemplate: SatirBicimi,
                    fileSizeLimitBytes: DosyaBoyutuSiniri,
                    rollOnFileSizeLimit: true,
                    retainedFileCountLimit: TutulacakDosyaSayisi)
                .CreateLogger();
        }
        catch
        {
            // Log dosyasi acilamadi (izin yok, disk dolu ...): uygulama durmaz,
            // hicbir yere yazmayan "bos" logger kullanilir (R6).
            _logger = new LoggerConfiguration().CreateLogger();
        }
    }

    public void Bilgi(string mesaj)
    {
        try
        {
            // "{Mesaj:l}": mesaj icinde { } olsa bile Serilog bunu sablon sanmasin, aynen yazsin.
            _logger.Information("{Mesaj:l}", mesaj);
        }
        catch
        {
            // R6: log hatasi uygulamayi asla durdurmaz.
        }
    }

    public void Uyari(string mesaj)
    {
        try
        {
            _logger.Warning("{Mesaj:l}", mesaj);
        }
        catch
        {
            // R6
        }
    }

    public void Hata(Exception hata, string mesaj)
    {
        try
        {
            // Hata nesnesi verilince Serilog yigin izini (stack trace) de alt satira yazar.
            _logger.Error(hata, "{Mesaj:l}", mesaj);
        }
        catch
        {
            // R6
        }
    }

    public void Dispose()
    {
        try
        {
            _logger.Dispose();   // bekleyen satirlari diske yazar, dosyayi kapatir
        }
        catch
        {
            // R6: kapanista bile exception disari cikmaz.
        }
    }
}
