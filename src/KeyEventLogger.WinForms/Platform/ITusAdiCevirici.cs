namespace KeyEventLogger.WinForms.Platform;

// Windows'un tus kodunu (Keys) okunur bir ada cevirir: Keys.Return -> "Enter", Keys.D5 -> "5".
public interface ITusAdiCevirici
{
    string Ad(Keys tusKodu);
}
