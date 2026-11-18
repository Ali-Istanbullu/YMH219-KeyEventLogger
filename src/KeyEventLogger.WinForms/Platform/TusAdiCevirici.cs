namespace KeyEventLogger.WinForms.Platform;

// Tek sorumlulugu: Keys degerini listede gosterilecek ada cevirmek (S). CK12.
// Neden gerekli? Keys enum'unda ayni sayiya iki ad var (Return = Enter = 13, Prior = PageUp ...);
// ToString() hangisini verecegi belli degil. Ayrica "D5", "ShiftKey" gibi adlar kullaniciya yabanci.
// Sozlukte olmayan tuslar Windows'un kendi adiyla yazilir (A, F5, Delete, Oemcomma ...).
public sealed class TusAdiCevirici : ITusAdiCevirici
{
    // DIKKAT: ayni sayiya karsilik gelen iki Keys degeri (Enter/Return gibi) sozluge iki kez
    // eklenemez; her biri bir kez yazildi.
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

    public string Ad(Keys tusKodu) =>
        OzelAdlar.TryGetValue(tusKodu, out string? ad) ? ad : tusKodu.ToString();
}
