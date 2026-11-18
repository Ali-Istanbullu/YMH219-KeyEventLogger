using KeyEventLogger.Core.Abstractions;
using KeyEventLogger.Core.Models;

namespace KeyEventLogger.Core.Services;

// Tek sorumlulugu: filtre degerlerinin gecerli olup olmadigina karar vermek (S).
// Kullaniciya mesaj gostermez; o karar presenter'in (koordinasyon).
public sealed class FiltreDogrulayici : IFiltreDogrulayici
{
    public FiltreSonucu Dogrula(bool filtreAcik, DateTime baslangic, DateTime bitis)
    {
        if (!filtreAcik)
        {
            return FiltreSonucu.Yok;                     // filtre kapali: hepsi gorunur
        }
        if (baslangic > bitis)
        {
            return new FiltreSonucu(null, GecersizMi: true);   // CK7: uygulanmaz, uyarilir
        }
        return new FiltreSonucu(new TarihAraligi(baslangic, bitis), GecersizMi: false);
    }
}
