using KeyEventLogger.Core.Models;
using KeyEventLogger.Core.Services;

namespace KeyEventLogger.Core.Tests.Services;

[TestClass]
public sealed class DosyaIcerigiOlusturucuTestleri
{
    private readonly DosyaIcerigiOlusturucu _olusturucu = new(new SatirBicimleyici());

    [TestMethod]
    public void Filtreli_BaslikFiltreVeSatirlar()
    {
        var filtre = new TarihAraligi(TestVerisi.Baz, TestVerisi.Baz.AddMinutes(1));

        string[] satirlar = _olusturucu.Olustur([TestVerisi.Basis("A")], filtre, TestVerisi.Baz.AddHours(1))
            .Split(Environment.NewLine);

        Assert.AreEqual("Key Event Logger - Tuş Kayıtları", satirlar[0]);
        Assert.AreEqual("Oluşturulma: 18.11.2026 11:00:00.000", satirlar[1]);
        Assert.AreEqual("Filtre: 18.11.2026 10:00:00.000 - 18.11.2026 10:01:00.000", satirlar[2]);
        Assert.AreEqual("Kayıt sayısı: 1", satirlar[3]);
        Assert.AreEqual("18.11.2026 10:00:00.000  KeyDown   A", satirlar[5]);
    }

    [TestMethod]
    public void Filtresiz_TumKayitlarYazar()
    {
        string metin = _olusturucu.Olustur([], null, TestVerisi.Baz);

        Assert.Contains("Filtre: yok (tüm kayıtlar)", metin);
        Assert.Contains("Kayıt sayısı: 0", metin);
    }
}
