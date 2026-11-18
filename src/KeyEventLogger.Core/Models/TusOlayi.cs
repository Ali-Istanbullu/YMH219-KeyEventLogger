namespace KeyEventLogger.Core.Models;

// Yakalanan TEK bir klavye olayi (listedeki bir satir).
// "record": degistirilemez veri nesnesi; olusturulduktan sonra alanlari degismez.
//
// Zaman    : olayin geldigi an (tarih + saat + milisaniye)
// Tur      : KeyDown / KeyPress / KeyUp
// Tus      : tusun adi ("A", "Enter", "ShiftKey", "F5" ...). KeyPress'te yazilan karakterin adi.
// Karakter : sadece KeyPress'te dolu (yazilan karakter), digerlerinde null
// Ctrl/Shift/Alt : olay aninda bu tuslar basili miydi
public sealed record TusOlayi(
    DateTime Zaman,
    OlayTuru Tur,
    string Tus,
    char? Karakter,
    bool Ctrl,
    bool Shift,
    bool Alt);
