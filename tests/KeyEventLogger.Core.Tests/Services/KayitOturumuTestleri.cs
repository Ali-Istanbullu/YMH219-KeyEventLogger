using KeyEventLogger.Core.Services;

namespace KeyEventLogger.Core.Tests.Services;

[TestClass]
public sealed class KayitOturumuTestleri
{
    [TestMethod]
    public void Baslangicta_KayitKapaliVeListeBos()
    {
        var oturum = new KayitOturumu();

        Assert.IsFalse(oturum.KayitAcikMi);
        Assert.IsEmpty(oturum.Olaylar);
    }

    [TestMethod]
    public void KayitKapaliyken_OlayEklenmez()
    {
        var oturum = new KayitOturumu();

        bool eklendi = oturum.Ekle(TestVerisi.Basis("A"));

        Assert.IsFalse(eklendi);
        Assert.IsEmpty(oturum.Olaylar);
    }

    [TestMethod]
    public void BaslatDurdur_SadeceAcikkenEklenir()
    {
        var oturum = new KayitOturumu();

        oturum.Baslat();
        oturum.Ekle(TestVerisi.Basis("A"));
        oturum.Durdur();
        oturum.Ekle(TestVerisi.Basis("B"));

        Assert.HasCount(1, oturum.Olaylar);
        Assert.AreEqual("A", oturum.Olaylar[0].Tus);
    }

    [TestMethod]
    public void Temizle_ListeyiBosaltirAmaDurumuDegistirmez()
    {
        var oturum = new KayitOturumu();
        oturum.Baslat();
        oturum.Ekle(TestVerisi.Basis("A"));

        oturum.Temizle();

        Assert.IsEmpty(oturum.Olaylar);
        Assert.IsTrue(oturum.KayitAcikMi);
    }
}
