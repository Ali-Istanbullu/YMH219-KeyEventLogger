using KeyEventLogger.Core.Services;

namespace KeyEventLogger.Core.Tests.Services;

// Dosya hatasinin sebebi kullaniciya anlasilir dille soylenmeli.
[TestClass]
public sealed class DosyaHatasiAciklayiciTestleri
{
    private readonly DosyaHatasiAciklayici _aciklayici = new();

    [TestMethod]
    public void YetkiYok()
    {
        Assert.AreEqual("Kayıtlar dosyaya yazılamadı: bu konuma yazma izniniz yok. Başka bir klasör seçin.",
            _aciklayici.Acikla(new UnauthorizedAccessException()));
    }

    [TestMethod]
    public void KlasorYok()
    {
        Assert.AreEqual("Kayıtlar dosyaya yazılamadı: seçilen klasör bulunamadı.",
            _aciklayici.Acikla(new DirectoryNotFoundException()));
    }

    [TestMethod]
    public void DosyaBaskaProgramdaAcik()
    {
        var hata = new IOException("kilitli", unchecked((int)0x80070020));   // ERROR_SHARING_VIOLATION

        Assert.AreEqual("Kayıtlar dosyaya yazılamadı: dosya başka bir programda açık. Kapatıp tekrar deneyin.",
            _aciklayici.Acikla(hata));
    }

    [TestMethod]
    public void DiskDolu()
    {
        var hata = new IOException("dolu", unchecked((int)0x80070070));      // ERROR_DISK_FULL

        Assert.AreEqual("Kayıtlar dosyaya yazılamadı: diskte yeterli boş yer yok.", _aciklayici.Acikla(hata));
    }

    [TestMethod]
    public void BilinmeyenHata_GenelMesaj()
    {
        Assert.AreEqual("Kayıtlar dosyaya yazılamadı.", _aciklayici.Acikla(new IOException("bilinmeyen")));
    }
}
