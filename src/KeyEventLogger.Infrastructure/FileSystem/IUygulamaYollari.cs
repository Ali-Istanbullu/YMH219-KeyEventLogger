namespace KeyEventLogger.Infrastructure.FileSystem;

// Uygulamanin log dosyasinin nerede durdugunu soyleyen sozlesme.
// Neden arayuz? Testlerde gercek AppData yerine gecici bir klasor verebilmek icin (DIP).
public interface IUygulamaYollari
{
    // %LocalAppData%\KeyEventLogger\logs  (yoksa olusturulur)
    string GunlukKlasoru();

    // logs\keyeventlogger.log
    string GunlukDosyasi();
}
