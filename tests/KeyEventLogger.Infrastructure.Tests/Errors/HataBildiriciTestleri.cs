using KeyEventLogger.Core.Models;
using KeyEventLogger.Infrastructure.Errors;
using KeyEventLogger.Infrastructure.Tests.Fakes;

namespace KeyEventLogger.Infrastructure.Tests.Errors;

[TestClass]
public sealed class HataBildiriciTestleri
{
    private readonly SahteGunlukcu _gunlukcu = new();
    private readonly SahteKullaniciBildirici _kullanici = new();

    private HataBildirici Bildirici() => new(_gunlukcu, _kullanici);

    [TestMethod]
    public void SadeceGunluk_LogaYazarEkranaGostermez()
    {
        Bildirici().Bildir(new IOException("x"), Gorunurluk.SadeceGunluk, "Mesaj");

        Assert.HasCount(1, _gunlukcu.Kayitlar);
        Assert.IsEmpty(_kullanici.Mesajlar);
    }

    [TestMethod]
    public void EkranVeGunluk_IkisineDeGider()
    {
        Bildirici().Bildir(new IOException("x"), Gorunurluk.EkranVeGunluk, "Kayıtlar dosyaya yazılamadı.");

        Assert.AreEqual("ERR Kayıtlar dosyaya yazılamadı.", _gunlukcu.Kayitlar[0]);
        Assert.AreEqual("Kayıtlar dosyaya yazılamadı.", _kullanici.Mesajlar[0]);
    }

    [TestMethod]
    public void BosMesaj_VarsayilanMetinGosterilir()
    {
        Bildirici().Bildir(new IOException("x"), Gorunurluk.EkranVeGunluk, " ");

        Assert.AreEqual("Beklenmeyen bir hata oluştu. Uygulama çalışmaya devam ediyor.", _kullanici.Mesajlar[0]);
    }

    [TestMethod]
    public void LogVeMesajKutusuHataVerse_DisariSizmaz()
    {
        _gunlukcu.HataFirlatsin = true;
        _kullanici.HataFirlatsin = true;

        Bildirici().Bildir(new IOException("x"), Gorunurluk.EkranVeGunluk, "Mesaj");   // R6: exception yok
    }

    [TestMethod]
    public void MesajKutusuHatasi_LogaYazilir()
    {
        _kullanici.HataFirlatsin = true;

        Bildirici().Bildir(new IOException("x"), Gorunurluk.EkranVeGunluk, "Mesaj");

        Assert.AreEqual("ERR Kullanıcıya hata mesajı gösterilemedi.", _gunlukcu.Kayitlar[1]);
    }
}
