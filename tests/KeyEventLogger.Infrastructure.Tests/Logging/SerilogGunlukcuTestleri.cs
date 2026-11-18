using System.Text;
using KeyEventLogger.Infrastructure.FileSystem;
using KeyEventLogger.Infrastructure.Logging;
using KeyEventLogger.Infrastructure.Tests.Fakes;

namespace KeyEventLogger.Infrastructure.Tests.Logging;

[TestClass]
public sealed class SerilogGunlukcuTestleri
{
    [TestMethod]
    public void Bilgi_DosyayaTarihVeSeviyeIleYazilir()
    {
        using var klasor = new GeciciKlasor();
        var yollar = new UygulamaYollari(klasor.Yol);

        using (var gunlukcu = new SerilogGunlukcu(yollar))
        {
            gunlukcu.Bilgi("Kayıt başlatıldı. {süslü}");
        }   // Dispose: dosya kapanir, satirlar diske yazilir

        string icerik = File.ReadAllText(yollar.GunlukDosyasi(), Encoding.UTF8);
        Assert.Contains("[INF] Kayıt başlatıldı. {süslü}", icerik);
    }

    [TestMethod]
    public void Hata_YiginIziyleYazilir()
    {
        using var klasor = new GeciciKlasor();
        var yollar = new UygulamaYollari(klasor.Yol);

        using (var gunlukcu = new SerilogGunlukcu(yollar))
        {
            gunlukcu.Hata(new IOException("disk dolu"), "Kayıtlar dosyaya yazılamadı.");
        }

        string icerik = File.ReadAllText(yollar.GunlukDosyasi(), Encoding.UTF8);
        Assert.Contains("[ERR] Kayıtlar dosyaya yazılamadı.", icerik);
        Assert.Contains("System.IO.IOException: disk dolu", icerik);
    }

    [TestMethod]
    public void YollarAlinamazsa_UygulamaCokmez()
    {
        // Gecersiz kok (yol icinde NUL karakteri): klasor olusturulamaz, ama exception da firlamaz.
        var yollar = new UygulamaYollari("gecersiz\0yol");

        using var gunlukcu = new SerilogGunlukcu(yollar);
        gunlukcu.Bilgi("yazilamayacak");
    }
}
