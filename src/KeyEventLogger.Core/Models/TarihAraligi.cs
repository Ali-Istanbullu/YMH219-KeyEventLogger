namespace KeyEventLogger.Core.Models;

// Filtre icin baslangic-bitis araligi (ikisi de DAHIL).
// Gecersiz aralik (baslangic > bitis) olusturulamaz: hata hemen ve acik verilir.
public sealed record TarihAraligi
{
    public DateTime Baslangic { get; }
    public DateTime Bitis { get; }

    public TarihAraligi(DateTime baslangic, DateTime bitis)
    {
        if (baslangic > bitis)
        {
            throw new ArgumentException("Başlangıç zamanı bitiş zamanından sonra olamaz.", nameof(baslangic));
        }
        Baslangic = baslangic;
        Bitis = bitis;
    }

    // Verilen an araligin icinde mi?
    public bool IcindeMi(DateTime an) => an >= Baslangic && an <= Bitis;
}
