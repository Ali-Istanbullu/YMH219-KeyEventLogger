namespace KeyEventLogger.Core.Models;

// Filtre dogrulamasinin sonucu.
// Aralik       : uygulanacak aralik; filtre kapali ya da gecersizse null (hepsi gorunur)
// GecersizMi   : filtre acik ama baslangic > bitis (kullanici uyarilmali)
public sealed record FiltreSonucu(TarihAraligi? Aralik, bool GecersizMi)
{
    public static FiltreSonucu Yok { get; } = new(null, false);
}
