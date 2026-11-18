using KeyEventLogger.Core.Models;
using KeyEventLogger.Core.Services;

namespace KeyEventLogger.Core.Tests.Services;

[TestClass]
public sealed class SatirBicimleyiciTestleri
{
    private readonly SatirBicimleyici _bicimleyici = new();

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
    public void EkTusunKendisi_TekrarYazilmaz()
    {
        var olay = new TusOlayi(TestVerisi.Baz, OlayTuru.TusBasildi, "Shift", null, Ctrl: true, Shift: true, Alt: false);

        Assert.AreEqual("18.11.2026 10:00:00.000  KeyDown   Shift           Ctrl", _bicimleyici.Satir(olay));
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
}
