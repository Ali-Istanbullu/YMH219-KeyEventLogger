using System.Runtime.InteropServices;

namespace KeyEventLogger.WinForms.Platform;

// IKlavyeDuzeni'nin Windows uygulamasi: user32.dll MapVirtualKeyW (CK21).
// Microsoft belgesi (MapVirtualKeyW, MAPVK_VK_TO_CHAR = 2):
//  - sanal tus kodu, alt 16 bitte Shift'siz karaktere cevrilir; karsiligi yoksa 0 doner
//  - "olu tus" (aksan) ise en ust bit 1 olur -> alt 16 biti aliriz
//  - A..Z tuslari klavye duzeninden bagimsiz olarak buyuk A..Z verir
// Cevirmeyi cagiran is parcaciginin (arayuz) aktif klavye duzenine gore yapar.
// Tek sorumlulugu: Windows'a sormak (S). Hata olursa '\0' doner; cagiran Windows'un adini kullanir (R6).
public sealed class WindowsKlavyeDuzeni : IKlavyeDuzeni
{
    private const uint MapvkVkToChar = 2;

    public char Karakter(Keys tusKodu)
    {
        try
        {
            uint sonuc = MapVirtualKeyW((uint)(tusKodu & Keys.KeyCode), MapvkVkToChar);
            return (char)(sonuc & 0xFFFF);
        }
        catch
        {
            // user32 cagrisi basarisizsa ad Windows'un kendi adiyla yazilir; uygulama devam eder.
            return '\0';
        }
    }

    [DllImport("user32.dll")]
    private static extern uint MapVirtualKeyW(uint uCode, uint uMapType);
}
