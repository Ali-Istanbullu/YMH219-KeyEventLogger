using System.Text;
using KeyEventLogger.Infrastructure.Files;
using KeyEventLogger.Infrastructure.Tests.Fakes;

namespace KeyEventLogger.Infrastructure.Tests.Files;

[TestClass]
public sealed class MetinDosyasiYaziciTestleri
{
    private readonly MetinDosyasiYazici _yazici = new();

    [TestMethod]
    public void Yaz_IcerikVeTurkceKarakterlerKorunur()
    {
        using var klasor = new GeciciKlasor();
        string yol = Path.Combine(klasor.Yol, "kayit.txt");

        _yazici.Yaz(yol, "Tuş Kayıtları: ığüşöç");

        Assert.AreEqual("Tuş Kayıtları: ığüşöç", File.ReadAllText(yol, Encoding.UTF8));
    }

    [TestMethod]
    public void Yaz_DosyaBasindaUtf8BomVar()
    {
        using var klasor = new GeciciKlasor();
        string yol = Path.Combine(klasor.Yol, "kayit.txt");

        _yazici.Yaz(yol, "a");

        byte[] baytlar = File.ReadAllBytes(yol);
        CollectionAssert.AreEqual(new byte[] { 0xEF, 0xBB, 0xBF, (byte)'a' }, baytlar);
    }

    [TestMethod]
    public void Yaz_VarOlanDosyaninUstuneYazar()
    {
        using var klasor = new GeciciKlasor();
        string yol = Path.Combine(klasor.Yol, "kayit.txt");
        _yazici.Yaz(yol, "uzun eski icerik");

        _yazici.Yaz(yol, "yeni");

        Assert.AreEqual("yeni", File.ReadAllText(yol, Encoding.UTF8));
    }

    [TestMethod]
    public void Yaz_KlasorYoksa_HataCagiranaGider()
    {
        // Hata yutulmamali: presenter yakalayip kullaniciya "yazilamadi" demeli.
        using var klasor = new GeciciKlasor();
        string yol = Path.Combine(klasor.Yol, "olmayan", "kayit.txt");

        Assert.ThrowsExactly<DirectoryNotFoundException>(() => _yazici.Yaz(yol, "a"));
    }
}
