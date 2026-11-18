namespace KeyEventLogger.WinForms.Platform;

// Uygulama Debug mi Release mi derlendi?
// Debug'da kodumuzdaki hatalar YAKALANMAZ: debugger hatanin oldugu satirda durur, degiskenler gorulur.
// Release'te (kullanicinin kurdugu surum) yakalanmayan hata loga yazilir, kullaniciya genel mesaj gosterilir.
public static class DerlemeModu
{
#if DEBUG
    public static readonly bool HataAyiklamaMi = true;
#else
    public static readonly bool HataAyiklamaMi = false;
#endif
}
