using KeyEventLogger.Core.Abstractions;
using KeyEventLogger.Core.Models;

namespace KeyEventLogger.Core.Services;

// Tek sorumlulugu: bir olayi tek satira cevirmek (S). CK3, CK4.
// Sutunlar sabit genislikte (PadRight): ListBox'ta esit genislikli yazi tipiyle (Consolas) hizali durur.
public sealed class SatirBicimleyici : ISatirBicimleyici
{
    public string Satir(TusOlayi olay)
    {
        ArgumentNullException.ThrowIfNull(olay);

        string zaman = olay.Zaman.ToString(BicimAyarlari.ZamanBicimi, BicimAyarlari.Turkce);
        string tur = OlayAdi(olay.Tur).PadRight(8);
        string tus = TusGosterimi(olay).PadRight(14);

        return $"{zaman}  {tur}  {tus}{EkTuslar(olay)}".TrimEnd();
    }

    // Dersin terimleriyle olay adi (PDF: KeyDown, KeyPress, KeyUp) - CK2.
    public static string OlayAdi(OlayTuru tur) => tur switch
    {
        OlayTuru.TusBasildi => "KeyDown",
        OlayTuru.KarakterYazildi => "KeyPress",
        OlayTuru.TusBirakildi => "KeyUp",
        _ => tur.ToString()
    };

    // KeyPress'te yazilan karakter gosterilir; gorunmeyen karakterlere ad verilir (CK4).
    private static string TusGosterimi(TusOlayi olay)
    {
        if (olay.Tur != OlayTuru.KarakterYazildi || olay.Karakter is not char karakter)
        {
            return olay.Tus;
        }
        return karakter switch
        {
            '\r' => "'Enter'",
            '\b' => "'Backspace'",
            '\t' => "'Tab'",
            '\u001b' => "'Esc'",
            ' ' => "'Boşluk'",
            _ when char.IsControl(karakter) => $"'U+{(int)karakter:X4}'",   // ornek: Ctrl+C -> U+0003
            _ => $"'{karakter}'"
        };
    }

    // Basili ek tuslar: "  Ctrl+Shift". Hicbiri yoksa bos.
    private static string EkTuslar(TusOlayi olay)
    {
        var parcalar = new List<string>(3);
        if (olay.Ctrl) parcalar.Add("Ctrl");
        if (olay.Shift) parcalar.Add("Shift");
        if (olay.Alt) parcalar.Add("Alt");
        return parcalar.Count == 0 ? string.Empty : "  " + string.Join('+', parcalar);
    }
}
