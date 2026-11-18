using System.Text;
using KeyEventLogger.Core.Abstractions;
using KeyEventLogger.Core.Models;

namespace KeyEventLogger.Core.Services;

// Tek sorumlulugu: .txt dosyasinin icerigini olusturmak (S).
// Satirlari kendisi bicimlemez; ISatirBicimleyici'den ister (ekranla dosya ayni kalsin diye).
public sealed class DosyaIcerigiOlusturucu : IDosyaIcerigiOlusturucu
{
    private readonly ISatirBicimleyici _satirBicimleyici;

    public DosyaIcerigiOlusturucu(ISatirBicimleyici satirBicimleyici)
    {
        ArgumentNullException.ThrowIfNull(satirBicimleyici);
        _satirBicimleyici = satirBicimleyici;
    }

    public string Olustur(IReadOnlyList<TusOlayi> olaylar, TarihAraligi? filtre, DateTime olusturmaZamani)
    {
        ArgumentNullException.ThrowIfNull(olaylar);

        var metin = new StringBuilder();
        metin.AppendLine("Key Event Logger - Tuş Kayıtları");
        metin.AppendLine($"Oluşturulma: {Zaman(olusturmaZamani)}");
        metin.AppendLine(filtre is null
            ? "Filtre: yok (tüm kayıtlar)"
            : $"Filtre: {Zaman(filtre.Baslangic)} - {Zaman(filtre.Bitis)}");
        metin.AppendLine($"Kayıt sayısı: {olaylar.Count}");
        metin.AppendLine(new string('-', 60));

        foreach (TusOlayi olay in olaylar)
        {
            metin.AppendLine(_satirBicimleyici.Satir(olay));
        }
        return metin.ToString();
    }

    private static string Zaman(DateTime an) => an.ToString(BicimAyarlari.ZamanBicimi, BicimAyarlari.Turkce);
}
