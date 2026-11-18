using KeyEventLogger.Core.Models;

namespace KeyEventLogger.Core.Abstractions;

// Tum catch bloklarinin kullandigi TEK giris noktasi.
// Hatayi HER ZAMAN loga yazar; gorunurluk EkranVeGunluk ise kullaniciya da gosterir.
public interface IHataBildirici
{
    void Bildir(Exception hata, Gorunurluk gorunurluk, string kullaniciMesaji);
}
