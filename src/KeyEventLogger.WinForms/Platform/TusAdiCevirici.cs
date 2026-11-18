using System.Globalization;

namespace KeyEventLogger.WinForms.Platform;

// Tek sorumlulugu: Keys degerini listede gosterilecek ada cevirmek (S).
//
// 1) Ozel tuslar sozlukten: Return -> Enter, ShiftKey -> Shift, D5 -> 5 ...
//    (Keys enum'unda ayni sayiya iki ad var, ToString() hangisini verecegi belli degil.)
// 2) Harf ve noktalama tuslari (A..Z, Oem...) KLAVYE DUZENINE gore: Windows'un tus kodlari ABD
//    klavyesine gore adlandirilmistir. Turkce Q'da "i" tusunun kodu OemQuotes, "ü" tusununki Oem6'dir.
//    Bu yuzden karakter klavye duzeninden sorulur ve Turkce kurala gore buyutulur: i -> İ, ü -> Ü.
// 3) Digerleri Windows'un kendi adiyla (F5, Delete, NumPad1 ...).
public sealed class TusAdiCevirici : ITusAdiCevirici
{
    private static readonly CultureInfo Turkce = CultureInfo.GetCultureInfo("tr-TR");

    // DIKKAT: ayni sayiya karsilik gelen iki Keys degeri (Enter/Return gibi) sozluge iki kez eklenemez.
    private static readonly Dictionary<Keys, string> OzelAdlar = new()
    {
        [Keys.Enter] = "Enter",
        [Keys.ShiftKey] = "Shift",
        [Keys.ControlKey] = "Ctrl",
        [Keys.Menu] = "Alt",
        [Keys.CapsLock] = "CapsLock",
        [Keys.Back] = "Backspace",
        [Keys.Escape] = "Esc",
        [Keys.Space] = "Space",
        [Keys.PageUp] = "PageUp",
        [Keys.PageDown] = "PageDown",
        [Keys.LWin] = "Win",
        [Keys.RWin] = "Win",
        [Keys.Apps] = "Menü",
        [Keys.D0] = "0", [Keys.D1] = "1", [Keys.D2] = "2", [Keys.D3] = "3", [Keys.D4] = "4",
        [Keys.D5] = "5", [Keys.D6] = "6", [Keys.D7] = "7", [Keys.D8] = "8", [Keys.D9] = "9"
    };

    private readonly IKlavyeDuzeni _klavyeDuzeni;

    public TusAdiCevirici(IKlavyeDuzeni klavyeDuzeni)
    {
        ArgumentNullException.ThrowIfNull(klavyeDuzeni);
        _klavyeDuzeni = klavyeDuzeni;
    }

    public string Ad(Keys tusKodu)
    {
        if (OzelAdlar.TryGetValue(tusKodu, out string? ad))
        {
            return ad;
        }

        if (KlavyeDuzenineBagliMi(tusKodu))
        {
            char karakter = _klavyeDuzeni.Karakter(tusKodu);
            if (karakter != '\0' && !char.IsControl(karakter) && !char.IsWhiteSpace(karakter))
            {
                return char.ToUpper(karakter, Turkce).ToString();   // i -> İ (Turkce kurali)
            }
        }
        return tusKodu.ToString();
    }

    // Harf tuslari ve OEM (noktalama / Turkce harf) tuslari klavye duzenine gore degisir.
    private static bool KlavyeDuzenineBagliMi(Keys tusKodu) =>
        tusKodu is >= Keys.A and <= Keys.Z
        || tusKodu.ToString().StartsWith("Oem", StringComparison.Ordinal);
}
