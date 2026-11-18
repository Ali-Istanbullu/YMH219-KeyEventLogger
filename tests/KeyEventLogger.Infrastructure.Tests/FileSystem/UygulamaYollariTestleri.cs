using KeyEventLogger.Infrastructure.FileSystem;
using KeyEventLogger.Infrastructure.Tests.Fakes;

namespace KeyEventLogger.Infrastructure.Tests.FileSystem;

[TestClass]
public sealed class UygulamaYollariTestleri
{
    [TestMethod]
    public void GunlukDosyasi_LogsKlasorundeVeKlasorOlusturulur()
    {
        using var klasor = new GeciciKlasor();
        var yollar = new UygulamaYollari(klasor.Yol);

        string dosya = yollar.GunlukDosyasi();

        Assert.AreEqual(Path.Combine(klasor.Yol, "logs", "keyeventlogger.log"), dosya);
        Assert.IsTrue(Directory.Exists(Path.Combine(klasor.Yol, "logs")));
    }

    [TestMethod]
    public void KlasorSilinirse_YenidenOlusturulur()
    {
        using var klasor = new GeciciKlasor();
        var yollar = new UygulamaYollari(klasor.Yol);
        Directory.Delete(yollar.GunlukKlasoru());

        yollar.GunlukKlasoru();

        Assert.IsTrue(Directory.Exists(Path.Combine(klasor.Yol, "logs")));
    }

    [TestMethod]
    public void VarsayilanKok_LocalAppDataAltinda()
    {
        string beklenen = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "KeyEventLogger");

        Assert.AreEqual(beklenen, UygulamaYollari.VarsayilanKokKlasor());
    }
}
