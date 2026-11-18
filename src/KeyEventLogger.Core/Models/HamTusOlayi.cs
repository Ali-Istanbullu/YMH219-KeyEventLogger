namespace KeyEventLogger.Core.Models;

// Formdan presenter'a gelen HAM olay bilgisi (henuz zamani yok).
// Zamani presenter ISaat'ten ekler; boylece testte zaman sabitlenebilir.
public sealed record HamTusOlayi(OlayTuru Tur, string Tus, char? Karakter, bool Ctrl, bool Shift, bool Alt);
