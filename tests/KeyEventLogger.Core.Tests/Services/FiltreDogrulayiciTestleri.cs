using KeyEventLogger.Core.Models;
using KeyEventLogger.Core.Services;

namespace KeyEventLogger.Core.Tests.Services;

[TestClass]
public sealed class FiltreDogrulayiciTestleri
{
    private readonly FiltreDogrulayici _dogrulayici = new();

    [TestMethod]
    public void FiltreKapali_AralikYokGecersizDegil()
    {
        FiltreSonucu sonuc = _dogrulayici.Dogrula(false, TestVerisi.Baz.AddHours(1), TestVerisi.Baz);

        Assert.IsNull(sonuc.Aralik);
        Assert.IsFalse(sonuc.GecersizMi);
    }

    [TestMethod]
    public void Gecerli_AralikDoner()
    {
        FiltreSonucu sonuc = _dogrulayici.Dogrula(true, TestVerisi.Baz, TestVerisi.Baz.AddMinutes(1));

        Assert.IsNotNull(sonuc.Aralik);
        Assert.AreEqual(TestVerisi.Baz, sonuc.Aralik.Baslangic);
        Assert.IsFalse(sonuc.GecersizMi);
    }

    [TestMethod]
    public void BaslangicBitistenSonra_GecersizVeAralikYok()
    {
        FiltreSonucu sonuc = _dogrulayici.Dogrula(true, TestVerisi.Baz.AddMinutes(1), TestVerisi.Baz);

        Assert.IsNull(sonuc.Aralik);
        Assert.IsTrue(sonuc.GecersizMi);
    }
}
