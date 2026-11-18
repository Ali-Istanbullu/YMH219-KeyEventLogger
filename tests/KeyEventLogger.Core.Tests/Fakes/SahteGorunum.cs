using KeyEventLogger.Core.Abstractions;
using KeyEventLogger.Core.Models;

namespace KeyEventLogger.Core.Tests.Fakes;

// Form yerine gecen sahte gorunum: butonlara "basar", gosterilenleri hatirlar.
public sealed class SahteGorunum : IKayitGorunumu
{
    public event EventHandler? BaslatTiklandi;
    public event EventHandler? DurdurTiklandi;
    public event EventHandler? TemizleTiklandi;
    public event EventHandler? KaydetTiklandi;
    public event EventHandler? FiltreDegisti;
    public event EventHandler<HamTusOlayi>? TusOlayiGeldi;

    public bool FiltreAcikMi { get; set; }
    public DateTime FiltreBaslangic { get; set; }
    public DateTime FiltreBitis { get; set; }

    public List<string> Satirlar { get; } = [];
    public bool? KayitAcikGosterildi { get; private set; }
    public IstatistikOzeti? SonIstatistik { get; private set; }
    public bool SatirEkleHataFirlatsin { get; set; }

    public void SatirEkle(string satir)
    {
        if (SatirEkleHataFirlatsin)
        {
            throw new InvalidOperationException("Sahte gorunum hatasi");
        }
        Satirlar.Add(satir);
    }

    public void ListeyiDoldur(IReadOnlyList<string> satirlar)
    {
        Satirlar.Clear();
        Satirlar.AddRange(satirlar);
    }

    public void KayitDurumunuGoster(bool kayitAcik) => KayitAcikGosterildi = kayitAcik;
    public void IstatistigiGoster(IstatistikOzeti ozet) => SonIstatistik = ozet;

    // ---- Testin "kullanici" gibi davranmasi icin ----
    public void BaslataBas() => BaslatTiklandi?.Invoke(this, EventArgs.Empty);
    public void DurduraBas() => DurdurTiklandi?.Invoke(this, EventArgs.Empty);
    public void TemizleyeBas() => TemizleTiklandi?.Invoke(this, EventArgs.Empty);
    public void KaydeteBas() => KaydetTiklandi?.Invoke(this, EventArgs.Empty);
    public void FiltreyiDegistir() => FiltreDegisti?.Invoke(this, EventArgs.Empty);

    // Bir tus vurusu: KeyDown + (varsa) KeyPress + KeyUp
    public void TusaBas(string tus, char? karakter = null)
    {
        TusOlayiGeldi?.Invoke(this, new HamTusOlayi(OlayTuru.TusBasildi, tus, null, false, false, false));
        if (karakter is not null)
        {
            TusOlayiGeldi?.Invoke(this, new HamTusOlayi(OlayTuru.KarakterYazildi, tus, karakter, false, false, false));
        }
        TusOlayiGeldi?.Invoke(this, new HamTusOlayi(OlayTuru.TusBirakildi, tus, null, false, false, false));
    }
}
