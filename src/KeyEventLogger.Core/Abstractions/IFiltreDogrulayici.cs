using KeyEventLogger.Core.Models;

namespace KeyEventLogger.Core.Abstractions;

// Formdaki filtre degerlerini dogrular ve uygulanacak araligi verir (S: presenter'dan ayrildi).
public interface IFiltreDogrulayici
{
    FiltreSonucu Dogrula(bool filtreAcik, DateTime baslangic, DateTime bitis);
}
