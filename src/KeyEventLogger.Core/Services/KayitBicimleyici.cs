using System.Globalization;
using System.Text;
using KeyEventLogger.Core.Abstractions;
using KeyEventLogger.Core.Models;

namespace KeyEventLogger.Core.Services;

// Tek sorumlulugu: olaylari okunur metne cevirmek (S).
// Ekrandaki satir ile .txt dosyasindaki satir AYNIDIR (kullanici ne goruyorsa onu kaydeder).
public sealed class KayitBicimleyici : IKayitBicimleyici
{
    // Tarih bicimi: gg.aa.yyyy ss:dd:sn.milisaniye (Turkiye bicimi)
    private const string ZamanBicimi = "dd.MM.yyyy HH:mm:ss.fff";
    private static readonly CultureInfo Turkce = CultureInfo.GetCultureInfo("tr-TR");

    public string Satir(TusOlayi olay)
    {
        ArgumentNullException.ThrowIfNull(olay);

        // Sutunlar sabit genislikte (PadRight): ListBox'ta esit genislikli yazi tipiyle hizali durur.
        string zaman = olay.Zaman.ToString(ZamanBicimi, Turkce);
        string tur = OlayAdi(olay.Tur).PadRight(8);
        string tus = TusGosterimi(olay).PadRight(14);
        string ekTuslar = EkTuslar(olay);

        return $"{zaman}  {tur}  {tus}{ekTuslar}".TrimEnd();
    }

    public string DosyaMetni(IReadOnlyList<TusOlayi> olaylar, TarihAraligi? filtre, DateTime olusturmaZamani)
    {
        ArgumentNullException.ThrowIfNull(olaylar);

        var metin = new StringBuilder();
        metin.AppendLine("Key Event Logger - Tuş Kayıtları");
        metin.AppendLine($"Oluşturulma: {olusturmaZamani.ToString(ZamanBicimi, Turkce)}");
        metin.AppendLine(filtre is null
            ? "Filtre: yok (tüm kayıtlar)"
            : $"Filtre: {filtre.Baslangic.ToString(ZamanBicimi, Turkce)} - {filtre.Bitis.ToString(ZamanBicimi, Turkce)}");
        metin.AppendLine($"Kayıt sayısı: {olaylar.Count}");
        metin.AppendLine(new string('-', 60));

        foreach (TusOlayi olay in olaylar)
        {
            metin.AppendLine(Satir(olay));
        }
        return metin.ToString();
    }

    // Dersin terimleriyle olay adi (PDF: KeyDown, KeyPress, KeyUp).
    public static string OlayAdi(OlayTuru tur) => tur switch
    {
        OlayTuru.TusBasildi => "KeyDown",
        OlayTuru.KarakterYazildi => "KeyPress",
        OlayTuru.TusBirakildi => "KeyUp",
        _ => tur.ToString()
    };

    // KeyPress'te yazilan karakter gosterilir; gorunmeyen karakterlere ad verilir.
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
