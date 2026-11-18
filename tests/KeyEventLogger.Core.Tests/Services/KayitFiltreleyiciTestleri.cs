using KeyEventLogger.Core.Models;
using KeyEventLogger.Core.Services;

namespace KeyEventLogger.Core.Tests.Services;

[TestClass]
public sealed class KayitFiltreleyiciTestleri
{
    private readonly KayitFiltreleyici _filtreleyici = new();
    private readonly List<TusOlayi> _olaylar =
    [
        TestVerisi.Basis("A", 0), TestVerisi.Basis("B", 10), TestVerisi.Basis("C", 20), TestVerisi.Basis("D", 30)
    ];

    [TestMethod]
    public void FiltreYoksa_HepsiDoner()
    {
        Assert.HasCount(4, _filtreleyici.Filtrele(_olaylar, null));
    }

    [TestMethod]
    public void Aralik_SinirlarDahil()
    {
        var aralik = new TarihAraligi(TestVerisi.Baz.AddSeconds(10), TestVerisi.Baz.AddSeconds(20));

        IReadOnlyList<TusOlayi> sonuc = _filtreleyici.Filtrele(_olaylar, aralik);

        CollectionAssert.AreEqual(new[] { "B", "C" }, sonuc.Select(o => o.Tus).ToArray());
    }

    [TestMethod]
    public void GecersizAralik_OlusturulamaZ()
    {
        Assert.ThrowsExactly<ArgumentException>(() =>
            new TarihAraligi(TestVerisi.Baz.AddSeconds(5), TestVerisi.Baz));
    }
}
