using KeyEventLogger.Core.Models;
using KeyEventLogger.Core.Services;

namespace KeyEventLogger.Core.Tests.Services;

[TestClass]
public sealed class TusIstatistikHesaplayiciTestleri
{
    private readonly TusIstatistikHesaplayici _hesaplayici = new();

    [TestMethod]
    public void BosListe_SifirVeBosListe()
    {
        IstatistikOzeti ozet = _hesaplayici.Hesapla([]);

        Assert.AreEqual(0, ozet.ToplamTus);
        Assert.IsEmpty(ozet.EnSikTuslar);
    }

    [TestMethod]
    public void SadeceKeyDownSayilir()
    {
        // Bir A vurusu = KeyDown + KeyPress + KeyUp -> 1 tus
        IstatistikOzeti ozet = _hesaplayici.Hesapla(
            [TestVerisi.Basis("A"), TestVerisi.Yazis('a'), TestVerisi.Birakis("A")]);

        Assert.AreEqual(1, ozet.ToplamTus);
        Assert.HasCount(1, ozet.EnSikTuslar);
        Assert.AreEqual(new TusSayisi("A", 1), ozet.EnSikTuslar[0]);
    }

    [TestMethod]
    public void EnSikTuslar_CoktanAzaVeEsitlikteAlfabetik()
    {
        IstatistikOzeti ozet = _hesaplayici.Hesapla(
        [
            TestVerisi.Basis("B"), TestVerisi.Basis("B"),
            TestVerisi.Basis("C"), TestVerisi.Basis("A"),
            TestVerisi.Basis("Enter"), TestVerisi.Basis("Enter"), TestVerisi.Basis("Enter")
        ]);

        Assert.AreEqual(7, ozet.ToplamTus);
        CollectionAssert.AreEqual(
            new[] { "Enter", "B", "A", "C" },
            ozet.EnSikTuslar.Select(t => t.Tus).ToArray());
    }

    [TestMethod]
    public void EnFazlaBesTusGosterilir()
    {
        IstatistikOzeti ozet = _hesaplayici.Hesapla(
            new[] { "A", "B", "C", "D", "E", "F", "G" }.Select(t => TestVerisi.Basis(t)));

        Assert.AreEqual(7, ozet.ToplamTus);
        Assert.HasCount(TusIstatistikHesaplayici.EnSikTusSayisi, ozet.EnSikTuslar);
    }
}
