using System.Runtime.InteropServices;

namespace KeyEventLogger.WinForms.Platform;

// IKlavyeDuzeni'nin Windows uygulamasi: user32.dll MapVirtualKeyW.
// Microsoft belgesi (MapVirtualKeyW, MAPVK_VK_TO_CHAR = 2):
//  - sanal tus kodu, alt 16 bitte Shift'siz karaktere cevrilir; karsiligi yoksa 0 doner
//  - "olu tus" (aksan) ise en ust bit 1 olur -> alt 16 biti aliriz
//  - A..Z tuslari klavye duzeninden bagimsiz olarak buyuk A..Z verir
// Cevirmeyi cagiran is parcaciginin (arayuz) aktif klavye duzenine gore yapar.
// Tek sorumlulugu: Windows'a sormak (S). Cagri yapilamazsa '\0' doner; cagiran Windows'un adini kullanir.
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
        catch (Exception hata) when (hata is DllNotFoundException or EntryPointNotFoundException)
        {
            // Beklenen: user32 cagrisi yapilamadi. Ad Windows'un kendi adiyla yazilir, uygulama devam eder.
            return '\0';
        }
    }

    [DllImport("user32.dll")]
    private static extern uint MapVirtualKeyW(uint uCode, uint uMapType);
}
