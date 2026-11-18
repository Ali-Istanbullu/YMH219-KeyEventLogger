namespace KeyEventLogger.Core.Models;

// Bir tusa basilinca Windows forma 3 ayri olay gonderir (PDF: KeyDown, KeyUp, KeyPress).
// Bu enum hangisinin geldigini tutar.
public enum OlayTuru
{
    // KeyDown: tusa basildi (her tus icin gelir: harf, Shift, oklar, F1 ...).
    // Tus basili tutulursa Windows bunu tekrar tekrar gonderir.
    TusBasildi,

    // KeyPress: bir karakter yazildi (sadece harf, rakam, isaret, Enter, Backspace gibi
    // karakter ureten tuslarda gelir; Shift, Ctrl, oklar icin GELMEZ).
    KarakterYazildi,

    // KeyUp: tus birakildi.
    TusBirakildi
}
