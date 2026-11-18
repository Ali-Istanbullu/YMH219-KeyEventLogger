using KeyEventLogger.Core.Models;
using KeyEventLogger.Core.Services;

namespace KeyEventLogger.Core.Tests.Services;

[TestClass]
public sealed class KayitBicimleyiciTestleri
{
    private readonly KayitBicimleyici _bicimleyici = new();

    [TestMethod]
    public void KeyDown_TarihOlayVeTusYazilir()
    {
        string satir = _bicimleyici.Satir(TestVerisi.Basis("A"));

        Assert.AreEqual("18.11.2026 10:00:00.000  KeyDown   A", satir);
    }

    [TestMethod]
    public void EkTuslar_SondaYazilir()
    {
        var olay = new TusOlayi(TestVerisi.Baz, OlayTuru.TusBasildi, "C", null, Ctrl: true, Shift: true, Alt: false);

        Assert.AreEqual("18.11.2026 10:00:00.000  KeyDown   C               Ctrl+Shift", _bicimleyici.Satir(olay));
    }

    [TestMethod]
    [DataRow('a', "'a'")]
    [DataRow('\r', "'Enter'")]
    [DataRow(' ', "'Boşluk'")]
    [DataRow('\u0003', "'U+0003'")]
    public void KeyPress_KarakterGosterilir(char karakter, string beklenen)
    {
        string satir = _bicimleyici.Satir(TestVerisi.Yazis(karakter));

        Assert.AreEqual($"18.11.2026 10:00:00.000  KeyPress  {beklenen}", satir);
    }

    [TestMethod]
    public void DosyaMetni_BaslikFiltreVeSatirlar()
    {
        var filtre = new TarihAraligi(TestVerisi.Baz, TestVerisi.Baz.AddMinutes(1));

        string metin = _bicimleyici.DosyaMetni([TestVerisi.Basis("A")], filtre, TestVerisi.Baz.AddHours(1));
        string[] satirlar = metin.Split(Environment.NewLine);

        Assert.AreEqual("Key Event Logger - Tuş Kayıtları", satirlar[0]);
        Assert.AreEqual("Oluşturulma: 18.11.2026 11:00:00.000", satirlar[1]);
        Assert.AreEqual("Filtre: 18.11.2026 10:00:00.000 - 18.11.2026 10:01:00.000", satirlar[2]);
        Assert.AreEqual("Kayıt sayısı: 1", satirlar[3]);
        Assert.AreEqual("18.11.2026 10:00:00.000  KeyDown   A", satirlar[5]);
    }
}
