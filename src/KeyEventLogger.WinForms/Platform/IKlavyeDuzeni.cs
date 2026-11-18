namespace KeyEventLogger.WinForms.Platform;

// Bir tusun, kullanicinin SU ANKI klavye duzeninde (Turkce Q, F, Ingilizce ...) hangi karakteri
// urettigini soyler. Ornek (Turkce Q): OemQuotes -> 'i', Oem6 -> 'u' yerine 'ü'.
public interface IKlavyeDuzeni
{
    // Shift'siz karakter; karsiligi yoksa '\0'.
    char Karakter(Keys tusKodu);
}
