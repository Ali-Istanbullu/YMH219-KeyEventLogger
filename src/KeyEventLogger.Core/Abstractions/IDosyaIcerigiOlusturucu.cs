using KeyEventLogger.Core.Models;

namespace KeyEventLogger.Core.Abstractions;

// .txt dosyasinin tam icerigini olusturur: baslik + filtre bilgisi + satirlar.
public interface IDosyaIcerigiOlusturucu
{
    string Olustur(IReadOnlyList<TusOlayi> olaylar, TarihAraligi? filtre, DateTime olusturmaZamani);
}
